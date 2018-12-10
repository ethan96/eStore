using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class MenuGlobalResource
    {
        public MenuGlobalResource()
        { }

        public MenuGlobalResource(string langCode)
        { 
           var lan = (new LanguageHelper()).getLangByCode(langCode);
           if (lan != null)
               this.LanguageId = lan.Id;
        }

        public void copyFrom(MenuGlobalResource source)
        {
            if (source != null)
            {
                this.StoreId = source.StoreId;
                //this.Id = source.Id;
                this.LocalName = source.LocalName;
                this.MenuId = source.MenuId;
                this.CreateBy = source.CreateBy;
                this.CreateDate = source.CreateDate;
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
