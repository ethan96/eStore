using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;


namespace eStore.POCOS.DAL
{

    public partial class SpecAttributeValuelangHelper : Helper
    {

        #region Business Read
        #endregion
        #region Creat Update Delete
        public int save(SpecAttributeValuelang _specattributevaluelang)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_specattributevaluelang == null || _specattributevaluelang.validate() == false)
            {
                _specattributevaluelang.error_message.Add(new PocoX.ErrorMessage("Error", "Specattributevaluelang is null."));
                return 1;
            }

            //Try to retrieve object from DB
            SpecAttributeValuelang _exist_specattributevaluelang = context.SpecAttributeValuelangs.FirstOrDefault(c => c.StoreID == _specattributevaluelang.StoreID
                                                    && c.ID == _specattributevaluelang.ID && c.AttrValueID == _specattributevaluelang.AttrValueID);
            try
            {
                if (_exist_specattributevaluelang == null)  //object not exist 
                {
                    //Insert
                    context.SpecAttributeValuelangs.AddObject(_specattributevaluelang);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.SpecAttributeValuelangs.ApplyCurrentValues(_specattributevaluelang);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                _specattributevaluelang.error_message.Add(new PocoX.ErrorMessage("Error", ex.Message));
                return -5000;
            }
        }

        public int delete(SpecAttributeValuelang _specattributevaluelang)
        {
            if (_specattributevaluelang == null || _specattributevaluelang.validate() == false)
            {
                _specattributevaluelang.error_message.Add(new PocoX.ErrorMessage("Error", "Specattributevaluelang is null."));
                return 1;
            }
            else
            {
                try
                {
                    context.DeleteObject(_specattributevaluelang);
                    context.SaveChanges();
                    return 0;
                }
                catch (Exception ex)
                {
                    eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                    _specattributevaluelang.error_message.Add(new PocoX.ErrorMessage("Error", ex.Message));
                    return -5000;
                }
            }

        }
        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(SpecAttributeValuelangHelper).ToString();
        }
        #endregion
    }
}