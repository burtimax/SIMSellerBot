using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BotLibrary.Classes.Data;
using BotLibrary.Classes.Message;
using BotLibrary.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotLibrary.Helpers
{
    public class HelperBot
    {
        public static MessagePhoto GetPhoto(TelegramBotClient bot, Message mes, PhotoQuality quality = PhotoQuality.High)
        {
            if (bot == null ||
                mes == null ||
                mes.Type != MessageType.Photo) return null;

            int qualityIndex = (int) Math.Round(((int)quality) / ((double)PhotoQuality.High) * mes.Photo.Length-1);
            string fileId = null;
            fileId = mes.Photo[qualityIndex].FileId;
            MessagePhoto photo = new MessagePhoto();
            photo.File = GetFile(bot, mes, fileId) ;
            return photo;
        }


        public static MessageAudio GetAudio(TelegramBotClient bot, Message mes)
        {
            if (bot == null ||
                mes == null ||
                mes.Type != MessageType.Audio) return null;

            MessageAudio audio = new MessageAudio();
            audio.File = GetFile(bot, mes, mes.Audio.FileId);
            return audio;
        }

        public static MessageVoice GetVoice(TelegramBotClient bot, Message mes)
        {
            if (bot == null || 
                mes == null ||
                mes.Type != MessageType.Voice) return null;

            MessageVoice voice = new MessageVoice();
            var fileId = mes.Voice.FileId;
            voice.File = GetFile(bot, mes, fileId);
            return voice;
        }

        private static FileData GetFile(TelegramBotClient bot, Message mes, string fileId)
        {
            if (bot == null || 
                mes == null ||
                string.IsNullOrEmpty(fileId)) return null ;

            FileData fileData = null;
            MemoryStream ms = new MemoryStream();
            fileData = new FileData();
            fileData.Info = bot.GetInfoAndDownloadFileAsync(fileId, ms).Result;
            fileData.Data = ms.ToArray();
            return fileData;
        }
    }
}
