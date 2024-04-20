using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Utils;
using System;
using System.Collections.Generic;
using TraceSeverity = RhDev.SharePoint.Common.Logging.TraceSeverity;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Logging
{
    public class SharePointTraceLogger : ITraceLogger
    {
        private DiagnosticsService DiagnosticsService => DiagnosticsService.Local(GetConfiguration);

        public virtual DiagnosticsServiceConfiguration GetConfiguration => Common.Logging.Constants.GetDiagnosticsServiceConfiguration;

        public void Register()
        {
            
            DiagnosticsService.Register(GetConfiguration);
        }

        public void Unregister()
        {            
            DiagnosticsService.Unregister(GetConfiguration);
        }

        public void Write(TraceSeverity severity, string message, params object[] parameters)
        {
            string formattedMessage = FormatMessage(message, parameters);
            DiagnosticsService.LogTrace(formattedMessage, 0, (Microsoft.SharePoint.Administration.TraceSeverity) severity, null);
        }

        public void Write(string message, params object[] parameters)
        {
            string formattedMessage = FormatMessage(message, parameters);
            DiagnosticsService.LogTrace(formattedMessage, 0, null);
        }

        public void Write(TraceCategory category, string message, params object[] parameters)
        {
            string formattedMessage = FormatMessage(message, parameters);
            string categoryPath = GetCategoryPath(category);

            DiagnosticsService.LogTrace(formattedMessage, 0, categoryPath);
        }
        
        public void Write(Exception exception, string additionalMessage, params object[] parameters)
        {
            string formattedMessage = FormatMessage(additionalMessage, parameters);
            string messageWithException = FormatMessageWithException(exception, formattedMessage);

            DiagnosticsService.LogTrace(messageWithException, 0, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, null);
        }

        public void Write(TraceSeverity severity, TraceCategory category, string message, params object[] parameters)
        {
            string formattedMessage = FormatMessage(message, parameters);
            string categoryPath = GetCategoryPath(category);

            DiagnosticsService.LogTrace(formattedMessage, 0, (Microsoft.SharePoint.Administration.TraceSeverity) severity, categoryPath);
        }

        public void Write(Exception exception, TraceSeverity severity, TraceCategory category)
        {
            string messageWithException = !Equals(null, exception.InnerException)
                ? string.Format("{0} - {1}", exception.InnerException, exception)
                : exception.ToString();

            string categoryPath = GetCategoryPath(category);

            DiagnosticsService.LogTrace(messageWithException, 0, (Microsoft.SharePoint.Administration.TraceSeverity) severity, categoryPath);
        }

        public void Write(Exception exception, TraceSeverity severity, TraceCategory category, string additionalMessage, params object[] parameters)
        {
            string formattedMessage = FormatMessage(additionalMessage, parameters);
            string messageWithException = FormatMessageWithException(exception, formattedMessage);
            string categoryPath = GetCategoryPath(category);

            DiagnosticsService.LogTrace(messageWithException, 0, (Microsoft.SharePoint.Administration.TraceSeverity) severity, categoryPath);
        }

        private static string FormatMessage(string message, object[] parameters)
        {
            return String.Format(message, parameters);
        }

        private static string GetCategoryPath(TraceCategory category)
        {
            return category != null ? category.ToString() : null;
        }

        private static string FormatMessageWithException(Exception exception, string formattedMessage)
        {
            return String.Format("{0}\nException: {1}", formattedMessage, exception);
        }

        public void Event(TraceCategory category, string message, params object[] parameters)
        {
            string formattedMessage = FormatMessage(message, parameters);
            string categoryPath = GetCategoryPath(category);

            DiagnosticsService.LogEvent(formattedMessage, 0, categoryPath);
        }
    }
}
