using System.Text;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML
{
    public interface IQueryModifier
    {
        void ModifyQuery(StringBuilder queryStringBuilder);
    }
}