using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Linq.Expressions;
namespace eStore.POCOS.DAL
{

    public partial class PeripheralHelper : Helper 
    {
        #region Business Read

        public List<PeripheralCatagory> getPeripheralCategories(string storeID) {
            var _categories = (from cat in context.PeripheralCatagories
                              where cat.StoreID == storeID
                              orderby cat.seq ascending,cat.name ascending
                              select cat).ToList();

            foreach(var c in _categories)
                c.helper = this;
            return _categories;   
        }

        public PeripheralProduct getPeripheralProduct(int peripheralid) {
            var _pp = (from pp in context.PeripheralProducts 
                       where pp.PeripheralID == peripheralid 
                              select pp).FirstOrDefault();

            if (_pp != null)
                return _pp;
            else
                return null;        
        }

        /// <summary>
        /// Get PeripheralProduct by partno keywords, search scope Partno or Description. 
        /// </summary>
        /// <param name="partnokeyword"></param>
        /// <returns></returns>
        public PeripheralProduct getPeripheralProduct(string partnokeyword,string storeID,int subCategoryId = 0)
        {
            PeripheralProduct _pp = null;
            //0 表示是search的时候
            if (subCategoryId == 0)
            {
                _pp = (from p in context.PeripheralProducts
                       where p.StoreID == storeID && p.SProductID.ToUpper() == partnokeyword.ToUpper()
                       select p).FirstOrDefault();
                if (_pp == null)
                {
                    _pp = (from pp in context.PeripheralProducts
                           where pp.StoreID == storeID
                           && pp.SProductID.ToUpper().Contains(partnokeyword.ToUpper()) ||
                           pp.Description.ToUpper().Contains(partnokeyword.ToUpper())
                           orderby pp.SProductID.IndexOf(partnokeyword)
                           select pp).FirstOrDefault();
                }
            }
            else
            {
                //add
                _pp = (from pp in context.PeripheralProducts
                       where pp.StoreID == storeID
                       && pp.SProductID.ToUpper() == partnokeyword.ToUpper()
                       && pp.SubCatagoryID == subCategoryId 
                       select pp).FirstOrDefault();
            }
            
            return _pp;
        }

        private  PeripheralCatagory getPeripheralCategory(int categoryid)
        {
            var _pp = (from pp in context.PeripheralCatagories
                       where  pp.catagoryID==categoryid
                       select pp).FirstOrDefault();

            if (_pp != null)
                return _pp;
            else
                return null;
        }

        public PeripheralCatagory getPeripheralCategoryByName(string storeid, string categoryName)
        {
            var _pp = (from pp in context.PeripheralCatagories
                       where pp.name == categoryName && pp.StoreID==storeid
                       select pp).FirstOrDefault();

            if (_pp != null)
            {
                _pp.helper = this;
                return _pp;
            }                
            else
                return null;
        }

        public List<PeripheralsubCatagory> getAllPeripheralSubCategoryByStoreId(string storeid)
        {
            List<PeripheralsubCatagory> ls = new List<PeripheralsubCatagory>();
            ls = (from c in context.PeripheralsubCatagories
                  where c.StoreID.Equals(storeid)
                  select c).ToList();
            if (ls.Any())
            {
                foreach (var li in ls)
                    li.helper = this;
            }
            return ls;
        }

        private PeripheralsubCatagory getPeripheralSubCategory(int subCategoryid)
        {
            var _pp = (from pp in context.PeripheralsubCatagories
                       where pp.subCatagoryID == subCategoryid
                       select pp).FirstOrDefault();

            if (_pp != null)
                return _pp;
            else
                return null;
        }

        public PeripheralsubCatagory getPeripheralSubCategoryByName(string subCategoryName,string storeid)
        {
            var _pp = (from pp in context.PeripheralsubCatagories.Include("PeripheralCatagory")
                       where pp.name.Equals(subCategoryName,StringComparison.OrdinalIgnoreCase)
                        && pp.StoreID.Equals(storeid)
                       select pp).FirstOrDefault();
            if (_pp != null)
                return _pp;
            else
                return null;
        }

