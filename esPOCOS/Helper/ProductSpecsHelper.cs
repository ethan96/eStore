using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class ProductSpecsHelper : Helper
    {
        #region Business Read
        public ProductSpec getSpecsByID(int id)
        {
             

            try
            {              
                var _productspec = (from ps in context.ProductSpecs
                                   where (ps.ID  == id)
                                   select ps).FirstOrDefault();

                if (_productspec != null)
                    _productspec.helper = this;

                return _productspec;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        
    
        #endregion

        #region Creat Update Delete


        public int save(ProductSpec _specs)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_specs == null || _specs.validate() == false) return 1;

            //Try to retrieve object from DB  
            ProductSpec  _exist_spec = getSpecsByID (_specs.ID );
            try
            {

                if (_exist_spec == null)  //Add 
                {                    
                    context.ProductSpecs .AddObject(_specs); //state=added.
                    context.SaveChanges();                                              
                    return 0;
                }
                else //Update 
                {
                    context.ProductSpecs.ApplyCurrentValues(_specs); //Even applycurrent value, cartmaster state still in unchanged.                 
                    context.SaveChanges();            
                }

                    return 0;               
            }

            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                
                return -5000;
            }

        }

        /// <summary>
        ///  Delete Productspec which is not return from Entity query.
        /// </summary>
        /// <param name="_spec"></param>
        /// <returns></returns>

         public int delete(ProductSpec _spec)
        {
            try
            {

            if (_spec == null || _spec.validate() == false) return 1;

            if (_spec.ID == 0)
            {
                ProductSpec exist = (from s in context.ProductSpecs
                                     where s.CatID == _spec.CatID && s.AttrID == _spec.AttrID && s.AttrValueID == _spec.AttrValueID
                                     select s).FirstOrDefault();
                context.DeleteObject(exist);
                context.SaveChanges();
            }
            else

                context = _spec.helper.context;
                context.DeleteObject(_spec);
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
            return typeof(CartHelper).ToString();
        }
        #endregion
    }
}