using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS.PocoX
{
 public   class XRuleMatcher
 {
   private  List<CartItem> Candidate = new List<CartItem>();
   private List<CartItem> Qualified = new List<CartItem>();
  private   XRuleSet _rule;
 

   private bool? _isMatched;
   public XRuleMatcher(XRuleSet rule, List<CartItem> candidate)
   {
       if(candidate!=null)
           Candidate = candidate.ToList();
       _rule = rule;
   }

   public int unitQty {

       get {
           return _rule == null ? 0 : _rule.UnitQty;
       }
   }
   public XRuleMatcher(XRuleSet rule, CartItem candidateitem)
   {
       if (candidateitem != null)
       {
           Candidate.Add(candidateitem);
       }
       _rule = rule;
   }
   private void init()
   {
       _isMatched = false;
       if (_rule == null || !Candidate.Any())
           return;
       List<CartItem> candidate = Candidate.ToList();
       if (_rule.isLeaf())
       {

           foreach (var item in candidate)
           {
               bool ismatcheditem = false;
               if (_rule.IncludeDetails.GetValueOrDefault())
               {
                   if (item.partX is Product_Ctos && item.btosX != null)
                       ismatcheditem = _rule.isMatched(item.btosX.parts);
                   else if (item.partX is Product_Bundle && item.bundleX != null)
                   {
                       ismatcheditem = _rule.isMatched(item.bundleX.parts);
                   }
               }
               else
               {
                   ismatcheditem = _rule.isMatched(item.partX);
               }

               if (ismatcheditem)
               {
                   int maximum = int.MaxValue;
                   moveToQualified(item, _rule.UnitQty, ref Candidate, ref Qualified, ref maximum);
                   _isMatched = true;
               }
           }
       }
       else
       {

           if (_rule.conjoinBy == eStore.POCOS.XRuleSet.XRuleSetConjoinBy.And)
           {
               List<CartItem> prematched = new List<CartItem>();
               List<XRuleMatcher> matchers = new List<XRuleMatcher>();
               if (_rule.checkLevel == eStore.POCOS.XRuleSet.XRuleSetCheckLevel.Cart)
               {
                   _isMatched = true;
                   foreach (XRuleSet xrule in _rule.Children.OrderBy(x => x.Priority))
                   {
                       XRuleMatcher childMather = new XRuleMatcher(xrule, candidate);
                       if (childMather.isMatched == false)
                       {
                           _isMatched = false;
                           break;
                       } 
                       else
                       {
                           prematched.AddRange(childMather.getQualified());
                           matchers.Add(childMather);
                       }
                   }
               }
               else
               {
                   _isMatched = false;
                   foreach (CartItem item in candidate)
                   {
                       List<XRuleMatcher> tmpmatchers = new List<XRuleMatcher>();
                       bool isitemmatched = true;
                       foreach (XRuleSet xrule in _rule.Children.OrderBy(x => x.Priority))
                       {
                           XRuleMatcher childMather = new XRuleMatcher(xrule, item);
                           if (!childMather.isMatched)
                           {
                               isitemmatched = false;

                           }
                           else
                           {
                               tmpmatchers.Add(childMather);
                           }

                       }
                       if (isitemmatched)
                       {
                           _isMatched = true;
                           prematched.Add(item);
                           matchers=tmpmatchers;;
                       }

                   }
               }
               if (_isMatched.GetValueOrDefault())
               {
                   int unitlimit = matchers.Select(m => m.getQualified().Sum(x => x.Qty) /m.unitQty).Min();

                   foreach (var m in matchers)
                   {
                       int maximum = m.unitQty * unitlimit;
                       foreach (var item in m.getQualified().OrderByDescending(x=>x.UnitPrice))
                       {
                           moveToQualified(item, _rule.UnitQty, ref m.Qualified, ref Qualified,ref maximum, unitlimit);
                       }
                   }

               }

           }
           else
           {
               _isMatched = false;
               foreach (XRuleSet xrule in _rule.Children.OrderBy(x => x.Priority))
               {
                   XRuleMatcher childMather = new XRuleMatcher(xrule, Candidate);
                   if (childMather.isMatched)
                   {
                       _isMatched = true;
                       int maximum = int.MaxValue;
                       foreach (var qualifiedItem in childMather.getQualified())
                       {
                           moveToQualified(qualifiedItem, xrule.UnitQty, ref childMather.Qualified, ref Qualified, ref maximum);
                       }
                   }
               }
           }
       }
   }

   private void moveToQualified(CartItem item, int unitqty, ref    List<CartItem> Candidate, ref    List<CartItem> Qualified,ref int maximum, int? unitlimit=null)
   {
       CartItem ci = Candidate.FirstOrDefault(x => x.ItemNo == item.ItemNo);
       if (ci == null || ci.Qty == 0 || maximum<=0)
           return;

       CartItem qi = Qualified.FirstOrDefault(x => x.ItemNo == item.ItemNo);
       int discountqty=0;
       if (unitqty == 0) 
       { 
           unitqty = ci.Qty;
       }

       int multiplier =    ci.Qty / unitqty;;
       if (unitlimit.HasValue && unitlimit.Value > 0 && unitlimit < multiplier)
           multiplier = unitlimit.Value;

       if (ci.Qty >= unitqty * multiplier)
       {
         
           discountqty = unitqty * multiplier;
           if (discountqty > maximum)
           {
               discountqty = maximum;
           }
           maximum -= discountqty;
           ci.Qty -= discountqty;
       }
       

       if (qi == null)
       {
           CartItem nqi = new CartItem();
           item.copyTo(nqi);
           nqi.Qty = discountqty;
           Qualified.Add(nqi);
       }
       else
       {
           qi.Qty += discountqty;
       }

   }



   public bool isMatched
   {
       get {
           if (!_isMatched.HasValue)
           {
               init();
           }
           return _isMatched.GetValueOrDefault();
       }
   }
   public List<CartItem> getQualified()
   {
       if (!_isMatched.HasValue)
       {
           init();
       }
       return Qualified;
   }


    }
}
