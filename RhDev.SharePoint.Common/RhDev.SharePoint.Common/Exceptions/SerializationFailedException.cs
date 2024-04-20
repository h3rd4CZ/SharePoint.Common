using System;

namespace RhDev.SharePoint.Common.Exceptions
{
    public class SerializationFailedException : Exception
    {
        public SerializationFailedException(Exception inner) : base("An error occured when serialization", inner)
        {

        }
    }
}
