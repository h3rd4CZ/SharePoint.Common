using RhDev.SharePoint.Common.Security;
using RhDev.SharePoint.Common.DataAccess;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;

namespace RhDev.SharePoint.Common.Impl.Logging
{
    public class ApplicationLogManager : IApplicationLogManager
    {
        private ISecurityContext _securityContext;
        private ICommonRepositoryFactory _commonRepositoryFactory;
        private readonly ICentralClockProvider centralClockProvider;

        public ApplicationLogManager(
            ISecurityContext securityCotext,
            ICommonRepositoryFactory comonRepositoryFactory,
            ICentralClockProvider centralClockProvider
            )
        {
            _securityContext = securityCotext;
            _commonRepositoryFactory = comonRepositoryFactory;
            this.centralClockProvider = centralClockProvider;
        }

        public void WriteLog(string msg, string source, ApplicationLogLevel level)
        {
            WriteLog(msg, source, string.Empty, level);
        }


        public void WriteLog(string msg, string source, string webUrl, ApplicationLogLevel level)
        {
            var s = string.IsNullOrEmpty(source) ? _securityContext.CurrentWebTitle : source;

            var now = centralClockProvider.Now();
            var time = now.ToLongTimeString();
            var repo = _commonRepositoryFactory.GetApplicationLogRepository(webUrl);

            repo.WriteLog(new LogItem()
            {
                Title = time,
                Level = level,
                Message = msg,
                Source = s
            });
        }
    }
}
