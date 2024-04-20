using System;

namespace RhDev.SharePoint.Common.Logging
{
    public class ConsoleTraceLogger : ITraceLogger
    {
        public void Register()
        {
            
        }
                
        public void Unregister()
        {
            
        }

        public void Write(TraceSeverity severity, string message, params object[] parameters)
        {
            Console.WriteLine(message, parameters);
        }

        public void Write(Exception exception, string additionalMessage, params object[] parameters)
        {
            Console.WriteLine(additionalMessage, parameters);
        }

        public void Write(TraceSeverity severity, TraceCategory category, string message, params object[] parameters)
        {
            Console.WriteLine(message, parameters);
        }

        public void Write(Exception exception, TraceSeverity severity, TraceCategory category)
        {
            Console.WriteLine(exception);
        }

        public void Write(Exception exception, TraceSeverity severity, TraceCategory category, string additionalMessage, params object[] parameters)
        {
            Console.WriteLine(additionalMessage, parameters);
        }

        public void Write(string message, params object[] parameters)
        {
            Console.WriteLine(message, parameters);
        }

        public void Write(TraceCategory category, string message, params object[] parameters)
        {
            Console.WriteLine(message, parameters);
        }

        public void Event(TraceCategory category, string message, params object[] parameters)
        {
            Console.WriteLine(message, parameters);
        }
    }
}
