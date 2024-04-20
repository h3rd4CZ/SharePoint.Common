using System;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Repository;
using Microsoft.SharePoint;


namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Installation.Builders
{
    public class FieldBuilder : ModificationBuilder<SPField>
    {
        private readonly Guid fieldId;

        public FieldBuilder(string webUrl, Guid fieldId)
            : this(webUrl, string.Empty, fieldId)
        {
            
        }

        public FieldBuilder(string webUrl, string listUrl, Guid fieldId)
            : base(webUrl, listUrl)
        {
            this.fieldId = fieldId;
        }

        public FieldBuilder ConnectLookup(string ListUrl)
        {
            AddModification((field, web) => ApplyLookupModification((SPFieldLookup)field, web, ListUrl));
            return this;
        }

        private bool ApplyLookupModification(SPFieldLookup lookup, SPWeb web, string targetListId)
        {
            if (!String.IsNullOrEmpty(lookup.LookupList))
                return false;

            SPList targetList = ListProvider.GetListByRelativeUrl(web, targetListId);

            if (string.IsNullOrEmpty(listUrl))
                throw new InvalidOperationException(
                    "List url from which the field would be received is not set, please set it first");

            var fldList = ListProvider.GetListByRelativeUrl(web, listUrl);

            var fld = fldList.Fields[fieldId];
            var lookupFld = (SPFieldLookup) fld;

            lookupFld.LookupList = targetList.ID.ToString();
            lookupFld.LookupWebId = web.ID;

            return true;
        }

        public FieldBuilder Rename(string newName)
        {
            AddModification((field, web) =>
                {
                    field.Title = newName;
                    field.PushChangesToLists = true;

                    return true;
                });

            return this;
        }

        public FieldBuilder AddChoiceValue(string choiceValue)
        {
            AddModification((field, web) =>
                {
                    SPFieldChoice choice = (SPFieldChoice) field;

                    choice.Choices.Add(choiceValue);

                return true;
            });

            return this;
        }

        protected override SPField PrepareObject(SPWeb web)
        {
            SPField fld = null;
            try
            {
                fld = web.Fields[fieldId];
            }
            catch{}

            try
            {
                fld = web.AvailableFields[fieldId];
            }
            catch{}

            if (Equals(null, fld)) throw new Exception(string.Format("Field with ID :{0} was not found", fieldId));

            return fld;
        }

        protected override void SaveObject(SPField obj)
        {
            if(string.IsNullOrEmpty(listUrl))
                obj.Update(true);
            else
                obj.Update();
        }
    }
}
