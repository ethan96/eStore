using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class TranslationGlobalResource
    {
        public TranslationGlobalResource()
        { }

        public TranslationGlobalResource(string langCode) 
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

        internal void copyfrom(TranslationGlobalResource c)
        {
            if (c != null)
            {
                this.StoreId = c.StoreId;
                //this.Key = c.Key;
                this.LanguageId = c.LanguageId;
                this.LocalName = c.LocalName;
                this.CreateDate = DateTime.Now;
                this.CreateBy = c.CreateBy;
            }
        }
    }
}
