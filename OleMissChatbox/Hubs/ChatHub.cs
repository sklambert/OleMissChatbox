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
        private readonly static List<ClassViewModel> _Classes = new List<ClassViewModel>();

        private readonly OleMissChatboxContext _context;

        public ChatHub(OleMissChatboxContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task CreateClass(string currentEmail, string className)
        {
            try
            {
                var user = _context.Users.Where(u => u.Email == currentEmail).FirstOrDefault();
                
                if (user.UserType != 1 && user.UserType != 2)
                {
                    await Clients.Caller.SendAsync("onError", "You do not have permission to create classes!");
                }

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
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "Couldn't create class: " + ex.Message);
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
    }
}
