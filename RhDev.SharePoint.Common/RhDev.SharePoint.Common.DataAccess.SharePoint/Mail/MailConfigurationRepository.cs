using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.Mail;
using RhDev.SharePoint.Common.Mail;
using RhDev.SharePoint.Common.Utils;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Mail
{
    public class MailConfigurationRepository : IMailConfigurationRepository
    {
        private readonly string siteUrl;

        public MailConfigurationRepository(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public MailConfiguration GetMailConfiguration()
        {
            using (var site = new SPSite(siteUrl))
            {
                Guard.NotNull(
                    site.WebApplication.OutboundMailSenderAddress, 
                    nameof(site.WebApplication.OutboundMailSenderAddress),
                    $"No outbound mail sender address was configured please check web application mail outgoing configuration");

                Guard.NotNull(
                    site.WebApplication.OutboundMailServiceInstance,
                    nameof(site.WebApplication.OutboundMailServiceInstance),
                    $"No outbound mail service instance was configured please check web application mail outgoing configuration");

                return new MailConfiguration(
                    site.WebApplication.OutboundMailSenderAddress,
                    site.WebApplication.OutboundMailReplyToAddress,
                    site.WebApplication.OutboundMailServiceInstance.Server.Address);
            }
        }
    }
}
