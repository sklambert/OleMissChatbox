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
        public readonly static List<UserViewModel> _Connections = new();
        public readonly static List<ClassViewModel> _Classes = new();
        public readonly static Dictionary<string, UserViewModel> _Users = new();

        private readonly OleMissChatboxContext _context;

        public ChatHub(OleMissChatboxContext context)
        {
            _context = context;
        }

        public void SetConnection(string currentEmail)
        {
            var user = _context.Users.Where(u => u.Email == currentEmail).FirstOrDefault();
            UserViewModel currentUser = new()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CurrentClass = ""
            };
            _Users.Add(Context.ConnectionId, currentUser);

            if (!_Connections.Any(u => u.Email == user.Email))
            {
                _Connections.Add(currentUser);
            }
        }

        public async Task SetUserPermissionLevel(string currentEmail)
        {
            //Get usertype from database
            var user = _context.Users.Where(u => u.Email == currentEmail).FirstOrDefault();
            var userType = user.UserType;

            await Clients.Caller.SendAsync("SetUserPermissionLevel", userType);
        }

        public async Task SendMessage(string email, string message, string className)
        {
            try
            {
                var user = _context.Users.Where(u => u.Email == email).FirstOrDefault();
                var currentClass = _context.Classes.Where(c => c.ClassName == className).FirstOrDefault();
                if (!string.IsNullOrEmpty(message.Trim()))
                {
                    var msg = new ChatMessage()
                    {
                        MessageString = message,
                        ClassId = currentClass.ClassId,
                        MessageDate = DateTime.Now
                    };
                    _context.ChatMessages.Add(msg);
                    _context.SaveChanges();
                    var createdMessage = _context.ChatMessages.Where(cm => cm.MessageDate == msg.MessageDate).FirstOrDefault();

                    var userChatMessage = new UserChatMessage()
                    {
                        UserId = user.UserId,
                        ChatMessageId = createdMessage.ChatMessageId
                    };
                }
            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("onError", "Message not sent!");
            }

            await Clients.Group(className).SendAsync("ReceiveMessage", email, message, className);
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
                            CreatedDate = DateTime.Now,
                        };
                        _context.Classes.Add(newClass);
                        _context.SaveChanges();

                        var createdClass = _context.Classes.Where(cm => cm.CreatedDate == newClass.CreatedDate).FirstOrDefault();

                        var newUserClass = new UserClass()
                        {
                            ClassId = createdClass.ClassId,
                            UserId = user.UserId,
                            IsOwner = true,
                            RegistrationDate = DateTime.Now,
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
                _Users.TryGetValue(Context.ConnectionId, out var userViewModel);
                var user = _Connections.Where(u => u.Email == userViewModel.Email).FirstOrDefault();
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

            List<ChatMessage> chatMessages = _context.ChatMessages.Where(cm => cm.ClassId == currentClass.ClassId).ToList();

            chatMessages = chatMessages
                .OrderByDescending(cm => cm.MessageDate)
                .Take(50)
                .Reverse()
                .ToList();

            List<ChatMessageViewModel> chatMessageViewModels = new();

            foreach (ChatMessage chatMessage in chatMessages)
            {
                var userChatMessage = _context.UserChatMessages.Where(ucm => ucm.ChatMessageId == chatMessage.ChatMessageId).FirstOrDefault();
                var user = _context.Users.Where(u => u.UserId == userChatMessage.UserId).FirstOrDefault();

                chatMessageViewModels.Add(new ChatMessageViewModel
                {
                    MessageString = chatMessage.MessageString,
                    MessageSender = user.FirstName + " " + user.LastName,
                    MessageDate = chatMessage.MessageDate
                });
            }

            return chatMessageViewModels;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                _Users.TryGetValue(Context.ConnectionId, out var user);
                var connectedUser = _Connections.Where(u => u.Email == user.Email).First();
                _Connections.Remove(connectedUser);

                Clients.OthersInGroup(connectedUser.CurrentClass).SendAsync("removeUser", connectedUser);
            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
            }
            return base.OnDisconnectedAsync(exception);
        }

        private User GetCurrentUser(string email)
        {
            return _context.Users.Where(u => u.Email == email).FirstOrDefault();
        }
    }
}
