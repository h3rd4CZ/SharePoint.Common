using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common.DataAccess.Repository.Entities
{
    public class SolutionFileInfo
    {
        public string FileName { get; set; }
        public byte[] Blob { get; set; }
    }
}
