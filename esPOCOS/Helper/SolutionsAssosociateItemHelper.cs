using System;
using System.Collections.Generic;
using System.Linq;
using eStore.Utilities;
using System.Text;

namespace eStore.POCOS.DAL
{
    public class SolutionsAssosociateItemHelper : Helper
    {
        internal int save(SolutionsAssosociateItem solutionsAssosociateItem)
        {
            //if parameter is null or validation is false, then return  -1 
            if (solutionsAssosociateItem == null || solutionsAssosociateItem.validate() == false) return 1;
            //Try to retrieve object from DB
            SolutionsAssosociateItem _exist_solutionsAssosociateItem = getSolutionsAssosociateItemById(solutionsAssosociateItem.Id, solutionsAssosociateItem.StoreID);//
            try
            {
                if (_exist_solutionsAssosociateItem == null)  //object not exist 
                {
                    //Insert
                    context.SolutionsAssosociateItems.AddObject(solutionsAssosociateItem);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.SolutionsAssosociateItems.ApplyCurrentValues(solutionsAssosociateItem);
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

        public SolutionsAssosociateItem getSolutionsAssosociateItemById(int id, string storeid)
        {
            SolutionsAssosociateItem solutionsAssosociateItem = context.SolutionsAssosociateItems.FirstOrDefault(s => s.Id == id && s.StoreID == storeid);
            return solutionsAssosociateItem;
        }

        public int delete(SolutionsAssosociateItem solutionsAssosociateItem)
        {
            if (solutionsAssosociateItem == null || solutionsAssosociateItem.validate() == false) return 1;
            try
            {
                var exsolution = getSolutionsAssosociateItemById(solutionsAssosociateItem.Id, solutionsAssosociateItem.StoreID);
                if (exsolution != null)
                {
                    context.DeleteObject(exsolution);
                    context.SaveChanges();
                    return 0;
                }
                return -5000; 
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
    }
}
