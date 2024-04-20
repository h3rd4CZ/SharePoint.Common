//===============================================================================
// Microsoft patterns & practices
// Developing Applications for SharePoint 2010
//===============================================================================
// Copyright Microsoft Corporation.  All rights reserved.
// This code released under the terms of the 
// Microsoft patterns & practices license (http://msdn.microsoft.com/en-us/library/ee663037.aspx)
//===============================================================================


using System.Collections.Generic;
using Microsoft.SharePoint.Administration;
using System;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Logging
{
    /// <summary>
    /// Class representing an area for diagnostic purpose.
    /// </summary>
    public class DiagnosticsArea
    {
        private string name;
        private DiagnosticsCategoryCollection diagnosticsCategories = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsArea"/> class.
        /// </summary>
        public DiagnosticsArea()
        {
            this.diagnosticsCategories = new DiagnosticsCategoryCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsArea"/> class.
        /// </summary>
        /// <param name="name">The name of the diagnostic area.</param>
        public DiagnosticsArea(string name) : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsArea"/> class.
        /// </summary>
        /// <param name="name">The name of the diagnostic area.</param>
        /// <param name="diagnosticsCategories">A collection of <see cref="DiagnosticsCategory"/> that will be part of the area.</param>
        public DiagnosticsArea(string name, DiagnosticsCategoryCollection diagnosticsCategories)
        {
            this.Name = name;

            if (diagnosticsCategories == null)
                this.diagnosticsCategories = new DiagnosticsCategoryCollection();
            else
                this.diagnosticsCategories = new DiagnosticsCategoryCollection(diagnosticsCategories);
        }

        /// <summary>
        /// The name of the diagnostic area
        /// </summary>
        public string Name
        {
            get { return name; }

             set
             {
                 if (value == null) throw new ArgumentNullException("name");
                name = value;
            }
        }
        
        /// <summary>
        /// Gets the collection of <see cref="DiagnosticsCategory"/>.
        /// </summary>
        public DiagnosticsCategoryCollection DiagnosticsCategories
        {
            get
            {
                if (diagnosticsCategories == null)
                    diagnosticsCategories = new DiagnosticsCategoryCollection();

                return diagnosticsCategories;
            }
        }

        /// <summary>
        /// Converts from <see cref="DiagnosticsArea"/> to <see cref="SPDiagnosticsArea"/> instance.
        /// </summary>
        /// <returns>the SPDiagnosticsArea created from this diagnostics area</returns>
        public SPDiagnosticsArea ToSPDiagnosticsArea()
        {
            var categories = new List<SPDiagnosticsCategory>();
            foreach (DiagnosticsCategory category in this.DiagnosticsCategories)
                categories.Add(category.ToSPDiagnosticsCategory());

            return new SPDiagnosticsArea(this.Name, categories);
        }
     }
 }
