using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhDev.SharePoint.Common.Composition.Factory;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Mail;
using RhDev.SharePoint.Common.Notifications;
using RhDev.SharePoint.Common.Security;
using System;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.Notification
{
    [TestClass]
    public class NotificationTest
    {
        [TestMethod]
        public void SendNotificationBase()
        {
            var container = ApplicationContainerFactory.Create(ContainerRegistrationDefinition.Empty, 
                postBuildActionsFrontend: c => c.Inject(typeof(ITraceLogger), new ConsoleTraceLogger()));

            var sender = container.Frontend.GetInstance<INotificationSender>();

            sender.Should().NotBeNull();

            var tl = container.Frontend.GetInstance<ITraceLogger>();
            var uip = container.Frontend.GetInstance<IUserInfoProvider>();
            var ui = uip.GetUserInfo(SectionDesignation.FromString(Const.SYSTEMINTEGRATIONTEST_URL), @"i:0#.w|rh\heriserr");
            var clock = CentralClock.FillFromDateTime(DateTime.Now);

            var ma = new MailQueueItemAttachment()
            {
                Data = new byte[] { 1, 2, 3 },
                Name = "File.txt"
            };

            sender.SendNotifications(SectionDesignation.FromString(Const.SYSTEMINTEGRATIONTEST_URL),
                new List<Notifications.Notification> {
                    new Notifications.Notification(
                        ui,
                        clock,
                        "<b>TEST MAIL content</b>", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 
                        subject:"System integration subject",
                        attachments: new List<MailQueueItemAttachment>{ ma })
                }, 
                notifySuccessLogger: (n, m) => tl.Write($"Successfully sent message : {m.Subject}"), 
                notifyFailedLogger : (m , e) => tl.Write($"Failed sent message : {m.Subject} : {e}"));
        }
    }
}
