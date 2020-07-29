//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BotLibrary.Classes;
//using BotLibrary.Classes.Bot;
//using SIMSellerBot.Source.Methods;
//using SIMSellerTelegramBot.Source.Constants;
//using SIMSellerBot.DataBase.Models;
//using SIMSellerBot.Source.ChatStates;
//using SIMSellerBot.Source.Constants;
//using Telegram.Bot;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.Enums;

//namespace SIMSellerTelegramBot.Source.ChatStates
//{
//    public class MainUser : ParentState
//    {
//        public MainUser(State state) : base(state)
//        {

//        }

//        public override Hop ProcessMessage(TelegramBotClient bot, InboxMessage mes)
//        {
//            switch (mes.Type)
//            {
//                case MessageType.Text:
//                    if (IsKeyboardCommand(mes.Data as string, mes.ChatId))
//                    {
//                        return this.ProcessCommand(bot, mes, mes.Data as string);
//                    }
//                    else
//                    {
//                        bot.SendTextMessageAsync(mes.ChatId, Answer.NotCommand);
//                    }

//                    return null;
//                    break;
//                default:
//                    bot.SendTextMessageAsync(mes.ChatId, Answer.NotCommand);
//                    break;
//            }

//            return base.ProcessMessage(bot, mes);
//        }

//        /// <summary>
//        /// Обрабатываем команды клавиатуры
//        /// </summary>
//        /// <param name="mes"></param>
//        /// <param name="command"></param>
//        private Hop ProcessCommand(TelegramBotClient bot, InboxMessage mes, string command)
//        {
//            switch (command)
//            {
//                case Answer.BtnSetFood:
//                    return this.SetFoodCommand(mes);
//                    break;
//                case Answer.BtnSetWeight:
//                    return this.SetWeightCommand(mes);
//                    break;
//                case Answer.BtnSetWater:
//                    return this.SetWaterCommand(mes);
//                    break;
//                case Answer.BtnWaterAdd300:
//                    return this.SetWaterAddRemove300Command(bot, mes, true);
//                    break;
//                case Answer.BtnWaterRemove300:
//                    return this.SetWaterAddRemove300Command(bot, mes, false);
//                    break;
//                case Answer.BtnSettings:
//                    return this.SetSettingsCommand(mes);
//                    break;
//                case Answer.BtnStatistics:
//                    return this.SetStatisticsCommand(mes);
//                    break;
//                case Answer.BtnConsultantPanel:
//                    return this.SetModeratorPanel(mes);
//                    break;
//                case Answer.BtnAdminPanel:
//                    return this.SetAdminPanel(mes);
//                    break;
//            }

//            return null;
//        }

//        private Hop SetFoodCommand(InboxMessage mes)
//        {
//            Hop SetFood = new Hop();
//            SetFood.NextStateName = "SetFood";
//            SetFood.Type = HopType.RootLevelHop;
//            return SetFood;
//        }

//        private Hop SetWeightCommand(InboxMessage mes)
//        {
//            Hop SetWeight = new Hop();
//            SetWeight.NextStateName = "SetWeight";
//            SetWeight.Type = HopType.RootLevelHop;
//            return SetWeight;
//        }

//        private Hop SetWaterCommand(InboxMessage mes)
//        {
//            Hop SetWater = new Hop();
//            SetWater.NextStateName = "SetWater";
//            SetWater.Type = HopType.RootLevelHop;
//            return SetWater;
//        }

//        private Hop SetSettingsCommand(InboxMessage mes)
//        {
//            Hop SetWater = new Hop();
//            SetWater.NextStateName = "SetSettings";
//            SetWater.Type = HopType.RootLevelHop;
//            return SetWater;
//        }

//        private Hop SetStatisticsCommand(InboxMessage mes)
//        {
//            Hop SetStatistics = new Hop();
//            SetStatistics.NextStateName = "UserStatistics";
//            SetStatistics.Type = HopType.RootLevelHop;
//            return SetStatistics;
//        }

//        private Hop SetWaterAddRemove300Command(TelegramBotClient bot, InboxMessage mes, bool IsAdditionalOperation)
//        {
//            var res = bot.GetUpdatesAsync(0, 10);
//            var updates = bot.GetUpdatesAsync(0, 10, 3000, new List<UpdateType>() {UpdateType.ChannelPost, UpdateType.Message});
//            Update u = new Update();
//            Water curWater = DbMethods.GetWaterByDate(this.Db, mes.ChatId, DateTime.Today);
//            int volume = 0;
//            if (IsAdditionalOperation)
//            {
//                if (curWater == null)
//                {
//                    volume = 300;
//                }
//                else
//                {
//                    volume = curWater.Quantity + 300;
//                }
//            }
//            //Subtraction
//            else
//            {
//                if (curWater == null)
//                {
//                    volume = 0;
//                }
//                else
//                {
//                    volume = Math.Max(curWater.Quantity - 300, 0);
//                }
//            }


//            DbMethods.SetTodayWater(this.Db, mes.ChatId, volume);
//            bot.SendTextMessageAsync(mes.ChatId, Answer.SuccessSetWater(volume));
//            return null;
//        }

//        private Hop SetModeratorPanel(InboxMessage mes)
//        {
//            Hop h = new Hop();
//            h.NextStateName = "ModeratorMain";
//            h.Type = HopType.RootLevelHop;
//            return h;
//        }

//        private Hop SetAdminPanel(InboxMessage mes)
//        {
//            Hop h = new Hop();
//            h.NextStateName = "AdminMain";
//            h.Type = HopType.RootLevelHop;
//            return h;
//        }

//    }
//}
