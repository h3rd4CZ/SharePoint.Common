//===============================================================================
// Microsoft patterns & practices
// Developing Applications for SharePoint 2010
//===============================================================================
// Copyright Microsoft Corporation.  All rights reserved.
// This code released under the terms of the 
// Microsoft patterns & practices license (http://msdn.microsoft.com/en-us/library/ee663037.aspx)
//===============================================================================


using Microsoft.SharePoint.Administration;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Utils;
using EventSeverity = RhDev.SharePoint.Common.Logging.EventSeverity;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Logging
{
    /// <summary>
    /// Class representing an area for diagnostic purpose.
    /// </summary>
    public class DiagnosticsCategory
    {
        /// <summary>
        /// The default event severity for a diagnostics category
        /// </summary>
        public const Microsoft.SharePoint.Administration.EventSeverity DefaultEventSeverity = Microsoft.SharePoint.Administration.EventSeverity.Warning;

        /// <summary>
        /// The default trace severity for a diagnostics category
        /// </summary>
        public const Microsoft.SharePoint.Administration.TraceSeverity DefaultTraceSeverity = Microsoft.SharePoint.Administration.TraceSeverity.Medium;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsCategory"/> class.
        /// </summary>
        public DiagnosticsCategory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsCategory"/> class.
        /// </summary>
        /// <param name="name">The name of the diagnostic category.</param>
         public DiagnosticsCategory(string name):
            this(name, DefaultEventSeverity, DefaultTraceSeverity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsCategory"/> class.
        /// </summary>
        /// <param name="name">The name of the diagnostic category.</param>
        /// <param name="eventSeverity">The category's event severity.</param>
        /// <param name="traceSeverity">The category´s trace severity</param>
        public DiagnosticsCategory(string name, Microsoft.SharePoint.Administration.EventSeverity eventSeverity, Microsoft.SharePoint.Administration.TraceSeverity traceSeverity)
        {
            this.Name = name;
            this.EventSeverity = eventSeverity;
            this.TraceSeverity = traceSeverity;
        }

        private uint id;
        private string name;
        private Microsoft.SharePoint.Administration.TraceSeverity traceSeverity;
        private Microsoft.SharePoint.Administration.EventSeverity eventSeverity;

        /// <summary>
        /// The id of the diagnostic category
        /// </summary>
        public uint Id
        {
            get { return id; }

            set { id = value; }
        }

        /// <summary>
        /// The name of the diagnostic category
        /// </summary>
        public string Name
        {
             get { return name; }

            set
            {
                Guard.StringNotNullOrWhiteSpace(value, nameof(value));
                name = value;
            }
        }

        /// <summary>
        /// Gets or sets the trace severity.
        /// </summary>
        public Microsoft.SharePoint.Administration.TraceSeverity TraceSeverity
        {
             get { return traceSeverity; }

            set { traceSeverity = value; }
        }

        /// <summary>
        /// Gets or sets the event severity.
        /// </summary>
        public Microsoft.SharePoint.Administration.EventSeverity EventSeverity
        {
            get { return eventSeverity; }

            set { eventSeverity = value; }
        }

        /// <summary>
        /// Converts from <see cref="DiagnosticsCategory"/> to <see cref="SPDiagnosticsCategory"/> instance.
        /// </summary>
        /// <returns></returns>
        public SPDiagnosticsCategory ToSPDiagnosticsCategory()
        {
            return new SPDiagnosticsCategory(
                this.Name, this.Name, this.TraceSeverity, this.EventSeverity, 0, this.Id, false, true);
        }

        /// <summary>
        /// Gets the default <see cref="SPDiagnosticsCategory"/> instance.
        /// </summary>
        public static SPDiagnosticsCategory DefaultSPDiagnosticsCategory
        {
            get
            {
                return new SPDiagnosticsCategory(Common.Logging.Constants.GeneralCategoryName,
                    Common.Logging.Constants.GeneralCategoryName, Microsoft.SharePoint.Administration.TraceSeverity.Medium, Microsoft.SharePoint.Administration.EventSeverity.Information, 0, 0, false, true);
            }
        }
    }
}
