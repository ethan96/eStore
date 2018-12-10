using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class StoreErrorCodeGlobalResource
    {
        public StoreErrorCodeGlobalResource()
        { }

        public StoreErrorCodeGlobalResource(string langCode) 
        {
            var lan = (new LanguageHelper()).getLangByCode(langCode);
            if (lan != null)
                this.LanguageId = lan.Id;
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

        internal void copyfrom(StoreErrorCodeGlobalResource c)
        {
            if (c != null)
            {
                this.StoreId = c.StoreId;
                this.LanguageId = c.LanguageId;
                this.ErrorCode = c.ErrorCode;
                this.LocalName = c.LocalName;
                this.CreateDate = DateTime.Now;
                this.CreateBy = c.CreateBy;
            }
        }
    }
}
