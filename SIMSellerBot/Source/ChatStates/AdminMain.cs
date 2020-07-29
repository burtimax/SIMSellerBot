//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using BotLibrary.Classes;
//using BotLibrary.Classes.Bot;
//using SIMSellerBot.Source.Methods;
//using SIMSellerTelegramBot.DataBase.Models;
//using SIMSellerTelegramBot.Source.Constants;
//using SIMSellerBot.DataBase.Models;
//using SIMSellerBot.Source.ChatStates;
//using SIMSellerBot.Source.Constants;
//using SIMSellerBot.Source.Db;
//using Telegram.Bot;
//using Telegram.Bot.Types.Enums;

//namespace SIMSellerTelegramBot.Source.ChatStates
//{
//    public class AdminMain : ParentState
//    {
//        public static Regex MessageRegex = new Regex(@"(?<ChatId>\d+)\s*:\s*(?<Message>\D+)");
//        public static Regex ApproveConsultantRegex = new Regex(@"/approve\s+(?<ChatId>\d+)");
//        public static Regex DenytConsultantRegex = new Regex(@"/deny\s+(?<ChatId>\d+)");

//        public AdminMain(State state) : base(state)
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
//                            case Answer.BtnGoToMain:
//                                Hop hop = new Hop()
//                                {
//                                    NextStateName = "MainUser",
//                                    Type = HopType.RootLevelHop,
//                                };

//                                return hop;
//                                break;

//                            case Answer.BtnAdminCommands:
//                                bot.SendTextMessageAsync(mes.ChatId, Answer.CommandsDescription,
//                                    replyMarkup: Keyboards.InlineWrapMessage.Value);
//                                return null;
//                                break;

//                            case Answer.BtnListQueries:
//                                return this.ShowQueryList(bot, mes);
//                                break;

//                            case Answer.BtnBotInfo:
//                                return this.ShowBotInfo(bot, mes);
//                                break;

//                            case Answer.BtnBotMarathons:
//                                return this.ShowBotMarathonsInfo(bot, mes);
//                                break;

//                            case Answer.BtnBotModerators:
//                                return this.ShowBotModerators(bot, mes);
//                                break;
//                        }
//                    }

//                    this.ProcessWrittenCommands(bot,mes,text);


//                    break;
//                default:

//                    break;
//            }


//            return base.ProcessMessage(bot, mes);
//        }


//        private void ProcessWrittenCommands(TelegramBotClient bot, InboxMessage mes, string text)
//        {
//            //быстро отправить сообщение
//            var res = MessageRegex.Match(text);
//            if (res.Success)
//            {
//                string strChatId = res.Groups["ChatId"].Value;

//                long receiverChatId;
//                if (long.TryParse(strChatId, out receiverChatId) == false)
//                {
//                    throw new Exception("Cant parse string to long!");
//                }

//                DataBase.Models.User receiver = DbMethods.GetUserByChatId(this.Db, receiverChatId);
//                if (receiver == null)
//                {
//                    bot.SendTextMessageAsync(mes.ChatId, Answer.NotFoundUser);
//                    return;
//                }

//                DataBase.Models.User me = DbMethods.GetUserByChatId(this.Db, mes.ChatId);
//                BotMethods.SendMessageToUser(bot, receiverChatId, me, res.Groups["Message"].Value);
//                bot.SendTextMessageAsync(mes.ChatId, Answer.AlreadySended);
//                return;
//            }

//            //Команда /approve chatId
//            var resApprove = ApproveConsultantRegex.Match(text);
//            if (resApprove.Success)
//            {
//                string strChatId = resApprove.Groups["ChatId"].Value;

//                long receiverChatId;
//                if (long.TryParse(strChatId, out receiverChatId) == false)
//                {
//                    throw new Exception("Cant parse string to long!");
//                }

