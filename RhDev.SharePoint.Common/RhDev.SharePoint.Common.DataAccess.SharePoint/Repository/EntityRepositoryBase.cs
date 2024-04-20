using System;
using System.Linq;
using System.Collections.Generic;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Extensions;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository
{
    public abstract class EntityRepositoryBase<TEntity> : RepositoryBase, IEntityRepository<TEntity>
    where TEntity : EntityBase, new()
    {
        protected virtual bool IsFolderStructurable => false;

        public ICentralClockProvider CentralClockProvider => SharePoint.CentralClockProvider.Get;

        protected virtual Func<TEntity, string> FolderStructureProvider =>
            entity =>
            {
                var nowDtm = CentralClockProvider.Now();
                var folder = GetFolderStructureFor(nowDtm.ExportDateTime);
                return folder;
            };

        protected EntityRepositoryBase(string webUrl, Func<SPWeb, SPList> listFetcher, SPContentTypeId? contentTypeId = null)
            : base(webUrl, listFetcher, contentTypeId)
        {
        }

        protected TEntity LoadEntity(SPListItem listItem)
        {
            TEntity entity = new TEntity();
            LoadData(listItem, entity);

            return entity;
        }

        protected TEntity QueryEntity(SPQuery query)
        {
            return QueryEntity(query, RequiresElevation);
        }

        protected TEntity QueryEntityElevated(SPQuery query)
        {
            return QueryEntity(query, true);
        }

        private TEntity QueryEntity(SPQuery query, bool requiresElevation)
        {
            return UsingSpList(list => QueryEntity(list, query), requiresElevation);
        }

        protected TEntity QueryEntity(SPList list, SPQuery query)
        {
            SPListItemCollection items = list.GetItems(query);

            if (items.Count == 0)
                return null;

            if (items.Count > 1)
                throw new DuplicateEntityException(items[0].Title);

            TEntity entity = LoadEntity(items[0]);
            return entity;
        }

        protected TCustomEntity QueryEntity<TCustomEntity>(SPQuery query, Func<SPListItem, TCustomEntity> entityLoader)
        {
            return QueryEntity(query, entityLoader, RequiresElevation);
        }

        protected TCustomEntity QueryEntityElevated<TCustomEntity>(SPQuery query, Func<SPListItem, TCustomEntity> entityLoader)
        {
            return QueryEntity(query, entityLoader, true);
        }

        private TCustomEntity QueryEntity<TCustomEntity>(SPQuery query, Func<SPListItem, TCustomEntity> entityLoader, bool requiresElevation)
        {
            return UsingSpList(list =>
            {
                SPListItemCollection items = list.GetItems(query);

                if (items.Count == 0)
                    return default(TCustomEntity);

                if (items.Count > 1)
                    throw new DuplicateEntityException(items[0].Title);

                return entityLoader(items[0]);
            },
                               requiresElevation);
        }

        private IList<TEntity> QueryEntities()
        {
            return QueryEntities(RequiresElevation);
        }

        private IList<TEntity> QueryEntitiesElevated()
        {
            return QueryEntities(true);
        }

        private IList<TEntity> QueryEntities(bool requiresElevation)
        {
            return UsingSpList(list =>
            {
                var listItems = list.GetItems().Cast<SPListItem>();
                var entities = listItems.Select(LoadEntity).ToList();
                return ProcessEntities(entities);
            }, requiresElevation);
        }

        private static Folder GetItemParentFolder(SPListItem listItem)
        {
            SPFile fileItem = listItem.Web.GetFile(listItem.Url);

            // Podminka znaci, ze nalezena ParentFolder neni slozka, ale seznam
            if (fileItem.ParentFolder.Item == null)
                return new Folder();

            return new Folder(fileItem.ParentFolder.Name);
        }

        #region query

        protected IList<TEntity> QueryEntities(SPQuery query)
        {
            return QueryEntities(query, RequiresElevation);
        }

        protected IList<TEntity> QueryEntitiesElevated(SPQuery query)
        {
            return QueryEntities(query, true);
        }

        private IList<TEntity> QueryEntities(SPQuery query, bool requiresElevation)
        {
            return UsingSpList(list => QueryEntities(list, query), requiresElevation);
        }

        protected IList<TEntity> QueryEntities(SPList list, SPQuery query)
        {
            var entities = list.GetItems(query).Cast<SPListItem>().Select(LoadEntity).ToList();
            return ProcessEntities(entities);
        }

        protected virtual IList<TEntity> ProcessEntities(IList<TEntity> entities)
        {
            return entities;
        }

        protected IList<TCustomEntity> QueryEntities<TCustomEntity>(Func<SPListItem, TCustomEntity> entityLoader)
        {
            return UsingSpList(list => list.GetItems().Cast<SPListItem>().Select(entityLoader).ToList());
        }

        protected IList<TCustomEntity> QueryEntities<TCustomEntity>(SPQuery query,
            Func<SPListItem, TCustomEntity> entityLoader)
        {
            return UsingSpList(list => list.GetItems(query).Cast<SPListItem>().Select(entityLoader).ToList());
        }

        public IList<TEntity> GetAllEntities()
        {
            return QueryEntities();
        }

        public IList<TEntity> GetAllEntitiesElevated()
        {
            return QueryEntitiesElevated();
        }

        #endregion

        protected virtual void LoadData(SPListItem listItem, TEntity entity)
        {
            LoadBaseFields(listItem, entity);
        }

        private static void LoadBaseFields(SPListItem listItem, TEntity entity)
        {
            entity.Id = listItem.ID;
            entity.Title = listItem.Title;
            entity.Name = listItem.Name;
            entity.CreatedOn = (DateTime)listItem[SPBuiltInFieldId.Created];
            entity.Author = listItem.GetUserInfoFromUserFieldValue(SPBuiltInFieldId.Author);

            var folder = GetItemParentFolder(listItem);
            if (folder.IsValidName())
                entity.Folder = folder;

            if (!Equals(null, listItem.File))
            {
                entity.FileLink = new EntityLink(
                    new Uri(listItem[SPBuiltInFieldId.EncodedAbsUrl].ToString()),
                    listItem.File.Name
                );
            }
        }

        protected virtual void UpdateData(SPListItem listItem, TEntity entity)
        {
            listItem[SPBuiltInFieldId.Title] = entity.Title;
        }

        public virtual TEntity GetById(int id)
        {
            return UsingSpList(list =>
            {
                try
                {
                    SPListItem item = list.GetItemById(id);
                    return LoadEntity(item);
                }
                catch (ArgumentException ex)
                {
                    throw new EntityNotFoundException(id.ToString(), ex);
                }
            });
        }

        #region create

        public int Create(TEntity entity)
        {
            return Create(entity, RequiresElevation);
        }


        public int CreateElevated(TEntity entity)
        {
            return Create(entity, true);
        }

        private int Create(TEntity entity, bool requiresElevation)
        {
            if (IsFolderStructurable)
            {
                return Create(entity, FolderStructureProvider(entity));
            }

            CheckMutability();

            SPListItem listItem = null;

            UsingSpList(list =>
            {
                listItem = CreateNewItem(list, entity);
                CreateData(listItem, entity);
                listItem.Update();
                LoadBaseFields(listItem, entity);
            },
                requiresElevation);

            return listItem.ID;
        }

        public int CreateFile(TEntity entity, string fileName, bool requiresElevation)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));

            SPListItem listItem = null;

            UsingSpList(list =>
            {
                var newFile = list.RootFolder.Files.Add(fileName, new byte[] { });

                listItem = newFile.Item;

                CreateFileData(listItem, entity);

                var refreshed = listItem.ParentList.GetItemById(listItem.ID);

                CreateData(refreshed, entity);

                refreshed.Update();

                LoadBaseFields(listItem, entity);

            }, requiresElevation);

            return listItem.ID;
        }

        public int UpdateFile(TEntity entity, bool requiresElevation)
        {

            SPListItem listItem = null;

            UsingSpList(list =>
            {
                listItem = list.GetItemById(entity.Id);

                CreateFileData(listItem, entity);

                var refreshed = listItem.ParentList.GetItemById(listItem.ID);

                UpdateData(refreshed, entity);

                refreshed.Update();

                LoadBaseFields(listItem, entity);

            }, requiresElevation);

            return listItem.ID;
        }

        protected int Create(TEntity entity, string folderStructure)
        {
            CheckMutability();

            SPListItem listItem = null;

            UsingSpList(list =>
            {
                var folder = EnsureStructure(folderStructure);

                if (Equals(null, folder))
                    throw new InvalidOperationException(string.Format("Folder by structure creating fail : {0}", folderStructure));

                listItem = list.AddItem(folder.ServerRelativeUrl, SPFileSystemObjectType.File);
                SetContentType(listItem);
                CreateData(listItem, entity);
                listItem.Update();
                LoadBaseFields(listItem, entity);
            },
                RequiresElevation);

            return listItem.ID;
        }

        protected int CreateElevated(TEntity entity, string folderStructure, bool requiresElevation)
        {
            CheckMutability();

            SPListItem listItem = null;

            UsingSpList(list =>
            {
                var folder = EnsureStructure(folderStructure);

                if (Equals(null, folder))
                    throw new InvalidOperationException($"Folder by structure creating fail : {folderStructure}");

                listItem = list.AddItem(folder.ServerRelativeUrl, SPFileSystemObjectType.File);
                SetContentType(listItem);
                CreateData(listItem, entity);
                listItem.Update();
                LoadBaseFields(listItem, entity);
            },
                requiresElevation);

            return listItem.ID;
        }

        protected virtual SPListItem CreateNewItem(SPList list, TEntity entity)
        {
            SPListItem listItem;
            Folder folder = entity.Folder;

            if (folder != null && folder.IsValidName())
            {
                var folderUrl = GetFolderUrlByName(folder, list);
                listItem = list.AddItem(folderUrl, SPFileSystemObjectType.File);
            }
            else
            {
                listItem = list.AddItem();
            }

            SetContentType(listItem);

            return listItem;
        }

        protected virtual void CreateData(SPListItem listItem, TEntity entity)
        {
            listItem[SPBuiltInFieldId.Title] = entity.Title;
        }

        protected virtual void CreateFileData(SPListItem listItem, TEntity entity)
        {

        }

        #endregion

        #region update

        public void Update(TEntity entity)
        {
            Update(entity, true);
        }

        public void Update(TEntity entity, bool createVersion)
        {
            Update(entity, createVersion, RequiresElevation);
        }

        public void UpdateElevated(TEntity entity)
        {
            UpdateElevated(entity, true);
        }

        public void UpdateElevated(TEntity entity, bool createVersion)
        {
            Update(entity, createVersion, true);
        }

        #endregion

        public virtual void MoveToFolder(int itemId, Folder folder)
        {
            UsingSpListElevated(list =>
            {
                string folderUrl = GetFolderUrlByName(folder, list);
                folderUrl = folderUrl.TrimEnd(new[] { '/' });

                SPListItem item = list.GetItemById(itemId);
                bool folderContainsItem = FolderContainsItem(item, folder);

                if (!folderContainsItem)
                {
                    SPWeb web = item.Web;                    
                    SPFile file = web.GetFile(item.Url);

                    string fileName = GetFileNameForFolderChange(file);
                    string destinationUrl = string.Format("{0}/{1}", folderUrl, fileName);

                    file.MoveTo(destinationUrl);
                    file.Update();
                }
            });
        }

        protected virtual string GetFileNameForFolderChange(SPFile file)
        {
            return String.Format("{0}_.000", file.Item.ID);
        }

        private bool FolderContainsItem(SPListItem item, Folder folder)
        {
            return item.Folder != null && item.Folder.Name == folder.Name;
        }

        private string GetFolderUrlByName(Folder folder, SPList list)
        {
            SPFolder foundFolder = GetFolder(list, folder.Name);
            return foundFolder.Url;
        }

        protected static SPFolder GetFolder(SPList list, string folderName)
        {
            SPFolder foundFolder = null;

            if (string.IsNullOrEmpty(folderName))
                return list.RootFolder;

            foreach (SPFolder folder in list.RootFolder.SubFolders)
            {
                if (folder.Name == folderName)
                {
                    foundFolder = folder;
                    break;
                }
            }

            if (foundFolder == null)
                throw new FolderNotFoundException(string.Format("Folder '{0}' not found at list '{1}'.", folderName, list.ID));

            return foundFolder;
        }

        protected SPFolder EnsureStructure(string structure)
        {
            SPFolder fldr = null;
            UsingSpList(list =>
            {
                var folderTitles = structure.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                var parentFolder = list.RootFolder;

                foreach (var t in folderTitles)
                {
                    var folderTitle = t;
                    if (folderTitle.EndsWith("."))
                        folderTitle = folderTitle.Substring(0, folderTitle.Length - 1);
                    SPFolder currentFolder = null;
                    try
                    {
                        currentFolder = parentFolder.SubFolders[folderTitle];
                    }
                    catch
                    {
                    }
                    if (currentFolder == null)
                    {
                        list.ParentWeb.AllowUnsafeUpdates = true;
                        var newFolder = list.Items.Add(parentFolder.ServerRelativeUrl, SPFileSystemObjectType.Folder,
                            list.BaseType == SPBaseType.DocumentLibrary ? folderTitle : null);
                        if (newFolder != null)
                        {
                            if (list.BaseType == SPBaseType.GenericList)
                                newFolder["Title"] = folderTitle;
                            newFolder.Update();
                            currentFolder = newFolder.Folder;
                        }
                    }

                    parentFolder = currentFolder;
                }

                fldr = parentFolder;
            });

            return fldr;
        }

        protected void RemoveEmptyFolders()
        {
            IList<SPFolder> foldersToDel = new List<SPFolder>();

            UsingSpList(list =>
            {
                var rootFolder = list.RootFolder;
                ProcessFolderToDelete(rootFolder, ref foldersToDel);
            });

            foreach (var spFolder in foldersToDel)
                spFolder.Recycle();
        }

        private void ProcessFolderToDelete(SPFolder currFolder, ref IList<SPFolder> foldersToDel)
        {
            if (currFolder == null) throw new ArgumentNullException(nameof(currFolder));
            if (foldersToDel == null) throw new ArgumentNullException(nameof(foldersToDel));

            var currItemsCount = Equals(null, currFolder.Item) ? int.MaxValue : Convert.ToInt32(currFolder.Item[SPBuiltInFieldId.ItemChildCount]);
            var currFoldersCount = Equals(null, currFolder.Item) ? int.MaxValue : Convert.ToInt32(currFolder.Item[SPBuiltInFieldId.FolderChildCount]);

            if (currItemsCount == 0 && currFoldersCount == 0) foldersToDel.Add(currFolder);

            foreach (var fldr in currFolder.SubFolders.OfType<SPFolder>().Where(f => !Equals(null, f.Item)))
                ProcessFolderToDelete(fldr, ref foldersToDel);
        }

        private void Update(TEntity entity, bool createVersion, bool requiresElevation)
        {
            CheckMutability();

            UsingSpList(list =>
            {
                SPListItem listItem = list.GetItemById(entity.Id);

                if (listItem == null)
                    throw new EntityNotFoundException(entity.Id.ToString());

                UpdateData(listItem, entity);

                if (createVersion)
                    listItem.Update();
                else
                    listItem.SystemUpdate(false);

                return LoadEntity(listItem);
            },
                requiresElevation);
        }

        public bool Exists(int itemId)
        {
            CAMLFilter filter = CAMLFilters.Equal(SPBuiltInFieldId.ID, itemId, CAMLType.Integer);
            SPQuery query = CAMLQueryBuilder.BuildQuery(filter);

            return QueryEntity(query, item => true);
        }


        public EntityLink GetRelativeLink(int itemId)
        {
            LocationInfo locationInfo = GetLocation(itemId);
            return new EntityLink(new Uri(locationInfo.RelativeUrl, UriKind.Relative), locationInfo.ItemTitle);
        }

        public EntityLink GetAbsoluteLink(string appUrl, int itemId)
        {
            EntityLink entityLink = GetRelativeLink(itemId);

            var fullUri = new Uri(new Uri(appUrl), entityLink.Uri);
            return new EntityLink(fullUri, entityLink.Description);
        }

        protected string GetFolderStructureFor(DateTime dtm)
        {
            return $"{dtm.Year}/{dtm.Month}/{dtm.Day}";
        }
    }
}