        public List<PeripheralProduct> getPeripheralProductsByKey(string key,string storeId)
        {
            List<PeripheralProduct> ls = new List<PeripheralProduct>();
            if(!string.IsNullOrEmpty(key))
            {
                ls = (from c in context.PeripheralProducts
                      where c.SProductID.Contains(key) && c.StoreID==storeId
                      select c).Take(10).ToList();
            }
            return ls;
        }

        /// <summary>
        ///  Import Excel to Peripheral product tables.
        /// </summary>
        /// <param name="partno"></param>
        /// <param name="main_category"></param>
        /// <param name="sub_category"></param>
        /// <param name="description"></param>
        /// <param name="price"></param>
        /// <param name="sapstatus"></param>
        /// <param name="version"></param>

        public int InsertPeripheral(string partno, string main_category, string sub_category,string description, decimal price, string sapstatus, string version, string storeid ) {

           int? rtn=  context.SPInsertPeripheralProducts(partno, main_category, sub_category, description, price, sapstatus, version,storeid).FirstOrDefault();

           if (rtn.HasValue)
               return rtn.Value;
           else
               return 0;

        }

        public PeripheralCompatible getPeripheralCompatible(PeripheralCompatible  _pheripheralC)
        {
            var _pp = (from pp in context.PeripheralCompatibles
                       where pp.PeripheralID == _pheripheralC.PeripheralID && pp.PartNo == _pheripheralC.PartNo && pp.StoreID == _pheripheralC.StoreID
                       select pp).FirstOrDefault();

            if (_pp != null)
            {
                _pp.helper = this;
                return _pp;
            }
            else
                return null;
        }

        public PeripheralAddOn getPeripheralAddOnById(string storeId,int addOnItemID)
        {
            var pItem = (from p in context.PeripheralAddOns
                     where p.StoreID == storeId && p.AddOnItemID == addOnItemID
                     select p).FirstOrDefault();
            return pItem;
        }

        public List<PeripheralProduct> getAllPeripheralProductByStoreId(string storeid)
        {
            List<PeripheralProduct> ls = new List<PeripheralProduct>();
            if (string.IsNullOrEmpty(storeid))
                return ls;
            ls = (from c in context.PeripheralProducts.Include("PeripheralsubCatagory")
                    where c.StoreID.Equals(storeid)
                    select c).ToList();
            if (ls.Any())
            {
                foreach (var li in ls)
                    li.helper = this;
            }
            return ls;
        }

        //获取Peripheral Product
        public Dictionary<string, string> getAllPeripheralProduct(string storeid, string keyword)
        {

            Dictionary<string, string> products = new Dictionary<string, string>();

            var _products = from p in context.PeripheralProducts
                            where p.StoreID == storeid && (p.SProductID.Contains(keyword))
                            select new {p.PeripheralID, p.SProductID , p.PeripheralsubCatagory.name};

            if (_products != null)
            {
                foreach (var p in _products.ToList())
                {
                    if (!products.ContainsKey(p.PeripheralID + p.SProductID))
                        products.Add(p.PeripheralID + p.SProductID, p.SProductID+"==>"+p.name);
                }
            }

            return products;
        }

        #endregion


        #region Create Update Delete
        

