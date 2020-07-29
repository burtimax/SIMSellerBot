using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DiaryClassLibStandart.Helpers;

namespace BotLibrary.Classes.Data
{
    public class MessageData
    {
        public FileData File { get; set; }
        public string Caption { get; set; }

        /// <summary>
        /// Сохранить данные в файл
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveDataToFile(string filePath)
        {
            HelperFileName.ParsePath(filePath, out var dir, out var filename, out var ext);

            if (string.IsNullOrEmpty(dir.Trim(' ')) ||
                string.IsNullOrEmpty(filename.Trim(' ')) ||
                string.IsNullOrEmpty(ext.Trim(' ')))
            {
                throw new ArgumentNullException("Один из параметров пуст!");
            }

            if (Directory.Exists(dir) == false)
            {
                throw new Exception($"Directory [{dir}] doesn't exists!");
            }

            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                 fs.WriteAsync(this.File.Data,0,this.File.Data.Length);   
            }
        }

        /// <summary>
        /// Считываем данные из файла
        /// </summary>
        /// <param name="filePath"></param>
        public void ReadDataFromFile(string filePath)
        {
            HelperFileName.ParsePath(filePath, out var dir, out var filename, out var ext);

            if (string.IsNullOrEmpty(dir.Trim(' ')) ||
                string.IsNullOrEmpty(filename.Trim(' ')) ||
                string.IsNullOrEmpty(ext.Trim(' ')))
            {
                throw new ArgumentNullException("Один из параметров пуст!");
            }

            if (System.IO.File.Exists(filePath) == false)
            {
                throw new Exception($"файл [{filePath}] не существует!");
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                fs.ReadAsync(this.File.Data, 0, this.File.Data.Length);
            }
        }
    }
}
