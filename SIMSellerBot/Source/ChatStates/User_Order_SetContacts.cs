using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    class User_Order_SetContacts : ParentState
    {
        private Regex regNumber = new Regex(@"(?<PhoneNumber>[\+]?[0-9]{1}?[\s(]*?[0-9]{3}[\s)]*?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{2}?[-\s\.]?[0-9]{2})");


        public User_Order_SetContacts(State state) : base(state)
        {

        }

        /// <summary>
        /// ToDo Что ожидается от пользователя
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
                    if (IsKeyboardCommand(text, mes.ChatId))
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
            if (string.Equals(command, Answer.BtnCancelOrder))
            {
                Hop hop = new Hop()
                {
                    NextStateName = "User_Main",
                    IntroductionString = Answer.AlreadyRequestCancelled,
                };
                return hop;
            }

            return null;
        }

        /// <summary>
        /// Ввод контактных данных.
        /// </summary>
        /// <returns></returns>
        private Hop ProcessTextMessage(User user, TelegramBotClient bot, InboxMessage mes, string text)
        {
            

            var res = regNumber.Match(text);

            if(res.Success == false)
            {
                bot.SendTextMessageAsync(user.ChatId, Answer.AskInputNumberAgainForSetContacts);
                return null;
            }

            //Берем контактный номер
            var contactNumber = res.Value;

            //меняем 8 на +7
            if (contactNumber.StartsWith("8"))
            {
                contactNumber = "+7" + contactNumber.Remove(0, 1);
            }

            

            List<string> wishNumbers = this.State.Data.Trim('#').Split('|')?.ToList();

            if (Equals(wishNumbers, null) || wishNumbers.Count == 0)
            {
                throw new Exception($"В заявку должны быть переданы номера для покупки!\nВ этой строке должны быть номера [{this.State.Data}]");
            }

            //Создать заявку
            var newRequest = DbMethods.CreateNumberRequest(this.Db, user, wishNumbers, contactNumber);

            //Оповестить менеджеров о новой заявке
            if (Equals(newRequest, null) == false)
            {
                BotMethods.NotifyManagers(bot, user, 
                    Answer.GetInfoAboutRequestNumber(user, newRequest));
            }

            Hop hop = this.State.HopOnSuccess.GetCopy();
            hop.IntroductionString = Answer.AlreadyRequestSended;
            hop.Type = HopType.RootLevelHop;
            return hop;
        }
    }
}