using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotLibrary.Classes.Bot
{
    /// <summary>
    /// Содержит информацию о переходе к следующему состоянию.
    /// </summary>
    public class Hop
    {
        [CanBeNull] public string NextStateName { get; set; }
        public HopType Type = HopType.RootLevelHop;
        public string IntroductionString { get; set; }

        //ToDo Сделать упрощенную передачу данных между состояниями.
        /// <summary>
        /// Data - для передачи данных при переходе в другое состояние.
        /// </summary>
        public string Data { get; set; }
        public MarkupWrapper<ReplyKeyboardMarkup> DynamicReplyKeyboard { get; set; }

        public Hop GetCopy()
        {
            Hop hop = new Hop()
            {
                NextStateName = this.NextStateName,
                IntroductionString = this.IntroductionString,
                Type = this.Type,
            };
            return hop;
        }
    }

    public enum HopType
    {
        RootLevelHop,
        NextLevelHop,
        CurrentLevelHop,
        BackToPreviosLevelHop
    }
}