using System;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Installation.Builders
{
    public class ContentTypeBuilder : ModificationBuilder<SPContentType>
    {
        private readonly SPContentTypeId contentTypeId;

        public ContentTypeBuilder(string webUrl, SPContentTypeId contentTypeId)
            : base(webUrl)
        {
            this.contentTypeId = contentTypeId;
        }

        public ContentTypeBuilder AddField(Guid fieldId)
        {
            AddModification((contentType, web) =>
            {
                if (contentType.Fields.Contains(fieldId))
                    return false;

                SPField field = web.Fields[fieldId];

                var fieldLink = new SPFieldLink(field);
                contentType.FieldLinks.Add(fieldLink);

                return true;
            });

            return this;
        }

        public ContentTypeBuilder RemoveField(Guid fieldId)
        {
            AddModification((contentType, web) =>
            {
                if (!contentType.Fields.Contains(fieldId))
                    return true;

                contentType.FieldLinks.Delete(fieldId);
                return true;
            });

            return this;
        } 

        public ContentTypeBuilder ConfigureForms(ContentTypeFormsUrl contentTypeFormsUrl)
        {
            AddModification((contentType, web) =>
            {
                if (contentTypeFormsUrl.DisplayFormUrl != null)
                    contentType.DisplayFormUrl = contentTypeFormsUrl.DisplayFormUrl;

                if (contentTypeFormsUrl.EditFormUrl != null)
                    contentType.EditFormUrl = contentTypeFormsUrl.EditFormUrl;

                if (contentTypeFormsUrl.NewFormUrl != null)
                    contentType.NewFormUrl = contentTypeFormsUrl.NewFormUrl;

                return true;
            });

            return this;
        }

        protected override SPContentType PrepareObject(SPWeb web)
        {
            return web.ContentTypes[contentTypeId];
        }

        protected override void SaveObject(SPContentType contentType)
        {
            contentType.Update(true);
        }
    }
}
