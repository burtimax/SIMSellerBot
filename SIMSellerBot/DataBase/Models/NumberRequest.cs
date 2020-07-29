using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMSellerBot.DataBase.Models
{
    public class NumberRequest
    {
        public int Id { get; set; }
        public long FromChatId { get; set; }
        public string WishNumber { get; set; }
        public string Note { get; set; }
        public string Contacts { get; set; }
        public string Status { get; set; }
        public DateTime CreateTime { get; set; }
        public long? RespondentChatId { get; set; }
        public string RespondentNote { get; set; }
        public DateTime? RespondTime { get; set; }
    }
}
