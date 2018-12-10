using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class CategoriesGlobalResource
    {
        public CategoriesGlobalResource()
        { }

        public CategoriesGlobalResource(string langCode) 
        {
            var lan = (new LanguageHelper()).getLangByCode(langCode);
            if (lan != null)
                this.LanguageId = lan.Id;
        }

        internal void copyfrom(CategoriesGlobalResource c)
        {
            if (c != null)
            {
                this.StoreId = c.StoreId;
                this.Categoryid = c.Categoryid;
                this.LanguageId = c.LanguageId;
                this.LocalName = c.LocalName;
                this.CreateDate = DateTime.Now;
                this.CreateBy = c.CreateBy;
                this.LocalDescription = c.LocalDescription;
                this.LocalExtDescription = c.LocalExtDescription;
            }
        }

        public Language LanguageX
        {
            get
            {
                if (this.Language == null)
                    this._language = (new LanguageHelper()).getLangById(this.LanguageId.Value);
                return this.Language;
            }
        }
    }
}
