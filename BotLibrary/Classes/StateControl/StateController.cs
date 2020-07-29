using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace BotLibrary.Classes.StateControl
{
    /// <summary>
    /// Контроллер состояний.
    /// </summary>
    public class StateController
    {
        private static Regex DataRegex = new Regex(@".*(?<dataContent>\[(?<data>.*)\]\s*)$");

        private List<String> ListStates;

        private string _state;
        
        /// <summary>
        /// Строка состояния.
        /// Эта строка будет в виде ххх/хх/хххх/ххх
        /// Таким образом мы можем хранить предыдущие состояния.
        /// Проще осуществлять переход на предыдущие состояния, контроль состояний
        /// </summary>
        public string State
        {
            get
            {
                return _state;
            }
            private set { _state = value; }
        }

        /// <summary>
        /// Данные, передающиеся вместе с состоянием в квадратных скобках
        /// Например /State1[data1]/State2/CurrentState[DataForCurrentState]
        /// </summary>
        [CanBeNull]
        public string Data
        {
            get
            {
                var res = DataRegex.Match(this.State);
                if (res.Success == false) return null;

                return res.Groups["data"].Value;
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public StateController()
        {
            this.ListStates = new List<string>();
        }

        public StateController(string currentState):this()
        {
            currentState = this.ProcessStateString(currentState);
            this.State = "/" + currentState;
            this.SetList();
        }

        private void SetList()
        {
            this.ListStates = this.State?.Trim('/')?.Split('/')?.ToList();
            for (var i = 0; i < this.ListStates.Count; i++)
            {
                this.ListStates[i] = RemoveDataFromString(this.ListStates[i]);
            }
        }

        /// <summary>
        /// Проверка строки состояний на пустую строку.
        /// </summary>
        /// <returns></returns>
        private bool IsEmptyStateString()
        {
            if (this.ListStates == null || this.ListStates.Count == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Приведем строку в единый вид.
        /// </summary>
        /// <param name="stateToProcess"></param>
        /// <returns></returns>
        private string ProcessStateString(string stateToProcess)
        {
            var res =  stateToProcess.Trim('/', ' ');

            //Здесь не должно быть перенесений на новую строку, а то регекс не сработает и будет ошибка
            //заменим \n на &
            res = res.Replace('\n', '&');
            
            if (string.IsNullOrEmpty(res))
            {
                throw new Exception("Передана пустая строка состояния!");
            }

            return res;
        }

        /// <summary>
        /// Устанавливает состояние, как корневое.
        /// </summary>
        /// <param name="rootState">имя корневого параметра</param>
        public void SetRootState(string rootState, string data = null)
        {
            rootState = ProcessStateString(rootState);

            this.State = "/" + rootState;

            this.ListStates.Clear();
            this.ListStates.Add(rootState);

            if (string.IsNullOrWhiteSpace(data) == true) return;

            SetData(data);
        }

        /// <summary>
        /// добавляем состояние к строке состояний, как состояние следующего уровня.
        /// </summary>
        /// <param name="stateNextLevel"></param>
        public void AddStateAsNextState(string stateNextLevel, string data = null)
        {
            stateNextLevel = ProcessStateString(stateNextLevel);

            this.State += "/" + stateNextLevel;

            this.ListStates.Add(stateNextLevel);

            if (string.IsNullOrWhiteSpace(data) == true) return;

            SetData(data);
        }
       
        /// <summary>
        /// удаляем текущее состояние, или переходим к предыдущему состоянию
        /// </summary>
        public void RemoveCurrentState()
        {
            if(IsEmptyStateString() == true) return;

            var lastSlashIndex = this.State.LastIndexOf('/');
            this.State = this.State.Remove(lastSlashIndex);

            this.ListStates?.Remove(this.ListStates?.LastOrDefault());
        }

        /// <summary>
        /// Поменять имя текущего состояния.
        /// </summary>
        public void ChangeCurrentStateName(string stateName, string data = null)
        {
            stateName = this.ProcessStateString(stateName);

            if(IsEmptyStateString() == false)
            {
                this.RemoveCurrentState();
            }

            this.AddStateAsNextState(stateName);

            if (string.IsNullOrWhiteSpace(data) == true) return;

            SetData(data);
        }

        /// <summary>
        /// Возвращаем название текущего состояния
        /// </summary>
        /// <returns></returns>
        public string GetCurrentStateName()
        {
            if (IsEmptyStateString() == true) return null;

            return RemoveDataFromString(this.ListStates.LastOrDefault());

        }


        /// <summary>
        /// Показать на каком уровне текущее состояние
        /// т.е. ааа/ббб/ххх (текущий уровень 3)
        /// </summary>
        /// <returns></returns>
        public int GetCurrentStateLevel()
        {
            if (IsEmptyStateString() == true)
            {
                return 0;
            }

            return this.ListStates.Count;
        }

        private void SetData(string data)
        {
            this.RemoveData();

            if (string.IsNullOrWhiteSpace(data))
            {
                return;
            }

            this.State += $"[{data}]";
        }

        /// <summary>
        /// удаляем данные для состояния
        /// </summary>
        public void RemoveData()
        {
            this.State = RemoveDataFromString(this.State);
        }


        private string RemoveDataFromString(string str)
        {
            var res = DataRegex.Match(str);

            if (res.Success == false) return str;

            return str.Replace(res.Groups["dataContent"].Value, "");
        }

    }
}
