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
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = SIMSellerTelegramBot.DataBase.Models.User;

namespace SIMSellerTelegramBot.Source.ChatStates
{
    class User_HelpPanel : ParentState
    {
        public User_HelpPanel(State state) : base(state)
        {

        }

        /// <summary>
        /// Нажатие на кнопку панели помощи / сообщений не ожидается.
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
            switch (command)
            {
                case Answer.BtnFAQ:
                    bot.SendTextMessageAsync(mes.ChatId, Answer.FAQ);
                    break;

                case Answer.BtnQuestionToManager:
                    Hop hopQuestion = new Hop()
                    {
                        NextStateName = "User_Question",
                        Type = HopType.RootLevelHop,
                    };
                    return hopQuestion;
                    break;

                case Answer.BtnChatWithManager:
                    bot.SendTextMessageAsync(mes.ChatId, DbMethods.GetRandomManagerChatLink(this.Db));
                    break;
               
                case Answer.BtnGoBack:
                    Hop hopBack = new Hop()
                    {
                        NextStateName = "User_Main",
                        Type = HopType.RootLevelHop,
                    };
                    return hopBack;
                    break;
            }

            return null;
        }

        /// <summary>
        /// Обработка текстового сообщения
        /// </summary>
        /// <returns></returns>
        private Hop ProcessTextMessage(User user, TelegramBotClient bot, InboxMessage mes, string text)
        {
            Hop hop = new Hop()
            {
                NextStateName = "User_HelpPanel",
                IntroductionString = Answer.DontUnderstandYouforgotPressButton,
                Type = HopType.CurrentLevelHop,
            };


            return hop;
        }
    }
}