//                DataBase.Models.User receiver = DbMethods.GetUserByChatId(this.Db, receiverChatId);
//                if (receiver == null)
//                {
//                    bot.SendTextMessageAsync(mes.ChatId, Answer.NotFoundUser);
//                    return;
//                }

//                DataBase.Models.User me = DbMethods.GetUserByChatId(this.Db, mes.ChatId);
               

//                DbMethods.ChangeUserToConsultant(this.Db, receiver, true);
//                BotMethods.SendMessageToUser(bot, receiverChatId, me, Answer.AlreadyConsultantQueryApproved);
//                DbMethods.DeleteConsultantQueryByChatId(this.Db, receiverChatId);
//                bot.SendTextMessageAsync(mes.ChatId, Answer.AlreadyAdminApprovedConsultantQuery);
//                return;
//            }

//            //Команда /deny chatId
//            var resDeny = DenytConsultantRegex.Match(text);
//            if (resDeny.Success)
//            {
//                string strChatId = resDeny.Groups["ChatId"].Value;

//                long receiverChatId;
//                if (long.TryParse(strChatId, out receiverChatId) == false)
//                {
//                    throw new Exception("Cant parse string to long!");
//                }

//                DataBase.Models.User receiver = DbMethods.GetUserByChatId(this.Db, receiverChatId);
//                if(receiver == null)
//                {
//                    bot.SendTextMessageAsync(mes.ChatId, Answer.NotFoundUser);
//                    return;
//                }

//                DataBase.Models.User me = DbMethods.GetUserByChatId(this.Db, mes.ChatId);
                

//                DbMethods.ChangeUserToConsultant(this.Db, receiver, false);
//                BotMethods.SendMessageToUser(bot, receiverChatId, me, Answer.AlreadyConsultantQueryDenied);
//                DbMethods.DeleteConsultantQueryByChatId(this.Db, receiverChatId);
//                bot.SendTextMessageAsync(mes.ChatId, Answer.AlreadyAdminDeniedConsultantQuery);
//                return;
//            }

//            bot.SendTextMessageAsync(mes.ChatId, Answer.NotCommand);

//        } 


//        private Hop ShowQueryList(TelegramBotClient bot, InboxMessage mes)
//        {
//            List<QueryToSupport> queries = DbMethods.GetAllConsultantQueryToSupport(this.Db);
//            Dictionary<QueryToSupport, DataBase.Models.User> dict =
//                new Dictionary<QueryToSupport, DataBase.Models.User>();
//            foreach (var q in queries)
//            {
//                var u = DbMethods.GetUserByChatId(this.Db, q.FromChatId);

//                if (Equals(u, null)) continue;

//                dict[q] = u;
//            }

//            if (dict.Count == 0)
//            {
//                bot.SendTextMessageAsync(mes.ChatId, Answer.NoConsultantQueries);
//                return null;
//            }

//            bot.SendTextMessageAsync(mes.ChatId, Answer.BtnListQueries,
//                replyMarkup: Keyboards.ConfigConsultantQueriesInlineKeyboard(dict).Value);
//            return null;
//        }


//        private Hop ShowBotInfo(TelegramBotClient bot, InboxMessage mes)
//        {
//            //В инфо будут отображаться данные:
//            //кол-во консультантов
//            var allConsultantsCount = DbMethods.ConsultantsCoundInDb(this.Db);
//            //кол-во участников
//            var allParticipantsCount = DbMethods.GetCountParticipants(this.Db);
//            //кол-во пользователей
//            var allUsersCount = DbMethods.GetUsersCount(this.Db);
//            //кол-во марафонов
//            var allMarathonsCount = DbMethods.GetActiveMarathonsCount(this.Db);
//            //Активность за последние 24 часа
//            var allMessagesCountForDay =
//                DbMethods.GetMessagesCountFromDate(this.Db, DateTime.Today);
//            //Общая активность за 10 дней
//            var allMessagesCount10days =
//                DbMethods.GetMessagesCountFromDate(this.Db, DateTime.Today.AddDays(-10));

