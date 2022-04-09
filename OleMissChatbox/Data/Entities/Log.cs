using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OleMissChatbox.Data.Entities
{
    public class Log
    {
        public int LogId { get; set; }
        public int LogSeverity { get; set; }
        public string LogMessage { get; set; }
        public DateTime LogDate { get; set; }
    }
}
