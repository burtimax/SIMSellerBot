using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotLibrary.Classes;
using SIMSellerBot.DataBase.Models;
using SIMSellerTelegramBot.DataBase.Context;
using SIMSellerTelegramBot.DataBase.Models;
using SIMSellerTelegramBot.Source.Constants;
using SIMSellerBot.Source.Constants;
using SIMSellerBot.Source.Methods;
using SIMSellerTelegramBot;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace SIMSellerBot.Source.Db
{
    public static class BotMethods
    {
        ///Метод расширения для TelegramBotClient
        public static void WriteToSupport(this TelegramBotClient bot, User sender, string message)
        {
            //if (string.IsNullOrEmpty(message))
            //{
            //    return;
            //}

            //string mes = $"В службу поддержки:\n{sender.FirstName} {sender.LastName} [{sender.ChatId}]\n" + message;
            //bot.SendTextMessageAsync(SIMSellerTelegramBot.Source.Constants.Constants.SupportChatId, mes, replyMarkup:Keyboards.InlineAnswerToMessage(sender.ChatId,
            //    sender.FirstName + " " + sender.LastName).Value);
        }

        ///Метод расширения для TelegramBotClient
        public static void WriteToConsultant(this TelegramBotClient bot, User sender, User consultant, string message)
        {
            //if (string.IsNullOrEmpty(message))
            //{
            //    return;
            //}

            //if (consultant == null || consultant?.Role != "moderator")
            //{
            //    return;
            //}

            //string mes = $"[{sender.FirstName} {sender.LastName}]\n" + message;
            //bot.SendTextMessageAsync(consultant.ChatId, mes, replyMarkup:Keyboards.InlineAnswerToMessage(sender.ChatId,
            //    sender.FirstName + " " + sender.LastName).Value);
        }

        /// <summary>
        /// Отправить заявку менеджерам.
        /// </summary>
        public static void SendRequestToManagers(TelegramBotClient bot, User user, NumberRequest request)
        {

        }


        /// <summary>
        /// Уведомить менеджеров
        /// </summary>
        public static void NotifyManagers(TelegramBotClient bot,
            User user,
            string notification,
            MarkupWrapper<InlineKeyboardMarkup> inline = null)
        {
            List<User> managers = null;
            using (BotDbContext db = new BotDbContext(HelperDataBase.DB_OPTIONS))
            {
                managers = DbMethods.GetAllManagers(db);
                if (Equals(managers, null)) return;
            }

            foreach (var m in managers)
            {
                bot.SendTextMessageAsync(m.ChatId, 
                    notification, 
                    replyMarkup: inline?.Value ?? Keyboards.AnswerInlineKeyboard(user.ChatId, user.FirstName+" "+user.LastName).Value);
            }
        }

        /// <summary>
        /// Отправить сообщение (Если сообщение отправил менеджер, то не пишем имя и фамилию отправителя
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        public static void SendMessageToUser(TelegramBotClient bot, long receiverChatId, User sender, string text)
        {
            if (Equals(sender, null) || string.IsNullOrEmpty(text)) return;

            string username = null;

            if (sender.Role == SIMSellerTelegramBot.Source.Constants.Constants.ROLE_MANAGER)
            {
                username = $"Менеджер({sender.FirstName})";
            }
            else
            {
                username = $"{sender.FirstName} {sender.LastName}";

            }

            string textToSend = $"СООБЩЕНИЕ\n" +
                          $"От: {username}\n\n" +
                          $"\"{text}\"";

            bot.SendTextMessageAsync(receiverChatId, 
                textToSend, 
                replyMarkup:Keyboards.AnswerInlineKeyboard(sender.ChatId, username).Value);
        }

        /// <summary>
        /// Широковещательное сообщение для всех пользователей
        /// </summary>
        public static void SendBroadcastMessageToAllUsers(BotDbContext db, TelegramBotClient bot, string text, long exceptChatId = -1)
        {
            List<long> chats = DbMethods.GetAllUsersChatId(db);

            foreach (var chatId in chats)
            {
                if(chatId == exceptChatId) continue;
                bot.SendTextMessageAsync(chatId, text);
            }
        }

    }
}

