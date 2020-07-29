using System;
using System.Collections.Generic;
using System.Text;
using BotLibrary.Classes.Bot;
using JetBrains.Annotations;
using Telegram.Bot;

namespace BotLibrary.Classes.Controller
{
    /// <summary>
    /// Класс, который содержит ссылки на важные методы в работе бота.
    /// Необходимые для функционирования логики бота.
    /// </summary>
    public class PullMethods
    {
        private Func<long, State> _getUserCurrentChatState;
        private Func<Telegram.Bot.Types.Message, (bool userActive, object userObj)> _checkUserBeforeMessageProcess;
        private Action<TelegramBotClient, InboxMessage> _onFailUserCheck;
        private Action<TelegramBotClient, long, State, Hop> _changeCurrentState;
        private Func<TelegramBotClient, State, InboxMessage, (bool,Hop)> _preprocessMessage;
        private Action<Exception, TelegramBotClient> _processException;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="getCurrentUserState">Метоод, который будет получать и возвращать для пользователя текущий статус.</param>
        /// <param name="checkUserBeforeMessageProcessing">Метод, который будет проверять пользователя на наличие в базе данных перед тем как обработать его сообщение.</param>
        /// <param name="changeCurrentChatState">Метод, которы будет менять в базе статус текущего состояние и делать переход в другое состояние бота.</param>
        public PullMethods([NotNull] Func<long, State> getCurrentUserState,
            [NotNull] Func<Telegram.Bot.Types.Message,(bool userActive, object userObj)> checkUserBeforeMessageProcessing,
            [NotNull] Action<TelegramBotClient, long, State, Hop> changeCurrentChatState,
            [NotNull] Action<Exception, TelegramBotClient> processBotException,
            [CanBeNull] Func<TelegramBotClient, State, InboxMessage, (bool, Hop)> preProcessMessage = null,
            [CanBeNull] Action<TelegramBotClient, InboxMessage> OnFailCheckUserAction = null)
        {
            this._getUserCurrentChatState = getCurrentUserState;
            this._checkUserBeforeMessageProcess = checkUserBeforeMessageProcessing;
            this._changeCurrentState = changeCurrentChatState;
            this._processException = processBotException;
            this._preprocessMessage = preProcessMessage;
            this._onFailUserCheck = OnFailCheckUserAction;
            
        }



        public State GetUserCurrentChatState(long chatId)
        {
            return this._getUserCurrentChatState(chatId) ?? null;
        }

        /// <summary>
        /// Проверить пользователя (есть ли он в базе или не заблокирован ли он для чата)
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns>Если true, то сообщения пользователя будут обрабатываться, иначе не будут обрабатываться</returns>
        public (bool, object) CheckUserBeforeMessageProcess(Telegram.Bot.Types.Message mes)
        {
            return this._checkUserBeforeMessageProcess(mes);
        }

        public void ChangeCurrentChatState(TelegramBotClient bot, long chatId, State currentState, Hop hop)
        {
            this._changeCurrentState(bot, chatId, currentState, hop);
        }

        /// <summary>
        /// Вызываем метод для первичной обработки сообщения.
        /// </summary>
        /// <returns>
        /// bool возвращает, нужно ли далее обрабатывать сообщение.
        /// Hop возвращает если нужно прыгнуть на другое состояние.
        /// </returns>
        public (bool, Hop) PreProcessMessage(TelegramBotClient bot, State state, InboxMessage inbox)
        {
            if(this._preprocessMessage == null)
            {
                return (true, null);
            }

            return this._preprocessMessage(bot, state, inbox);
        }

        /// <summary>
        /// Метод, который вызывается при неудачной проверки пользователя
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="inbox"></param>
        public void OnFailUserCheck(TelegramBotClient bot, InboxMessage inbox)
        {
            if (this._onFailUserCheck == null)
            {
                return;
            }

            this._onFailUserCheck(bot, inbox);
        }

        /// <summary>
        /// Метод для обработки ошибки.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="bot"></param>
        public void ProcessException(Exception exception, TelegramBotClient bot)
        {
            this._processException(exception, bot);
        }

    }
}
