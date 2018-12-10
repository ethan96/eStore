using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class Language
    {
        public Language()
        { }

        public Language(string code)
        {
            this.Code = code;
            LanguageHelper helper = new LanguageHelper();
            var lan = helper.getLangByCode(code);
            if (lan != null)
                copyFrom(lan);
        }

        public void copyFrom(Language source)
        {
            this.Id = source.Id;
            this.Code = source.Code;
            this.Name = source.Name;
        }
    }
}
