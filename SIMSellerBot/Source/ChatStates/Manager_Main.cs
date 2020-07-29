using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotLibrary.Classes;
using BotLibrary.Classes.Bot;
using SIMSellerBot.DataBase.Models;
using SIMSellerBot.Source.Methods;
using SIMSellerBot.Source.ChatStates;
using SIMSellerBot.Source.Constants;
using SIMSellerTelegramBot.Source.Constants;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Message = SIMSellerBot.DataBase.Models.Message;
using User = SIMSellerTelegramBot.DataBase.Models.User;

namespace SIMSellerTelegramBot.Source.ChatStates
{
    class Manager_Main : ParentState
    {
        public Manager_Main(State state) : base(state)
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
            switch (command)
            {
                case Answer.BtnRequests:
                    Hop hopReq = new Hop()
                    {
                        NextStateName = "Manager_Requests",
                        Type = HopType.RootLevelHop,
                    };
                    return hopReq;
                    break;

                case Answer.BtnQuestions:
                    return ShowQuestions(user, bot, mes);
                    break;

                case Answer.BtnStatistics:

                    break;

                case Answer.BtnBroadcastNotification:
                    Hop hop = new Hop()
                    {
                        NextStateName = "Manager_BroadcastMessage",
                    };
                    return hop;
                    break;

                case Answer.BtnGoToUserPanel:
                    Hop hopToUser = new Hop()
                    {
                        NextStateName = "User_Main",
                        Type = HopType.RootLevelHop,
                    };
                    return hopToUser;
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



            return this.State.HopOnSuccess;
        }


        public override Hop ProcessCallback(object userObj, TelegramBotClient bot, InboxMessage mes,
            CallbackQuery callback, string data)
        {
            User user = userObj as User;

            //Поменять страницу пагинации в списке вопросов
            if (data.StartsWith(Answer.CallbackSetOffsetQuestionList))
            {
                int offset = Convert.ToInt32(data.Replace(Answer.CallbackSetOffsetQuestionList, ""));
                return ShowQuestions(user, bot, mes, offset, callback.Message.MessageId);
            }

            //Показать информацию о вопросе
            if (data.StartsWith(Answer.CallbackShowQuestionById))
            {
                int questionId = Convert.ToInt32(data.Replace(Answer.CallbackShowQuestionById, ""));
                return ShowOneQuestion(user, bot, mes, questionId, callback.Message.MessageId);
            }

            if (data.StartsWith(Answer.CallbackSetProcessedQuestion))
            {
                int questionId = Convert.ToInt32(data.Replace(Answer.CallbackSetProcessedQuestion, ""));
                //Установить пометку, что вопрос обработан
                DbMethods.CloseQuestionById(this.Db, questionId);
                bot.AnswerCallbackQueryAsync(callback.Id, Answer.AlreadyQuestionClosed);
                //Открыть список вопросов
                ShowQuestions(user, bot, mes, 0, callback.Message.MessageId);
            }

            return base.ProcessCallback(userObj, bot, mes, callback, data);
        }



        private Hop ShowQuestions(User user, TelegramBotClient bot, InboxMessage mes, int offset = 0,
            int messageId = -1)
        {
            var res = DbMethods.GetOpenQuestionsSafe(this.Db, offset, Constants.Constants.REQUESTS_FETCH);

            var inline = Keyboards.InlineQuestions(res.list,
                res.offset,
                res.maxRequests,
                Answer.CallbackShowQuestionById,
                Answer.CallbackSetOffsetQuestionList);

            if (Equals(inline, null))
            {
                bot.SendTextMessageAsync(mes.ChatId, Answer.NoQuestions);
            }
            else
            {
                if (messageId == -1)
                {
                    bot.SendTextMessageAsync(mes.ChatId, Answer.ChooseQuestion, replyMarkup: inline.Value);
                }
                else
                {
                    bot.EditMessageTextAsync(mes.ChatId, messageId, Answer.ChooseQuestion,
                        replyMarkup: inline.Value as InlineKeyboardMarkup);
                }

            }

            return null;
        }

        /// <summary>
        /// Показать один вопрос и инлайн
        /// </summary>
        /// <returns></returns>
        private Hop ShowOneQuestion(User user, TelegramBotClient bot, InboxMessage mes, int questionId,
            int messageId = -1)
        {
            //Скрыть список вопросов и показать один вопрос с кнопками (ответить/назад/свернуть)
            Question q = DbMethods.GetQuestionById(this.Db, questionId);
            if(Equals(q, null))
            {
                throw new Exception("Question is NULL. ID=" + questionId);
            }

            User sender = DbMethods.GetUserByChatId(this.Db, q.FromChatId);
            string questionInfo = Answer.GetInfoAboutQuestion(sender, q);
            var inline = Keyboards.InlineForQuestion(sender, q);
            if (messageId == -1)
            {
                bot.SendTextMessageAsync(mes.ChatId, questionInfo, replyMarkup: inline.Value);
            }
            else
            {
                bot.EditMessageTextAsync(mes.ChatId, mes.MessageId, questionInfo,
                    replyMarkup: inline.Value as InlineKeyboardMarkup);
            }

            return null;
        }
    }
}