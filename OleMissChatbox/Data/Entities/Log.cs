using System;

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
