using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BotLibrary.Classes.Data;
using BotLibrary.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace BotLibrary.Classes.Message
{
    public class MessageAudio : MessageData
    {

        public string Title { get; set; }
        public InputMedia Thumb { get; set; }

        public MessageAudio(FileData file)
        {
            this.File = file;
        }

        public MessageAudio()
        {

        }


        public MessageAudio(string filePath)
        {
            if (System.IO.File.Exists(filePath) == false)
            {
                throw new Exception("File not found!!!");
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                this.File = new FileData();
                this.File.Data = System.IO.File.ReadAllBytes(filePath);
                this.File.Info = new File();
                this.File.Info.FilePath = filePath;
                this.File.Info.FileSize = this.File.Data.Length;
            }
        }

    }
}