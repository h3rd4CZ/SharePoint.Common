using System;
using System.Collections.Generic;
using System.Linq;

namespace RhDev.SharePoint.Common.Taxonomy
{
    public class TermPath
    {
        private const char PathSeparator = '/';

        private readonly string[] pathElements;

        public TermPath(string termPath)
        {
            pathElements = GetPathElements(termPath);
        }

        public string Root
        {
            get { return pathElements.First(); }
        }

        public IEnumerable<string> GetSubElements()
        {
            return pathElements.Skip(1);
        }

        public static string[] GetPathElements(string termPath)
        {
            return termPath.Split(new[] { PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string CombinePath(string parentPath, string childPath)
        {
            return String.Format("{0}{1}{2}", parentPath, PathSeparator, childPath);
        }
    }
}
