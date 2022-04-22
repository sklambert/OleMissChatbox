using Microsoft.AspNetCore.SignalR;
using OleMissChatbox.Data;
using OleMissChatbox.Data.Entities;
using OleMissChatbox.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OleMissChatbox.Hubs
{
    public class ChatHub : Hub
    {
        public readonly static List<UserViewModel> _Connections = new List<UserViewModel>();
        public readonly static List<ClassViewModel> _Classes = new List<ClassViewModel>();

        private readonly OleMissChatboxContext _context;

        public ChatHub(OleMissChatboxContext context)
        {
            _context = context;
        }

        public async Task SetUserPermissionLevel(string currentEmail)
        {
            //Get usertype from database
            var user = _context.Users.Where(u => u.Email == currentEmail).FirstOrDefault();
            var userType = user.UserType;

            await Clients.Caller.SendAsync("SetUserPermissionLevel", userType);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task CreateClass(string currentEmail, string className)
        {
            try
            {
                var user = GetCurrentUser(currentEmail);

                if (user.UserType != 1 && user.UserType != 2)
                {
                    await Clients.Caller.SendAsync("onError", "You do not have permission to create classes!");
                }
                else
                {
                    if (_context.Classes.Any(c => c.ClassName == className))
                    {
                        await Clients.Caller.SendAsync("onError", "Another class with this name exists!");
                    }
                    else
                    {
                        // Create and save class in database
                        var newClass = new Class()
                        {
                            ClassName = className,
                            CreatedDate = System.DateTime.Now,
                        };
                        _context.Classes.Add(newClass);
                        _context.SaveChanges();

                        var newUserClass = new UserClass()
                        {
                            ClassId = newClass.ClassId,
                            UserId = user.UserId,
                            IsOwner = true,
                            RegistrationDate = System.DateTime.Now,
                        };
                        _context.UserClasses.Add(newUserClass);
                        _context.SaveChanges();

                        // Update class list
                        var classViewModel = new ClassViewModel()
                        {
                            ClassName = newClass.ClassName,
                            CreatedDate = newClass.CreatedDate
                        };
                        _Classes.Add(classViewModel);
                        await Clients.All.SendAsync("addClass");
                    }
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "Couldn't create class: " + ex.Message);
            }
        }

        public async Task JoinClass(string className)
        {
            try
            {
                var user = _Connections.Where(u => u.Email == IdentityName).FirstOrDefault();
                if (user != null && user.CurrentClass != className)
                {
                    if (!string.IsNullOrEmpty(user.CurrentClass))
                    {
                        await Clients.OthersInGroup(user.CurrentClass).SendAsync("removeUser", user);
                    }

                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.CurrentClass);
                    await Groups.AddToGroupAsync(Context.ConnectionId, className);
                    user.CurrentClass = className;

                    await Clients.OthersInGroup(className).SendAsync("addUser", user);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "You failed to join the class!" + ex.Message);
            }
        }

        public IEnumerable<ClassViewModel> GetClasses()
        {
            List<ClassViewModel> classes = new();

            foreach (var c in _context.Classes)
            {
                classes.Add(new ClassViewModel()
                {
                    ClassName = c.ClassName,
                    CreatedDate = c.CreatedDate
                });
            }

            return classes;
        }

        public IEnumerable<UserViewModel> GetUsers(string className)
        {
            return _Connections.Where(u => u.CurrentClass == className).ToList();
        }

        public IEnumerable<ChatMessageViewModel> GetMessages(string className)
        {
            var currentClass = _context.Classes.Where(c => c.ClassName == className).FirstOrDefault();
            List<UserClass> userClasses = _context.UserClasses.Where(uc => uc.ClassId == currentClass.ClassId).ToList();
            List<UserChatMessage> userChatMessages = new();

            foreach (UserClass userClass in userClasses)
            {
                var currentUserChatMessages = _context.UserChatMessages.Where(ucm => ucm.UserId == userClass.UserId).ToList();
                userChatMessages.AddRange(currentUserChatMessages);
            }

            List<ChatMessage> chatMessages = new();

            foreach (UserChatMessage userChatMessage in userChatMessages)
            {
                var currentChatMessages = _context.ChatMessages.Where(cm => cm.ChatMessageId == userChatMessage.ChatMessageId).ToList();
                chatMessages.AddRange(currentChatMessages);
            }

            chatMessages = chatMessages
                .OrderByDescending(cm => cm.MessageDate)
                .Take(20)
                .AsEnumerable()
                .Reverse()
                .ToList();

            List<ChatMessageViewModel> chatMessageViewModels = new();

            foreach (ChatMessage chatMessage in chatMessages)
            {
                var currentUserChatMessage = _context.UserChatMessages.Where(ucm => ucm.ChatMessageId == chatMessage.ChatMessageId).FirstOrDefault();
                var user = _context.Users.Where(u => u.UserId == currentUserChatMessage.UserId).FirstOrDefault();

                var chatMessageViewModel = new ChatMessageViewModel
                {
                    MessageString = chatMessage.MessageString,
                    MessageSender = user.FirstName + " " + user.LastName,
                    MessageDate = chatMessage.MessageDate
                };

                chatMessageViewModels.Add(chatMessageViewModel);
            }

            return chatMessageViewModels;
        }

        private string IdentityName
        {
            get { return Context.User.Identity.Name; }
        }

        private User GetCurrentUser(string email)
        {
            return _context.Users.Where(u => u.Email == email).FirstOrDefault();
        }
    }
}
