using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.Utils;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Utils
{
    public interface IWebPartTemplator
    {
        WebPart InstantiateWebPart(SPWeb web, PageArrangementConfiguration.WebPartZoneRegisterWebPart wp);
    }
}
