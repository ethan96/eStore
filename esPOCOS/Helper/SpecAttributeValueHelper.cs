using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;


namespace eStore.POCOS.DAL
{

    public partial class SpecAttributeValueHelper : Helper
    {

        #region Business Read
        #endregion
        public SpecAttributeValue getSpecAttributeValueByPath(string categoryPath)
        {
            SpecAttributeValue spv = null;
            spv = context.SpecAttributeValues.FirstOrDefault(c => c.AttrValueName.Trim().ToUpper() == categoryPath.Trim().ToUpper());
            return spv;
        }



        #region Creat Update Delete
        public int save(SpecAttributeValue _specattributevalue)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_specattributevalue == null || _specattributevalue.validate() == false)
            {
                _specattributevalue.error_message.Add(new PocoX.ErrorMessage("Error", "SpecAttributeValue is null."));
                return 1;
            }
            else
            {
                //Try to retrieve object from DB
                SpecAttributeValue _exist_specattributevalue = context.SpecAttributeValues.FirstOrDefault(c => c.AttrValueID == _specattributevalue.AttrValueID
                    && c.AttrValueName == _specattributevalue.AttrValueName);
                try
                {
                    if (_exist_specattributevalue == null)  //object not exist 
                    {
                        //Insert
                        context.SpecAttributeValues.AddObject( _specattributevalue);
                        context.SaveChanges();
                        return 0;
                    }
                    else
                    {
                        //Update
                        //_exist_specattributevalue.SpecAttributeValuelangs = _specattributevalue.SpecAttributeValuelangs;
                        context.SpecAttributeValues.ApplyCurrentValues( _specattributevalue);
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                    _specattributevalue.error_message.Add(new PocoX.ErrorMessage("Error", ex.Message));
                    return -5000;
                }
            }
            
        }

        public int delete(SpecAttributeValue _specattributevalue)
        {
            try
            {
                if (_specattributevalue == null || _specattributevalue.validate() == false)
                {
                    _specattributevalue.error_message.Add(new PocoX.ErrorMessage("Error", "Specattributevalue is null."));
                    return 1;
                }
                else
                {
                    context.SpecAttributeValues.DeleteObject(_specattributevalue);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                _specattributevalue.error_message.Add(new PocoX.ErrorMessage("Error", ex.Message));
                return -5000;
            }


        }
        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(SpecAttributeValueHelper).ToString();
        }
        #endregion
    }
}