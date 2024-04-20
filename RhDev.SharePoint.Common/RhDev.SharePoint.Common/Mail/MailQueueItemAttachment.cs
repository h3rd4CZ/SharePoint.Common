using System.Net.Mime;

namespace RhDev.SharePoint.Common.Mail
{
    public class MailQueueItemAttachment
    {
        public byte[] Data { get; set; }
        public string Name { get; set; }
        public ContentType ContentType { get; set; }
    }
}
