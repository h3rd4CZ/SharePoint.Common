using System;
using System.Collections;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Taxonomy
{
    public class TermValue
    {
        public TermValue(Guid guid, string path, string name, IList<TermValueLabel> labels)
        {
            Guid = guid;
            Path = path;
            Name = name;
            Labels = labels;
        }

        public Guid Guid { get; }

        public string Path { get; }
        public string Name { get; }
        public IList<TermValueLabel> Labels { get; }
    }
}
