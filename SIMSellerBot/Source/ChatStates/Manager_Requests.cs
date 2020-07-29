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
using User = SIMSellerTelegramBot.DataBase.Models.User;

namespace SIMSellerTelegramBot.Source.ChatStates
{
    class Manager_Requests : ParentState
    {
        public Manager_Requests(State state) : base(state)
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
                case Answer.BtnNewRequests:
                    return ShowNewRequests(user, bot, mes);
                    break;

                case Answer.BtnProcessedRequests:
                    return ShowProcessedRequests(user, bot, mes);
                    break;

                case Answer.BtnGoBack:
                    Hop hopBack = new Hop()
                    {
                        NextStateName = "Manager_Main",
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



            return null;
        }

        public override Hop ProcessCallback(object userObj, TelegramBotClient bot, InboxMessage mes,
            CallbackQuery callback, string data)
        {
            User user = userObj as User;

            //Поменять страницу пагинации
            if (data.StartsWith(Answer.CallbackSetOffsetNewNumberRequestList))
            {
                int offset = Convert.ToInt32(data.Replace(Answer.CallbackSetOffsetNewNumberRequestList, ""));
                ShowNewRequests(user, bot, mes, offset, callback.Message.MessageId);
            }

            //Показать список обработанных заявок
            if (data.StartsWith(Answer.CallbackSetOffsetProcessedNumberRequestList))
            {
                int offset = Convert.ToInt32(data.Replace(Answer.CallbackSetOffsetProcessedNumberRequestList, ""));
                ShowProcessedRequests(user, bot, mes, offset, callback.Message.MessageId);
            }

            //Показать информацию по новой заявке
            if (data.StartsWith(Answer.CallbackShowNewNumberRequestId))
            {
                int reqId = Convert.ToInt32(data.Replace(Answer.CallbackShowNewNumberRequestId, ""));
                ShowNewNumberRequest(user, bot, mes, reqId, callback.Message.MessageId);
            }

            //Показать информацию по обработанной заявке
            if (data.StartsWith(Answer.CallbackShowProcessedNumberRequestId))
            {
                int reqId = Convert.ToInt32(data.Replace(Answer.CallbackShowProcessedNumberRequestId, ""));
                ShowProcessedNumberRequest(user, bot, mes, reqId, callback.Message.MessageId);
            }

            //Вернуть обработанную заявку в список необработанных
            if (data.StartsWith(Answer.CallbackReturnToNewNumberRequests))
            {
                int reqId = Convert.ToInt32(data.Replace(Answer.CallbackReturnToNewNumberRequests, ""));
                //Установить заявку необработанной
                DbMethods.SetNumberRequestStatusById(this.Db, reqId, Constants.Constants.REQUEST_NUMBER_STATUS_OPEN);
                bot.AnswerCallbackQueryAsync(callback.Id, Answer.AlreadyReturnToNewNumberRequestsList);
                //Удалить сообщение с заявкой
                bot.DeleteMessageAsync(mes.ChatId, callback.Message.MessageId);
            }

            //Установить заявку, как обработанную
            if (data.StartsWith(Answer.CallbackSetProcessedNumberRequest))
            {
                int reqId = Convert.ToInt32(data.Replace(Answer.CallbackSetProcessedNumberRequest, ""));
                //Установить заявку обработанной
                DbMethods.SetNumberRequestStatusById(this.Db, reqId, Constants.Constants.REQUEST_NUMBER_STATUS_SUCCESS);
                bot.AnswerCallbackQueryAsync(callback.Id, Answer.AlreadyNumberRequestProcessed);
                //Открыть список новых заявок
                ShowNewRequests(user, bot, mes, 0, callback.Message.MessageId);
            }

            return base.ProcessCallback(userObj, bot, mes, callback, data);
        }

