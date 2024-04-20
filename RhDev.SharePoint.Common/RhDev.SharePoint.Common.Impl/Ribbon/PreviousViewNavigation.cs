using System.Web;
using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Security;

namespace RhDev.SharePoint.Common.Impl.Ribbon
{
    public class PreviousViewNavigation : IAutoRegisteredService
    {
        private readonly ISecurityContext _securityContext;
        public PreviousViewNavigation(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public string GetPreviousUrlFromQueryString()
        {
            var source = HttpContext.Current.Request.QueryString["Source"];

            return string.IsNullOrEmpty(source) ? _securityContext.CurrentWebUrl : HttpUtility.UrlDecode(source);
        }
    }
}
