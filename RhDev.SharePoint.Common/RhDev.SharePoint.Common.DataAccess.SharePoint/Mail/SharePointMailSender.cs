using System;
using RhDev.SharePoint.Common.DataAccess.Mail;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Mail;
using Microsoft.SharePoint.Utilities;
using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Mail
{
    public class SharePointMailSender : ServiceBase, IMailSender
    {
        private SPSite site;
        private SPWeb web;

        protected override TraceCategory TraceCategory => TraceCategories.Common;

        public SharePointMailSender(ITraceLogger traceLogger, string webUrl) : base(traceLogger)
        {
            site = new SPSite(webUrl);
            web = site.OpenWeb();
        }

        public void Send(MailQueueItem mailItem)
        {
            if (mailItem == null)
                throw new ArgumentNullException("mailItem");

            if (String.IsNullOrEmpty(mailItem.To))
                throw new ArgumentException("Mail recipient not set.");

            WriteTrace("Sending e-mail to {0} with subject {1}", mailItem.To, mailItem.Subject);

            try
            {
                bool success = SPUtility.SendEmail(web, mailItem.IsHtml, false, mailItem.To, mailItem.Subject, mailItem.Body);

                if (!success)
                    throw new MailSendingFailedException(String.Format("SharePoint failed to send e-mail to {0}.", mailItem.To));

                WriteTrace("E-mail sent successfully");
            }
            catch (MailSendingFailedException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new MailSendingFailedException("Mail sending failed.", e);
            }
        }

        public void Dispose()
        {
            if (web != null)
            {
                web.Dispose();
                web = null;
            }

            if (site != null)
            {
                site.Dispose();
                site = null;
            }
        }
    }
}
