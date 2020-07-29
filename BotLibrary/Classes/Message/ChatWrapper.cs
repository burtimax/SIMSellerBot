using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace BotLibrary.Classes
{
    public class ChatWrapper
    {
        public Chat InnerChat;

        public string FirstName;
        public string LastName;
        public ChatId Id;
        
       

        public ChatWrapper(Chat chat)
        {
            this.InnerChat = chat;
            if (this.InnerChat != null)
            {
                Init();
            }
        }

        private void Init()
        {
            this.FirstName = this.InnerChat.FirstName;
            this.LastName = this.InnerChat.LastName;
            if (this.InnerChat.Id != null && this.InnerChat.Id > 0)
            {
                this.Id = new ChatId(this.InnerChat.Id);
            }
            else
            {
                this.Id = new ChatId(this.InnerChat.Username);
            }
        }

    }
}
