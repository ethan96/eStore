using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Data.Entity;
using System.Data.EntityClient;
namespace eStore.POCOS.DAL
{

    public partial class XRuleSetHelper : Helper
    {
        #region Creat Update Delete
        public int save(XRuleSet _xrule)
        {
            try
            {
                if (_xrule != null)
                {
                    var _exRule = getXRuleSet(_xrule.RulesetID);
                    if (_exRule == null)
                    {
                        context.XRuleSets.AddObject(_xrule);
                        context.SaveChanges();
                    }
                    else
                    {
                        if (_xrule.helper != null)
                        {
                            this.setContext(_xrule.helper.context);
                            context.XRuleSets.ApplyCurrentValues(_xrule);
                            context.SaveChanges();
                        }
                    }
                    
                }
                return 0;
            }
            catch (Exception)
            {
                return 5000;
            }
        }

        public int delete(XRuleSet _xrule)
        {
            if (_xrule == null)
                return -5000;
            try
            {
                var cc = getXRuleSet(_xrule.RulesetID);
                if (cc != null)
                {
                    if (cc.Children.Any())
                    {
                        foreach (var c in cc.Children.ToList())
                            c.delete();
                    }
                    context = new eStore3Entities6();
                    var qq = getXRuleSet(_xrule.RulesetID);
                    context.XRuleSets.DeleteObject(qq);
                    context.SaveChanges();
                    return 0;
                }
                else
                    return -5000;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
            
        }

        public XRuleSet getXRuleSet(int id)
        {
            return context.XRuleSets.FirstOrDefault(x => x.RulesetID == id);
        }
        #endregion
    }
}