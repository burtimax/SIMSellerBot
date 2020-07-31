using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotLibrary.Classes;
using BotLibrary.Classes.Bot;
using SIMSellerBot.Source.Methods;
using SIMSellerTelegramBot.DataBase.Models;
using JetBrains.Annotations;
using SIMSellerBot.DataBase.Models;
using SIMSellerBot.Source.Constants;
using Telegram.Bot.Types.ReplyMarkups;

namespace SIMSellerTelegramBot.Source.Constants
{
    public class Keyboards
    {
        /// <summary>
        /// Отменить
        /// </summary>
        public static MarkupWrapper<ReplyKeyboardMarkup> CancelOperationKeyboard =
            new MarkupWrapper<ReplyKeyboardMarkup>(true)
                .NewRow()
                .Add(Answer.BtnCancel);

        /// <summary>
        /// Свернуть
        /// </summary>
        public static MarkupWrapper<InlineKeyboardMarkup> InlineWrapMessage =
            new MarkupWrapper<InlineKeyboardMarkup>(true)
                .NewRow()
                .Add(Answer.Wrap, Answer.CallbackWrapThisMessage);

        /// <summary>
        /// Пропустить
        /// </summary>
        public static MarkupWrapper<ReplyKeyboardMarkup> ButtonSkipKeyboard = new MarkupWrapper<ReplyKeyboardMarkup>(true)
            .NewRow()
            .Add(Answer.BtnSkip);

        /// <summary>
        /// Да/Нет
        /// </summary>
        public static MarkupWrapper<ReplyKeyboardMarkup> KeyboardYesNo =
        new MarkupWrapper<ReplyKeyboardMarkup>(true)
                .NewRow()
                .Add(Answer.BtnNo)
                .Add(Answer.BtnYes);

        /// <summary>
        /// User_Main_Keyboard
        /// </summary>
        public static MarkupWrapper<ReplyKeyboardMarkup> User_Main_Keyboard =
            new MarkupWrapper<ReplyKeyboardMarkup>(true)
                .NewRow()
                .Add(Answer.BtnOrderNumber)
                .NewRow()
                .Add(Answer.BtnHelp);
        //.NewRow()
        //.Add(Answer.BtnAdditional);


        /// <summary>
        /// GoTo_Manager_Main_Keyboard
        /// </summary>
        public static MarkupWrapper<ReplyKeyboardMarkup> GoTo_Manager_Main_Keyboard =
            new MarkupWrapper<ReplyKeyboardMarkup>(true)
                .NewRow()
                .Add(Answer.BtnGoToManagerPanel);

        /// <summary>
        /// GoTo_User_Main_Keyboard
        /// </summary>
        public static MarkupWrapper<ReplyKeyboardMarkup> GoTo_User_Main_Keyboard =
            new MarkupWrapper<ReplyKeyboardMarkup>(true)
                .NewRow()
                .Add(Answer.BtnGoToUserPanel);

        /// <summary>
        /// Manager_Main_Keyboard
        /// </summary>
        public static MarkupWrapper<ReplyKeyboardMarkup> Manager_Main_Keyboard =
            new MarkupWrapper<ReplyKeyboardMarkup>(true)
                .NewRow()
                .Add(Answer.BtnRequests)
                .Add(Answer.BtnQuestions)
                .NewRow()
                .Add(Answer.BtnBroadcastNotification)
                .NewRow()
                .Add(Answer.BtnStatistics);


        /// <summary>
        /// Manager_Requests
        /// </summary>
        public static MarkupWrapper<ReplyKeyboardMarkup> Manager_Requests_Keyboard =
            new MarkupWrapper<ReplyKeyboardMarkup>(true)
                .NewRow()
                .Add(Answer.BtnNewRequests)
                .NewRow()
                .Add(Answer.BtnProcessedRequests)
                .NewRow()
                .Add(Answer.BtnGoBack);


        /// <summary>
        /// Отменить заявку
        /// </summary>
        public static MarkupWrapper<ReplyKeyboardMarkup> CancelOrderOperationKeyboard =
            new MarkupWrapper<ReplyKeyboardMarkup>(true)
                .NewRow()
                .Add(Answer.BtnCancelOrder);


