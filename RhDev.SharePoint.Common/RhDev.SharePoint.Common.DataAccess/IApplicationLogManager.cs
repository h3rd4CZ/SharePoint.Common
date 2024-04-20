using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;

namespace RhDev.SharePoint.Common.DataAccess
{
    public interface IApplicationLogManager : IAutoRegisteredService
    {
        void WriteLog(string msg, string source, ApplicationLogLevel level);        
        void WriteLog(string msg, string source, string webUrl, ApplicationLogLevel level);
    }
}