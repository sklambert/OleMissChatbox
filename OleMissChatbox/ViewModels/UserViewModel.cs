using System.ComponentModel.DataAnnotations;

namespace OleMissChatbox.ViewModels
{
    public class UserViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CurrentClass { get; set; }
    }
}
