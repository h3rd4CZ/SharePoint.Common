using RhDev.SharePoint.Common.Caching.Composition;
using System.Net;

namespace RhDev.SharePoint.Common.DataAccess.Security
{
    public interface IExternalCredentialsRepository : IService
    {
        string IssueTicket();

        NetworkCredential GetCredentials(string applicationId);

        NetworkCredential GetCredentials(string applicationId, string ticket);
    }
}
