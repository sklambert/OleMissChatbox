using System;

namespace OleMissChatbox.Data.Entities
{
    public class UserChatMessage
    {
        public int UserChatMessageId { get; set; }
        public int UserId { get; set; }
        public int ChatMessageId { get; set; }
        // public int IsSender { get; set; }
    }
}
