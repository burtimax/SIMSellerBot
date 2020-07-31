using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIMSellerTelegramBot.DataBase.Models;
using SIMSellerBot.DataBase.Models;

namespace SIMSellerBot.Source.Constants
{
    public class Answer
    {
        // TEXT
        public const string YouAreBlocked = "Вы были заблокированы администратором!";
        public const string NotActiveManagers = "Ссылка на чат с менеджером отсутствует!\nИспользуйте кнопку [Вопрос менеджеру] чтобы задать вопрос!";
        public const string FAQ = "Раздел FAQ пуст!";
        public const string Wrap = "Свернуть";
        public const string NoNewRequests = "Нет новых заявок!";
        public const string NoProcessedRequests = "Нет обработанных заявок!";
        public const string NoQuestions = "Нет вопросов от пользователей";
        public const string ChooseRequestForProcess = "Выберете заявку для обработки";
        public const string ChooseRequestForView = "Выберете заявку";
        public const string ChooseQuestion = "Выберете вопрос";
        public const string GoToChat = "Перейти в чат";
        public const string ReturnToNewNumberRequestsList = "Вернуть в список необработанных";
        public const string NoData = "Нет данных!";
        public const string DontUnderstandYouforgotPressButton = "Не понимаю!\nВозможно вы забыли нажать на кнопку.";

        //Ask
        public const string AskInputNumberAgain =
            "Вы не ввели номер(а) телефона, который хотите заказать.\nВведите номер(а) снова!\n(Например 89051234567)";
        public const string AskInputNumberAgainForSetContacts =
            "Номер телефона не распознан!\nВведите вашы контакты снова, чтобы менеджер мог связаться с вами.";

        public const string AskInputMessage = "Введите сообщение";

        //Introduction
        public const string IntroductionMainMenu = "Главное меню!";
        public const string IntroductionManagerRequests = "Заявки";
        public const string IntroductionManagerBroadcastMessage = "Введите сообщение для всех пользователей";
        public const string IntroductionHelpPanel = "Помощь";
        public const string IntroductionUserQuestion = "Введи свой вопрос?";

        public const string IntroductionOrderNumber =
            "Введите номер телефона (или несколько номеров), который хотите заказать!";

        public const string IntroductionSetContacts =
            "Как с вами связаться?\nВведите ваш номер телефона, чтобы менеджер мог связаться с вами и обработать заявку.";

        //Already
        public const string AlreadyRequestCancelled = "Заявка отменена";
        public const string AlreadyCancelled = "Отменено";
        public const string AlreadySended = "Отправлено";
        public const string AlreadySendedToAllUsers = "Отправлено всем пользователям!";
        public const string AlreadyRequestSended = "Заявка отправлена.\nМенеджер свяжется с вами в ближайшее время.";
        public const string AlreadyQuestionSended = "Вопрос отправлен.\nМенеджер ответит в ближайшее время!";
        public const string AlreadyNumberRequestProcessed = "Заявка обработана";
        public const string AlreadyReturnToNewNumberRequestsList = "Заявка перемещена в список необработанных";
        public const string AlreadyQuestionClosed = "Вопрос обработан";


        // BUTTON
        public const string BtnSkip = "Пропустить";
        public const string BtnGoBack = "Назад";
        public const string BtnYes = "Да";
        public const string BtnNo = "Нет";

        public const string BtnOrderNumber = "Заказать номер";
        public const string BtnHelp = "Помощь";
        public const string BtnAdditional = "Дополнительно";

        public const string BtnCancel = "Отменить";
        public const string BtnCancelOrder = "Отменить заявку";
        public const string BtnAnswer = "Ответить";

        public const string BtnFAQ = "FAQ";
        public const string BtnQuestionToManager = "Вопрос менеджеру";
        public const string BtnChatWithManager = "Чат с менеджером";

        public const string BtnRequests = "Заявки";
        public const string BtnQuestions = "Вопросы";
        public const string BtnStatistics = "Статистика";
        public const string BtnBroadcastNotification = "Рассылка для пользователей";
        public const string BtnGoToManagerPanel = "Переход в панель менеджера";
        public const string BtnGoToUserPanel = "Переход в панель пользователя";
        
        public const string BtnNewRequests = "Новые заявки";
        public const string BtnProcessedRequests = "Обработанные заявки";

        public const string BtnWriteToSender = "Написать отправителю!";
        public const string BtnSetNumberRequestProcessed = "Заявка обработана";
        public const string BtnSetQuestionProcessed = "Вопрос обработан";


        // EMOJI
        public const string EmojiWasteBasket = "🗑️";
        public const string EmojiPen = "🖊️";
        public const string BtnArrowRight = "►";
        public const string BtnArrowLeft = "◄";

        // CALLBACK
        public static string CallbackWrapThisMessage = "wrap_message";
        public static string CallbackAnswerToMessage = "answer_message_to_chat_id";
        public static string CallbackAnswerToQuestion = "answer_question_to_chat_id";
        public static string CallbackShowNewNumberRequestId = "show_new_number_request_id";
        public static string CallbackShowProcessedNumberRequestId = "show_processed_number_request_id";
        public static string CallbackShowQuestionById = "show_question_by_id";
        public static string CallbackSetOffsetNewNumberRequestList = "set_offset_new_number_request_list";
        public static string CallbackSetOffsetProcessedNumberRequestList = "set_offset_processed_number_request_list";
        public static string CallbackSetOffsetQuestionList = "set_offset_question_list";
        public static string CallbackSetProcessedNumberRequest = "set_processed_number_request";
        public static string CallbackSetProcessedQuestion = "set_processed_question";
        public static string CallbackReturnToNewNumberRequests = "return_to_new_number_requests";



        //Params String
        public static string AskInputMessageToUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return AskInputMessage;
            }
            return $"Введите сообщение для {username}";
        }

        public static string GetChatLink(string username)
        {
            return $"https://t.me/{username}";
        }

        /// <summary>
        /// Получить информацию по новой заявке
        /// </summary>
        /// <param name="user"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetInfoAboutRequestNumber(User requestSender, NumberRequest request)
        {
            if (Equals(requestSender, null) || Equals(request, null))
            {
                return null;
            }

            string res = null;

            if (request.Status == SIMSellerTelegramBot.Source.Constants.Constants.REQUEST_NUMBER_STATUS_OPEN)
            {
                res += "НОВАЯ ЗАЯВКА\n";
            }
            else
            {
                res += "ОБРАБОТАНО\n";
            }

            
            res += $"[{request.CreateTime}]\n" +
                   $"От [{requestSender.FirstName} {requestSender.LastName}]\n\n" +
                   $"Желаемые номера:\n";

            int c = 1;
            foreach (var num in request.WishNumber.Split('\n').ToList())
            {
                if (string.IsNullOrWhiteSpace(num)) continue;

                res += $"{c}) {num}\n";
                c++;
            }

            res += $"\nКонтакты для связи:\n{request.Contacts}";

            return res;
        }

        public static string NotifyNewUser(User user)
        {
            if (Equals(user, null))
            {
                return null;
            }

            return $"НОВЫЙ ПОЛЬЗОВАТЕЛЬ\n" +
                   $"[{user.FirstName} {user.LastName}]";
        }

        public static string GetInfoAboutQuestion(User sender, Question question)
        {
            if(Equals(sender, null) || Equals(question, null))
            {
                return Answer.NoData;
            }

            return $"ВОПРОС!\n" +
                   $"[{question.CreateTime.ToString()}]\n" +
                   $"От: {sender.FirstName} {sender.LastName}\n\n" +
                   $"\"{question.Text}\"";
        }
    }
}
