using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotLibrary.Classes;
using BotLibrary.Classes.Bot;
using SIMSellerBot.Source.Methods;
using SIMSellerTelegramBot.DataBase.Models;
using SIMSellerBot.Source.ChatStates;
using SIMSellerBot.Source.Constants;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace SIMSellerTelegramBot.Source.ChatStates
{
    class User_Main : ParentState
    {
        public User_Main(State state) : base(state)
        {

        }

        /// <summary>
        /// Любое сообщение от пользователя
        /// </summary>
        public override Hop ProcessMessage(object userObj, TelegramBotClient bot, InboxMessage mes)
        {
            User user = userObj as User;
            if (Equals(user, null)) throw new Exception("Не определен пользователь!");

            switch (mes.Type)
            {
                case MessageType.Text:
                    string text = mes.Data as string;
                    if (IsKeyboardCommand(text, user))
                    {
                        return this.ProcessKeyboardCommand(user, bot, mes, text);
                    }

                    return ProcessTextMessage(user, bot, mes, text);
                    break;
                default:

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
                case Answer.BtnOrderNumber:
                    //Переход на состояние заказа номера
                    Hop hopOrder = new Hop()
                    {
                        NextStateName = "User_Order_WishNumber",
                    };
                    return hopOrder;
                    break;

                case Answer.BtnHelp:
                    //Переход в панель помощи
                    Hop hopHelp = new Hop()
                    {
                        NextStateName = "User_HelpPanel",
                    };
                    return hopHelp;
                    break;

                case Answer.BtnGoToManagerPanel:
                    //Переход в панель менеджера.
                    Hop hop = new Hop()
                    {
                        NextStateName = "Manager_Main",
                        Type = HopType.RootLevelHop,
                    };
                    return hop;
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
            Hop hop = this.State.HopOnSuccess.GetCopy();
            hop.IntroductionString = Answer.DontUnderstandYouforgotPressButton;

            return hop;
        }


    }
}