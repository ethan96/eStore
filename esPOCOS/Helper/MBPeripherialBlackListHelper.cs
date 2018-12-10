using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class MBPeripherialBlackListHelper : Helper { 
        #region Business Read
        public MBPeripherialBlackList get(int id)
        {
            return context.MBPeripherialBlackLists.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.MBPeripherialBlackLists.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(MBPeripherialBlackList _mbperipherialblacklist)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_mbperipherialblacklist == null || _mbperipherialblacklist.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_mbperipherialblacklist.Id ==0 || !isExists(_mbperipherialblacklist.Id))  //object not exist 
                {
                    //Insert
                    context.MBPeripherialBlackLists.AddObject( _mbperipherialblacklist);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.MBPeripherialBlackLists.ApplyCurrentValues( _mbperipherialblacklist);
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
        public int delete(MBPeripherialBlackList _mbperipherialblacklist)
        {
       
            if (_mbperipherialblacklist == null || _mbperipherialblacklist.validate() == false) return 1;
            try
            {
                context.DeleteObject(_mbperipherialblacklist);
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
	} 
 }