using System;
using RhDev.SharePoint.Common.Logging;

namespace RhDev.SharePoint.Common.Caching.Composition
{
    public abstract class ServiceBase : IService
    {
        private readonly ITraceLogger traceLogger;
        protected virtual TraceCategory TraceCategory => TraceCategories.General;

        protected ServiceBase(ITraceLogger traceLogger)
        {
            this.traceLogger = traceLogger;
        }

        protected void WriteVerboseTrace(string message, params object[] args)
        {
            traceLogger.Write(TraceSeverity.Verbose, TraceCategory, message, args);
        }

        protected void WriteTrace(string message, params object[] args)
        {
            traceLogger.Write(TraceCategory, message, args);
        }

        protected void WriteTrace(TraceSeverity severity, string message, params object[] args)
        {
            traceLogger.Write(severity, TraceCategory, message, args);
        }

        protected void WriteUnexpectedTrace(string message, params object[] args)
        {
            traceLogger.Write(TraceSeverity.Unexpected, TraceCategory, message, args);
        }

        protected void WriteUnexpectedTrace(Exception exception, string message, params object[] args)
        {
            traceLogger.Write(exception, TraceSeverity.Unexpected, TraceCategory, message, args);
        }
    }
}
