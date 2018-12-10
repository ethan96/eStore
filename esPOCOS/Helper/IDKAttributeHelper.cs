using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class IDKAttributeHelper : Helper
    {

        internal int save(IDKAttribute iDKAttribute)
        {
            if (iDKAttribute == null || iDKAttribute.validate() == false) return 1;
            IDKAttribute _exist_module;
            if(iDKAttribute.Id == 0)
                _exist_module = getIDKAttributebyID(iDKAttribute);
            else
                _exist_module = getIDKAttributebyID(iDKAttribute.Id);
            try
            {
                if (_exist_module == null)  //object not exist 
                {
                    //Insert
                    context.IDKAttributes.AddObject(iDKAttribute);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.IDKAttributes.ApplyCurrentValues(iDKAttribute);
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

        private IDKAttribute getIDKAttributebyID(int p)
        {
            return context.IDKAttributes.FirstOrDefault(c => c.Id == p);
        }

        private IDKAttribute getIDKAttributebyID(IDKAttribute iDKAttribute)
        {
            return context.IDKAttributes.FirstOrDefault(c => c.SProductID == iDKAttribute.SProductID
                    && c.AttributeName == iDKAttribute.AttributeName && c.AttributeValue == iDKAttribute.AttributeValue);
        }

        internal int delete(IDKAttribute iDKAttribute)
        {
            throw new NotImplementedException();
        }
    }
}
