using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;
using System.Linq;
using eStore.POCOS.DAL;


namespace eStore.POCOS
{
    public partial class uStore : Store
    {
        /// <summary>
        /// make sure add QUSU/OUSU to allsequence talbe, otherwise, will always return 5000
        /// </summary>
        public override String nextQuotationNumber
        {
            get
            {
                String prefix = "Q" + StoreID.Substring(1)+"U";
                String nextId = "";
                String nextIdCandidate = AllSequenceHelper.GetNewSeq(this, prefix);
                try
                {
                    nextId = String.Format("{0:D5}", Convert.ToInt32(nextIdCandidate));
                }
                catch (Exception ex)
                {
                    nextId = nextIdCandidate;
                }

                return prefix + nextId;
            }
        }

        public override String nextOrderNumber
        {
            get
            {
                String prefix = "O" + StoreID.Substring(1) + "U";
                String nextId = "";
                String nextIdCandidate = AllSequenceHelper.GetNewSeq(this, prefix);
                try
                {
                    nextId = String.Format("{0:D5}", Convert.ToInt32(nextIdCandidate));
                }
                catch (Exception ex)
                {
                    nextId = nextIdCandidate;
                }

                return prefix + nextId;
            }
        }

        /// <summary>
        /// for OM set ustore menu, for UI, will prepend ustore when getting menu by helper
        /// </summary>
        public override List<string> MenuTypeList
        {
            get
            {
                List<string> uStoreMenuTypeList = new List<string>();
                foreach (string s in base.MenuTypeList)
                {
                    uStoreMenuTypeList.Add("uStore" + s);
                }
                return uStoreMenuTypeList;
            }
        }

        /// <summary>
        /// overwrite mail template files forder
        /// </summary>
        public virtual string templateSubFolder
        {
            get
            {
                return "uStore";
            }
        }

        public  override string Title
        {
            get {
                if (!string.IsNullOrEmpty(uStoreTitle))
                    return uStoreTitle;
                else

                    return base.Title;
            }
        }

        public override string MetaKeywords
        {
            get
            {
                if (!string.IsNullOrEmpty(uStoreMetaKeywords))
                    return uStoreMetaKeywords;
                else

                    return base.MetaKeywords;
            }
        }

        public override string MetaDesc
        {
            get
            {
                if (!string.IsNullOrEmpty(uStoreMetaDesc))
                    return uStoreMetaDesc;
                else

                    return base.MetaDesc;
            }
        }
    }
}
