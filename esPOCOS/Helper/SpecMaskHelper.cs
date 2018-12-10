using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class SpecMaskHelper : Helper
    {
        #region Business Read
        public List<SpecMask> getSpecMaskbyCategoryPath(string categorypath, string storeid)
        {

            if (String.IsNullOrEmpty(categorypath)) return null;

            try
            {

                var _sm = (from sm in context.SpecMasks
                           where sm.Categorypath.ToUpper().Equals(categorypath.ToUpper())
                           && sm.StoreId.Equals(storeid)
                           select sm);
                return _sm.ToList();

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// For Save use
        /// </summary>
        /// <param name="categorypath"></param>
        /// <param name="AttrCatId"></param>
        /// <param name="Attrid"></param>
        /// <returns></returns>
        public SpecMask getSpecMaskByKey(string categorypath, string storeid, int AttrCatId, int Attrid)
        {

            if (String.IsNullOrEmpty(categorypath)) return null;

            try
            {
                var _sm = (from sm in context.SpecMasks
                           where sm.Categorypath.ToUpper().Equals(categorypath.ToUpper())
                           && sm.AttrCatId == AttrCatId && sm.Attrid == Attrid
                           && sm.StoreId == storeid
                           select sm).FirstOrDefault();
                return _sm;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }


        /// <summary>
        /// return CTOSSpec mask
        /// </summary>
        /// <param name="categorypath"></param>
        /// <returns></returns>
        public List<CTOSSpecMask> getCTOSSpecMask(string categorypath,string storeid)
        {

            if (String.IsNullOrEmpty(categorypath)) return null;

            try
            {
                var _sm = (from sm in context.CTOSSpecMasks
                           where sm.CategoryPath .ToUpper().Equals(categorypath.ToUpper()) && sm.StoreID==storeid
                           select sm);
                return _sm.ToList();

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }




        #endregion

        #region Creat Update Delete
        public int save(SpecMask _specmask)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_specmask == null || _specmask.validate() == false) return 1;
            //Try to retrieve object from DB

            SpecMask _exist_smask = getSpecMaskByKey(_specmask.Categorypath, _specmask.StoreId, _specmask.AttrCatId, _specmask.Attrid);
            try
            {
                if (_exist_smask == null)  //object not exist 
                {
                    //Insert                  
                    context.SpecMasks.AddObject(_specmask);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update                   
                    if (_specmask.helper != null && _specmask.helper.context != null)
                        context = _specmask.helper.context;

                    context.SpecMasks.ApplyCurrentValues(_specmask);
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

        public int delete(SpecMask _specmask)
        {

            if (_specmask == null || _specmask.validate() == false) return 1;

            try
            {
                SpecMask _exist_smask = getSpecMaskByKey(_specmask.Categorypath, _specmask.StoreId, _specmask.AttrCatId, _specmask.Attrid);
                if (_exist_smask != null)
                {
                    context.DeleteObject(_exist_smask);
                    context.SaveChanges();
                    return 0;
                }
                else
                    return -1;

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
            return typeof(SpecMaskHelper).ToString();
        }
        #endregion
    }
}