namespace RhDev.SharePoint.Common.Logging
{
    public enum TraceSeverity
    {
        /// <summary>
        /// Writes high-level detail to the trace log file.
        /// </summary>
        High = 20,
        /// <summary>
        /// Writes medium-level detail to the trace log file
        /// </summary>
        Medium = 50,
        /// <summary>
        ///  Represents an unusual code path and actions that should be monitored.  
        /// </summary>
        Monitorable = 15,
        /// <summary>
        ///  Writes no trace information to the trace log file.  
        /// </summary>
        None = 0,
        /// <summary>
        ///  Represents an unexpected code path and actions that should be monitored.
        /// </summary>
        Unexpected = 10,
        /// <summary>
        ///  Writes low-level detail to the trace log file.   
        /// </summary>
        Verbose = 100
    }
}
