using RhDev.SharePoint.Common.Caching.Composition;
using System;

namespace RhDev.SharePoint.Common.Logging
{
    public interface ITraceLogger : IService
    {
        void Register();

        void Unregister();

        /// <summary>
        /// Write a diagnostic message into the log for the default category with severity.
        /// </summary>
        /// <param name="severity">The severity of the exception.</param>
        /// <param name="message">The message to write into the log.</param>
        /// <param name="parameters"> </param>
        void Write(TraceSeverity severity, string message, params object[] parameters);

        /// <summary>
        /// Writes information about an exception into the log.
        /// </summary>
        /// <param name="exception">The exception to write into the log to be read by operations. </param>
        /// <param name="additionalMessage">Additional information about the exception message.</param>
        /// <param name="parameters"> </param>
        void Write(Exception exception, string additionalMessage, params object[] parameters);

        /// <summary>
        /// Writes a diagnostic message into the trace log, with specified <see cref="TraceSeverity"/>.
        /// </summary>
        /// <param name="severity">The severity of the trace.</param>
        /// <param name="category">The category to write the message to.</param>
        /// <param name="message">The message to write into the log.</param>
        /// <param name="parameters"> </param>
        void Write(TraceSeverity severity, TraceCategory category, string message, params object[] parameters);

        /// <summary>
        /// Writes information about an exception into the log.
        /// </summary>
        /// <param name="exception">The exception to write into the log. </param>
        /// <param name="severity">The severity of the exception.</param>
        /// <param name="category">The category to write the message to.</param>
        void Write(Exception exception, TraceSeverity severity, TraceCategory category);

        /// <summary>
        /// Writes information about an exception into the log.
        /// </summary>
        /// <param name="exception">The exception to write into the log. </param>
        /// <param name="severity">The severity of the exception.</param>
        /// <param name="category">The category to write the message to.</param>
        /// <param name="additionalMessage">Additional information about the exception message.</param>
        /// <param name="parameters"> </param>
        void Write(Exception exception, TraceSeverity severity, TraceCategory category, string additionalMessage, params object[] parameters);

        void Write(string message, params object[] parameters);

        void Write(TraceCategory category, string message, params object[] parameters);

        void Event(TraceCategory category, string message, params object[] parameters);
    }
}
