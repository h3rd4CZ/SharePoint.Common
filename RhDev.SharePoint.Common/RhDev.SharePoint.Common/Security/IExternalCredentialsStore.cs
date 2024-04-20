using System.Net;
using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.Security
{
    public interface IExternalCredentialsStore : IAutoRegisteredService
    {
        NetworkCredential GetCredentials(string applicationId);

        NetworkCredential GetCredentials(string applicationId, string ticket);

        string IssueTicket();
    }
}
