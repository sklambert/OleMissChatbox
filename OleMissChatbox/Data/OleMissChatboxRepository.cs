using OleMissChatbox.Data.Entities;
using System.Linq;

namespace OleMissChatbox.Data
{
    public class OleMissChatboxRepository : IOleMissChatboxRepository
    {
        private readonly OleMissChatboxContext _context;

        public OleMissChatboxRepository(OleMissChatboxContext context)
        {
            _context = context;
        }

        public User GetUserByEmail(string email)
        {
            try
            {
                return _context.Users
                 .Where(u => u.Email == email)
                 .FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public bool AddUser(string firstname, string lastname, string email, string password, int usertype)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstname,
                    LastName = lastname,
                    Email = email,
                    Password = password,
                    UserType = usertype
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return true;
            }
            return false;
        }
    }
}
