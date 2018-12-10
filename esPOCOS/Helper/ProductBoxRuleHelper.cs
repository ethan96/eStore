using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class ProductBoxRuleHelper : Helper
    {

        #region Business Read

        /// <summary>
        /// Get a particular Product box Rule
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ProductBoxRule getBoxRulebyID(int id)
        {

            try
            {
                var _productboxrule = (from x in context.ProductBoxRules
                                       where x.ID == id
                                       select x).FirstOrDefault();
                return _productboxrule;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }

        }

        /// <summary>
        /// This method return all rules associate with the given parprefix
        /// </summary>
        /// <param name="partprefix"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<ProductBoxRule> getProductBoxRules(string partprefix,string storeid)
        {

            try
            {
                var _productboxrule = (from x in context.ProductBoxRules
                                       where x.StoreID.Equals(storeid) && x.PartNoPrefix.ToUpper().StartsWith(partprefix.ToUpper())
                                       select x);
                if (_productboxrule != null && _productboxrule.Count() > 0)
                {
                    foreach (ProductBoxRule pbr in _productboxrule)
                        pbr.helper = this;
                    return _productboxrule.ToList();
                }
                else {

                    return new List<ProductBoxRule>();
                }

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }

        }


        /// <summary>
        /// Return all rules associate with the given boxid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ProductBoxRule> getBoxRulebyboxid(int id, string storeid)
        {
            try
            {
                var _productboxrule = (from x in context.ProductBoxRules
                                       where x.BoxID.Equals(id) && x.StoreID.Equals(storeid)
                                       select x);
                if (_productboxrule != null)
                    return _productboxrule.ToList();
                else
                    return new List<ProductBoxRule>();

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }

        }

        /// <summary>
        /// Return product rules with the productRuleId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProductBoxRule getProductBoxRulesById(int id)
        {
            try
            {
                ProductBoxRule productRule = (from x in context.ProductBoxRules
                                       where x.ID.Equals(id) 
                                       select x).FirstOrDefault();
                if (productRule != null)
                    productRule.helper = this;
                return productRule;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        #endregion


        #region Creat Update Delete

        /// <summary>
        /// Update and new a product box rule
        /// </summary>
        /// <param name="_boxrule"></param>
        /// <returns></returns>

        public int save(ProductBoxRule _boxrule)
        {

            if (_boxrule == null || _boxrule.validate() == false) return 1;
            ProductBoxRule _exist_user = getBoxRulebyID(_boxrule.ID);

            try
            {
                if (_exist_user == null)  //object not exist 
                {
                    //Insert                  
                    context.ProductBoxRules.AddObject(_boxrule);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update            
                    if (_boxrule.helper != null && _boxrule.helper.context != null)
                        context = _boxrule.helper.context;

                    context.ProductBoxRules.ApplyCurrentValues(_boxrule);
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


        public int delete(ProductBoxRule _boxrule)
        {

            if (_boxrule == null || _boxrule.validate() == false) return 1;

            try
            {
                context.DeleteObject(_boxrule);
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
            return typeof(ProductBoxRuleHelper).ToString();
        }
        #endregion
    }
}