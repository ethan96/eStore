using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class RuleSetHelper : Helper
    {
        #region Business Read
        public RuleSet  getRuleSetByID(int rulesetid)
        {          

            try
            {
                var _ruleset = (from _rs in context.RuleSets 
                              where (_rs.RuleSetID == rulesetid )
                              select _rs).FirstOrDefault();

                if (_ruleset != null)
                    _ruleset.helper = this;

                return _ruleset;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }


   


        #endregion

        #region Creat Update Delete


        public int save(RuleSet _ruleset)
        {         
            //if parameter is null or validation is false, then return  -1 
            if (_ruleset == null || _ruleset.validate() == false) return 1;
            //Try to retrieve object from DB             


            RuleSet _exist_rule = getRuleSetByID(_ruleset.RuleSetID );
            try
            {

                if (_exist_rule == null) 
                {
                    context.RuleSets.AddObject(_ruleset);     
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context = _exist_rule.helper.context;
                    context.RuleSets.ApplyCurrentValues(_ruleset);                          
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

        public int deleteRuleDetail(RuleSetDetail rsdetail) {
            if (rsdetail == null) return 1;

            try
            {
                
                context.DeleteObject(rsdetail);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        
        }


        public int delete(RuleSet  _ruleset)
        {
            if (_ruleset == null) return 1;

            try
            {
                context.DeleteObject(_ruleset);
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
            return typeof(RuleSetHelper).ToString();
        }
        #endregion
    }
}