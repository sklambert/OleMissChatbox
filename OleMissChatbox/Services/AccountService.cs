using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using OleMissChatbox.Data;
using OleMissChatbox.Data.Entities;
using System;
using System.Text;

namespace OleMissChatbox.Services
{
    public class AccountService
    {
        private readonly IOleMissChatboxRepository _repo;
        private readonly string _salt = "V8PXmg3e9B";

        public AccountService(IOleMissChatboxRepository repo)
        {
            _repo = repo;
        }

        public bool RegisterUser(string firstname, string lastname, string email, string password)
        {
            return _repo.AddUser(firstname, lastname, email, HashPassword(password, Encoding.ASCII.GetBytes(_salt)));
        }

        public User GetAuthenticatedUser(string email, string password)
        {
            // hash & salt the password
            string hashedPassword = HashPassword(password, Encoding.ASCII.GetBytes(_salt));

            // call the context to query the matching email address' password value from the database
            var user = _repo.GetUserByEmail(email);

            if (hashedPassword == user.Password)
            {
                // compare the two
                // if match, then return user
                return user;
            }
            else
            {
                // if no match, then return null to indicate unauthorized
                return null;
            }
        }

        private static string HashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));
        }
    }
}
