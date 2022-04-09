using System;

namespace OleMissChatbox.Data.Entities
{
    public class ChatMessage
    {
        public int ChatMessageId { get; set; }
        public string MessageString { get; set; }
        public DateTime MessageDate { get; set; }
    }
}
