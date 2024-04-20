using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebPartPages;
using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Utils;
using SystemWebPart = System.Web.UI.WebControls.WebParts.WebPart;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Utils
{
    public class PageArrangement : ServiceBase
    {
        protected override TraceCategory TraceCategory => TraceCategories.FrontEnd;

        public PageArrangement(ITraceLogger traceLogger) : base(traceLogger) { }
        
        public void Process(SPWeb web, PageArrangementConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (web == null)
                throw new ArgumentNullException("web");

            if (config.Pages != null)
                foreach (PageArrangementConfiguration.ArrangementPage page in config.Pages)
                {
                    if (page.WebPartZones == null) continue;
                    ProcessPageArrangement(web, page);
                }
        }

        private void ProcessPageArrangement(SPWeb web, PageArrangementConfiguration.ArrangementPage page)
        {
            // ensure page
            if (page.CreatePage)
                EnsurePageCreated(web, page);

            SPLimitedWebPartManager manager = null;
            try
            {
                if (page.PublishingPage)
                {
                    SPFile file = web.GetFile(page.Url);
                    if (file != null && file.Exists)
                    {
                        try
                        {
                            file.CheckOut();
                        }
                        catch (SPFileCheckOutException)
                        {

                        }
                    }
                }


                manager = web.GetLimitedWebPartManager(page.Url, PersonalizationScope.Shared);
                if (manager == null)
                {
                    WriteTrace("Cannot obtain web part manager");
                    return;
                }

                foreach (PageArrangementConfiguration.WebPartZone wpZone in page.WebPartZones)
                {
                    if (wpZone.InitAction != PageArrangementConfiguration.WebPartZoneInitAction.None)
                        PerformWebPartZoneAction(manager, wpZone);

                    if (wpZone.WebParts != null)
                        foreach (PageArrangementConfiguration.WebPartZoneRegisterWebPart wp in wpZone.WebParts)
                        {
                            try
                            {
                                if (string.IsNullOrEmpty(wp.ContextCreatorTypeName))
                                    RegisterWebPart(manager, wpZone, wp);
                                else
                                    InstantiateWebPart(manager, wpZone, wp);
                            }
                            catch (WebPartInstantiateException ex)
                            {
                                string message = string.Format("Webovou část '{0}' se nepodařilo vložit. Kontaktujte administrátory.", ex.TypeName);

                                WriteUnexpectedTrace(ex, message);

                                SimpleFormWebPart errWp = new SimpleFormWebPart
                                {
                                    Content = string.Format("<span class=\"dmss-error\">{0}</span>", message)
                                };

                                manager.AddWebPart(errWp, wpZone.WebPartZoneId, wp.WebPartZoneIndex);
                            }
                        }
                }

                if (page.SetHomePage)
                {
                    SPFolder oFolder = web.RootFolder;
                    oFolder.WelcomePage = page.Url;
                    oFolder.Update();
                }
            }
            finally
            {
                if (page.PublishingPage)
                {
                    SPFile file = web.GetFile(page.Url);
                    if (file != null && file.Exists)
                        file.CheckIn("File checked-in by page arrangement.", SPCheckinType.MajorCheckIn);
                }

                if (manager != null)
                    manager.Dispose();
            }
        }

        private void EnsurePageCreated(SPWeb web, PageArrangementConfiguration.ArrangementPage page)
        {
            web.AllowUnsafeUpdates = true;

            string hive = SPUtility.GetVersionedGenericSetupPath(string.Format("TEMPLATE\\{0}\\STS\\DOCTEMP\\SMARTPGS\\", web.Language), 16);

            string templatePath = Path.Combine(hive, page.TemplatePath);
            string[] urlParts = page.Url.Split('/');
            if (urlParts.Length != 2)
                throw new InvalidOperationException("Invalid page URL. Expected 2 parts.");

            SPFolder folder = web.GetFolder(urlParts[0]);
            if (folder == null)
            {
                WriteUnexpectedTrace("Root folder is null '{0}'", urlParts[0]);
                return;
            }

            WriteTrace("Publish page (web: '{0}' newfile: '{1}' template: '{2}' listId: '{3}')", web.Url, page.Url, templatePath, folder.ParentListId);

            if (page.PublishingPage)
            {
                try
                {
                    SPFile oldFile = folder.Files[page.Url];

                    if (oldFile != null && oldFile.Exists)
                    {
                        try
                        {
                            oldFile.CheckOut();
                        }
                        catch { }
                        
                    }
                }
                catch { }
                
            }

            WriteTrace("Thread identity: {0}", Thread.CurrentPrincipal.Identity.Name);

            var windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity != null)
                WriteTrace("Windows identity: {0}", windowsIdentity.Name);

            using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                //try
                //{
                SPFile file = folder.Files.Add(urlParts[1], fileStream, true);

                if (!string.IsNullOrEmpty(page.Title))
                {
                    SPItem item = file.Item;

                    item["Title"] = page.Title;
                    item.Update();
                }

                if (page.PublishingPage)
                    file.CheckIn("File checked-in by page arrangement", SPCheckinType.MajorCheckIn);

                WriteTrace("File successfully created: '{0}'", file.Url);
                //}
                //catch (Exception ex)
                //{
                //    Logger.Log(typeof(PagePublisher), LogLevel.Error, () => new LogDetails(ex, "Error during publish page (web: {0} newfile: {1} templateName: {2} listId: {3})", web.Url, newFileName, templateName, listId));
                //}
            }

            web.AllowUnsafeUpdates = false;
        }

        private void PerformWebPartZoneAction(SPLimitedWebPartManager manager, PageArrangementConfiguration.WebPartZone webPartZone)
        {
            switch (webPartZone.InitAction)
            {
                case PageArrangementConfiguration.WebPartZoneInitAction.Clear:
                    ClearZone(manager, webPartZone.WebPartZoneId);
                    break;
            }
        }

        #region ------ Add Web Part ---------------------------------------------------------------

        private void RegisterWebPart(SPLimitedWebPartManager manager, PageArrangementConfiguration.WebPartZone wpZone, PageArrangementConfiguration.WebPartZoneRegisterWebPart wp)
        {
            WriteTrace("Creating web part of type: {0}...", wp.TypeName);

            // get web part type
            Type webPartType = Type.GetType(wp.TypeName, false);
            if (webPartType == null)
            {
                throw new WebPartInstantiateException(
                    string.Format("Web part not found: {0}", wp.TypeName),
                    wp.TypeName);
            }

            // create web part
            SystemWebPart webPart = Activator.CreateInstance(webPartType) as SystemWebPart;
            if (webPart == null)
            {
                throw new WebPartInstantiateException(
                    string.Format("'{0}' is not web part.", webPartType.Name),
                    webPartType.Name);
            }

            WriteTrace("Web part created.");

            // add web part to zone
            manager.AddWebPart(webPart, wpZone.WebPartZoneId, wp.WebPartZoneIndex);

            WriteTrace("Web part added into '{0}' zone.", wpZone.WebPartZoneId);

            // setup web part
            SetupWebPart(webPart, wp);
            manager.SaveChanges(webPart);

            WriteTrace("Web part arrangement OK.");
        }

        private void InstantiateWebPart(SPLimitedWebPartManager manager, PageArrangementConfiguration.WebPartZone wpZone, PageArrangementConfiguration.WebPartZoneRegisterWebPart wp)
        {
            WriteTrace("Instantiating web part of type: {0}...", wp.ContextCreatorTypeName);

            // get web part type
            Type instPartType = Type.GetType(wp.ContextCreatorTypeName, false);
            if (instPartType == null)
            {
                throw new WebPartInstantiateException(
                    string.Format("Template part not found: {0}", wp.ContextCreatorTypeName),
                    wp.ContextCreatorTypeName);
            }

            // create web part
            IWebPartTemplator templator = Activator.CreateInstance(instPartType) as IWebPartTemplator;
            if (templator == null)
            {
                throw new WebPartInstantiateException(
                    string.Format("'{0}' is not web part templator.", instPartType.Name),
                    instPartType.GetType().FullName);
            }

            WriteTrace("Web part created.");

            SystemWebPart webPart = null;
            try
            {
                webPart = templator.InstantiateWebPart(manager.Web, wp);
            }
            catch (Exception ex)
            {
                throw new WebPartInstantiateException("Error instantiating webpart", templator.GetType().FullName, ex);
            }

            if (webPart == null)
                throw new WebPartInstantiateException("Webpart was not instantiated.", templator.GetType().FullName);

            WriteTrace("Web part created: {0}", webPart.GetType().FullName);

            // add web part to zone
            manager.AddWebPart(webPart, wpZone.WebPartZoneId, wp.WebPartZoneIndex);

            WriteTrace("Web part added into '{0}' zone.", wpZone.WebPartZoneId);

            // setup web part
            SetupWebPart(webPart, wp);
            manager.SaveChanges(webPart);

            WriteTrace("Web part arrangement OK.");
        }

        private void SetupWebPart(SystemWebPart webPart, PageArrangementConfiguration.WebPartZoneRegisterWebPart webPartItem)
        {
            if (webPartItem.Properties == null || webPartItem.Properties.Length == 0)
                return;

            WriteTrace("Setuping web part. Type={0}", webPartItem.TypeName);
            foreach (PageArrangementConfiguration.WebPartProperty property in webPartItem.Properties)
            {
                WriteTrace("{0} = {1}", property.Name, property.Value);

                SetWebPartProperty(webPart, property.Name, property.Value);
            }
        }

        #endregion

        #region ------ Set Web Part Property ------------------------------------------------------

        private void SetWebPartProperty(SystemWebPart webPart, string propertyName, string value)
        {
            switch (propertyName)
            {
                case "ChromeType":
                    SetWebPartChromeTypeProperty(webPart, value);
                    return;
                case "ChromeState":
                    SetWebPartChromeStateProperty(webPart, value);
                    return;
                case "AllowMinimize":
                    SetWebPartAllowMinimizeProperty(webPart, value);
                    return;
                case "AllowClose":
                    SetWebPartAllowCloseProperty(webPart, value);
                    return;
                case "Hidden":
                    SetWebPartHiddenProperty(webPart, value);
                    return;
                case "Width":
                    SetWebPartWidthProperty(webPart, value);
                    return;

                default:
                    break;
            }

            if (webPart is ContentEditorWebPart && propertyName == "Content")
            {
                SetContentEditorWebPartContent(webPart as ContentEditorWebPart, value);
                return;
            }

            if (webPart is TitleBarWebPart && propertyName == "Title")
            {
                SetTitleBarWebPart(webPart as TitleBarWebPart, value);
                return;
            }

            if (webPart is TitleBarWebPart && propertyName == "Image")
            {
                SetImageTitleBarWebPart(webPart as TitleBarWebPart, value);
                return;
            }

            if (webPart is DataFormWebPart && propertyName == "XslLink")
            {
                SetXslLinkDataFormWebPart(webPart as DataFormWebPart, value);
                return;
            }

            /*if (webPart is PeopleCoreResultsWebPart)
            {
                SearchWebpartUtility.SetupSearchResultsBaseWebPart(webPart as SearchResultsBaseWebPart, propertyName, value);
                SearchWebpartUtility.SetupCoreResultsWebPart(webPart as CoreResultsWebPart, propertyName, value);

                // SearchWebpartUtility.SetupPeopleCoreResultsWebPart(webPart as PeopleCoreResultsWebPart, propertyName, value);
                return;
            }

            if (webPart is TopFederatedResultsWebPart)
            {
                SearchWebpartUtility.SetupSearchResultsBaseWebPart(webPart as SearchResultsBaseWebPart, propertyName, value);
                return;
            }

            if (webPart is AdvancedSearchBox)
            {
                SearchWebpartUtility.SetupAdvancedSearchBoxWebpart(webPart as AdvancedSearchBox, propertyName, value);
                return;
            }

            if (webPart is HighConfidenceWebPart)
            {
                SearchWebpartUtility.SetupSearchResultsBaseWebPart(webPart as SearchResultsBaseWebPart, propertyName, value);
                SearchWebpartUtility.SetupHighConfidenceWebPart(webPart as HighConfidenceWebPart, propertyName, value);
                return;
            }

            if (webPart is SearchSummaryWebPart)
            {   
                // SearchWebpartUtility.SetupSearchSummaryWebPart(webPart as SearchSummaryWebPart, propertyName, value);
                return;
            }

            if (webPart is RefinementWebPart)
            {
                // SearchWebpartUtility.SetupRefinementWebPart(webPart as RefinementWebPart, propertyName, value);
                return;
            }

            if (webPart is SearchStatsWebPart)
            {
                // SearchWebpartUtility.SetupSearchStatsWebPart(webPart as SearchStatsWebPart, propertyName, value);
                return;
            }

            if (webPart is CoreResultsWebPart)
            {   


                SearchWebpartUtility.SetupSearchResultsBaseWebPart(webPart as SearchResultsBaseWebPart, propertyName, value);
                SearchWebpartUtility.SetupCoreResultsWebPart(webPart as CoreResultsWebPart, propertyName, value);
                return;
            }

            if (webPart is SearchPagingWebPart)
            {
                // SearchWebpartUtility.SetupSearchPagingWebPart(webPart as SearchPagingWebPart, propertyName, value);
                return;
            }

            if (webPart is QuerySuggestionsWebPart)
            {
                SearchWebpartUtility.SetupSearchResultsBaseWebPart(webPart as SearchResultsBaseWebPart, propertyName, value);
                SearchWebpartUtility.SetupQuerySuggestionsWebPart(webPart as QuerySuggestionsWebPart, propertyName, value);
                return;
            }

            if (webPart is FederatedResultsWebPart)
            {
                SearchWebpartUtility.SetupSearchResultsBaseWebPart(webPart as SearchResultsBaseWebPart, propertyName, value);
                SearchWebpartUtility.SetupFederatedResultsWebPart(webPart as FederatedResultsWebPart, propertyName, value);
                return;
            }

            if (webPart is PeopleSearchBoxEx)
            {
                SearchWebpartUtility.SetupSearchBoxExWebPart(webPart as SearchBoxEx, propertyName, value);
                return;
            }

            if (webPart is SearchBoxEx)
            {
                SearchWebpartUtility.SetupSearchBoxExWebPart(webPart as SearchBoxEx, propertyName, value);
                return;
            }*/

            Type webPartType = webPart.GetType();
            PropertyInfo property = webPartType.GetProperty(propertyName);
            if (property == null)
            {
                WriteTrace("'{0}' web part has no '{1}' property.", webPartType.Name, propertyName);
                return;
            }

            property.SetValue(webPart, value, null);
        }

        private void SetWebPartChromeTypeProperty(SystemWebPart webPart, string value)
        {
            try
            {
                PartChromeType chromeType = (PartChromeType)Enum.Parse(typeof(PartChromeType), value);
                webPart.ChromeType = chromeType;
            }
            catch
            {
                WriteTrace("Error setting ChromeType");
            }
        }

        private void SetWebPartChromeStateProperty(SystemWebPart webPart, string value)
        {
            try
            {
                PartChromeState chromeState = (PartChromeState)Enum.Parse(typeof(PartChromeState), value);
                webPart.ChromeState = chromeState;
            }
            catch (Exception ex)
            {
                WriteUnexpectedTrace(ex, "Error setting ChromeType");
            }
        }

        private void SetWebPartHiddenProperty(SystemWebPart webPart, string value)
        {
            try
            {
                bool hidden = false;
                bool.TryParse(value, out hidden);
                webPart.Hidden = hidden;
            }
            catch (Exception ex)
            {
                WriteUnexpectedTrace(ex, "Error setting Hidden");
            }
        }

        private void SetWebPartAllowCloseProperty(SystemWebPart webPart, string value)
        {
            try
            {
                bool allowClose = false;
                bool.TryParse(value, out allowClose);
                webPart.AllowClose = allowClose;
            }
            catch (Exception ex)
            {
                WriteUnexpectedTrace(ex, "Error setting AllowClose");
            }
        }

        private void SetWebPartAllowMinimizeProperty(SystemWebPart webPart, string value)
        {
            try
            {
                bool allowMinimize = false;
                bool.TryParse(value, out allowMinimize);
                webPart.AllowMinimize = allowMinimize;
            }
            catch (Exception ex)
            {
                WriteUnexpectedTrace(ex, "Error setting AllowMinimize");
            }
        }

        private void SetWebPartWidthProperty(SystemWebPart webPart, string value)
        {
            try
            {
                webPart.Width = Unit.Parse(value);
            }
            catch (Exception ex)
            {
                WriteUnexpectedTrace(ex, "Error setting Width");
            }
        }

        private void SetContentEditorWebPartContent(ContentEditorWebPart webPart, string content)
        {
            XmlElement xmlElement = webPart.Content;
            xmlElement.InnerText = content;
            webPart.Content = xmlElement;
        }

        private void SetImageTitleBarWebPart(TitleBarWebPart webPart, string image)
        {
            webPart.Image = image;
        }

        private void SetTitleBarWebPart(TitleBarWebPart webPart, string title)
        {
            webPart.HeaderTitle = title;
        }

        private void SetXslLinkDataFormWebPart(DataFormWebPart webPart, string xslLink)
        {
            webPart.XslLink = xslLink;
        }

        #endregion

        #region ------ Clear Zone -----------------------------------------------------------------

        private void ClearZone(SPLimitedWebPartManager manager, string zoneId)
        {
            if (string.IsNullOrEmpty(zoneId))
                throw new ArgumentNullException("zoneId");

            WriteTrace("Removing webparts from zone " + zoneId);
            List<SystemWebPart> webPartsToDelete = manager.WebParts.OfType<SystemWebPart>()
                .Where(webPart => manager.GetZoneID(webPart) == zoneId)
                .ToList();

            WriteTrace("{0} web parts to delete...", webPartsToDelete.Count);
            foreach (SystemWebPart webPart in webPartsToDelete)
                manager.DeleteWebPart(webPart);

            WriteTrace("ClearZone finished OK.");
        }

        #endregion
    }
}