        /// <summary>
        /// Показать InlineNewRequests
        /// </summary>
        /// <param name="user"></param>
        /// <param name="bot"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        private Hop ShowNewRequests(User user, TelegramBotClient bot, InboxMessage mes, int offset = 0,
            int messageId = -1)
        {
            var res = DbMethods.GetNewNumberRequestsSafe(this.Db, offset, Constants.Constants.REQUESTS_FETCH);

            var inline = Keyboards.InlineRequests(res.list,
                res.offset,
                res.maxRequests,
                Answer.CallbackShowNewNumberRequestId,
                Answer.CallbackSetOffsetNewNumberRequestList);

            if (Equals(inline, null))
            {
                bot.SendTextMessageAsync(mes.ChatId, Answer.NoNewRequests);
            }
            else
            {
                if (messageId == -1)
                {
                    bot.SendTextMessageAsync(mes.ChatId, Answer.ChooseRequestForProcess, replyMarkup: inline.Value);
                }
                else
                {
                    bot.EditMessageTextAsync(mes.ChatId, messageId, Answer.ChooseRequestForProcess,
                        replyMarkup: inline.Value as InlineKeyboardMarkup);
                }

            }

            return null;
        }


        /// <summary>
        /// Показать InlineProcessedRequests
        /// </summary>
        /// <param name="user"></param>
        /// <param name="bot"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        private Hop ShowProcessedRequests(User user, TelegramBotClient bot, InboxMessage mes, int offset = 0,
            int messageId = -1)
        {
            var res = DbMethods.GetProcessedNumberRequestsSafe(this.Db, offset, Constants.Constants.REQUESTS_FETCH);

            var inline = Keyboards.InlineRequests(res.list,
                res.offset,
                res.maxRequests,
                Answer.CallbackShowProcessedNumberRequestId,
                Answer.CallbackSetOffsetProcessedNumberRequestList);

            if (Equals(inline, null))
            {
                bot.SendTextMessageAsync(mes.ChatId, Answer.NoProcessedRequests);
            }
            else
            {
                if (messageId == -1)
                {
                    bot.SendTextMessageAsync(mes.ChatId, Answer.ChooseRequestForView, replyMarkup: inline.Value);
                }
                else
                {
                    bot.EditMessageTextAsync(mes.ChatId, messageId, Answer.ChooseRequestForView,
                        replyMarkup: inline.Value as InlineKeyboardMarkup);
                }

            }

            return null;
        }

        /// <summary>
        /// Показать данные по выбранной заявке
        /// </summary>
        /// <param name="user"></param>
        /// <param name="bot"></param>
        /// <param name="mes"></param>
        /// <param name="requestId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        private Hop ShowNewNumberRequest(User user, TelegramBotClient bot, InboxMessage mes, int requestId,
            int messageId = -1)
        {
            NumberRequest r = DbMethods.GetNumberRequestById(this.Db, requestId);

            if (Equals(r, null))
            {
                throw new Exception("NumberRequest == null. (ID)=" + requestId + " !");
            }

            var sender = DbMethods.GetUserByChatId(this.Db, r.FromChatId);
            var inline = Keyboards.InlineForNewNumberRequest(sender, r);

            string reqStr = Answer.GetInfoAboutRequestNumber(user, r);

            if (messageId == -1)
            {
                bot.SendTextMessageAsync(mes.ChatId, reqStr, replyMarkup: inline.Value);
            }
            else
            {
                bot.EditMessageTextAsync(mes.ChatId, messageId, reqStr,
                    replyMarkup: inline.Value as InlineKeyboardMarkup);
            }

            return null;
        }

        /// <summary>
        /// Показать информацию по обработанной заявке
        /// </summary>
        /// <param name="user"></param>
        /// <param name="bot"></param>
        /// <param name="mes"></param>
        /// <param name="requestId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        private Hop ShowProcessedNumberRequest(User user, TelegramBotClient bot, InboxMessage mes, int requestId,
            int messageId = -1)
        {
            NumberRequest r = DbMethods.GetNumberRequestById(this.Db, requestId);
            if (Equals(r, null))
            {
                throw new Exception("NumberRequest == null. (ID)=" + requestId + " !");
            }

            User sender = DbMethods.GetUserByChatId(this.Db, r.FromChatId);
            bot.SendTextMessageAsync(mes.ChatId, Answer.GetInfoAboutRequestNumber(sender, r), replyMarkup:Keyboards.InlineForProcessedNumberRequest(r).Value);

            return null;
        }
    }
}