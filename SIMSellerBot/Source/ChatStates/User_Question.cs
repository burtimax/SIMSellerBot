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
using SIMSellerTelegramBot.Source.Constants;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = SIMSellerTelegramBot.DataBase.Models.User;

namespace SIMSellerTelegramBot.Source.ChatStates
{
    class User_Question : ParentState
    {
        public User_Question(State state) : base(state)
        {

        }

        /// <summary>
        /// Пользователь должен ввести вопрос для менеджера
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
            if(string.Equals(command, Answer.BtnCancel))
            {
                Hop hop = this.State.HopOnFailure.GetCopy();
                hop.IntroductionString = Answer.AlreadyCancelled;
                return hop;
            }

            return null;
        }

        /// <summary>
        /// Пользователь вводит вопрос для менеджера.
        /// </summary>
        /// <returns></returns>
        private Hop ProcessTextMessage(User user, TelegramBotClient bot, InboxMessage mes, string text)
        {
            //Добавить в базу данных вопрос.
            var question = DbMethods.CreateQuestion(this.Db, user, text);
            //Отправить вопрос менеджерам.
            if (Equals(question, null) == false)
            {
                BotMethods.NotifyManagers(bot, 
                    user, 
                    Answer.GetInfoAboutQuestion(user, question)
                    /*Keyboards.AnswerToQuestionInlineKeyboard(user.ChatId)*/);
            }

            Hop hopSuc = this.State.HopOnSuccess.GetCopy();
            hopSuc.IntroductionString = Answer.AlreadyQuestionSended;
            return hopSuc;
        }
    }
}