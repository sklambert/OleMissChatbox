using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OleMissChatbox.Data.Entities
{
    public class UserClass
    {
        public int UserClassId { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int UserType { get; set; }
    }
}
