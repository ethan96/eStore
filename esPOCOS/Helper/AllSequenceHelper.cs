using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class AllSequenceHelper
    {

        #region Business Read

        #endregion

        #region Creat Update Delete

        public static string GetNewSeq(Store _store, String _seqname)
        {
            return GetNewSeq(_store.StoreID, _seqname);
        }

        public static string GetNewSeq(string _storeid, String _seqname)
        {
            string newseq = "1";
            try
            {
                eStore3Entities6 context2 = new eStore3Entities6();

                foreach (string seq in context2.GetNewSeqVal(_seqname, _storeid))
                {
                    newseq = seq;

                }
                return newseq;

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return "-5000";
            }
        }

        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(StoreHelper).ToString();
        }
        #endregion
    }
}