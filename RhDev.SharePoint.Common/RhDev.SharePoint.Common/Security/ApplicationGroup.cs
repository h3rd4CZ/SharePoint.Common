using System;

namespace RhDev.SharePoint.Common.Security
{
    public abstract class ApplicationGroup
    {
        public abstract string Name { get; }
        public abstract string Description { get;}
        public virtual string ApplicationName => Constants.SOLUTION_NAME;
        /// <summary>
        /// Name provider with parameter as web title
        /// </summary>
        public virtual Func<string, string> CustomNameProvider { get; }
    }
}
