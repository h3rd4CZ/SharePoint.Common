using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public class PermissionSetDefinition
    {
        public string Name { get; set; }
        public SPBasePermissions Permissions { get; set; }
    }
}
