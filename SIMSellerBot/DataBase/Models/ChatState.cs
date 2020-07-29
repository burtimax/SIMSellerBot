using System;
using System.Collections.Generic;
using System.Text;

namespace SIMSellerTelegramBot.DataBase.Models
{
    public class ChatState
    {
        public int id { get; set; }
        public long ChatId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string State { get; set; }
    }
}
