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
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = SIMSellerTelegramBot.DataBase.Models.User;

namespace SIMSellerTelegramBot.Source.ChatStates
{
    class User_Order_WishNumber : ParentState
    {
        private Regex regNumber = new Regex(@"(?<PhoneNumber>[\+]?[0-9]?[\s(]*?[0-9]{3}[\s)]*?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{2}?[-\s\.]?[0-9]{2})");

        public User_Order_WishNumber(State state) : base(state)
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
        /// Ввод желаемых номеров телефона
        /// </summary>
        /// <returns></returns>
        private Hop ProcessTextMessage(User user, TelegramBotClient bot, InboxMessage mes, string text)
        {
            //Пользователь вводит номера через новую строку, заменим все \n на &
            text = text.Replace('\n', '&');

            var numsMatches = regNumber.Matches(text);

            //Если не нашли ни один номер, вывести сообщение и попробовать снова.
            if(numsMatches.Count == 0)
            {
                bot.SendTextMessageAsync(mes.ChatId, Answer.AskInputNumberAgain);
                return null;
            }

            //Строку данных будем записывать в виде {#num01|num02|...|numN#}
            string dataStr = "#";

            foreach (Match match in numsMatches)
            {
                var curNum = match.Groups["PhoneNumber"].Value;

                curNum = curNum
                    .Replace(" ", null)
                    .Replace("(", null)
                    .Replace(")", null)
                    .Replace("-", null);

                //Меняем 8 на +7
                if (curNum.StartsWith("8"))
                {
                    curNum.Remove(0, 1);
                    curNum = "+7" + curNum;
                }

                //Если номер 10 значный, то сделаем его 11 значным
                if (curNum.Length == 10 && curNum.StartsWith("9"))
                {
                    curNum = "+7" + curNum;
                }

                dataStr += curNum + "|";
            }

            //Удалить последний слэш
            dataStr.Remove(dataStr.Length - 1);
            dataStr += "#";

            //Переходим на следующий уровень заполнения заявки.
            Hop hop = this.State.HopOnSuccess.GetCopy();
            hop.Data = dataStr;
            hop.Type = HopType.NextLevelHop;
            return hop;
        }
    }
}