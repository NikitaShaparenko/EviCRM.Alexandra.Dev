using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EviCRM.Backend4.Core
{
    public class AlexandraLog
    {
        bool isDebug = false;

        public AlexandraLog(bool is_Debug)
        {
            isDebug = is_Debug;
        }

        public void Log(string message)
        {
            string msg = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " : ";

            msg += message;

            Console.WriteLine(msg);

            if (isDebug)
            {
                Debug.WriteLine(msg);
            }
        }

        public void Log(string message, LogStatus ls)
        {
            string msg = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " : ";

            switch(ls)
            {
                case LogStatus.Error:
                    msg = "[ ERROR ] : ";
                    break;

                case LogStatus.Warning:
                    msg = "[ Warning ] : ";
                    break;

                case LogStatus.Debug:
                    msg = "[ Debug ] : ";
                    break;

                case LogStatus.Info:
                    msg = "[ Info ] : ";
                    break;

                case LogStatus.Fatal:
                    msg = "[ FATAL ] : ";
                    break;
            }

            msg += message;

            Console.WriteLine(msg);

            if (isDebug)
            {
                Debug.WriteLine(msg);
            }
        }

        public void Log(string control, string message, LogStatus ls)
        {
            string msg = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " : ";

            switch (ls)
            {
                case LogStatus.Error:
                    msg = "[ ERROR ] : ";
                    break;

                case LogStatus.Warning:
                    msg = "[ Warning ] : ";
                    break;

                case LogStatus.Debug:
                    msg = "[ Debug ] : ";
                    break;

                case LogStatus.Info:
                    msg = "[ Info ] : ";
                    break;

                case LogStatus.Fatal:
                    msg = "[ FATAL ] : ";
                    break;
            }
            msg += " < " + control + " > ";
            msg += message;

            Console.WriteLine(msg);

            if (isDebug)
            {
                Debug.WriteLine(msg);
            }
        }

        public enum LogStatus
        {
            Debug,
            Warning,
            Error,
            Info,
            Fatal,
        }
    }
  
}
