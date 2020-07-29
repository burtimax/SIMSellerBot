using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BotLibrary.Classes;
using BotLibrary.Classes.Bot;
using BotLibrary.Classes.Controller;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace BotLibrary.Classes.Controller
{
    public class MessageProcessor
    {
        private PullMethods Methods;
        private ReflectionInfo Reflection;

        public MessageProcessor([NotNull] ReflectionInfo reflection, [NotNull] PullMethods methods)
        {
            this.Methods = methods;
            this.Reflection = reflection;
        }

        /// <summary>
        /// Обработчик сообщения.
        /// </summary>
        /// <param name="mes"></param>
        public void ProcessCurrentMessage(Object inboxMessage)
        {
            InboxMessage mes = inboxMessage as InboxMessage;

            try
            {
                
                //Проверим пользователя если вдруг его нет в базе, то добавить его.
                 (bool userApprove, object userObj) result = this.Methods.CheckUserBeforeMessageProcess(mes?.BaseMessage);
                 bool approveUser = result.userApprove;

                //Если пользователь не прошел проверку, то не обрабатываем сообщение. Вызываем метод из PullMethods
                if (approveUser == false)
                {
                    this.Methods.OnFailUserCheck(mes.Bot, mes);
                    return;
                }

                //запускаем метод из PullMethods для получения текущего состояния пользователя
                State currentChatState = Methods.GetUserCurrentChatState(mes.ChatId);

                //Получим тип состояния.
                string typeName = this.Reflection.StatesNamespace.Trim(' ', '.') + "." + currentChatState.Name;
                Type type = this.Reflection.Assembly.GetType(typeName, false);
                if (type == null)
                {
                    throw new NullReferenceException($"Не найден тип состояния [{typeName}]");
                }

                //Первичная обработка сообщения
                (bool NeedProcessMessage, Hop Hop) resultPreProcess =
                    this.Methods.PreProcessMessage(mes.Bot, currentChatState, mes);

                Hop hop = resultPreProcess.Hop;

                //Если нужно далее обработать сообщение
                if (resultPreProcess.NeedProcessMessage)
                {
                    //Создадим экземпляр класса состояния и вызовем метод обработки сообщения у экземпляра класса
                    object instance = Activator.CreateInstance(type, currentChatState);

                    //ProcessMessage - название абстрактного метода в классе BaseState
                    MethodInfo method = type.GetMethod("ProcessMessage");

                    //Проверим объекты на null
                    if(Equals(instance, null) || Equals(mes?.Bot, null))
                    {
                        throw new Exception("Переданы нулевые параметры");
                    }

                    //Запускаем метод обработки входящего сообщения, получаем переход после обработки. Переход может быть нулевым.
                    hop = method?.Invoke(instance, new object[] {result.userObj, mes.Bot, mes}) as Hop;
                }

                if (hop != null && string.IsNullOrEmpty(hop?.NextStateName?.Trim(' ')) == false)
                {
                    this.Methods.ChangeCurrentChatState(mes.Bot, mes.ChatId, currentChatState, hop);
                }
            }
            catch(Exception e)
            {
                this.Methods.ProcessException(e, mes.Bot);
            }
        }

        /// <summary>
        /// Обработчик Callback запросов (InlineKeyboard button click returns callback)
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="bot"></param>
        /// <param name="chatId"></param>
        public void ProcessCallback(TelegramBotClient bot, InboxMessage mes, CallbackQuery callback, long chatId)
        {

            try
            {
                //Проверим пользователя если вдруг его нет в базе, то добавить его.
                (bool userApprove, object userObj) result = this.Methods.CheckUserBeforeMessageProcess(callback.Message);
                bool approveUser = result.userApprove;

                //Если пользователь не прошел проверку, то не обрабатываем сообщение. Вызываем метод из PullMethods
                if (approveUser == false)
                {
                    return;
                }

                //запускаем метод из PullMethods для получения текущего состояния пользователя
                State currentChatState = Methods.GetUserCurrentChatState(chatId);

                //Получим тип состояния.
                string typeName = this.Reflection.StatesNamespace.Trim(' ', '.') + "." + currentChatState.Name;
                Type type = this.Reflection.Assembly.GetType(typeName, false);
                if (type == null)
                {
                    throw new NullReferenceException($"Не найден тип состояния [{typeName}]");
                }


                //Создадим экземпляр класса состояния и вызовем метод обработки сообщения у экземпляра класса
                object instance = Activator.CreateInstance(type, currentChatState);

                //ProcessCallback - название абстрактного метода в классе BaseState
                MethodInfo method = type.GetMethod("ProcessCallback");

                //Запускаем метод обработки входящего callback, получаем переход после обработки. Переход может быть нулевым.
                Hop hop = method?.Invoke(instance, new object[] {result.userObj, bot, mes, callback, callback.Data}) as Hop;


                if (hop != null && string.IsNullOrEmpty(hop?.NextStateName?.Trim(' ')) == false)
                {
                    this.Methods.ChangeCurrentChatState(bot, chatId, currentChatState, hop);
                }

            }
            catch (Exception e)
            {
                this.Methods.ProcessException(e, bot);
            }
            finally
            {
           
            }
        }
    }
}