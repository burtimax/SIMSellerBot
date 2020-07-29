//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BotLibrary.Classes;
//using BotLibrary.Classes.Bot;
//using SIMSellerBot.Source.Methods;
//using SIMSellerTelegramBot.DataBase.Models;
//using SIMSellerTelegramBot.Source.Constants;
//using SIMSellerBot.Source.ChatStates;
//using SIMSellerBot.Source.Constants;
//using SIMSellerBot.Source.Methods;
//using Telegram.Bot;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.Enums;
//using Telegram.Bot.Types.ReplyMarkups;

//namespace SIMSellerTelegramBot.Source.ChatStates
//{
//    public class ModeratorMain : ParentState
//    {
//        public ModeratorMain(State state) : base(state)
//        {

//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public override Hop ProcessMessage(TelegramBotClient bot, InboxMessage mes)
//        {
//            switch (mes.Type)
//            {
//                case MessageType.Text:
//                    string text = mes.Data as string;
//                    if (IsKeyboardCommand(text, mes.ChatId))
//                    {
//                        switch (text)
//                        {
//                            case Answer.BtnUserPanel:
//                                return GoToUserPanel();
//                                break;
//                            case Answer.BtnMarathons:
//                                return GoToModeratorMarathons();
//                                break;
//                            case Answer.BtnParticipantsData:
//                                return ShowAllMarathonsOrShowParticipants(bot, mes.ChatId);
//                                break;
//                            case Answer.BtnModeratorWritePanel:
//                                return GoToModeratorWritePanel();
//                                break;
//                        }
//                        return this.State.HopOnSuccess;
//                    }

//                    return this.State.HopOnSuccess;
//                    break;
//                default:
                    
//                    break;
//            }


//            return base.ProcessMessage(bot, mes);
//        }


//        /// <summary>
//        /// Обработка CallBack запросов
//        /// </summary>
//        /// <returns></returns>
//        public override Hop ProcessCallback(TelegramBotClient bot, long chatId, CallbackQuery callback, string data)
//        {
//            //Если кликнули на марафон, то вывести список его участников
//            if (data.StartsWith(Answer.CallbackShowMarathon))
//            {
//                string publicKey = data.Replace(Answer.CallbackShowMarathon, "");

//                Marathon marathon = DbMethods.GetMarathonByPublicKey(this.Db, publicKey);
//                var users = DbMethods.GetAllParticipantsForMarathon(this.Db, marathon);

//                //Если выбрали марафон, а у него нет участников, то просто кинем оповещалку, что нет участников
//                if (users?.Count == 0)
//                {
//                    bot.AnswerCallbackQueryAsync(callback.Id, Answer.NoParticipantsInConcreteMarathon(marathon));
//                    return null;
//                }

//                var inline = Keyboards.GetShowParticipantsDataInline(users, true).Value as InlineKeyboardMarkup;
//                bot.AnswerCallbackQueryAsync(callback.Id);
//                bot.EditMessageTextAsync(chatId, callback.Message.MessageId, Answer.ParticipantListOfConcreteMarathon(marathon), replyMarkup: inline);
//                //bot.EditMessageReplyMarkupAsync(chatId, callback.Message.MessageId, inline);
//                return null;
//            }

//            //Если кликнули на участника, товыведем сообщением его дневник питания за последние N дней с кнопкой (свернуть)
//            if (data.StartsWith(Answer.CallbackShowUserData))
//            {
//                //Получим данные, передавали в виде (days|chatId|fullname)
//                List<string> datas = data.Replace(Answer.CallbackShowUserData, "").Split('|').ToList();

//                int days = Convert.ToInt32(datas[0]);
//                long userChatId = Convert.ToInt64(datas[1]);
//                string userName = datas[2];

//                Statistics stat = new Statistics();
//                string resStatistics = stat.GetUserStatistics(this.Db, userChatId, days, userName);

//                bot.AnswerCallbackQueryAsync(callback.Id);
//                bot.SendTextMessageAsync(chatId, resStatistics,
//                    replyMarkup: Keyboards.InlineWrapMessage.Value as InlineKeyboardMarkup);
//                return null;
//            }

//            //Если нажали на InlineButton (Свернуть), то удалить сообщение
//            if (data.StartsWith(Answer.CallbackWrapThisMessage))
//            {
//                bot.AnswerCallbackQueryAsync(callback.Id);
//                bot.DeleteMessageAsync(chatId, callback.Message.MessageId);
//                return null;
//            }

//            //По нажатию inline кнопки (назад) нужно показать список марафонов.
//            if (data.StartsWith(Answer.CallbackGoBack))
//            {
//                bot.AnswerCallbackQueryAsync(callback.Id);
//                ShowAllMarathonsOrShowParticipants(bot, chatId, callback.Message.MessageId);
//            }


//            return base.ProcessCallback(bot, chatId, callback, data);
//        }

//        private Hop GoToUserPanel()
//        {
//            Hop hop = new Hop();
//            hop.NextStateName = "MainUser";
//            hop.Type = HopType.RootLevelHop;
//            return hop;
//        }

//        private Hop GoToModeratorMarathons()
//        {
//            Hop hop = new Hop();
//            hop.NextStateName = "ModeratorMarathons";
//            hop.Type = HopType.RootLevelHop;
//            return hop;
//        }


//        private Hop ShowAllMarathonsOrShowParticipants(TelegramBotClient bot, long chatId, int messageId = -1)
//        {
//            //Получаем все марафоны консультанта
//            var marathons = DbMethods.GetMarathonsByConsultantChatId(this.Db, chatId);

//            //Если нет марафонов, вывести сообщение, что марафонов нет, создайте марафон.
//            if(marathons?.Count == 0)
//            {
//                bot.SendTextMessageAsync(chatId, Answer.YouHaventParticipants);
//                return null;
//            }

//            //Если всего один марафон, то не нужно выводить список марафонов, сразу стоит вывести список участников
//            if (marathons?.Count == 1)
//            {
//                var users = DbMethods.GetAllParticipantsForMarathon(this.Db, marathons[0]);
//                //Если нет участников, то вывести сообщение
//                if (users?.Count == 0)
//                {
//                    bot.SendTextMessageAsync(chatId, Answer.YouHaventParticipants);
//                    return null;
//                }

//                //Вывести список участников
//                InlineKeyboardMarkup inlKeyboard = Keyboards.GetShowParticipantsDataInline(users).Value as InlineKeyboardMarkup;

//                bot.SendTextMessageAsync(chatId, Answer.ParticipantListOfConcreteMarathon(marathons[0]), replyMarkup: inlKeyboard);
                
//                return null;

//            }

//            //Если количество марафонов > 1, то выведем список марафонов для выбора консультанту.
//            //Не выводим марафоны с нулевым количеством участников
//            marathons?.RemoveAll(m => DbMethods.GetCountParticipantsOfMarathon(this.Db, m.PublicKey) == 0);
//            var inline = Keyboards.GetShowMarathonsInline(marathons).Value as InlineKeyboardMarkup;
//            if (messageId > -1)
//            {
//                bot.EditMessageTextAsync(chatId, messageId, Answer.ParticipantListOfConcreteMarathon(marathons[0]), replyMarkup: inline);
//            }
//            else
//            {
//                bot.SendTextMessageAsync(chatId, Answer.ChoseMarathonForShowParticipants, replyMarkup: inline);
//            }
//            return null;
//        }


//        private Hop GoToModeratorWritePanel()
//        {
//            Hop hop = new Hop();
//            hop.NextStateName = "ModeratorWritePanel";
//            hop.Type = HopType.RootLevelHop;
//            return hop;
//        }

//    }
//}