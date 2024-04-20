using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Extensions
{
    public static class SPListExtensions
    {
        public static int? GetLookupFieldValue(this SPListItem listItem, string fieldName)
        {
            SPField field = listItem.Fields.GetFieldByInternalName(fieldName);
            return GetLookupFieldValue(listItem, field);
        }

        public static int? GetLookupFieldValue(this SPListItem listItem, Guid fieldId)
        {
            SPField field = listItem.Fields[fieldId];
            return GetLookupFieldValue(listItem, field);
        }

        public static int? GetLookupFieldValue(SPListItem listItem, SPField field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            if (!(field is SPFieldLookup))
                throw new InvalidOperationException("Field with ID=" + field.Id.ToString() + " is not lookup!");

            var str = listItem[field.Id];
            if (str == null)
                return null;

            SPFieldLookup lookup = (SPFieldLookup)field;
            SPFieldLookupValue value = (SPFieldLookupValue)lookup.GetFieldValue(listItem[field.Id].ToString());
            return value.LookupId;
        }

        public static string MapEnumValueToChoice(this SPList list, Guid fieldId, int enumValue)
        {
            var choice = (SPFieldChoice)list.Fields[fieldId];
            string[] choices = choice.Choices.Cast<string>().ToArray();

            int matchingChoiceIndex = enumValue;

            if (matchingChoiceIndex > choices.Length - 1)
                throw new ArgumentException("Enum does not match choices.", "enumValue");

            string choiceValue = choices[matchingChoiceIndex];
            return choiceValue;
        }

        public static void ClearItems(this SPList list)
        {
            IList<int> itemIds = new List<int>();

            foreach (SPListItem item in list.Items)
                itemIds.Add(item.ID);

            foreach (int itemId in itemIds)
                list.Items.DeleteItemById(itemId);

            list.Update();
        }

        public static void EnableFolderCreation(this SPList list)
        {
            using (SPSite site = new SPSite(list.ParentWeb.Url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;

                    var newList = web.Lists[list.ID];

                    newList.EnableFolderCreation = true;
                    newList.Update();
                }
            }
        }

        public static void DisableFolderCreation(this SPList list)
        {
            using (SPSite site = new SPSite(list.ParentWeb.Url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;

                    var newList = web.Lists[list.ID];

                    newList.EnableFolderCreation = false;
                    newList.Update();
                }
            }
        }
    }
}
