using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SIMSellerTelegramBot.Source.Constants
{
    public class Constants
    {
        public static string DIRECTORY
        {
            get { return Directory.GetCurrentDirectory(); }
        }

        public static string CONNECTION_STRING_FILEPATH
        {
            get { return DIRECTORY + "\\" + "connection_database.txt"; }
        }

        /// <summary>
        /// Chat Id of Support
        /// </summary>
        public static long SupportChatId = 672312299;
       
        
        public const string ROLE_USER = "user";
        public const string ROLE_DILER = "diler";
        public const string ROLE_MANAGER = "manager";
        public const string ROLE_ADMIN = "admin";
        
        public const string MARATHON_STATUS_ACTIVE = "active";
        public const string MARATHON_STATUS_DELETED = "deleted";

        public const string QUERY_CONSULTANT = "consultant";

        public const string REQUEST_NUMBER_STATUS_OPEN = "open";
        public const string REQUEST_NUMBER_STATUS_FAIL = "fail";
        public const string REQUEST_NUMBER_STATUS_SUCCESS = "success";
        public const string REQUEST_NUMBER_STATUS_DENY = "deny";
        public const string REQUEST_NUMBER_STATUS_DELETE = "delete";
        
        public const string QUESTION_STATUS_OPEN = "open";
        public const string QUESTION_STATUS_CLOSE = "close";
        
        public const string SPECIAL_COMMAND_START =  "/start";
        public const string SPECIAL_COMMAND_BECOME_MANAGER = "/wantbecomemanager";
        public const string SPECIAL_COMMAND_BECOME_USER = "/wantbecomeuser";
        
        
        public const int REQUESTS_FETCH = 4;



    }
}
