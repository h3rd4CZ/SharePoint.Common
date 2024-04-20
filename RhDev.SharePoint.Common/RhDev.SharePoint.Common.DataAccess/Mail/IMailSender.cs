using System;
using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Mail;

namespace RhDev.SharePoint.Common.DataAccess.Mail
{

    /// <summary>
    /// Interface for sending mails in more configurable way
    /// </summary>
    public interface IMailSender : IService, IDisposable
    {
        /// <summary>
        /// Sends item
        /// </summary>
        /// <param name="mailItem">mail item</param>
        void Send(MailQueueItem mailItem);
    }
}
