using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using SIMSellerTelegramBot.DataBase.Models;
using SIMSellerBot.DataBase.Models;

namespace SIMSellerTelegramBot.DataBase.Models
{
    public class User
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public bool? Active { get; set; }
        public string Role { get; set; } //(user/moderator/admin)
        public bool? IsNewsSubscriber { get; set; }
        public ChatState ChatState { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateTime { get; set; }

        public User()
        {
        }
    }

}
