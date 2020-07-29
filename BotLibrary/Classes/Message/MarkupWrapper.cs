using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotLibrary.Classes
{
    public class MarkupWrapper<T>
    {
        private bool ResizeKeyboard = false;
        private IReplyMarkup MarkUp;
        private IReplyMarkup innerMarkUp;
        private IKeyboardButton[][] keyboard;
        private List<List<IKeyboardButton>> Keyboard;

        List<IKeyboardButton> LastRow
        {
            get
            {
                if(this.Keyboard.Count>0) return this.Keyboard.LastOrDefault();
                return null;
            }
        }

        public IReplyMarkup Value
        {
            get
            {
                ConfigureArray();
                return this.MarkUp;
            }
        }

        public MarkupWrapper(bool resizeKeyboard = false)
        {
            this.Keyboard = new List<List<IKeyboardButton>>();
            this.ResizeKeyboard = resizeKeyboard;
        }

        public MarkupWrapper<T> NewRow()
        {
            this.Keyboard.Add(new List<IKeyboardButton>());
            return this;
        }

        /// <summary>
        /// Добавляем кнопку в клавиатуру
        /// </summary>
        /// <param name="text">Имя кнопки</param>
        /// <param name="callBack">Используется только для InlineKeyboardMarkup</param>
        /// <returns></returns>
        public MarkupWrapper<T> Add(string text, string callBack = "default")
        {

            if (typeof(T) == typeof(ReplyKeyboardMarkup)){
                KeyboardButton btn = new KeyboardButton();
                btn.Text = text;
                LastRow.Add(btn);
            }

            if (typeof(T) == typeof(InlineKeyboardMarkup))
            {
                InlineKeyboardButton btn = new InlineKeyboardButton();
                btn.Text = text;
                btn.CallbackData = callBack;
                LastRow.Add(btn);
            }

            return this;
        }

        private void ConfigureArray()
        {
            if (typeof(T) == typeof(InlineKeyboardMarkup))
            {
                this.keyboard = new InlineKeyboardButton[Keyboard.Count][];
                for (int i = 0; i < this.keyboard.Length; i++)
                {
                    this.keyboard[i] = new InlineKeyboardButton[this.Keyboard[i].Count];
                }

                for (int i = 0; i < Keyboard.Count; i++)
                {
                    for (int j = 0; j < Keyboard[i].Count; j++)
                    {
                        this.keyboard[i][j] = this.Keyboard[i][j];
                    }
                }
            }


            if (typeof(T) == typeof(ReplyKeyboardMarkup))
            {
                this.keyboard = new KeyboardButton[Keyboard.Count][];
                for (int i = 0; i < this.keyboard.Length; i++)
                {
                    this.keyboard[i] = new KeyboardButton[this.Keyboard[i].Count];
                }

                for (int i = 0; i < Keyboard.Count; i++)
                {
                    for (int j = 0; j < Keyboard[i].Count; j++)
                    {
                        this.keyboard[i][j] = this.Keyboard[i][j];
                    }
                }
            }


            if (typeof(T) == typeof(InlineKeyboardMarkup))
            {
                this.MarkUp = new InlineKeyboardMarkup(this.keyboard as InlineKeyboardButton[][]);
            }

            if (typeof(T) == typeof(ReplyKeyboardMarkup))
            {
                this.MarkUp = new ReplyKeyboardMarkup(this.keyboard as KeyboardButton[][], this.ResizeKeyboard);
            }


        }

        /// <summary>
        /// Вернуть строки всех кнопок;
        /// </summary>
        public List<string> ButtonTexts()
        {
            if (Equals(this.Keyboard, null) == true ||
                this.Keyboard?.Count == 0)
            {
                return null;
            }
            List<string> res = new List<string>();
            for(var i = 0; i < this.Keyboard.Count; i++)
            {
                for (var j = 0; j < this.Keyboard[i].Count; j++)
                {
                    res.Add(this.Keyboard[i][j].Text);
                }
            }

            return res;
        }
    }
}
