using RhDev.SharePoint.Common.DataAccess.Mail;
using RhDev.SharePoint.Common.Mail;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Mail
{
    public class MailConfigurationProvider : IMailConfigurationProvider
    {
        private readonly IMailConfigurationRepository mailConfigurationRepository;

        public MailConfigurationProvider(IMailConfigurationRepository mailConfigurationRepository)
        {
            this.mailConfigurationRepository = mailConfigurationRepository;
        }

        public MailConfiguration GetMailConfiguration()
        {
            return mailConfigurationRepository.GetMailConfiguration();
        }
    }
}