//            string resMessage = $"ИНФО\n" +
//                                $"Консультанты [{allConsultantsCount} чел.]\n" +
//                                $"Участники марафонов [{allParticipantsCount} уч.]\n" +
//                                $"Пользователи [{allUsersCount} чел.]\n" +
//                                $"Кол-во марафонов [{allMarathonsCount} шт.]\n\n" +
//                                $"Активность:\n" +
//                                $"Сегодня [{allMessagesCountForDay} сообщений]\n" +
//                                $"10 дней [{allMessagesCount10days} сообщений]\n";
//            bot.SendTextMessageAsync(mes.ChatId, resMessage,
//                replyMarkup: Keyboards.InlineWrapMessage.Value);
//            return null;
//        }

//        private Hop ShowBotMarathonsInfo(TelegramBotClient bot, InboxMessage mes)
//        {
//            string resMessage = null;
//            //вывести список марафонов (active status) с участниками (ID/(консультант/ID)/кол-во участников/активность за (10/5/1) дн/)
//            var mp = DbMethods.GetAllActiveMarathonsWithParticipants(this.Db);

//            if (mp == null || mp.Count == 0)
//            {
//                bot.SendTextMessageAsync(mes.ChatId, Answer.NoData);
//                return null;
//            }

//            int count = 1;
//            foreach (var item in mp)
//            {
//                var consultant = DbMethods.GetConsultantByMarathon(this.Db, item.Key);

//                List<long> usersChatId = new List<long>();
//                foreach (var user in item.Value)
//                {
//                    usersChatId.Add(user.ChatId);
//                }

//                resMessage += $"{count}) Марафон: {item.Key.Name}\n" +
//                              $"Данные: [ID: {item.Key.PublicKey}] [{item.Value.Count} уч.]\n" +
//                              $"Консультант: {consultant.FirstName} {consultant.LastName} [{consultant.ChatId}]\n" +
//                              $"Активность (10/5/1): [{DbMethods.GetGroupUsersMessagesCountFromDate(this.Db, usersChatId, DateTime.Today.AddDays(-10))}/" +
//                              $"{DbMethods.GetGroupUsersMessagesCountFromDate(this.Db, usersChatId, DateTime.Today.AddDays(-5))}/" +
//                              $"{DbMethods.GetGroupUsersMessagesCountFromDate(this.Db, usersChatId, DateTime.Today)}]\n\n";
//                count++;
//            }

//            bot.SendTextMessageAsync(mes.ChatId, resMessage, replyMarkup: Keyboards.InlineWrapMessage.Value);
//            return null;
//        }

//        private Hop ShowBotModerators(TelegramBotClient bot, InboxMessage mes)
//        {
//            //Показать всех консультантов (ChatId/Количество марафонов/кол-во участников)
//            string resMessage = null;
//            var consultants = DbMethods.GetAllActiveConsultants(this.Db);

//            if (consultants == null || consultants.Count == 0)
//            {
//                bot.SendTextMessageAsync(mes.ChatId, Answer.NoData);
//                return null;
//            }

//            int count = 1;
//            foreach (var c in consultants)
//            {
//                var marathons = DbMethods.GetMarathonsByConsultantChatId(this.Db, c.ChatId);
//                List<string> marathonsPublicKey = new List<string>();
//                foreach (var m in marathons)
//                {
//                    marathonsPublicKey.Add(m.PublicKey);
//                }

//                int consultantParticipantsCount = DbMethods.GetConsultantParticipantsCount(this.Db, marathonsPublicKey);

//                resMessage += $"{count}) {c.FirstName} {c.LastName} [{c.ChatId}]\n" +
//                              $"[{marathons.Count} марафона] [{consultantParticipantsCount} участника]\n\n";
//                count++;
//            }

//            bot.SendTextMessageAsync(mes.ChatId, resMessage, replyMarkup: Keyboards.InlineWrapMessage.Value);

//            return null;
//        }
//    }
//}