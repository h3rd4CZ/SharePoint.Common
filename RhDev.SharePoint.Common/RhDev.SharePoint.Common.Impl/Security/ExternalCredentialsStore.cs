using RhDev.SharePoint.Common.Security;
using RhDev.SharePoint.Common.DataAccess.Security;
using System.Net;

namespace RhDev.SharePoint.Common.Impl.Security
{
    public class ExternalCredentialsStore : IExternalCredentialsStore
    {
        private readonly ISecurityRepositoryFactory repositoryFactory;

        public ExternalCredentialsStore(ISecurityRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory;
        }

        public NetworkCredential GetCredentials(string applicationId)
        {
            return GetCredentials(applicationId, null);
        }

        public NetworkCredential GetCredentials(string applicationId, string ticket)
        {
            IExternalCredentialsRepository repository = repositoryFactory.CreateExternalCredentialsRepository();
            return repository.GetCredentials(applicationId, ticket);
        }

        public string IssueTicket()
        {
            IExternalCredentialsRepository repository = repositoryFactory.CreateExternalCredentialsRepository();
            return repository.IssueTicket();
        }
    }
}
