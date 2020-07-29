using BotLibrary.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using BotLibrary.Classes.Controller;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace BotLibrary.Classes.Controller
{
    public class BotController
    {
        private BaseBot Bot;
        private PullMethods Methods;
        private ReflectionInfo Reflection;
        private MessageProcessor proc;

        public BotController([NotNull] TelegramBotClient bot,[NotNull] ReflectionInfo reflectionInfo,[NotNull] PullMethods pullMethods)
        {
            this.Bot = new BaseBot(bot);
            this.Methods = pullMethods;
            this.Reflection = reflectionInfo;

            proc = new MessageProcessor(this.Reflection, this.Methods);
            Init();
        }

        private void Init()
        {
            this.Bot.TelegramClient.OnMessage -= ProcessMessage;
            this.Bot.TelegramClient.OnMessage += ProcessMessage;

            //this.Bot.TelegramClient.OnUpdate -= OnUpdate;
            //this.Bot.TelegramClient.OnUpdate += OnUpdate;


            this.Bot.TelegramClient.OnCallbackQuery -= OnCallback;
            this.Bot.TelegramClient.OnCallbackQuery += OnCallback;
        }

        /// <summary>
        /// Обработчик события сообщения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ProcessMessage(object sender, MessageEventArgs args)
        {
            try
            {
                InboxMessage mes = new InboxMessage(this.Bot.TelegramClient, args.Message);
                proc.ProcessCurrentMessage(mes);
            }
            catch (Exception e)
            {
                Methods.ProcessException(e, this.Bot.TelegramClient);
            }
        }



        private void OnCallback(object sender, CallbackQueryEventArgs args)
        {
            this.Bot.TelegramClient.AnswerCallbackQueryAsync(args.CallbackQuery.Id);
            int chatId = args.CallbackQuery.From.Id;
            InboxMessage mes = new InboxMessage(this.Bot.TelegramClient, args.CallbackQuery.Message);

            proc.ProcessCallback(this.Bot.TelegramClient, mes, args.CallbackQuery, chatId);
            
        }

        public void StartBot()
        {
            this.Bot.Start();
        }

        public void StopBot()
        {
            this.Bot.Stop();
        }



        //private void OnUpdate(object sender, UpdateEventArgs args)
        //{
        //    if(Equals(args.Update.CallbackQuery, null) == false)
        //    {
        //        this.Bot.TelegramClient.AnswerCallbackQueryAsync(args.Update.CallbackQuery.Id);
        //        /*proc.ProcessCallback(this.Bot.TelegramClient,
        //            args.Update.CallbackQuery,
        //            args.Update.CallbackQuery.From.Id);*/
        //    }

        //    if (Equals(args.Update.Message, null) == false)
        //    {

        //    }
        //}

    }

}
