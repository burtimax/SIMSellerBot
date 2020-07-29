using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotLibrary.Classes;
using BotLibrary.Classes.Bot;
using BotLibrary.Classes.Controller;
using SIMSellerBot.Source.Methods;
using SIMSellerTelegramBot;
using SIMSellerTelegramBot.DataBase.Context;
using SIMSellerTelegramBot.Source.Constants;
using SIMSellerBot.Source.Constants;
using SIMSellerBot.Source.Db;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = SIMSellerTelegramBot.DataBase.Models.User;

namespace SIMSellerBot.Source.ChatStates
{
    public class ParentState : BaseState
    {
        protected BotDbContext Db;

        public ParentState(State state) : base(state)
        {
            this.Db = new BotDbContext(HelperDataBase.DB_OPTIONS);
        }

        public override Hop ProcessMessage(object userObj, TelegramBotClient bot, InboxMessage mes)
        {
            return null;
        }

        /// <summary>
        /// Проверяем, является ли сообщение коммандой клавиатуры
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected bool IsKeyboardCommand(string command, SIMSellerTelegramBot.DataBase.Models.User user)
        {
            if (Equals(this.State, null)) throw new NullReferenceException("State is NULL!");

            var keyboard = Keyboards.GetCurrentUserKeyboard(user, this.State);

            //Не может являться командой, так как в состоянии нет даже клавиатуры
            if (Equals(keyboard, null)){
                return false;
            }

            //Не забываем, что динамическая клавиатура в приоритете на статической.
            foreach (var buttonText in (keyboard?.ButtonTexts()))
            {
                if (string.Equals(buttonText, command))
                {
                    return true;
                }
            }

            return false;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="chatId">chatId пользователя</param>
        /// <returns></returns>
        protected bool IsKeyboardCommand(string command, long chatId)
        {
            SIMSellerTelegramBot.DataBase.Models.User user = DbMethods.GetUserByChatId(this.Db, chatId);
            return IsKeyboardCommand(command, user);
        }


        public override Hop ProcessCallback(object userObj, TelegramBotClient bot, InboxMessage mes, CallbackQuery callback, string data)
        {
            User user = userObj as User;
            if (Equals(user, null)) throw new Exception("Не определен пользователь!");

            //Обработка callback запроса (Свернуть/скрыть)
            if (data.StartsWith(Answer.CallbackWrapThisMessage))
            {
                bot.DeleteMessageAsync(mes.ChatId, callback.Message.MessageId);
            }

            //Обработка callback запроса (Ответить на сообщение)
            if (data.StartsWith(Answer.CallbackAnswerToMessage))
            {
                return AnswerToMessage(bot, callback, data);
            }

            return base.ProcessCallback(userObj, bot, mes, callback, data);
        }

        /// <summary>
        /// Обработчик на случай если не ожидаются сообщения не текстового типа
        /// </summary>
        protected void DontUndertandMessage(TelegramBotClient bot, InboxMessage mes)
        {
            bot.SendTextMessageAsync(mes.ChatId, "Понимаю только текстовые сообщения!");
        }



        /// <summary>
        /// Callback Ответить на сообщение.
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="callback"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Hop AnswerToMessage(TelegramBotClient bot, CallbackQuery callback, string data)
        {
            //Забираем данные об отправители сообщения (формат данных [chatId|username])
            data = data.Replace(Answer.CallbackAnswerToMessage, "");
            var datas = data.Split('|')?.ToList();
            if (datas?.Count == 0)
            {
                throw new NullReferenceException("Не получил данные");
            }

            string strChatId = datas[0];
            string username = datas.ElementAtOrDefault(1);

            long senderChatId;
            if (long.TryParse(strChatId, out senderChatId) == false)
            {
                throw new Exception("Не могу преобразовать string to long!");
            }

            //Получили данные, теперь нужно перейти на состояние отправки сообщения
            //hop.Type должен быть NextLevelHop
            //Обязательно запомнить на каком сейчас мы состоянии, нужно будет вернуться обратно.
            Hop hop = new Hop();
            hop.NextStateName = "AnswerToUserMessage";
            hop.Type = HopType.NextLevelHop;//!!!!!!!!!!!
            hop.IntroductionString = Answer.AskInputMessageToUser(username);
            hop.Data = data;
            return hop;
        }

    }
}
