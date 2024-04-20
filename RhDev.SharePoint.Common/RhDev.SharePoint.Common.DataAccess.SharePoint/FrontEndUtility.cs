using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Composition;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Logging;
using RhDev.SharePoint.Common.Security;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint
{
    public class FrontEndUtility : IAutoRegisteredService
    {
        private enum TermControlValue
        {
            Code,
            Name
        }

        private readonly IUserInfoProvider userInfoProvider;
        private readonly FarmConfiguration farmConfiguration;

        public FrontEndUtility(
            IUserInfoProvider userInfoProvider,
            FarmConfiguration farmConfiguration)
        {
            this.userInfoProvider = userInfoProvider;
            this.farmConfiguration = farmConfiguration;
        }

        public static CultureInfo CzechCulture
        {
            get { return new CultureInfo("cs-CZ"); }
        }

        public int LCIDCzech
        {
            get { return CzechCulture.LCID; }
        }

        public string DocumentBlobQueryString
        {
            get { return "DocumentFileId"; }
        }

        public string AttachmentDataQueryString
        {
            get { return "AttachmentFileId"; }
        }

        private static readonly string ChooseDropDownItemValue = String.Empty;

        public static void SendFileDownload(string fileName, byte[] fileBlob)
        {
            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;

            response.Clear();
            response.ContentType = "application/octet-stream";

            string contentDisposition = MakeContentDisposition(fileName, context.Request.Browser);
            response.AddHeader("content-disposition", contentDisposition);

            response.BufferOutput = true;
            response.OutputStream.Write(fileBlob, 0, fileBlob.Length);
            response.Flush();
            response.End();
        }

        private static string MakeContentDisposition(string fileName, HttpBrowserCapabilities browser)
        {
            string contentDisposition;

            if (browser.Browser == "IE" && (browser.Version == "7.0" || browser.Version == "8.0"))
                contentDisposition = "attachment; filename=" + Uri.EscapeDataString(fileName);
            else
                contentDisposition = "attachment; filename*=UTF-8''" + Uri.EscapeDataString(fileName);

            return contentDisposition;
        }


        public static string GetItemDisplayFormUrl(SPListItem listItem)
        {
            SPList list = listItem.ParentList;
            SPWeb web = listItem.Web;

            string dispUrl = listItem.ContentType.DisplayFormUrl;

            if (dispUrl == "")
                dispUrl = list.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url;

            bool isLayouts = dispUrl.StartsWith("_layouts/", StringComparison.CurrentCultureIgnoreCase);

            dispUrl = String.Format("{0}/{1}?ID={2}", web.ServerRelativeUrl, dispUrl, listItem.ID);

            if (isLayouts)
                dispUrl = String.Format("{0}&List={1}", dispUrl, SPEncode.UrlEncode(list.ID.ToString()));

            return dispUrl;
        }

        public string GetDocumentDataDownloadRequestUrl(SPListItem documentMetadataItem)
        {
            return String.Format("{0}&{1}={2}", GetItemDisplayFormUrl(documentMetadataItem), DocumentBlobQueryString,
                documentMetadataItem.ID);
        }

        public string GetAttachmentDataDownloadRequestUrl(SPListItem documentMetadataItem, int attachmentMetadataId)
        {
            return String.Format("{0}&{1}={2}", GetItemDisplayFormUrl(documentMetadataItem), AttachmentDataQueryString,
                attachmentMetadataId);
        }

        public bool ValidatePage(Page page)
        {
            page.Validate();
            return page.IsValid;
        }

        public bool ValidatePage(Page page, string validationGroup)
        {
            page.Validate(validationGroup);
            return page.IsValid;
        }

        public void ValidateNotEmpty(ServerValidateEventArgs args, string value)
        {
            args.IsValid = !String.IsNullOrEmpty(value);
        }

        public void ValidatePrice(ServerValidateEventArgs args, string value)
        {
            ValidateNotEmpty(args, value);

            if (!args.IsValid)
                return;

            double outValue;

            Double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out outValue);
            args.IsValid = outValue >= 0;
        }

        public string GetCurrentListViewUrl()
        {
            SPList list = SPContext.Current.List;
            SPWeb web = SPContext.Current.Web;
            return GetListViewUrl(list, web);
        }

        public string GetListViewUrl(SPList list)
        {
            return GetListViewUrl(list, list.ParentWeb);
        }

        public string GetListViewUrl(SPList list, SPWeb web)
        {
            return web.Url + "/" + list.RootFolder.Url;
        }

        public string GetSectionIdFromDropDownList(string company, string section)
        {
            return String.Format("{0}/{1}", company, section);
        }

        public string GetValueFromDropDownListControl(DropDownList control)
        {
            var selectedItem = control.SelectedItem;

            if (!control.Enabled && !selectedItem.Value.Equals(ChooseDropDownItemValue))
                return selectedItem.Text;

            if (selectedItem.Value.Equals(ChooseDropDownItemValue))
                return String.Empty;

            return selectedItem.Text;
        }

        public int GetUserIdFromPeoplePicker(PeopleEditor peopleEditor, SectionDesignation sectionDesignation)
        {
            string userName = peopleEditor.CommaSeparatedAccounts;
            var userInfo = userInfoProvider.GetUserInfo(sectionDesignation, userName);

            return userInfo.Id;
        }

        public void RedirectToCurrentDisplayForm()
        {
            var context = SPContext.Current;
            RedirectToDisplayForm(context.ListItem);
        }
        
        public void RedirectToCentralWeb()
        {
            HttpContext.Current.Response.Redirect(farmConfiguration.AppSiteUrl, true);
        }

        public void RedirectTo(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(url));

            HttpContext.Current.Response.Redirect(url, true);
        }

        public static string GetExecuteOrDelayUntilScriptLoaded(string script)
        {
            return String.Format("ExecuteOrDelayUntilScriptLoaded(function() {{ {0} }},\"sp.js\");", script);
        }

        public ListItem CreateChooseDropDownItem()
        {
            return new ListItem("Vyberte...", ChooseDropDownItemValue);
        }

        public string GetCorrectDisplayText(string text)
        {
            return String.IsNullOrEmpty(text) ? String.Empty : HttpUtility.HtmlDecode(text.Replace("&quot;", "\""));
        }

        public static void RegisterPageStatus(Page page, string statusText, string statusHeader = null)
        {
            if (page == null) throw new ArgumentNullException("page");
            if (statusText == null) throw new ArgumentNullException("statusText");

            if (string.IsNullOrEmpty(statusHeader))
                statusHeader = "Info";

            string statusScript =
                string.Format("var strStatusId = SP.UI.Status.addStatus(\"{0}\",\"{1}\",true); " +
                              "SP.UI.Status.setStatusPriColor(strStatusId, \"blue\");", statusHeader, statusText);

            string script = GetExecuteOrDelayUntilScriptLoaded(statusScript);

            page.ClientScript.RegisterStartupScript(page.GetType(), string.Format("DMSStatusScript_{0}", Guid.NewGuid()),
                script, true);
        }

        public static string WfeErrMsg()
        {
            var dtmNow = DateTime.Now;
            var cid = CorrelationId.GetCurrentCorrelationToken();
            var dtm = $"{dtmNow.ToLongDateString()} - {dtmNow.ToLongTimeString()}";
            var srv = SPServer.Local.Name;

            return $"CORRELATION ID : {cid}, DATE : {dtm}, SRV: {srv}";
        }

        public static void RedirectToDisplayForm(SPListItem listItem)
        {
            HttpContext context = HttpContext.Current;

            string dispUrl = GetItemDisplayFormUrl(listItem);
            context.Response.Redirect(dispUrl, true);
        }

        public static void RegisterStartUp(Page page, string script)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));
            if (string.IsNullOrWhiteSpace(script))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(script));

            page.ClientScript.RegisterStartupScript(page.GetType(), $"MP_NC_STARTUP_{Guid.NewGuid()}", script, true);
        }
    }
}
