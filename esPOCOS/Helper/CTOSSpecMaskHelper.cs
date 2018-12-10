using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class CTOSSpecMaskHelper : Helper
    {
        #region Business Read


        public CTOSSpecMask getCTOSSpecMaskByKey(CTOSSpecMask _ctosspec)
        {

            if (_ctosspec==null) return null;

            try
            {
                var _csm = (from csm in context.CTOSSpecMasks
                           where csm.StoreID == _ctosspec.StoreID && csm.CategoryPath == _ctosspec.CategoryPath
                           && csm.AttrID == _ctosspec.AttrID && _ctosspec.Name == _ctosspec.Name 
                           select csm).FirstOrDefault();
                if (_csm != null)
                    _csm.helper = this;
                return _csm;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        public List<CTOSSpecMask> getAllCTOSSpecMask(string categorypath,string storeid)
        {
            if (String.IsNullOrEmpty(categorypath)) return null;
         
            try
            {
                var _csm = (from sm in context.CTOSSpecMasks 
                           where sm.CategoryPath.ToUpper().Equals(categorypath.ToUpper()) && sm.StoreID == storeid
                           select sm);

                foreach (CTOSSpecMask c in _csm)
                {
                    c.helper = this;
                }
                return _csm.ToList();

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }
        #endregion

        #region Creat Update Delete
        public int save(CTOSSpecMask _specmask)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_specmask == null || _specmask.validate() == false) return 1;
            //Try to retrieve object from DB

            CTOSSpecMask _exist_smask = getCTOSSpecMaskByKey(_specmask);
            try
            {
                if (_exist_smask == null)  //object not exist 
                {
                    //Insert                  
                    context.CTOSSpecMasks.AddObject(_specmask);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update                   
                    context.CTOSSpecMasks.ApplyCurrentValues(_specmask);
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

        public int delete(CTOSSpecMask _specmask)
        {

            if (_specmask == null || _specmask.validate() == false) return 1;

            try
            {
                context.DeleteObject(_specmask);
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
            return typeof(CTOSSpecMaskHelper).ToString();
        }
        #endregion
    }
}