using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotLibrary.Classes;
using BotLibrary.Classes.Bot;
using SIMSellerBot.Source.ChatStates;
using SIMSellerBot.Source.Constants;
using SIMSellerBot.Source.Db;
using SIMSellerBot.Source.Methods;
using SIMSellerTelegramBot.DataBase.Models;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace SIMSellerTelegramBot.Source.ChatStates
{
    class AnswerToUserMessage : ParentState
    {
        public AnswerToUserMessage(State state) : base(state)
        {

        }

        /// <summary>
        /// Вводит сообщение для другого пользователя
        /// </summary>
        public override Hop ProcessMessage(object userObj, TelegramBotClient bot, InboxMessage mes)
        {
            User user = userObj as User;
            if (Equals(user, null)) throw new Exception("Не определен пользователь!");

            switch (mes.Type)
            {
                case MessageType.Text:
                    string text = mes.Data as string;

                    //нажал на кнопку отменить
                    if (IsKeyboardCommand(text, mes.ChatId))
                    {
                        return this.State.HopOnFailure;
                    }

                    //Получаем данные (формат данных [senderChatId|senderName??null])
                    //Мне нужно только senderChatId
                    List<string> data = this.State.Data.Split('|')?.ToList();

                    if (data?.Count == 0)
                    {
                        throw new NullReferenceException("Не получил данные");
                    }

                    string strChatId = data[0];
                    long receiverChatId;
                    if (long.TryParse(strChatId, out receiverChatId) == false)
                    {
                        throw new Exception("Не могу преобразовать string to long!");
                    }
                    
                    BotMethods.SendMessageToUser(bot, receiverChatId, user, text);

                    Hop hop = this.State.HopOnSuccess.GetCopy();
                    hop.IntroductionString = Answer.AlreadySended;
                    return hop;
                    break;
                default:
                    
                    break;
            }


            return base.ProcessMessage(user, bot, mes);
        }


    }
}