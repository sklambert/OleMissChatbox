using System;

namespace OleMissChatbox.Data.Entities
{
    public class UserClass
    {
        public int UserClassId { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public bool IsOwner { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