        /// <summary>
        /// User_HelpPanel
        /// </summary>
        public static MarkupWrapper<ReplyKeyboardMarkup> UserHelpPanelKeyboard =
            new MarkupWrapper<ReplyKeyboardMarkup>(true)
                .NewRow()
                .Add(Answer.BtnFAQ)
                .NewRow()
                .Add(Answer.BtnQuestionToManager)
                .NewRow()
                .Add(Answer.BtnChatWithManager)
                .NewRow()
                .Add(Answer.BtnGoBack);


        /// <summary>
        /// Ответить (Inline Keyboard)
        /// </summary>
        public static MarkupWrapper<InlineKeyboardMarkup> AnswerInlineKeyboard(long chatId, string username = null)
        {
            return new MarkupWrapper<InlineKeyboardMarkup>(true)
                .NewRow()
                .Add(Answer.BtnAnswer, Answer.CallbackAnswerToMessage + chatId + "|" + username);
        }

        /// <summary>
        /// Ответить (Inline Keyboard)
        /// </summary>
        public static MarkupWrapper<InlineKeyboardMarkup> AnswerToQuestionInlineKeyboard(long chatId)
        {
            return new MarkupWrapper<InlineKeyboardMarkup>(true)
                .NewRow()
                .Add(Answer.BtnAnswer, Answer.CallbackAnswerToQuestion + chatId);
        }



        /// <summary>
        /// список заявок с пагинацией
        /// </summary>
        public static MarkupWrapper<InlineKeyboardMarkup> InlineRequests(List<NumberRequest> requests, int offset, int max, string requestCallback, string offsetCallBack)
        {
            MarkupWrapper<InlineKeyboardMarkup> inline = new MarkupWrapper<InlineKeyboardMarkup>(true);

            if (Equals(requests, null) || requests.Count == 0)
            {
                return null;
            }

            foreach(var r in requests)
            {
                inline = inline.NewRow().Add(r.CreateTime.ToString(), requestCallback + r.Id);
            }

            inline = inline.NewRow();

            if(offset > 0)
            {
                int prevOffset = Math.Max(offset - Constants.REQUESTS_FETCH, 0);
                inline = inline.Add(Answer.BtnArrowLeft, offsetCallBack + prevOffset);
            }

            if (max > Constants.REQUESTS_FETCH)
            {
                int curPage = offset / Constants.REQUESTS_FETCH + 1;
                inline = inline.Add(curPage.ToString());
            }

            if (offset + Constants.REQUESTS_FETCH < max)
            {
                int nextOffset = (offset + Constants.REQUESTS_FETCH);
                inline = inline.Add(Answer.BtnArrowRight, offsetCallBack + nextOffset);
            }

            inline = inline.NewRow().Add(Answer.Wrap, Answer.CallbackWrapThisMessage);

            return inline;
        }

        /// <summary>
        /// Список вопросов с пагинацией
        /// </summary>
        public static MarkupWrapper<InlineKeyboardMarkup> InlineQuestions(List<Question> questions, int offset, int max, string requestCallback, string offsetCallBack)
        {
            MarkupWrapper<InlineKeyboardMarkup> inline = new MarkupWrapper<InlineKeyboardMarkup>(true);

            if (Equals(questions, null) || questions.Count == 0)
            {
                return null;
            }

            foreach (var r in questions)
            {
                inline = inline.NewRow().Add(r.CreateTime.ToString(), requestCallback + r.Id);
            }

            inline = inline.NewRow();

            if (offset > 0)
            {
                int prevOffset = Math.Max(offset - Constants.REQUESTS_FETCH, 0);
                inline = inline.Add(Answer.BtnArrowLeft, offsetCallBack + prevOffset);
            }

            if (max > Constants.REQUESTS_FETCH)
            {
                int curPage = offset / Constants.REQUESTS_FETCH + 1;
                inline = inline.Add(curPage.ToString());
            }

            if (offset + Constants.REQUESTS_FETCH < max)
            {
                int nextOffset = (offset + Constants.REQUESTS_FETCH);
                inline = inline.Add(Answer.BtnArrowRight, offsetCallBack + nextOffset);
            }

            inline = inline.NewRow().Add(Answer.Wrap, Answer.CallbackWrapThisMessage);

            return inline;
        }

