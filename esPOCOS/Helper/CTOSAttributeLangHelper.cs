using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Linq.Expressions;
using eStore.POCOS;
using eStore.POCOS.Sync;
namespace eStore.POCOS.DAL
{

    public partial class CTOSAttributeLangHelper
    {

        #region Business Read

        private eStore3Entities6 context;
        private PISEntities pcontext;

        public CTOSAttributeLangHelper()
        {
            context = new eStore3Entities6();
            pcontext = new PISEntities();
        }

        ~CTOSAttributeLangHelper()
        {
            if (context != null)
                context.Dispose();
            if (pcontext != null)
                pcontext.Dispose();
        }


       
        public CTOSAttributelang getCTOSLang(int attrid, string langid)
        {
            
            try
            {
                var _ctoslang = (from spr in context.CTOSAttributelangs
                              where spr.AttrID == attrid && spr.LangID == langid 
                              select spr).FirstOrDefault();

                if (_ctoslang != null)
                    _ctoslang.helper = this;

                return _ctoslang;

            }
            catch (Exception ex)
            {

                Utilities.eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }

        }

     

        #endregion

        #region Creat Update Delete

        /// <summary>
        /// This for ctosattribute localization
        /// </summary>
        /// <param name="_spec"></param>
        /// <returns></returns>

        public int save(CTOSAttributelang  _spec)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_spec == null || _spec.validate() == false) return 1;

            //Try to retrieve object from DB  
            CTOSAttributelang _exist_lang = getCTOSLang(_spec.AttrID, _spec.LangID);
            try
            {

                if (_exist_lang == null)  //Add 
                {
                    context.CTOSAttributelangs.AddObject(_spec); 
                    context.SaveChanges();
                    return 0;
                }
                else //Update 
                {
                    context.CTOSAttributelangs.ApplyCurrentValues(_spec);                  
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


        public int delete(CTOSAttributelang _spec)
        {

            if (_spec == null || _spec.validate() == false) return 1;

            try
            {
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

        public List<CTOSAttributelang> getCTOSAttributeLang(string langid)
        {

            try
            {
                var cc = (from arr in context.CTOSAttributes
                                   join arrlang in context.CTOSAttributelangs.Where(y => y.LangID == langid)
                                   on arr.AttrID equals arrlang.AttrID into temp
                                   from c in temp.DefaultIfEmpty()
                                   select new
                                   {
                                       AttrID = arr.AttrID,
                                       LangValue = string.IsNullOrEmpty(c.LangValue) ? arr.AttrName : c.LangValue
                                   }).ToList();
                List<CTOSAttributelang> arrLangList = new List<CTOSAttributelang>();
                if (cc != null && cc.Count > 0)
                {
                    foreach (var c in cc)
                    {
                        arrLangList.Add(new CTOSAttributelang()
                        {
                            AttrID = c.AttrID,
                            LangValue = c.LangValue,
                            LangID = langid,
                            helper = this
                        });
                    }
                }
                return arrLangList;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<CTOSAttributelang>();
            }
            
        }



        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(CTOSAttributeLangHelper).ToString();
        }
        #endregion
    }


}