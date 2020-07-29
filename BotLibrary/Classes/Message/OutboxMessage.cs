using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BotLibrary.Classes.Data;
using BotLibrary.Classes.Message;
using BotLibrary.Enums;
using BotLibrary.Interfaces;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotLibrary.Classes
{
    public class OutboxMessage
    {
        public OutboxMessageType Type { get; private set; }
        public object Data { get; private set; }
        public List<OutboxMessage> NestedElements { get; private set; }
        public IReplyMarkup ReplyMarkup { get; set; }
        public int ReplyToMessageId { get; set; }

        private ParseMode _parseMode = ParseMode.Default;
        public ParseMode ParseMode
        {
            get { return this._parseMode; }
            set { this._parseMode = value; }
        }

        #region Constructors

    
        public OutboxMessage()
        {
            this.NestedElements = new List<OutboxMessage>();
        }

        private OutboxMessage(object data) : this()
        {
            this.Data = data;
        }

        public OutboxMessage(MessagePhoto photo) : this((object) photo)
        {
            this.Type = OutboxMessageType.Photo;
        }

        public OutboxMessage(MessageDocument document) : this((object) document)
        {
            this.Type = OutboxMessageType.Document;
        }

        public OutboxMessage(MessageAudio Audio) : this((object) Audio)
        {
            this.Type = OutboxMessageType.Audio;
        }

        public OutboxMessage(MessageVoice Voice) : this((object)Voice)
        {
            this.Type = OutboxMessageType.Voice;
        }

        public OutboxMessage(string text) : this((object) text)
        {
            this.Type = OutboxMessageType.Text;
        }

        #endregion

        public OutboxMessage this[int index]
        {
            get
            {
                if (NestedElements.Count == 0) return null;

                if (index > NestedElements.Count || index<0)
                {
                    throw new Exception("Выход за границы списка!");
                }

                return this.NestedElements.ElementAt(index);
            }
            set
            {
                if (NestedElements == null || this.NestedElements.Count == 0)
                {
                    throw new Exception("Выход за границы списка!");
                }

                if (index > NestedElements.Count || index < 0)
                {
                    throw new Exception("Выход за границы списка!");
                }

                this.NestedElements[index] = value;
            }
        }



        public void AddMessageElement(OutboxMessage elem)
        {
            if (elem == null && elem.Data == null) return;
            
            if (NestedElements == null)
            {
                NestedElements = new List<OutboxMessage>();
            }

            this.NestedElements.Add(elem);
        }



        public void RemoveMessageElement(OutboxMessage elem)
        {
            if (NestedElements == null && NestedElements?.Count == 0) return;

            if (NestedElements.Contains(elem))
            {
                NestedElements.Remove(elem);
            }
        }


    }
}