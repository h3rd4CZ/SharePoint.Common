using System;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.Logging;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Utils
{
    public class DisabledEventReceiversScope : SPItemEventReceiver, IDisposable
    {
        readonly bool oldValue;
        private readonly ITraceLogger traceLogger;

        public DisabledEventReceiversScope(ITraceLogger traceLogger)
        {
            this.traceLogger = traceLogger;

            WriteToLog("Disabling event receivers for scope");

            oldValue = EventFiringEnabled;
            EventFiringEnabled = false;
        }

        public void Dispose()
        {
            EventFiringEnabled = oldValue;

            WriteToLog("Event receivers has been returned to old values");
        }

        private void WriteToLog(string message)
        {
            traceLogger.Write(TraceCategories.General, message);
        }
    }
}
