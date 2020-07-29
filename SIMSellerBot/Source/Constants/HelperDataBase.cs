using SIMSellerTelegramBot.DataBase.Context;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using SIMSellerTelegramBot.Source.Constants;
using Microsoft.EntityFrameworkCore;

namespace SIMSellerTelegramBot
{
    public class HelperDataBase
    {
        private static string default_connection = @"Server=(localdb)\mssqllocaldb;Database=SIMSellerBot;Trusted_Connection=True;";
        private static string connection_string; 
        public static string CONNECTION_STRING
        {
            get
            {
                if (string.IsNullOrEmpty(connection_string))
                {
                    try
                    {
                        return File.ReadAllText(Constants.CONNECTION_STRING_FILEPATH);
                    }
                    catch
                    {
                        return default_connection;
                    }
                }

                return connection_string;
            }
        } 

        public static DbContextOptions<BotDbContext> DB_OPTIONS
        {
            get
            {
                var optionBuilder = new DbContextOptionsBuilder<BotDbContext>();
                return optionBuilder.UseSqlServer(CONNECTION_STRING).Options;
            }
        }


        private static BotDbContext db_context_instance;
        public static BotDbContext DB_CONTEXT_INSTANCE
        {
            get
            {
                if (db_context_instance == null)
                {
                    db_context_instance = new BotDbContext(DB_OPTIONS);
                }

                return db_context_instance;
            }
            set { db_context_instance = value; }
        }



    }
}
