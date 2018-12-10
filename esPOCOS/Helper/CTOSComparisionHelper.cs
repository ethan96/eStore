using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class CTOSComparisionHelper : Helper
    {
        #region Business Read
        public List<CTOSAttribute> getCtosAttribute(string langid)
        {

            try
            {
                var _ctosAttribute = (from cn in context.CTOSAttributes.Include("CTOSAttributelangs")
                                      from cnl in context.CTOSAttributelangs
                                      where cn.AttrID == cnl.AttrID && cnl.LangID == langid
                                      select cn);

                if (_ctosAttribute != null)
                {
                    foreach (CTOSAttribute ct in _ctosAttribute)
                        ct.helper = this;

                    return _ctosAttribute.ToList();
                }
                else
                {
                    return new List<CTOSAttribute>();
                }

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<CTOSAttribute>();
            }
        }

        
        public CTOSAttribute getCtosAttributeByKey(int attributeid  ) {
            var _ctosa = (from ca in context.CTOSAttributes
                         where ca.AttrID == attributeid
                         select ca).FirstOrDefault();
            return _ctosa;
        
        }

        public CTOSComparision getCTOScomparision(int   id)
        {
            var _ctoscomp = (from ca in context.CTOSComparisions
                            where ca.ID == id
                             select ca).FirstOrDefault();

            if (_ctoscomp != null)
                _ctoscomp.helper = this;

            return _ctoscomp;

        }


        #endregion

        /// <summary>
        /// For OM , save Ctos attribute and attribute value
        /// </summary>
        /// <param name="_ctosattribute"></param>
        /// <returns></returns>
        public int save(CTOSAttribute _ctosattribute)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_ctosattribute == null || _ctosattribute.validate() == false) return 1;
            //Try to retrieve object from DB
            CTOSAttribute _exist_ctosattribute = getCtosAttributeByKey(_ctosattribute.AttrID);
            try
            {
                if (_exist_ctosattribute == null)  //object not exist 
                {

                    context.CTOSAttributes .AddObject(_ctosattribute);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context = _ctosattribute.helper.context;
                    context.CTOSAttributes.ApplyCurrentValues(_ctosattribute);
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

        public int delete(CTOSAttribute _ctosattribute)
        {

            if (_ctosattribute == null || _ctosattribute.validate() == false) return 1;
            try
            {
                context.DeleteObject(_ctosattribute);
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
        /// For OM , save Ctoscomparision
        /// </summary>
        /// <param name="_ctosattribute"></param>
        /// <returns></returns>
        public int save(CTOSComparision  _ctoscomparision)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_ctoscomparision == null || _ctoscomparision.validate() == false) return 1;
            //Try to retrieve object from DB
            CTOSComparision _exist_ctoscomp = getCTOScomparision(_ctoscomparision.ID );
            try
            {
                if (_exist_ctoscomp == null)  //object not exist 
                {
                    context.CTOSComparisions.AddObject(_ctoscomparision);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context = _ctoscomparision.helper.context;
                    context.CTOSComparisions.ApplyCurrentValues(_ctoscomparision);
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

        /// <summary>
        /// Delete ctoscomparision
        /// </summary>
        /// <param name="_ctoscomparision"></param>
        /// <returns></returns>

        public int delete(CTOSComparision _ctoscomparision)
        {

            if (_ctoscomparision == null || _ctoscomparision.validate() == false) return 1;
            try
            {
                if (_ctoscomparision.helper != null && _ctoscomparision.helper.context != null)
                    context = _ctoscomparision.helper.context;

                
                context.DeleteObject(_ctoscomparision);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

    

        #region Creat Update Delete

        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(CartHelper).ToString();
        }
        #endregion
    }
}