using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

 

namespace Log
{
    public class LogHelper:ILog
    {
        public static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("RunLogger");
        public static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("ErrorLogger");
        public static readonly log4net.ILog logConsole = log4net.LogManager.GetLogger("ConsoleLogger");

        public void Info(string info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
            }
            if (logConsole.IsInfoEnabled)
            {
                logConsole.Info(info);
            }
        }

        public  void Debug(string info)
        {
            if (loginfo.IsDebugEnabled)
            {
                loginfo.Debug(info);
            }
            if (logConsole.IsDebugEnabled)
            {
                logConsole.Debug(info);
            }
        }

        public  void Warn(string info)
        {
            if (loginfo.IsWarnEnabled)
            {
                loginfo.Warn(info);
            }
            if (logConsole.IsWarnEnabled)
            {
                logConsole.Warn(info);
            }
        }
        public  void Error(string info)
        {
            if (loginfo.IsErrorEnabled)
            {
                loginfo.Error(info);
            }

            if (logerror.IsErrorEnabled)
            {
                logerror.Error(info);
            }
            if (logConsole.IsErrorEnabled)
            {
                logConsole.Error(info);
            }
        }
       
        public  void Fetal(string info)
        {
            if (loginfo.IsFatalEnabled)
            {
                loginfo.Fatal(info);
            }

            if (logerror.IsFatalEnabled)
            {
                logerror.Fatal(info);
            }
            if (logConsole.IsFatalEnabled)
            {
                logConsole.Fatal(info);
            }
        }
        


    }
}
