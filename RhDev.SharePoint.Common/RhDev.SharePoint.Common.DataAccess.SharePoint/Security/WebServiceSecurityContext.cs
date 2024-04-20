using System;
using RhDev.SharePoint.Common.DataAccess.Security;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public class WebServiceSecurityContext : IWebServiceSecurityContext
    {
        private readonly ISharePointContext sharePointContext;

        public WebServiceSecurityContext(ISharePointContext sharePointContext)
        {
            this.sharePointContext = sharePointContext;
        }

        private void ValidateSharePointWeb()
        {
            if (sharePointContext.Instance == null)
                throw new InvalidOperationException("No SharePoint context.");

            if (sharePointContext.Instance.Web == null)
                throw new InvalidOperationException("No SharePoint web for this context");
        }

        public bool IsCurrentUserInGroup(string groupName)
        {
            ValidateSharePointWeb();

            SPWeb web = sharePointContext.Instance.Web;
            SPGroup group = web.Groups[groupName];

            return web.IsCurrentUserMemberOfGroup(group.ID);
        }
    }
}