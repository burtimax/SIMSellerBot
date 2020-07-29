using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIMSellerTelegramBot.DataBase.Models;

namespace SIMSellerBot.DataBase.Models
{
    public class Message
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string Text { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
        public DateTime CreateTime { get; set; }
    }
}