        /// <summary>
        /// Inline для new NumberRequest
        /// </summary>
        public static MarkupWrapper<InlineKeyboardMarkup> InlineForNewNumberRequest(User user, NumberRequest request)
        {
            MarkupWrapper<InlineKeyboardMarkup> inline = new MarkupWrapper<InlineKeyboardMarkup>(true);

            if(Equals(user, null) == false && 
               string.IsNullOrWhiteSpace(user.Username) == false)
            {
                inline = inline.NewRow().Add(Answer.GoToChat, url: Answer.GetChatLink(user.Username));
            }

            inline = inline
                .NewRow()
                .Add(Answer.BtnWriteToSender, Answer.CallbackAnswerToMessage + request.FromChatId)
                .NewRow()
                .Add(Answer.BtnSetNumberRequestProcessed, Answer.CallbackSetProcessedNumberRequest + request.Id)
                .NewRow()
                .Add(Answer.BtnGoBack, Answer.CallbackSetOffsetNewNumberRequestList + 0) //0 - offset
                .NewRow()
                .Add(Answer.Wrap, Answer.CallbackWrapThisMessage);

            return inline;
        }




        /// <summary>
        /// Inline для Processed NumberRequest
        /// </summary>
        public static MarkupWrapper<InlineKeyboardMarkup> InlineForProcessedNumberRequest(NumberRequest request)
        {
            MarkupWrapper<InlineKeyboardMarkup> inline = new MarkupWrapper<InlineKeyboardMarkup>(true);

            inline = inline
                .NewRow()
                .Add(Answer.ReturnToNewNumberRequestsList,
                Answer.CallbackReturnToNewNumberRequests + request.Id)
                .NewRow()
                .Add(Answer.Wrap, Answer.CallbackWrapThisMessage);
            return inline;
        }



        /// <summary>
        /// Inline для Question
        /// </summary>
        public static MarkupWrapper<InlineKeyboardMarkup> InlineForQuestion(User questionSender, Question question)
        {
            MarkupWrapper<InlineKeyboardMarkup> inline = new MarkupWrapper<InlineKeyboardMarkup>(true);

            inline = inline
                .NewRow();

            if (string.IsNullOrWhiteSpace(questionSender?.Username) == false)
            {
                inline = inline
                    .Add(Answer.GoToChat, url: Answer.GetChatLink(questionSender.Username));
            }

            inline = inline
                .Add(Answer.BtnAnswer, Answer.CallbackAnswerToMessage + questionSender.ChatId)
                .NewRow()
                .Add(Answer.BtnSetQuestionProcessed, Answer.CallbackSetProcessedQuestion + question.Id)
                .NewRow()
                .Add(Answer.BtnGoBack, Answer.CallbackSetOffsetQuestionList + 0)
                .Add(Answer.Wrap, Answer.CallbackWrapThisMessage);
            return inline;
        }











        /// <summary>
            /// Определяем текущую панель пользователя
            /// </summary>
            /// <param name="user"></param>
            /// <param name="state"></param>
            /// <returns></returns>
            public static MarkupWrapper<ReplyKeyboardMarkup> GetCurrentUserKeyboard(DataBase.Models.User user, State state)
        {
            ///Панели бывают как динамические, так и статические.
            ///Динамические изменяются в процессе, это зависит от статуса пользователя.
            ///Динамические панели в приоритете над статическими.

            if (string.Equals(state.Name, "MainUser") == true && 
                (user.Role == Constants.ROLE_MANAGER))
            {
                return Keyboards.GoTo_Manager_Main_Keyboard;
            }

            //if (string.Equals(state.Name, "MainUser") == true && user.Role == Constants.ROLE_MODERATOR)
            //{
            //    return Keyboards.MainUserKeyboardWithModeratorButton;
            //}

            if (Equals(state.DynamicReplyKeyboard, null) == false)
            {
                return state.DynamicReplyKeyboard;
            }

            return state.ReplyKeyboard;
        }

    }
}
