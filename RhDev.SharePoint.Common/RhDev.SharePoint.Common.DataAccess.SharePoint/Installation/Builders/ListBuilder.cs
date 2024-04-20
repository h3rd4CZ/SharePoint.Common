using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Repository;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Installation.Builders
{
    public class ListBuilder : ModificationBuilder<SPList>
    {
        private readonly string listId;

        public ListBuilder(string webUrl, string listId)
            : base(webUrl)
        {
            this.listId = listId;
        }

        protected override SPList PrepareObject(SPWeb web)
        {
            return ListProvider.GetListByRelativeUrl(web, listId);
        }

        protected override void SaveObject(SPList list)
        {
            list.Update();
        }

        public ListBuilder HideTitleField()
        {
            AddModification((list, web) =>
            {
                var titleFld = list.Fields[SPBuiltInFieldId.Title];

                titleFld.Hidden = true;

                titleFld.Update();

                list.Update();

                return true;
            });

            return this;
        }

        public ListBuilder BreakPermissionInheritance(bool copyAssignments)
        {
            AddModification((list, web) =>
                {
                    if (list.HasUniqueRoleAssignments)
                        return false;

                    list.BreakRoleInheritance(copyAssignments);
                    return true;
                });

            return this;
        }

        public ListBuilder NoCrawl()
        {
            AddModification((list, web) => list.NoCrawl = true);
            return this;
        }

        public ListBuilder AddField(Guid fieldId)
        {
            AddModification((list, web) =>
                {
                    if (list.Fields.Contains(fieldId))
                        return false;

                    SPField field = list.ParentWeb.AvailableFields[fieldId];
                    list.Fields.Add(field);

                    return true;
                });

            return this;
        }

        public ListBuilder DoNotShowPagesInDialogs()
        {
            AddModification((list, web) => list.NavigateForFormsPages = true);
            return this;
        }

        public ListBuilder AddPermission(string groupName, string permissionSetName)
        {
            AddModification((list, web) =>
                {
                    if (!list.HasUniqueRoleAssignments)
                        throw new InvalidOperationException("Permission inheritance is active.");

                    SPGroup group = web.SiteGroups[groupName];
                    var newRoleAssignmentToAdd = new SPRoleAssignment(group);

                    SPRoleDefinition roleDefinition = web.RoleDefinitions[permissionSetName];
                    newRoleAssignmentToAdd.RoleDefinitionBindings.Add(roleDefinition);

                    list.RoleAssignments.Add(newRoleAssignmentToAdd);
                    return true;
                });

            return this;
        }

        public ListBuilder RenameTitleField(string newTitle)
        {
            AddModification((list, web) => RenameField(list, SPBuiltInFieldId.Title, newTitle));
            AddModification((list, web) => RenameField(list, SPBuiltInFieldId.LinkTitle, newTitle));
            AddModification((list, web) => RenameField(list, SPBuiltInFieldId.LinkTitleNoMenu, newTitle));

            return this;
        }

        public ListBuilder RenameField(Guid fieldId, string newName)
        {
            AddModification((list, web) => RenameField(list, fieldId, newName));

            return this;
        }

        private static bool RenameField(SPList list, Guid fieldId, string newName)
        {
            SPField field = list.Fields[fieldId];
            field.Title = newName;
            field.Update();

            foreach (CultureInfo culture in list.ParentWeb.SupportedUICultures)
                field.TitleResource.SetValueForUICulture(culture, newName);

            field.TitleResource.Update();
            return true;
        }

        public ListBuilder RemoveFieldFromView(string fieldName)
        {
            AddModification((list, web) =>
            {

                for (int i = 0; i < list.Views.Count; i++)
                {
                    if (list.Views[i].ViewFields.Exists(fieldName))
                    {
                        list.Views[i].ViewFields.Delete(fieldName);
                        list.Update();
                    }
                }
                return true;
            });

            return this;
        }

        public ListBuilder EnableMajorVersioning()
        {
            AddModification((list, web) =>
            {

                list.EnableVersioning = true;
                list.EnableMinorVersions = false;
                return true;
            });

            return this;
        }

        public ListBuilder AddContentTypeForm(SPContentTypeId ctId, ContentTypeFormsUrl contentTypeFormsUrl)
        {
            AddModification((list, web) =>
            {

                var ct = list.ContentTypes[list.ContentTypes.BestMatch(ctId)];

                if (contentTypeFormsUrl.DisplayFormUrl != null)
                    ct.DisplayFormUrl = contentTypeFormsUrl.DisplayFormUrl;

                if (contentTypeFormsUrl.EditFormUrl != null)
                    ct.EditFormUrl = contentTypeFormsUrl.EditFormUrl;

                if (contentTypeFormsUrl.NewFormUrl != null)
                    ct.NewFormUrl = contentTypeFormsUrl.NewFormUrl;

                ct.Update();

                return true;
            });

            return this;
        }

        public ListBuilder RenameLibraryTitleField(string newTitle)
        {
            AddModification((list, web) => RenameField(list, SPBuiltInFieldId.LinkFilenameNoMenu, newTitle));
            return this;
        }

        public ListBuilder AddEventReceiver(string name, Type type, SPEventReceiverType eventType)
        {
            AddModification((list, web) =>
                {
                    if (IsEventReceiverRegistered(list, name))
                        return false;

                    AddEventReceiver(list, name, type, eventType);
                    return true;
                });

            return this;
        }

        public ListBuilder AddIndexedField(Guid fieldIdGuid)
        {
            AddModification((list, web) =>
            {
                var fld = list.Fields[fieldIdGuid];
                if (Equals(null, fld)) return false;
                fld.Indexed = true;
                fld.Update();
                return true;
            });

            return this;
        }

        private static bool IsEventReceiverRegistered(SPList list, string name)
        {
            return list.EventReceivers
                .Cast<SPEventReceiverDefinition>()
                .Any(receiver => receiver.Name.Equals(name));
        }

        private static void AddEventReceiver(SPList list, string name, Type type, SPEventReceiverType eventType)
        {
            SPEventReceiverDefinition eventReceiver = list.EventReceivers.Add();
            eventReceiver.Name = name;
            eventReceiver.Type = eventType;
            eventReceiver.Assembly = type.Assembly.FullName;
            eventReceiver.Class = type.FullName;
            eventReceiver.SequenceNumber = 100;
            eventReceiver.Update();
        }

        public ListBuilder RemoveExtraFields(SPContentType ctToCompare)
        {
             AddModification((list, web) =>
            {
                var thisCt = list.ContentTypes[0];
                List<SPField> toRemove = new List<SPField>();



                foreach (var fld in thisCt.Fields.Cast<SPField>())
                {                    
                    if (!ctToCompare.Fields.Contains(fld.Id))
                    {
                        // Remove
                        toRemove.Add(fld);                        
                    }
                
                }
                foreach (var f in toRemove)
                {
                    var fld = list.Fields[f.Id];
                    fld.Delete();
                }
                return true;
            });

            return this;            
        }
    }
}