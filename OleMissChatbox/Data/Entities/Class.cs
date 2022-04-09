using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OleMissChatbox.Data.Entities
{
    public class Class
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