        public int save(PeripheralProduct  _pheripheral)
        {

            if (_pheripheral == null || _pheripheral.validate() == false) return 1;
            //Try to retrieve object from DB

            PeripheralProduct _existPD = getPeripheralProduct(_pheripheral.PeripheralID);
            try
            {
                if (_existPD == null)  //object not exist 
                {
                    //Insert            
 
                    context.PeripheralProducts.AddObject(_pheripheral);
                    context.SaveChanges();
                    return 0;

                }
                else
                {
                    //Update                  
                    context.PeripheralProducts.ApplyCurrentValues(_pheripheral);
                    context.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    return 0;

                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }



        public int save(PeripheralCompatible  _pheripheralC)
        {

            if (_pheripheralC == null || _pheripheralC.validate() == false) return 1;
            //Try to retrieve object from DB

            PeripheralCompatible _existPD = getPeripheralCompatible(_pheripheralC);
            try
            {
                if (_existPD == null)  //object not exist 
                {
                    //Insert            

                    context.PeripheralCompatibles.AddObject(_pheripheralC);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update                  
                    context.PeripheralCompatibles.ApplyCurrentValues(_pheripheralC);
                    context.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int save(PeripheralsubCatagory _pheripheralC)
        {

            if (_pheripheralC == null || _pheripheralC.validate() == false) return 1;
            //Try to retrieve object from DB

            PeripheralsubCatagory _existPD = getPeripheralSubCategory(_pheripheralC.subCatagoryID);
            try
            {
                if (_existPD == null)  //object not exist 
                {
                    //Insert            

                    context.PeripheralsubCatagories.AddObject(_pheripheralC);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update                  
                    context.PeripheralsubCatagories.ApplyCurrentValues(_pheripheralC);
                    context.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        /// <summary>
        /// Save pheripheral category for seq
        /// </summary>
        /// <param name="_pheripheralC"></param>
        /// <returns></returns>
        public int saveCategory(PeripheralCatagory _pheripheralC)
        {

            if (_pheripheralC == null || _pheripheralC.validate() == false) return 1;
            //Try to retrieve object from DB

            PeripheralCatagory _existPD = getPeripheralCategory(_pheripheralC.catagoryID);
            try
            {
                if (_existPD == null)  //object not exist 
                {
                    //Insert            

                    context.PeripheralCatagories.AddObject(_pheripheralC);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update                  
                    context.PeripheralCatagories.ApplyCurrentValues(_pheripheralC);
                    context.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        

        public int delete(PeripheralProduct _pheripheral)
        {
            if (_pheripheral == null || _pheripheral.validate() == false) return 1;
            _pheripheral = getPeripheralProduct(_pheripheral.PeripheralID);
            try
            {
                
                context.PeripheralProducts.DeleteObject(_pheripheral);
                context.SaveChanges();
                return 0;

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(PeripheralsubCatagory _pheripheralSubCategory)
        {
            if (_pheripheralSubCategory == null || _pheripheralSubCategory.validate() == false) return 1;
            _pheripheralSubCategory = getPeripheralSubCategory(_pheripheralSubCategory.subCatagoryID);
            try
            {

                context.PeripheralsubCatagories.DeleteObject(_pheripheralSubCategory);
                context.SaveChanges();
                return 0;

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(PeripheralCompatible _pheripheralC)
        {
            PeripheralCompatible p = _pheripheralC;
            if (_pheripheralC == null || _pheripheralC.validate() == false) return 1;
            try
            {
                if (_pheripheralC.helper != null && _pheripheralC.helper.context != null)
                    context = _pheripheralC.helper.context;
                else
                {
                    p = getPeripheralCompatible(_pheripheralC);
                }
                context.DeleteObject(p);
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
            return typeof(PeripheralHelper).ToString();
        }
        #endregion

        internal int delete(PeripheralCatagory peripheralCatagory)
        {
            PeripheralCatagory p = peripheralCatagory;
            if (peripheralCatagory == null || peripheralCatagory.validate() == false) return 1;
            try
            {
                p = getPeripheralCategory(peripheralCatagory.catagoryID);
                context.DeleteObject(p);
                context.SaveChanges();
                return 0;

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        /// <summary>
        /// this function will delete store all peripheral product data
        /// include PeripheralCatagory,PeripheralsubCatagory,PeripheralProducts,PeripheralCompatible
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public bool deleteOldPeripheral(string storeid)
        {
            if (context != null && context.Connection != null)
            {
                context.Connection.Open();
                System.Data.Common.DbTransaction tran = context.Connection.BeginTransaction();
                try
                {
                    context.ExecuteStoreCommand("DELETE PeripheralCompatible WHERE StoreID = '" + storeid + "'", null);
                    context.ExecuteStoreCommand("DELETE PeripheralProducts WHERE StoreID = '" + storeid + "'", null);
                    context.ExecuteStoreCommand("DELETE PeripheralsubCatagory WHERE StoreID = '" + storeid + "'", null);
                    context.ExecuteStoreCommand("DELETE PeripheralCatagory WHERE StoreID = '" + storeid + "'", null);
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return false;
                }
            }
            return false;
        }
    }

   
}