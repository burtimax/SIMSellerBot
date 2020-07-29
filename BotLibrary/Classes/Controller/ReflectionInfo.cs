using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BotLibrary.Classes.Controller
{
    public class ReflectionInfo
    {

        public Assembly Assembly;
        public string StatesNamespace;
        

        public ReflectionInfo(Assembly asmbl, string namespaceOfAllStates)
        {
            this.Assembly = asmbl;
            this.StatesNamespace = namespaceOfAllStates;
        }
    }
}
