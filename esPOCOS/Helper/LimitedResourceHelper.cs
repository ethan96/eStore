using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class LimitedResourceHelper : Helper
    {
        public LimitedResource getLimitedResourceByID(int _id)
        {
            var _limited = (from c in context.LimitedResources
                            where c.ResourceID == _id
                            select c).FirstOrDefault();
            return _limited;
        }

        public LimitedResource getLimitedResourceByID(int _id,string _storeID)
        {
            var _limited = (from c in context.LimitedResources
                            where c.ResourceID == _id && c.StoreID == _storeID
                            select c).FirstOrDefault();
            return _limited;
        }

        public LimitedResource getLimitedResouceByName(string _name)
        {
            var _limited = (from c in context.LimitedResources
                            where c.Name.ToUpper() == _name.ToUpper()
                            select c).FirstOrDefault();
            return _limited;
        }

        public List<LimitedResource> getAllLimitedResource(Store _store)
        {
            var _limitedLS = (from c in context.LimitedResources
                              where c.StoreID == _store.StoreID
                              select c).ToList();
            return _limitedLS;
        }


        #region Creat Update Delete
        public int save(LimitedResource _limiteresource)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_limiteresource == null) return 1;
            //Try to retrieve object from DB
            LimitedResource _exist_limiteresource = getLimitedResourceByID(_limiteresource.ResourceID);
            try
            {
                if (_exist_limiteresource == null)  //object not exist 
                {
                    //Insert
                    context.LimitedResources.AddObject(_limiteresource);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.LimitedResources.ApplyCurrentValues(_limiteresource);
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

        public int delete(LimitedResource _limiteresource)
        {
            eStoreLoger.Fatal("LimitedResource can not delete", "", "", "");
            return -5000;
        }
        #endregion 

    
    }
}
