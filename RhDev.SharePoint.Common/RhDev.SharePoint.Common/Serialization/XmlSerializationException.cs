using System;

namespace RhDev.SharePoint.Common.Serialization
{
    public class XmlSerializationException : Exception
    {
        public XmlSerializationException(string message)
            : base(message)
        { }

        public XmlSerializationException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
