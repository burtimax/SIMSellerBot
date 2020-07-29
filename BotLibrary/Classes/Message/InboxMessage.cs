using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BotLibrary.Classes.Data;
using BotLibrary.Classes.Message;
using BotLibrary.Enums;
using BotLibrary.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace BotLibrary.Classes
{
    public class InboxMessage
    {
        public Telegram.Bot.Types.Message BaseMessage;

        public long ChatId;

        public ChatWrapper Chat;
        public MessageType Type;
        public int MessageId;
        private object _data = null;

        //Данные в сообщении (фото, текст, аудио и прочее)
        public object Data
        {
            get
            {
                if (_data == null)
                {
                    if (this.BaseMessage == null) return null;

                    _data = GetData(this.BaseMessage);
                }

                return _data;
            }
        }

        public TelegramBotClient Bot { get; private set; }

        public InboxMessage(TelegramBotClient botClient, Telegram.Bot.Types.Message baseMessage)
        {
            if (baseMessage == null) return;
            this.Bot = botClient;
            this.BaseMessage = baseMessage;
            Init(this.BaseMessage);
            
        }

        /// <summary>
        /// Initialize InboxMessage
        /// </summary>
        /// <param name="mes"></param>
        private void Init(Telegram.Bot.Types.Message mes)
        {
            this.Chat = new ChatWrapper(BaseMessage.Chat);
            this.ChatId = this.Chat.Id.Identifier;
            this.MessageId = mes.MessageId;
            this.Type = BaseMessage.Type;
        }

        

        /// <summary>
        /// Initialize Message Data (Text, Photo, Audio, Document and ect.)
        /// </summary>
        /// <param name="mes"></param>
        private object GetData(Telegram.Bot.Types.Message mes)
        {
            object data = null;

            if (mes == null) return data;

            switch (mes.Type)
            {
                case MessageType.Text:
                    data = mes.Text;
                    break;
                case MessageType.Audio:
                    data = mes.Audio;
                    break;
                case MessageType.Document:
                    //ToDo Init MessageDocument object
                    //this.Data = 
                    break;
                case MessageType.Photo:
                    data = GetMessagePhoto(PhotoQuality.High);
                break;
            }

            return data;

        }

        /// <summary>
        /// Получаем фотку из сообщения.
        /// </summary>
        /// <param name="quality"></param>
        /// <returns></returns>
        public MessagePhoto GetMessagePhoto(PhotoQuality quality)
        {
            return HelperBot.GetPhoto(this.Bot, this.BaseMessage, quality);
        }


        public MessageAudio GetMessageAudio()
        {
            return HelperBot.GetAudio(this.Bot, BaseMessage);
        }


        public MessageVoice GetMessageVoice()
        {
            return HelperBot.GetVoice(this.Bot, BaseMessage);
        }

    }

}
