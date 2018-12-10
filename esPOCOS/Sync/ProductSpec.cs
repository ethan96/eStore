using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.WindowService
{
    class ProductSpecProcess
    {
        eStore3Entities6 context = new eStore3Entities6();

        public void createMainCatAttributeValue( ) {
            var _vcategory = from ca in context.ProductCategories 
                             where  ca.ParentCategoryID ==null  
                             select ca;

            foreach (ProductCategory vc in _vcategory.ToList())
            {
                SpecAttributeValue spv = new SpecAttributeValue();
                SpecAttributeValuelang spvlang = new SpecAttributeValuelang();

                spv.AttrID = 999999; //Base.ProductType
                spv.AttrValueName = vc.CategoryName ;
                spvlang.DisplayName = vc.CategoryName ;
                spvlang.StoreID = vc.Storeid;
                
                Console.WriteLine(vc.Storeid + ":" + spv.AttrID + ":" + spv.AttrValueName + ":" + spvlang.DisplayName);
                save(spv, spvlang);
            }

        }
        
        
        
        /// <summary>
        /// Create subcategory as attribute
        /// </summary>
        /// <param name="storeid"></param>
        public void createAttributeValue()
        {
            //Select products in particular subcategory, Add main product category/subcategory as spec.
            var _vcategory = (from ca in context.vALLCategories                             
                             select ca).Distinct();

            foreach (vALLCategory vc in _vcategory.ToList())
            {
                SpecAttributeValue spv = new SpecAttributeValue();
                SpecAttributeValuelang spvlang = new SpecAttributeValuelang();

                spv.AttrID = 999999; //Base.ProductType
                spv.AttrValueName = vc.CategoryID;
                spvlang.DisplayName = vc.LocalCategoryName;
                spvlang.StoreID = vc.Storeid;
                save(spv, spvlang);
                Console.WriteLine(vc.Storeid + ":" + vc.CategoryID + ":" + vc.LocalCategoryName);
                                
            }

            var _vcategoryroot = (from ca in context.vRootCategories 
                              select ca).Distinct();


            foreach (vRootCategory   vc in _vcategoryroot.ToList())
            {
                SpecAttributeValue spv = new SpecAttributeValue();
                SpecAttributeValuelang spvlang = new SpecAttributeValuelang();

                spv.AttrID = 999999; //Base.ProductType
                spv.AttrValueName = vc.categoryid;
                spvlang.DisplayName = vc.LocalCategoryName;
                spvlang.StoreID = vc.Storeid;
                 save(spv, spvlang);
                Console.WriteLine(vc.Storeid + ":" + vc.categoryid + ":" + vc.LocalCategoryName);
                
            }
        }


     

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeid"></param>

        public void createproductspec() {

            //Select root category
            var _products = from b in context.vproductcategories      
                            where string.IsNullOrEmpty(b.AttrValueName)==false
                            select b;

            foreach (vproductcategory  vp in _products.ToList())
            {

                ProductSpec ps = new ProductSpec();
                ps.ProductNo = vp.ProductNo ;
                ps.CatID = 999999;
                ps.AttrID = 999998;
                ps.AttrValueID = vp.AttrValueID ;
                ps.created_date = DateTime.Now;
                Console.WriteLine(ps.ProductNo + ":" + ps.CatID + ":" + ps.AttrID + ":" + ps.AttrValueID);
                save(ps);

                var _pc = (from p in context.ProductCategories                          
                          where   p.CategoryPath == vp.AttrValueName 
                          select p).FirstOrDefault();

                if (_pc != null) {

                    ProductCategory parent = _pc.ParentCategory;

                    while (parent != null)
                    {
                        var _spec = (from spec in context.SpecAttributeValues 
                                   where spec.AttrValueName == parent.CategoryPath 
                                   select spec).FirstOrDefault();

                        ProductSpec ps2 = new ProductSpec();
                        ps2.ProductNo = vp.ProductNo;
                        ps2.CatID = 999999;
                        ps2.AttrID = 999999;
                        ps2.AttrValueID = _spec.AttrValueID;
                        ps2.created_date = DateTime.Now;
                        save(ps2);

                        parent = parent.ParentCategory;
                    }
                
                }

            }
        
        }

        /// <summary>
        /// Save Product spec
        /// </summary>
        /// <param name="ps"></param>
        public void save(ProductSpec ps) {
            var _ps = (from p in context.ProductSpecs
                            where p.AttrValueID  ==ps.AttrValueID && p.AttrID ==ps.AttrID && p.CatID == ps.CatID 
                            && p.ProductNo == ps.ProductNo 
                            select p).FirstOrDefault();

            try
            {
                if (_ps == null)
                {
                    Console.WriteLine("save:" + ps.ProductNo);
                    context.ProductSpecs.AddObject(ps);
                    context.SaveChanges();
                }
            }
            catch (Exception e) {

                throw e;
            }
        
        }


        public void createAttributeValue(int catid, int attrid,string name, string localname, string storeid)
        {
            SpecAttributeValue spv = new SpecAttributeValue();
            spv.AttrValueName  = name;

            SpecAttributeValuelang splang = new SpecAttributeValuelang();
            splang.DisplayName = localname;
            splang.StoreID = storeid;

            save(spv, splang);

        }

        public void createAttribute(string name, string localname, string storeid)
        {
            SpecAttributeCat sp = new SpecAttributeCat();
            sp.AttrCatName = name;
            sp.AttrCatNote = "System create";
            
            SpecAttributeCatlang splang = new SpecAttributeCatlang();
            splang.DisplayName = localname;
            splang.StoreID = storeid;

        }

        /// <summary>
        /// Spec category
        /// </summary>
        /// <param name="spc"></param>
        /// <param name="splang"></param>
        public void save(SpecAttributeCat spc, SpecAttributeCatlang splang)
        {

            var _sp = (from sp in context.SpecAttributeCats
                       where sp.AttrCatName == spc.AttrCatName
                       select sp).FirstOrDefault();

            if (_sp == null)
            {
                context.SpecAttributeCats.AddObject(spc); context.SaveChanges();
                splang.CatID = spc.AttrCatID;
                context.SpecAttributeCatlangs.AddObject(splang);
                context.SaveChanges();

            }

        }

        /// <summary>
        /// svae spec attribute
        /// </summary>
        /// <param name="speca"></param>
        /// <param name="splang"></param>

        public void save(SpecAttribute speca, SpecAttributelang splang)
        {

            var _sp = (from sp in context.SpecAttributes
                       where sp.AttrName == speca.AttrName
                       select sp).FirstOrDefault();

            if (_sp == null)
            {
                context.SpecAttributes.AddObject(speca); context.SaveChanges();
                splang.AttrID = speca.AttrID;
                context.SpecAttributelangs.AddObject(splang);
                context.SaveChanges();

            }


        }

        /// <summary>
        /// Save Spec Values
        /// </summary>
        /// <param name="speca"></param>
        /// <param name="splang"></param>
        public void save(SpecAttributeValue speca, SpecAttributeValuelang splang)
        {

            var _sp = (from sp in context.SpecAttributeValues
                       where sp.AttrValueName == speca.AttrValueName
                       select sp).FirstOrDefault();

            if (_sp == null)
            {
                context.SpecAttributeValues.AddObject(speca); context.SaveChanges();
                splang.AttrValueID = speca.AttrValueID;
                context.SpecAttributeValuelangs.AddObject(splang);
                context.SaveChanges();
            }
            else {
                splang.AttrValueID = _sp.AttrValueID;

                var _sp2 = (from sp in context.SpecAttributeValuelangs
                            where sp.AttrValueID == splang.AttrValueID && sp.StoreID == splang.StoreID
                            select sp).FirstOrDefault();

                if (_sp2 == null)
                {
                    
                    context.SpecAttributeValuelangs.AddObject(splang);
                    context.SaveChanges();
                }

            }        

        }

        //ReCreate Rules details
        public void createRules(string storeid) {

            //Loop all ProductCategories 
            var _v = from pc in context.ProductCategories
                     where pc.Storeid == storeid
                     select pc;
            
            foreach (ProductCategory pc in _v.ToList()) {
                if (pc.RuleSetId.HasValue && pc.RuleSetId.Value > 0) { 
                    //delete all Rule
                    delRuleset(pc.RuleSetId.Value);
                }

                RuleSet rs = new RuleSet();
                rs.CreatedBy = "system";
                rs.CreatedDate = DateTime.Now;

                try
                {
                    context.RuleSets.AddObject(rs);
                    context.SaveChanges();

                }catch(Exception) {                    
                    
                }

                //Create rules for this category
                createRuleDetail(pc,rs);

                //Create a ruledetail for every parent category
                ProductCategory parent = pc.ParentCategory;

                while (parent != null)
                {
                    createRuleDetail(parent,rs);
                    parent = parent.ParentCategory;                  
                }

                pc.RuleSetId = rs.RuleSetID;
                context.ProductCategories.ApplyCurrentValues(pc);
                context.SaveChanges();

                //reset Menu rulesetID
                setMenuRuleset(rs, pc.CategoryPath);

            }

        }

        private void delRuleset(int rulesetid)
        {

            var rs = (from r in context.RuleSets
                     where r.RuleSetID == rulesetid
                     select r).FirstOrDefault();

            if (rs != null)
            {
                context.RuleSets.DeleteObject(rs);
                context.SaveChanges();
            }
        }


        private void createRuleDetail(ProductCategory pc, RuleSet rs)
        {
            //find the attributeID where valuename=pc.categorypath
            var rules = (from a in context.SpecAttributeValues
                         where a.AttrValueName == pc.CategoryPath || a.AttrValueName == pc.CategoryPath  
                         select a).FirstOrDefault();
            if (rules != null)
                save(rules, rs);
        
        }



        private bool save(SpecAttributeValue spv, RuleSet rs)
        {
            RuleSetDetail rsd = new RuleSetDetail();
            rsd.RuleSetId = rs.RuleSetID;
            rsd.AttributeCatId = 999999;
            rsd.AttributeId = spv.AttrID ;
            rsd.AttributeValueId = spv.AttrValueID;
            rs.CreatedDate = DateTime.Now;
            rs.CreatedBy = "System";
            try
            {
                context.RuleSetDetails.AddObject(rsd);
                context.SaveChanges();
                return true;
            }catch(Exception) {
                return false;
            
            }

        
        }

        private bool save(RuleSet rs)
        {            
            try
            {
                context.RuleSets.AddObject(rs);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }


        private void setMenuRuleset(RuleSet rs, string categorypath) {
            var _menu = (from m in context.Menus
                        where m.CategoryPath == categorypath
                        select m);

            foreach (Menu m in _menu.ToList()) {
                m.RuleSet = rs;
                context.Menus.ApplyCurrentValues(m);
                context.SaveChanges();
            }
        
        }



        
    }
}
