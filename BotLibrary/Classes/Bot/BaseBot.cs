using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BotLibrary.Classes.Data;
using BotLibrary.Interfaces;
using DiaryClassLibStandart.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace BotLibrary.Classes
{
    public class BaseBot : IBot
    {
        private TelegramBotClient _bot;

        #region Constructors
        protected TelegramBotClient Bot
        {
            get { return this._bot; }
            private set { this._bot = value; }
        }

        public BaseBot(string token) : this()
        {
            this.Bot = new TelegramBotClient(token);
        }

        public BaseBot (TelegramBotClient bot)
        {
            this.Bot = bot;
        }

        private BaseBot()
        {
            
        }
        #endregion

        /// <summary>
        /// В случае если нужно поработать с объектом TelegramBotClient.
        /// </summary>
        public TelegramBotClient TelegramClient
        {
            get { return this.Bot; }
        }

            /// <summary>
        /// Запускаем бота 
        /// </summary>
        public void Start() 
        {
            if (Bot.IsReceiving)
            {
                return;
            }
            this.Bot.StartReceiving(new UpdateType[]
            {
                UpdateType.Message,
                UpdateType.CallbackQuery,
            });
        }

        /// <summary>
        /// останавливаем бота
        /// </summary>
        public void Stop()
        {
            if (Bot.IsReceiving == false)
            {
                return;
            }
            this.Bot.StopReceiving();
        }



        #region Methods
        //public void SendTextMessage

        public bool SaveTelegramFile(FileData file, string filePath)
        {
            if(file == null || string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            HelperFileName.ParsePath(filePath, out var dir, out var filename, out var extension);
            
            if (Directory.Exists(dir) == false)
            {
                return false;
            }

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                File.Create(filePath).WriteAsync(file.Data, 0, file.Data.Length);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

    }
}