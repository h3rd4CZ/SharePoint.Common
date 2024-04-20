using RhDev.SharePoint.Common.DataAccess.Mail;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Mail;
using RhDev.SharePoint.Common.Utils;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Mail
{
    public class MailingFactory : IMailingFactory
    {
        private readonly ITraceLogger traceLogger;

        public MailingFactory(ITraceLogger traceLogger)
        {
            this.traceLogger = traceLogger;
        }

        public IMailSender CreateMailSender(string appSiteUrl)
        {
            Guard.StringNotNullOrWhiteSpace(appSiteUrl, nameof(appSiteUrl));
            return new SmtpMailSender(CreateMailConfigurationProvider(appSiteUrl), traceLogger);
        }

        public IMailSender CreateSPMailSender(SectionDesignation sectionDesignation)
        {
            return new SharePointMailSender(traceLogger, sectionDesignation.Address);
        }

        public IMailConfigurationProvider CreateMailConfigurationProvider(string appSiteUrl)
        {
            IMailConfigurationRepository mailConfigurationRepository =
                new MailConfigurationRepository(appSiteUrl);

            return new MailConfigurationProvider(mailConfigurationRepository);
        }
    }
}
