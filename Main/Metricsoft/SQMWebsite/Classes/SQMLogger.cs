using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace SQM.Website
{

    public static class SQMLogger
    {

        static public void LogFunctionEntry()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            LogEntry log = new LogEntry();
            log.Message = "Enter: " + methodBase.Name;
            log.Severity = TraceEventType.Start;


            Logger.Write(log);

        }

        static public void LogFunctionExit()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            LogEntry log = new LogEntry();
            log.Message = "Exit: " + methodBase.Name;
            log.Severity = TraceEventType.Stop;


            Logger.Write(log);

        }

        static public void LogException(Exception e)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            LogEntry log = new LogEntry();
            log.Message = "Method: " + methodBase.Name + " Exception: " + e.Message + " Stack Trace: " + stackTrace.ToString();
            log.Severity = TraceEventType.Critical;


            Logger.Write(log);

        }

        static public void LogException(Exception e, Guid errorIndex)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            String additionalMsg = ""; 

            if (e.Message == "File does not exist.")
            {
                additionalMsg = HttpContext.Current.Request.Url.ToString();
            }

            LogEntry log = new LogEntry();
            log.Message = "Error Index: " + errorIndex.ToString() + " Method: " + methodBase.Name + " Exception: " + e.Message + " Stack Trace: " + stackTrace.ToString();
            if (!String.IsNullOrEmpty(additionalMsg))
            {
                log.Message += "Additional Info: " + additionalMsg;
            }
            log.Severity = TraceEventType.Critical;

            Logger.Write(log);

        }

        static public void Log(String msg)
        {
            LogEntry log = new LogEntry();
            log.Message = msg;
            log.Severity = TraceEventType.Information;
            Logger.Write(log);
        }

        static public void Log(LogEntry le)
        {
            Logger.Write(le);
        }
    }
}
