//===============================================================================
// Microsoft patterns & practices
// Developing Applications for SharePoint 2010
//===============================================================================
// Copyright Microsoft Corporation.  All rights reserved.
// This code released under the terms of the 
// Microsoft patterns & practices license (http://msdn.microsoft.com/en-us/library/ee663037.aspx)
//===============================================================================


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Resources;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;
using RhDev.SharePoint.Common.Utils;
using RhDev.SharePoint.Common.Logging;
using System.Collections.Concurrent;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Logging
{
    /// <summary>
    /// Provides a diagnostic logging for Windows SharePoint Services.
    /// </summary>
    [Guid("1BA36583-1EDB-4AB6-92C5-ACF18FB742AA")]
    public class DiagnosticsService : SPDiagnosticsServiceBase
    {
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public DiagnosticsService()
            : base(Common.Logging.Constants.DefaultAreaName, SPFarm.Local)
        {
        }
        /// <summary>
        /// Initializes a new instance of the DiagnosticService class.
        /// </summary>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public DiagnosticsService(DiagnosticsServiceConfiguration config)
            : base($"{config.Name} - Diagnostics service", SPFarm.Local)
        {
            Guard.NotNull(config, nameof(config));
            Guard.StringNotNullOrWhiteSpace(config.Name, nameof(config.Name));
            
            Area = config.Name;

            if (!Equals(null, config.AvailableCategories))
            {
                Categories = config.AvailableCategories.Select(c => c.Category).ToArray();
                TraceSeverities = config.AvailableCategories.Select(c => c.TraceSeverity).ToArray();
                EventSeverities = config.AvailableCategories.Select(c => c.EventSeverity).ToArray();
            }
        }

        private volatile static ConcurrentDictionary<string, DiagnosticsService> _localServices 
            = new ConcurrentDictionary<string, DiagnosticsService>();
                
        [Persisted]
        private readonly string Area;
        [Persisted]
        private readonly string[] Categories;
        [Persisted]
        private readonly Common.Logging.TraceSeverity[] TraceSeverities;
        [Persisted]
        private readonly Common.Logging.EventSeverity[] EventSeverities;

        private static DiagnosticsService GetLocalService(DiagnosticsServiceConfiguration config)
        {
            Guard.NotNull(config, nameof(config));
            Guard.StringNotNullOrWhiteSpace(config.Name, nameof(config.Name));

            lock(typeof(DiagnosticsService))
            {
                if (_localServices.TryGetValue(config.Name, out DiagnosticsService ds)) return ds;

                var newDs = new DiagnosticsService(config);

                if (!_localServices.TryAdd(config.Name, newDs)) throw new Exception("Cannot add new key to concurrent dictionary used by local service");

                return newDs;
            }
        }


        /// <summary>
        /// Gets the local instance of the class and registers it.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public static DiagnosticsService Local(DiagnosticsServiceConfiguration config)
        {
            return GetLocalService(config);
        }

        /// <summary>
        /// Registers the class.  
        /// </summary>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public static void Register(DiagnosticsServiceConfiguration config)
        {
            Guard.NotNull(config, nameof(config));

            DiagnosticsService diagnositicService = new DiagnosticsService(config);            
            diagnositicService.Update();

            if (diagnositicService.Status != SPObjectStatus.Online)
                diagnositicService.Provision();
        }

        /// <summary>
        /// Unregisters the class. 
        /// </summary>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public static void Unregister(DiagnosticsServiceConfiguration config)
        {
            Guard.NotNull(config, nameof(config));

            var ds = SPFarm.Local.Services.FirstOrDefault(s => s.Name.Equals(config.Name));

            if (!Equals(null, ds))
            {
                ds.Delete();
                ds.Unprovision();
                ds.Uncache();
            }

        }

        /// <summary>
        /// Finds the desired SharePoint diagnostic category based on the name passed in.
        /// </summary>
        /// <param name="categoryPath">>The name of the diagnostic category.</param>
        /// <returns>The sharepoint diagnostics category found.</returns>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public virtual SPDiagnosticsCategory FindCategory(string categoryPath, out string categoryDescriptor)
        {
            Guard.StringNotNullOrWhiteSpace(categoryPath, nameof(categoryPath));

            string[] categoryPathElements = ParseCategoryPath(categoryPath);
            string areaName = categoryPathElements[0];
            string categoryName = categoryPathElements[1];

            categoryDescriptor = categoryName;

            SPDiagnosticsCollection<SPDiagnosticsArea> areas = this.Areas;

            if (areas == null)
                return DefaultCategory();

            foreach (SPDiagnosticsArea area in areas)
            {
                if (areaName.Trim().Equals(area.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    SPDiagnosticsCategory foundCategory = area.Categories.FirstOrDefault(delegate(SPDiagnosticsCategory category)
                    {
                        return categoryName.Equals(category.Name, StringComparison.OrdinalIgnoreCase);
                    });

                    return foundCategory;
                }
            }

            return null;
        }

        /// <summary>
        /// Provides the collection of diagnostic areas getting them from the configuration.
        /// </summary>
        /// <returns>An Enumerable collection of diagnostic areas.</returns>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            var categories = new List<SPDiagnosticsCategory>();
            //categories.Add(DiagnosticsCategory.DefaultSPDiagnosticsCategory);

            AddCategories(categories);

            SPDiagnosticsArea area = new SPDiagnosticsArea(Area, categories);
            IList<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea> { area };

            return areas;
        }

        private void AddCategories(List<SPDiagnosticsCategory> categories)
        {
            Guard.NotNull(Categories, nameof(Categories));
                        
            for (var i = 0; i < this.Categories.Length;i++)
            {
                var categoryToAdd = new DiagnosticsCategory(
                    this.Categories[i],
                    Equals(null, this.EventSeverities) ? DiagnosticsCategory.DefaultEventSeverity : (Microsoft.SharePoint.Administration.EventSeverity)this.EventSeverities[i],
                    Equals(null, this.TraceSeverities) ? DiagnosticsCategory.DefaultTraceSeverity : (Microsoft.SharePoint.Administration.TraceSeverity)this.TraceSeverities[i]);

                categories.Add(categoryToAdd.ToSPDiagnosticsCategory());
            }
        }

        /// <summary>
        /// Returns the default diagnostic category.
        /// </summary>
        /// <returns>A default instance of SPDiagnosticsCategory.</returns>

        public virtual SPDiagnosticsCategory DefaultCategory(string categoryDescriptor = null)
        {
            var local = GetLocalService(Common.Logging.Constants.GetDiagnosticsServiceConfiguration);
            Guard.NotNull(local, nameof(local));

            var area = local.Area;
            var categories = local.Categories;

            Guard.StringNotNullOrWhiteSpace(area, nameof(area), "No default RhDev Logging service was registered. Please register this service first");
            Guard.CollectionNotNullAndNotEmpty(categories, nameof(categories), "No categories for default RhDev Logging service was registered.");

            var defaultArea = local.Areas[area];

            Guard.NotNull(defaultArea, nameof(defaultArea));

            Guard.CollectionNotNullAndNotEmpty(
                defaultArea.Categories.ToList(),
                nameof(defaultArea), "Default area categories are empty, please specify default categories when register the diagnostics service");

            var category = !string.IsNullOrWhiteSpace(categoryDescriptor) ? defaultArea.Categories[categoryDescriptor] : defaultArea.Categories[0];

            if (Equals(null, category)) category = defaultArea.Categories[categories[0]];

            return category;
        }

        /// <summary>
        /// Logs an event to the event log using default severity for category.
        /// </summary>
        /// <param name="message">the message to log</param>
        /// <param name="eventId">the id of the event</param>
        /// <param name="category">the cateogry for the event</param>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public virtual void LogEvent(string message, int eventId, string category)
        {
            Guard.StringNotNullOrWhiteSpace(message, nameof(message));

            SPDiagnosticsCategory spCategory = GetCategory(category);
            LogEvent(message, eventId, spCategory.DefaultEventSeverity, spCategory);
        }

        /// <summary>
        /// Logs an event to the event log.
        /// </summary>
        /// <param name="message">the message to log</param>
        /// <param name="eventId">the id of the event</param>
        /// <param name="severity">the severity of the event</param>
        /// <param name="categoryName">the cateogry for the event</param>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public virtual void LogEvent(string message, int eventId, Microsoft.SharePoint.Administration.EventSeverity severity, string categoryName)
        {
            Guard.StringNotNullOrWhiteSpace(message, nameof(message));

            SPDiagnosticsCategory category = GetCategory(categoryName);
            LogEvent(message, eventId, severity, category);
        }


        private void LogEvent(string message, int eventId, Microsoft.SharePoint.Administration.EventSeverity severity, SPDiagnosticsCategory spCategory)
        {
            string formattedMessage = string.Format(CultureInfo.CurrentCulture, "Category: {0}: {1}", spCategory.Name, message);
            base.WriteEvent((ushort)eventId, spCategory, severity, formattedMessage, null);
        }


        /// <summary>
        /// Logs an trace to the ULS log.
        /// </summary>
        /// <param name="message">the message to log</param>
        /// <param name="eventId">the id of the trace</param>
        /// <param name="severity">the severity of the trace</param>
        /// <param name="categoryName">the category for the trace</param>       
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public virtual void LogTrace(string message, int eventId, Microsoft.SharePoint.Administration.TraceSeverity severity, string categoryName)
        {
            Guard.StringNotNullOrWhiteSpace(message, nameof(message));

            SPDiagnosticsCategory category = GetCategory(categoryName);
            base.WriteTrace((uint)eventId, category, severity, message, null);
        }

        /// <summary>
        /// Logs an trace to the ULS log.
        /// </summary>
        /// <param name="message">the message to log</param>
        /// <param name="eventId">the id of the trace</param>
        /// <param name="category">the category for the trace</param>       
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public virtual void LogTrace(string message, int eventId, string category)
        {
            SPDiagnosticsCategory spCategory = GetCategory(category);
            base.WriteTrace((uint)eventId, spCategory, spCategory.DefaultTraceSeverity, message, null);
        }

        #region Private Members

        /// <summary>
        /// Gets the category for the provided category name.  Throws a logging exception if the 
        /// category is not found.  Returns default category for null or empty category name.
        /// </summary>
        /// <param name="categoryName">The category to find</param>
        /// <returns>the SPCategory for the category name provided</returns>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        private SPDiagnosticsCategory GetCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                return DefaultCategory();
            else
            {
                SPDiagnosticsCategory foundCategory = FindCategory(categoryName, out string categoryDescriptor) ?? DefaultCategory(categoryDescriptor);
                if (foundCategory == null)
                    throw new LoggingException(string.Format(CultureInfo.CurrentCulture, DiagnosticsServiceStrings.CategoryNotFoundExceptionMessage, categoryName));
                return foundCategory;
            }
        }

        private static string[] ParseCategoryPath(string categoryPath)
        {
            if (categoryPath.IndexOf(Common.Logging.Constants.CategoryPathSeparator) <= 0)
            {
                throw new LoggingException(String.Format(DiagnosticsServiceStrings.CategoryNotFoundExceptionMessage, categoryPath));
            }

            string[] categoryPathElements = categoryPath.Split(new char[] { Common.Logging.Constants.CategoryPathSeparator }, StringSplitOptions.RemoveEmptyEntries);

            if (categoryPathElements.Length != 2)
            {
                throw new LoggingException(string.Format(CultureInfo.CurrentCulture, DiagnosticsServiceStrings.InvalidCategoryFormat, categoryPath));
            }

            return categoryPathElements;
        }

        #endregion
    }
}
