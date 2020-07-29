using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BotLibrary.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace BotLibrary.Classes.Data
{
    public class MessageVoice : MessageData
    {

        public MessageVoice(FileData file)
        {
            this.File = file;
        }

        public MessageVoice()
        {
        }

        public MessageVoice(string filePath)
        {
            if (System.IO.File.Exists(filePath) == false)
            {
                throw new Exception("File not found!!!");
            }

            
            this.File = new FileData();
            this.File.Data = System.IO.File.ReadAllBytes(filePath);
            this.File.Info = new File();
            this.File.Info.FilePath = filePath;
            this.File.Info.FileSize = this.File.Data.Length;
        
        }
    }
}