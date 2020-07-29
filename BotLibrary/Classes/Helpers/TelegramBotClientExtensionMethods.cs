using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BotLibrary.Classes.Data;
using BotLibrary.Classes.Message;
using BotLibrary.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotLibrary.Classes.Helpers
{
    public static class TelegramBotClientExtensionMethods
    {
        public static void SendOutboxMessage(this TelegramBotClient bot, ChatId chatId, OutboxMessage message)
        {
            SendOutboxMessageToChat(bot, chatId, message);
        }
        public static void SendOutboxMessage(this TelegramBotClient bot, long id, OutboxMessage message)
        {
            SendOutboxMessageToChat(bot, new ChatId(id), message);
        }
        public static void SendOutboxMessage(this TelegramBotClient bot, string username, OutboxMessage message)
        {
            SendOutboxMessageToChat(bot, new ChatId(username), message);
        }

        private static void SendOutboxMessageToChat(this TelegramBotClient bot, ChatId chatId, OutboxMessage message)
        {
            
            if (bot == null ||
                ((chatId == null || chatId?.Identifier == 0) && 
                 string.IsNullOrEmpty(chatId?.Username)))
            {
                throw new ArgumentNullException();
            }

            switch (message.Type)
            {
                //Send Text
                case OutboxMessageType.Text:
                        bot.SendTextMessageAsync(
                        chatId: chatId, 
                        text: (string) message.Data, 
                        replyMarkup:message.ReplyMarkup,
                        parseMode: message.ParseMode,
                        replyToMessageId: message.ReplyToMessageId);
                    break;

                //Send MessagePhoto Entity
                case OutboxMessageType.Photo:
                    MessagePhoto photo = (MessagePhoto) message.Data; 
                    bot.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputOnlineFile(photo.File.Stream),
                        caption: photo.Caption,
                        replyToMessageId: message.ReplyToMessageId,
                        parseMode: message.ParseMode);
                    break;
                //Send MessageAudio Entity
                case OutboxMessageType.Audio:
                    MessageAudio audio = (MessageAudio)message.Data;
                    bot.SendAudioAsync(
                        chatId: chatId, 
                        audio: new InputOnlineFile(audio.File.Stream),
                        caption: audio.Caption,
                        replyMarkup: message.ReplyMarkup,
                        replyToMessageId: message.ReplyToMessageId,
                        thumb: audio.Thumb,
                        title: audio.Title,
                        parseMode: message.ParseMode);
                    break;

                //Send MessageVoice Entity
                case OutboxMessageType.Voice:
                    MessageVoice voice = (MessageVoice)message.Data; 
                    bot.SendVoiceAsync(
                        chatId: chatId, 
                        voice: new InputOnlineFile(voice.File.Stream), 
                        replyMarkup: message.ReplyMarkup,
                        replyToMessageId: message.ReplyToMessageId,
                        caption: voice.Caption,
                        parseMode: message.ParseMode);
                    break;
                //ToDo other Types of message!
                default:
                    throw new Exception("Не поддерживаемый тип отправки сообщений");
                    break;
            }
            
            //Рекурсивно вызываем отправку вложенных элементов сообщения.
            foreach (var item in message.NestedElements)
            {
                SendOutboxMessageToChat(bot, chatId, item);
            }
        }

    }
}
