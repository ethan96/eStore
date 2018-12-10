using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public class LanguageResource
    {
        public int DocId { get; set; }//transtionId, menuId, categoryId

        public string StoreId { get; set; }

        public string LanguageName { get; set; }

        public string DisplayKeyName { get; set; }//transtion key,Menu mentName,Category categoryName

        public string DisplayKeyValue { get; set; }

        public string LocalName { get; set; }

        public string LocalDesc { get; set; }

        public string DisplayExtendedDesc { get; set; }//category Golbal 新加的列

        public string LocalExtendedDesc { get; set; }//category Golbal 新加的列
    }
}
