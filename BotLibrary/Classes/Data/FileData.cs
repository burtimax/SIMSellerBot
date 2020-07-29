using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using File = Telegram.Bot.Types.File;

namespace BotLibrary.Classes.Data
{
    public class FileData
    {
        public File Info { get; set; }
        
        private MemoryStream _stream = null;
        public MemoryStream Stream
        {
            get { return _stream; }
            private set { _stream = value; }
        }

        public byte[] Data
        {
            set
            {
                this._stream = new MemoryStream(value);
            }
            get { return this.Stream.ToArray(); }
        }

    }
}
