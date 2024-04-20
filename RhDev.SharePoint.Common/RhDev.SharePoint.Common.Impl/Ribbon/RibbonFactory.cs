using System.Web.UI;
using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.Impl.Ribbon
{
    public class RibbonFactory : IService
    {
        public RibbonFactory() { }
        
        public DefaultRibbon GetDefaultRibbon(Page page, string url, bool isSaveDisabled)
        {
            return new DefaultRibbon(page, url, isSaveDisabled);
        }
    }
}
