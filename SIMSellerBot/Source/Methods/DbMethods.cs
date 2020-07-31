using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BotLibrary.Classes;
using SIMSellerTelegramBot;
using SIMSellerTelegramBot.DataBase.Context;
using SIMSellerTelegramBot.DataBase.Models;
using SIMSellerTelegramBot.Source.Constants;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SIMSellerBot.DataBase.Models;
using SIMSellerBot.Source.Constants;
using SIMSellerBot.Source.Db;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Message = SIMSellerBot.DataBase.Models.Message;
using User = SIMSellerTelegramBot.DataBase.Models.User;

namespace SIMSellerBot.Source.Methods
{
    public class DbMethods
    {
        private static void CheckDbContext(BotDbContext db)
        {
            if(Equals(db, null)) throw new NullReferenceException("DB is null!");
        }

        /// <summary>
        /// Добавить в базу User если не существует
        /// </summary>
        public static (User user, bool isNew) AddUserIfNeed(BotDbContext db, Telegram.Bot.Types.Message telegramMessage)
        {
            var user = GetUserByChatId(db, telegramMessage.Chat.Id);

            if (Equals(user, null))
            {
                return (AddUser(db, telegramMessage), true);
            }

            return (user, false);

        }

        /// <summary>
        /// Добавляем пользователя без проверки на существование
        /// </summary>
        /// <param name="db"></param>
        /// <param name="telegramMessage"></param>
        /// <returns></returns>
        public static User AddUser(BotDbContext db, Telegram.Bot.Types.Message telegramMessage)
        {
            //Создадим пользователя.
            User newUser = new User()
            {
                ChatId = telegramMessage.Chat.Id,
                Role = "user",
                Active = true,
                FirstName = telegramMessage.From.FirstName,
                LastName = telegramMessage.From.LastName,
                Username = telegramMessage.From.Username,
                IsNewsSubscriber = true,
            };
            db.Users.Add(newUser);


            //Создадим сущность ChatState
            ChatState chatState = new ChatState()
            {
                ChatId = telegramMessage.Chat.Id,
                UserId = newUser.Id,
                User = newUser,
            };
            db.ChatStates.Add(chatState);

            //Привяжем ChatState к пользователю
            newUser.ChatState = chatState;

            //Сохраняем все изменения и все добавленные сущности.
            db.SaveChanges();

            return newUser;
        }

        /// <summary>
        /// Получить пользователя из базы данных, не может вернуть Null, так как пользователь создается сразу же
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ChatId"></param>
        /// <returns></returns>
        [CanBeNull]
        public static User GetUserByChatId([NotNull] BotDbContext db, long ChatId)
        {
            CheckDbContext(db);

            return (from u in db.Users
                where u.ChatId == ChatId
                select u)?.FirstOrDefault();
        }

       

        /// <summary>
        /// Возвращает клавиатуру динамически в зависимости от уровня пользователя
        /// </summary>
        /// <param name="db"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public static MarkupWrapper<ReplyKeyboardMarkup> GetMainKeyboardforUser(string stateName, BotDbContext db, long chatId)
        {
            CheckDbContext(db);

            User user = (from u in db.Users
                where u.ChatId == chatId
                select u).First();

            return GetMainKeyboardforUser(stateName, db, user);
        }

        /// <summary>
        /// Возвращает клавиатуру динамически в зависимости от уровня пользователя и состояния
        /// </summary>
        public static MarkupWrapper<ReplyKeyboardMarkup> GetMainKeyboardforUser(string stateName, BotDbContext db, User user)
        {
            CheckDbContext(db);

            //if (user.ChatId == Constants.SupportChatId)
            //{
            //    return Keyboards.MainUserKeyboardWithAdminButton;
            //}
            if (string.Equals(stateName, "User_Main"))
            {
                switch (user.Role)
                {
                    case SIMSellerTelegramBot.Source.Constants.Constants.ROLE_USER:
                        return Keyboards.User_Main_Keyboard;
                        break;
                    case SIMSellerTelegramBot.Source.Constants.Constants.ROLE_MANAGER:
                        return Keyboards.GoTo_Manager_Main_Keyboard;
                        break;
                    //case SIMSellerTelegramBot.Source.Constants.Constants.ROLE_ADMIN:
                    //    return Keyboards.MainUserKeyboardWithAdminButton;
                    //    break;
                }
            }

            if (string.Equals(stateName, "Manager_Main"))
            {
                switch (user.Role)
                {
                    case SIMSellerTelegramBot.Source.Constants.Constants.ROLE_USER:
                        return Keyboards.GoTo_User_Main_Keyboard;
                        break;
                    case SIMSellerTelegramBot.Source.Constants.Constants.ROLE_MANAGER:
                        return Keyboards.Manager_Main_Keyboard;
                        break;
                }
            }

            return Keyboards.User_Main_Keyboard;

            return null;
        }

