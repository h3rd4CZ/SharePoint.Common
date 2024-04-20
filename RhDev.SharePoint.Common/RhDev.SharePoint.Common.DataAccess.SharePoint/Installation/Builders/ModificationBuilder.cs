using System;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Installation.Builders
{
    public abstract class ModificationBuilder<TObject>
    {
        private readonly IList<Func<TObject, SPWeb, bool>> modifications = new List<Func<TObject, SPWeb, bool>>();

        private readonly string webUrl;
        protected readonly string listUrl;

        protected ModificationBuilder(string webUrl)
        {
            this.webUrl = webUrl;
        }

        protected ModificationBuilder(string webUrl, string lstUrl)
        {
            this.webUrl = webUrl;
            this.listUrl = lstUrl;
        }


        protected void AddModification(Func<TObject, SPWeb, bool> modification)
        {
            modifications.Add(modification);
        }

        public void Apply()
        {
            using (var site = new SPSite(webUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    TObject obj = PrepareObject(web);
                    bool objectModified = false;

                    foreach (Func<TObject, SPWeb, bool> modification in modifications)
                        objectModified |= modification(obj, web);

                    if (objectModified)
                        SaveObject(obj);
                }
            }
        }

        protected abstract TObject PrepareObject(SPWeb web);

        protected abstract void SaveObject(TObject obj);
    }
}
