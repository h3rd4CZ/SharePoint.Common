using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common
{
    [Serializable]
    public abstract class EntityBase
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsSaved => Id != 0;

        public string Title { get; set; }

        public string Name { get; set; }

        public Folder Folder { get; set; }
        public UserInfo Author { get; set; }
        public override string ToString()
        {
            return String.Format("{0} (ID {1})", Title, Id);
        }

        public EntityLink FileLink
        {
            get;
            set;
        }
    }
}