        /// <summary>
        /// Создать заявку в базу данных
        /// </summary>
        public static NumberRequest CreateNumberRequest(BotDbContext db, User user, List<String> wishNumbers, string userContacts)
        {
            if (Equals(db, null) ||
                Equals(user, null) ||
                Equals(wishNumbers, null) ||
                wishNumbers.Count == 0)
            {
                return null;
            }

            NumberRequest request = new NumberRequest()
            {
                Status = SIMSellerTelegramBot.Source.Constants.Constants.REQUEST_NUMBER_STATUS_OPEN,
                FromChatId = user.ChatId,
                Contacts = userContacts,
            };

            request.WishNumber = null;
            foreach (var num in wishNumbers)
            {
                request.WishNumber += num + "\n";
            }

            db.NumberRequests.Add(request);
            db.SaveChanges();
            return request;
        }


        /// <summary>
        /// Создать заявку в базу данных
        /// </summary>
        public static Question CreateQuestion(BotDbContext db, User user, string question)
        {
            if (Equals(user, null) || string.IsNullOrEmpty(question)) return null;

            Question q = new Question()
            {
                FromChatId = user.ChatId,
                Status = SIMSellerTelegramBot.Source.Constants.Constants.QUESTION_STATUS_OPEN,
                Text = question,
            };
            db.Questions.Add(q);
            db.SaveChanges();

            return q;
        }

        /// <summary>
        /// Получить список всех менеджеров.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static List<User> GetAllManagers(BotDbContext db)
        {
            return (from m in db.Users
                where m.Role == SIMSellerTelegramBot.Source.Constants.Constants.ROLE_MANAGER &&
                      m.Active == true
                select m)?.ToList() ?? null;
        }

        /// <summary>
        /// Получить ссылку на чат случайного менеджера.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static string GetRandomManagerChatLink(BotDbContext db)
        {
            var managers = GetAllManagers(db)?.Where(m=>string.IsNullOrEmpty(m.Username) == false).ToList();

            if (managers ==null || managers?.Count == 0)
            {
                return Answer.NotActiveManagers;
            }

            Random r = new Random(DateTime.Now.Millisecond);

            var selectedManager = managers.ElementAt(r.Next(0, managers.Count));

            return Answer.GetChatLink(selectedManager.Username);
        }

        /// <summary>
        /// Получить количество необработанных заявок
        /// </summary>
        public static int GetCountNewNumberRequests(BotDbContext db)
        {
            return (from r in db.NumberRequests
                where r.Status == SIMSellerTelegramBot.Source.Constants.Constants.REQUEST_NUMBER_STATUS_OPEN
                select r)?.Count() ?? 0;
        }

        /// <summary>
        /// Get slice of NumberRequest from db
        /// </summary>
        private static List<NumberRequest> GetNewNumberRequests(BotDbContext db, int offset, int fetch)
        {
            return (from r in db.NumberRequests
                where r.Status == SIMSellerTelegramBot.Source.Constants.Constants.REQUEST_NUMBER_STATUS_OPEN
                select r)?.Skip(offset)?.Take(fetch)?.ToList() ?? null;
        }

        /// <summary>
        /// Получить новые заявки
        /// </summary>
        /// <returns></returns>
        public static (List<NumberRequest>list, int offset, int maxRequests) GetNewNumberRequestsSafe(BotDbContext db, int offset, int fetch)
        {
            int max = GetCountNewNumberRequests(db);

            if (offset > max) offset = max - fetch;
            if (offset < 0) offset = 0;
            if (offset + fetch > max) fetch = max - offset;

            var list = GetNewNumberRequests(db, offset, fetch);
            //В обратном порядке, чтобы последние заявки стояли на первом месте
            list.Reverse();
            return (list, offset, max);
        }


        /// <summary>
        /// Получить количество обработанных заявок
        /// </summary>
        public static int GetCountProcessedNumberRequests(BotDbContext db)
        {
            return (from r in db.NumberRequests
                where r.Status != SIMSellerTelegramBot.Source.Constants.Constants.REQUEST_NUMBER_STATUS_OPEN
                select r)?.Count() ?? 0;
        }

        /// <summary>
        /// Get slice of processed NumberRequest from db
        /// </summary>
        private static List<NumberRequest> GetProcessedNumberRequests(BotDbContext db, int offset, int fetch)
        {
            if (fetch == 0) return new List<NumberRequest>();

            return (from r in db.NumberRequests
                where r.Status != SIMSellerTelegramBot.Source.Constants.Constants.REQUEST_NUMBER_STATUS_OPEN
                select r)?.Skip(offset)?.Take(fetch)?.ToList() ?? null;
        }

        /// <summary>
        /// Получить обработанные заявки
        /// </summary>
        /// <returns></returns>
        public static (List<NumberRequest> list, int offset, int maxRequests) GetProcessedNumberRequestsSafe(BotDbContext db, int offset, int fetch)
        {
            int max = GetCountProcessedNumberRequests(db);

            if (offset > max) offset = max - fetch;
            if (offset < 0) offset = 0;
            if (offset + fetch > max) fetch = max - offset;

            var list = GetProcessedNumberRequests(db, offset, fetch);
            //В обратном порядке, чтобы последние заявки стояли на первом месте
            list.Reverse();
            return (list, offset, max);
        }


