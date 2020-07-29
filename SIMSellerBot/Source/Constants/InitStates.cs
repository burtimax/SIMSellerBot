using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotLibrary.Classes.Bot;
using SIMSellerBot.Source.Constants;

namespace SIMSellerTelegramBot.Source.Constants
{
    public class InitStates
    {
        /// <summary>
        /// Возвращаем состояние по имени из BotStates
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static State GetStateByName(string name)
        {
            if (BotStates?.Count == 0) return null;

            foreach (var state in BotStates)
            {
                if (string.Equals(state.Name, name, StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    return state;
                }
            }

            return null;
        }

        public static List<State> BotStates = new List<State>()
        {

            #region USER_STATES
            new State("User_Main")
            {
                Hops = new List<Hop>()
                {
                    new Hop(){NextStateName = "User_Main"}
                },

                IntroductionString = Answer.IntroductionMainMenu,
                HopOnSuccess = new Hop(){NextStateName = "User_Main"},
                ReplyKeyboard = Keyboards.User_Main_Keyboard,
            },

            new State("User_Order_WishNumber")
            {
                Hops = new List<Hop>()
                {
                    new Hop(){NextStateName = "User_Order_WishNumber"}
                },

                IntroductionString = Answer.IntroductionOrderNumber,
                HopOnSuccess = new Hop(){NextStateName = "User_Order_SetContacts"},
                HopOnFailure = new Hop(){NextStateName = "User_Order_WishNumber"},
                ReplyKeyboard = Keyboards.CancelOrderOperationKeyboard,
            },

            new State("User_Order_SetContacts")
            {
                Hops = new List<Hop>()
                {
                    new Hop(){NextStateName = "User_Order_SetContacts"}
                },

                IntroductionString = Answer.IntroductionSetContacts,
                HopOnSuccess = new Hop(){NextStateName = "User_Main"},
                HopOnFailure = new Hop(){NextStateName = "User_Order_SetContacts"},
                ReplyKeyboard = Keyboards.CancelOrderOperationKeyboard,
            },

            new State("User_HelpPanel")
            {
                Hops = new List<Hop>()
                {
                    new Hop(){NextStateName = "User_HelpPanel"}
                },

                IntroductionString = Answer.IntroductionHelpPanel,
                HopOnSuccess = new Hop(){NextStateName = "User_HelpPanel"},
                HopOnFailure = new Hop(){NextStateName = "User_HelpPanel"},
                ReplyKeyboard = Keyboards.UserHelpPanelKeyboard,
            },

            new State("User_Question")
            {
                Hops = new List<Hop>()
                {
                    new Hop(){NextStateName = "User_Question"}
                },

                IntroductionString = Answer.BtnQuestionToManager,
                HopOnSuccess = new Hop(){NextStateName = "User_HelpPanel"},
                HopOnFailure = new Hop(){NextStateName = "User_HelpPanel"},
                ReplyKeyboard = Keyboards.CancelOperationKeyboard,
            },
            #endregion


            #region MANAGER_STATES

            new State("Manager_Main")
            {
                Hops = new List<Hop>()
                {
                    new Hop(){NextStateName = "Manager_Main"}
                },

                IntroductionString = Answer.IntroductionMainMenu,
                HopOnSuccess = new Hop(){NextStateName = "Manager_Main"},
                ReplyKeyboard = Keyboards.Manager_Main_Keyboard,
            },

            new State("Manager_Requests")
            {
                Hops = new List<Hop>()
                {
                    new Hop(){NextStateName = "Manager_Requests"}
                },

                IntroductionString = Answer.IntroductionManagerRequests,
                HopOnSuccess = new Hop(){NextStateName = "Manager_Requests"},
                ReplyKeyboard = Keyboards.Manager_Requests_Keyboard,
            },

            new State("Manager_BroadcastMessage")
            {
                Hops = new List<Hop>()
                {
                    new Hop(){NextStateName = "Manager_BroadcastMessage"}
                },

                IntroductionString = Answer.IntroductionManagerBroadcastMessage,
                HopOnSuccess = new Hop(){NextStateName = "Manager_Main"},
                HopOnFailure = new Hop(){NextStateName = "Manager_Main"},
                ReplyKeyboard = Keyboards.CancelOperationKeyboard,
            },

            #endregion






            #region ADMIN_STATES
           

            #endregion
            


            new State("AnswerToUserMessage")
            {
                Hops = new List<Hop>()
                {
                    new Hop(){NextStateName = "MainUser"},
                },

                //IntroductionString = Answer.AskInputMessage,
                HopOnSuccess = new Hop()
                {
                    NextStateName = "MainUser",
                    Type = HopType.BackToPreviosLevelHop,
                },
                HopOnFailure = new Hop()
                {
                    NextStateName = "MainUser",
                    Type = HopType.BackToPreviosLevelHop,
                },
                ReplyKeyboard = Keyboards.CancelOperationKeyboard,
            },
        };
    }
}
