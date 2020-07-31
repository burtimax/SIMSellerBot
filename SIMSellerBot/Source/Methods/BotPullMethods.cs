using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using SIMSellerTelegramBot;
using SIMSellerTelegramBot.DataBase.Context;
using SIMSellerTelegramBot.DataBase.Models;
using BotLibrary.Classes;
using BotLibrary.Classes.Bot;
using BotLibrary.Classes.Helpers;
using BotLibrary.Classes.StateControl;
using SIMSellerBot.Source.Methods;
using SIMSellerTelegramBot.Source.Constants;
using SIMSellerBot.Source.Constants;
using SIMSellerBot.Source.Db;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SIMSellerTelegramBot.Source.SIMSellerBot.Source.Methods
{
    public class BotPullMethods
    {
        /// <summary>
        /// Метод, для передачи в конструктор BotController
        /// Первичная обработка сообщения
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        public static (bool, Hop) BotPreProcessMessage(TelegramBotClient bot, State state, InboxMessage mes)
        {
            switch (mes.Type)
            {
                //Строковое сообщение
                case MessageType.Text:

                    using (BotDbContext db = new BotDbContext(HelperDataBase.DB_OPTIONS))
                    {
                        string text = mes.Data as string;

                        //Сохранить сообщение в базу данных
                        DbMethods.SaveMessage(db, mes);

                        //Если прислана команда [/start], просто обновить текущее состояние, вывести клавиатуру.
                        if (string.Equals((text), Constants.Constants.SPECIAL_COMMAND_START) == true)
                        {
                            if (string.IsNullOrEmpty(state?.Name) == false)
                            {
                                Hop hop = new Hop()
                                {
                                    NextStateName = state.Name,
                                };
                                return (false, hop);
                            }
                        }

                        //Если прислана команда [/wantbecomemanager]остояние, вывести клавиатуру.
                        if (string.Equals((text), Constants.Constants.SPECIAL_COMMAND_BECOME_MANAGER) == true)
                        {
                            DbMethods.SetUserRole(db, mes.ChatId, Constants.Constants.ROLE_MANAGER);
                        }

                        //Если прислана команда [/wantbecomeuser]остояние, вывести клавиатуру.
                        if (string.Equals((text), Constants.Constants.SPECIAL_COMMAND_BECOME_USER) == true)
                        {
                            DbMethods.SetUserRole(db, mes.ChatId, Constants.Constants.ROLE_USER);
                        }
                    }
                    break;
                default:

                    break;
            }


            return (true, null);
        }


        /// <summary>
        /// Перед обработкой сообщения проверяем, что пользователь есть в базе данных.
        /// </summary>
        /// <param name="chatId"></param>
        public static (bool userActive, object userObj) CheckUserBeforeMessageProcessing(TelegramBotClient bot, Telegram.Bot.Types.Message telegramMessage)
        {
            using (BotDbContext db = new BotDbContext(HelperDataBase.DB_OPTIONS))
            {
                DataBase.Models.User user = null;
                if (IsUserInDatabase(db, telegramMessage.Chat.Id, out user) == false)
                {
                    var res = AddUserToDb(db, telegramMessage);
                    user = res.user;

                    //Если это новый пользователь, то уведомим менеджеров о появлении нового пользователя.
                    if (res.isNew)
                    {
                        BotMethods.NotifyManagers(bot, res.user, Answer.NotifyNewUser(res.user));
                    }

                    return (true, user);
                }

               
                return (user.Active != false, user);
            }
        }

        /// <summary>
        /// Если пользователь не прошел проверку, наверное он заблокирован.
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="mes"></param>
        public static void OnFailUserCheck(TelegramBotClient bot, InboxMessage mes)
        {
            //Вывести сообщение, что пользователь заблокирован.
            bot.SendTextMessageAsync(mes.ChatId, Answer.YouAreBlocked);
        }


        /// <summary>
        /// Метод получения текущего состояния чата по ChatId
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public static State GetCurrentUserState(long chatId)
        {
            if (InitStates.BotStates?.Count == 0) return null;

            using (BotDbContext db = new BotDbContext(HelperDataBase.DB_OPTIONS))
            {
                //Выбираем состояние чата.
                ChatState chatState = 
                    (from state in db.ChatStates
                    where state.ChatId == chatId
                    select state).FirstOrDefault();

                string stateString = chatState.State;

                //Если состояние чата равно null
                if (string.IsNullOrEmpty(stateString?.Trim(' ')) == true)
                {
                    //То записать в пустую ячейку состояния имя первого состояния.
                    //ToDo Пока берем просто первый элемент из состояний, но нужно самому определять первый элемент.
                    stateString = InitStates.BotStates.FirstOrDefault().Name;

                    //ToDo изменить состояние(установить первое состояние, отправить приветствие, отправить клавиатуру через метод ChangeCurrentStateAndMakeHop
                    //Изменить состояние в базе данных и сохранить изменения
                    ChangeCurrentChatState(chatId, HopType.RootLevelHop, stateString);
                    //chatState.State = stateString;
                    //db.SaveChanges();
                }
                //Найти в общем пуле состояний состояние с таким же именем.
                StateController stateCtrl = new StateController(stateString);
                State resState =  InitStates.GetStateByName(stateCtrl.GetCurrentStateName());
                resState.Data = stateCtrl.Data;
                return resState;
            }

        }

        /// <summary>
        /// Поменять состояние.
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="state"></param>
        /// <param name="hop"></param>
        public static async void ChangeCurrentStateAndMakeHop(TelegramBotClient bot, long chatId, State state, Hop hop)
        {
            if (bot == null ||
                hop == null || 
                string.IsNullOrEmpty(hop.NextStateName)) return;

            //Отправить AnswerOnSuccess.
            if(state != null && string.IsNullOrEmpty(state.AnswerOnSuccess) == false)
            {
                await bot.SendTextMessageAsync(chatId, state.AnswerOnSuccess);
            }

            //Поменяем текущее состояние
            string currentStateName = ChangeCurrentChatState(chatId, hop.Type, hop.NextStateName, hop.Data);

            //Когда состояние в базе поменяли, тогда нужно выслать приветствие в новом состоянии и клавиатуру нового состояния.
            State st = InitStates.GetStateByName(currentStateName);

            if (Equals(hop.DynamicReplyKeyboard?.Value, null) == false)
            {
                st.DynamicReplyKeyboard = hop.DynamicReplyKeyboard;
            }

            //ToDo Убрать костыль (Сделал чтобы главная клавиатура менялась динамически в зависимости от уровня пользователя
            if (st.Name == "User_Main")
            {
                using (BotDbContext db = new BotDbContext(HelperDataBase.DB_OPTIONS))
                {
                    st.DynamicReplyKeyboard = DbMethods.GetMainKeyboardforUser(st.Name, db, chatId);
                }
            }
            if (st.Name == "Manager_Main")
            {
                using (BotDbContext db = new BotDbContext(HelperDataBase.DB_OPTIONS))
                {
                    st.DynamicReplyKeyboard = DbMethods.GetMainKeyboardforUser(st.Name, db, chatId);
                }
            }
            //ToDo здесь закончился костыль

            //Формируем приветственное сообщение (не забываем подключить клавиатуру)
            //Сначала смотрим приветственное сообщение, которое возможно было передано через Hop
            //Если через хоп не было передано приветственное сообщение, то берем приветственное сообщение из State
            OutboxMessage outMes = new OutboxMessage(hop.IntroductionString ?? st.IntroductionString ?? "");
            if (st.ReplyKeyboard != null)
            {
                outMes.ReplyMarkup = st.DynamicReplyKeyboard?.Value ?? st.ReplyKeyboard.Value;
            }
            else
            {
                outMes.ReplyMarkup = new ReplyKeyboardRemove();
            }

            bot.SendOutboxMessage(chatId, outMes);
            
        }


        /// <summary>
        /// Поменять текущее состояние в базе данных или содать это состояние.
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="hopType"></param>
        /// <param name="newStateName"></param>
        /// <return>Возвращает название текущего состояния</return>
        private static string ChangeCurrentChatState(long chatId, HopType hopType, string newStateName, string data = null)
        {
            using (BotDbContext db = new BotDbContext(HelperDataBase.DB_OPTIONS))
            {
                //Возьмем значение состояния из базы
                ChatState chatState =
                    (from s in db.ChatStates
                        where s.ChatId == chatId
                        select s).FirstOrDefault();

                string stateString = chatState?.State;

                //Если ячейка в базе пустая, то заполним её чем-нибудь и поменяем тип хопа чтобы потом переписать несусветицу.
                if (string.IsNullOrEmpty(stateString?.Trim(' ')))
                {
                    stateString = "___";
                    hopType = HopType.RootLevelHop;
                }

                //Поменять состояние в базе данных.
                StateController stateCtrl = new StateController(stateString);

                //Будем изменять значение состояния в зависимости от типа перехода состояния.
                switch (hopType)
                {
                    case HopType.NextLevelHop:
                        stateCtrl.AddStateAsNextState(newStateName, data);
                        break;
                    case HopType.CurrentLevelHop:
                        stateCtrl.ChangeCurrentStateName(newStateName, data);
                        break;
                    case HopType.RootLevelHop:
                        stateCtrl.SetRootState(newStateName, data);
                        break;
                    case HopType.BackToPreviosLevelHop:
                        stateCtrl.RemoveCurrentState();
                        break;
                }

                //получаем измененную строку состояния и сохраняем в базу
                string newStateString = stateCtrl.State;
                chatState.State = newStateString;
                db.SaveChanges();

                return stateCtrl.GetCurrentStateName();
            }
        }


        /// <summary>
        /// Проверка, есть ли пользователь в базе данных.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public static bool IsUserInDatabase(BotDbContext db, long chatId, out DataBase.Models.User user)
        {
            user =  
                (from u in db.Users
                where u.ChatId == chatId
                select u)?.FirstOrDefault();

            if (user == null)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Добавить пользователя в базу данных.
        /// Добавить все зависимые сущности, чтобы не проверять их потом на null.
        /// Аккуратно добавить все зависимости, внешние IDшники.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="chatId"></param>
        public static (User user, bool isNew) AddUserToDb(BotDbContext db, Telegram.Bot.Types.Message telegramMessage)
        {
            return DbMethods.AddUserIfNeed(db, telegramMessage);
        }

        /// <summary>
        /// Обработка всех ошибок здесь.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="bot"></param>
        public static void ProcessException(Exception e, TelegramBotClient bot)
        {
            string res = "ОШИБКА!!!\n\n";
            res += $"METHOD TARGET: {e.TargetSite.Name}\n\n" +
                   $"HRESULT: {e.HResult}\n\n" +
                   $"EXCEPTION MESSAGE: {e.Message}\n\n" +
                   $"STACK TRACE: {e.StackTrace}\n\n" +
                   $"";
            bot.SendTextMessageAsync(Constants.Constants.SupportChatId, res /*replyMarkup:Keyboards.InlineWrapMessage.Value*/);
        }

    }
}