        public static NumberRequest GetNumberRequestById(BotDbContext db, int requestId)
        {
            return db.NumberRequests.Find(requestId) ?? null;
        }

        /// <summary>
        /// Установить новый статус заявки
        /// </summary>
        /// <param name="db"></param>
        /// <param name="requestId"></param>
        /// <param name="newStatus"></param>
        public static void SetNumberRequestStatusById(BotDbContext db, int requestId, string newStatus)
        {
            var req = GetNumberRequestById(db, requestId);

            if (Equals(null, req))
            {
                return;
            }

            req.Status = newStatus;
            db.SaveChanges();
        }


        /// <summary>
        /// Получить количество открытых вопросов
        /// </summary>
        public static int GetCountOpenedQuestions(BotDbContext db)
        {
            return (from q in db.Questions
                where q.Status == SIMSellerTelegramBot.Source.Constants.Constants.QUESTION_STATUS_OPEN
                select q)?.Count() ?? 0;
        }

        /// <summary>
        /// Get slice of processed opened Question from db
        /// </summary>
        private static List<Question> GetOpenedQuestions(BotDbContext db, int offset, int fetch)
        {
            if (fetch == 0) return new List<Question>();

            return (from q in db.Questions
                where q.Status == SIMSellerTelegramBot.Source.Constants.Constants.QUESTION_STATUS_OPEN
                select q)?.Skip(offset)?.Take(fetch)?.ToList() ?? null;
        }

        /// <summary>
        /// Получить вопросы
        /// </summary>
        /// <returns></returns>
        public static (List<Question> list, int offset, int maxRequests) GetOpenQuestionsSafe(BotDbContext db, int offset, int fetch)
        {
            int max = GetCountOpenedQuestions(db);

            if (offset > max) offset = max - fetch;
            if (offset < 0) offset = 0;
            if (offset + fetch > max) fetch = max - offset;

            var list = GetOpenedQuestions(db, offset, fetch);
            //В обратном порядке, чтобы последние заявки стояли на первом месте
            list.Reverse();
            return (list, offset, max);
        }

        /// <summary>
        /// Получить вопрос по ID
        /// </summary>
        /// <param name="db"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public static Question GetQuestionById(BotDbContext db, int questionId)
        {
            return db.Questions.Find(questionId) ?? null;
        }

        //Закрыть вопрос по Id
        public static void CloseQuestionById(BotDbContext db, int questionId)
        {
            var q = GetQuestionById(db, questionId);

            if (Equals(q, null)) return;

            q.Status = SIMSellerTelegramBot.Source.Constants.Constants.QUESTION_STATUS_CLOSE;
            db.SaveChanges();
        }

        /// <summary>
        /// Получить чаты всех пользователей
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static List<long> GetAllUsersChatId(BotDbContext db)
        {
            return (from u in db.Users
                select u.ChatId)?.ToList() ?? new List<long>();
        }

        /// <summary>
        /// Установить роль пользователя
        /// </summary>
        /// <param name="db"></param>
        /// <param name="chatId"></param>
        /// <param name="role"></param>
        public static void SetUserRole(BotDbContext db, long chatId, string role)
        {
            User user = GetUserByChatId(db, chatId);

            if (Equals(user, null)) return;

            user.Role = role;
            db.SaveChanges();
        }


        /// <summary>
        /// Сохранить сообщение в базу данных
        /// </summary>
        public static void SaveMessage(BotDbContext db, InboxMessage mes)
        {
            if (mes.Type == MessageType.Text)
            {
                Message m = new Message()
                {
                    ChatId = mes.ChatId,
                    Text = mes.Data as string,
                };

                db.Messages.Add(m);
                db.SaveChanges();
            }
           
        }

        /// <summary>
        /// Получить статистику по боту
        /// </summary>
        public static string GetBotStatisticsString(BotDbContext db)
        {
            string res = null;
            res += $"Статистика бота:\n" +
                   $"Пользователи: [{GetCountUsers(db)}] чел.\n" +
                   $"Активность (сообщ.):\n" +
                   $"Все время: [{GetCountMessagesLastDays(db)}]\n" +
                   $"        10 дн.: [{GetCountMessagesLastDays(db, 10)}]\n" +
                   $"          3 дн.: [{GetCountMessagesLastDays(db, 3)}]\n" +
                   $"    Сегодня: [{GetCountMessagesLastDays(db, 1)}]\n";

            return res;
        }

        /// <summary>
        /// Количество сообщений за прошлые days дней
        /// </summary>
        /// <returns></returns>
        public static int GetCountMessagesLastDays(BotDbContext db, int days = -1)
        {
            if (days == -1)
            {
                return db.Messages?.Count() ?? 0;
            }

            DateTime startDate = DateTime.Now.Date.AddDays(-1 * days + 1).Date;

            return (from m in db.Messages
                where m.CreateTime.Date >= startDate
                select m)?.Count() ?? 0;
        }

        /// <summary>
        /// Количество пользователей бота
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static int GetCountUsers(BotDbContext db)
        {
            return db.Users?.Count() ?? 0;
        }
    }
}
