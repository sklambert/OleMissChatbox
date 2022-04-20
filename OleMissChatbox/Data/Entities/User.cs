using System;

namespace OleMissChatbox.Data.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
