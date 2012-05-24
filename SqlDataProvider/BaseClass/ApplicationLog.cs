using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Xml;

using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace SqlDataProvider.BaseClass
{
    public static class ApplicationLog
    {
        #region
        public static void WriteError(String message)
        {
            WriteLog(TraceLevel.Error, message);
        }
        private static void WriteLog(TraceLevel level, String messageText)
        {
            try
            {
                EventLogEntryType LogEntryType;
                switch (level)
                {
                    case TraceLevel.Error:
                        LogEntryType = EventLogEntryType.Error;
                        break;
                    default:
                        LogEntryType = EventLogEntryType.Error;
                        break;
                }
                String LogName = "Application";
                if (!EventLog.SourceExists(LogName))
                {
                    EventLog.CreateEventSource(LogName, "BIZ");
                }

                EventLog eventLog = new EventLog(LogName, ".", LogName);//日志所以的机器
                eventLog.WriteEntry(messageText, LogEntryType);
            }
            catch
            { 
            }
        }
        #endregion
    }
}
