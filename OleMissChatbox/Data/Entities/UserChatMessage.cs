using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OleMissChatbox.Data.Entities
{
    public class UserChatMessage
    {
        public int UserChatMessageId { get; set; }
        public int UserId { get; set; }
        public int MessageId { get; set; }
        public int IsSender { get; set; }
    }
}
