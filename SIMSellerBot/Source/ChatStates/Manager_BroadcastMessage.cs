using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotLibrary.Classes;
using BotLibrary.Classes.Bot;
using SIMSellerBot.Source.Methods;
using SIMSellerBot.Source.ChatStates;
using SIMSellerBot.Source.Constants;
using SIMSellerBot.Source.Db;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = SIMSellerTelegramBot.DataBase.Models.User;

namespace SIMSellerTelegramBot.Source.ChatStates
{
    class Manager_BroadcastMessage : ParentState
    {
        public Manager_BroadcastMessage(State state) : base(state)
        {

        }

        /// <summary>
        /// Менеджер вводит сообщение (оповещение для всех пользователей)
        /// </summary>
        public override Hop ProcessMessage(object userObj, TelegramBotClient bot, InboxMessage mes)
        {
            User user = userObj as User;
            if (Equals(user, null)) throw new Exception("Не определен пользователь!");

            switch (mes.Type)
            {
                //Обработка текстового сообщения
                case MessageType.Text:
                    string text = mes.Data as string;
                    if (IsKeyboardCommand(text, user))
                    {
                        return this.ProcessKeyboardCommand(user, bot, mes, text);
                    }

                    return ProcessTextMessage(user, bot, mes, text);
                    break;
                default:
                    //Понимаю только текстовые сообщения
                    DontUndertandMessage(bot, mes);
                    break;
            }


            return base.ProcessMessage(userObj, bot, mes);
        }

        /// <summary>
        /// Обработка нажатия на кнопку клавиатуры
        /// </summary>
        /// <returns></returns>
        private Hop ProcessKeyboardCommand(User user, TelegramBotClient bot, InboxMessage mes, string command)
        {
            if (string.Equals(command, Answer.BtnCancel))
            {
                Hop hop = this.State.HopOnFailure.GetCopy();
                hop.IntroductionString = Answer.AlreadyCancelled;
                return hop;
            }

            return null;
        }

        /// <summary>
        /// Обработка текстового сообщения
        /// </summary>
        /// <returns></returns>
        private Hop ProcessTextMessage(User user, TelegramBotClient bot, InboxMessage mes, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            BotMethods.SendBroadcastMessageToAllUsers(this.Db, bot, text, user.ChatId);

            Hop hopSuc = this.State.HopOnSuccess.GetCopy();
            hopSuc.IntroductionString = Answer.AlreadySendedToAllUsers;

            return hopSuc;
        }
    }
}