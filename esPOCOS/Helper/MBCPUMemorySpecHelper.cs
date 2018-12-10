using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class MBCPUMemorySpecHelper : Helper { 
        #region Business Read
        public MBCPUMemorySpec get(int id)
        {
            return context.MBCPUMemorySpecs.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.MBCPUMemorySpecs.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(MBCPUMemorySpec _mbcpumemoryspec)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_mbcpumemoryspec == null || _mbcpumemoryspec.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_mbcpumemoryspec.Id ==0 || !isExists(_mbcpumemoryspec.Id))  //object not exist 
                {
                    //Insert
                    context.MBCPUMemorySpecs.AddObject( _mbcpumemoryspec);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.MBCPUMemorySpecs.ApplyCurrentValues( _mbcpumemoryspec);
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
        public int delete(MBCPUMemorySpec _mbcpumemoryspec)
        {
       
            if (_mbcpumemoryspec == null || _mbcpumemoryspec.validate() == false) return 1;
            try
            {
                context.DeleteObject(_mbcpumemoryspec);
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