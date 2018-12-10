using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class DMFHelper : Helper
    {
        #region Business Read

        public List<DMF> getDMFs()
        {
            var _dmfs = from dmf in context.DMFs
                        select dmf;
            if (_dmfs != null)
                return _dmfs.ToList();
            else
                return new List<DMF>();
        }

        
        public DMF getDMFbyID(string dmfid)
        {
            var _dmf = (from dmf in context.DMFs
                        where dmf.DMFID.ToUpper() == dmfid.ToUpper()
                        select dmf).FirstOrDefault();
            return _dmf;
        }

        #endregion

        #region Creat Update Delete
        public int save(DMF _dmf)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_dmf == null || _dmf.validate() == false) return 1;
            //Try to retrieve object from DB
            DMF _exist_dmf = getDMFbyID(_dmf.DMFID);
            try
            {
                if (_exist_dmf == null)  //object not exist 
                {
                    //Insert
                    context.DMFs.AddObject( _dmf);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.DMFs.ApplyCurrentValues( _dmf);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(DMF _dmf)
        {

            if (_dmf == null || _dmf.validate() == false) return 1;
            try
            {
                context.DeleteObject(_dmf);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(DMFHelper).ToString();
        }
        #endregion
    }
}