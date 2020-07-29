using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using BotLibrary.Classes.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = Telegram.Bot.Types.File;

namespace BotLibrary.Classes.Helpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get file extension (File class)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFileExtension(this File file)
        {
            string path = file.FilePath;
            return path.Substring(path.LastIndexOf('.'));
        }

    }
}
