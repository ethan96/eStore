using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace eStore.UI.Models
{
    public class CategoryHelper
    {
        public List<CategoryAttrCate> margSpec(POCOS.ProductCategory category)
        {
            if (category.categoryTypeX == POCOS.ProductCategory.Category_Type.CTOS)
            {
                return margCtosSpec(category);
            }
            else
                return margProductSpec(category);
            
        }

        private List<CategoryAttrCate> margProductSpec(POCOS.ProductCategory category)
        {
            List<CategoryAttrCate> specs = new List<CategoryAttrCate>();

            category.resetProductList();
            var psr = Presentation.eStoreContext.Current.Store.getMatchProducts(category.CategoryPath, "");

            if (psr._specrules != null && psr._specrules.Count > 0)
            {
                specs = (from cat in psr._specrules
                         group cat by
                         new { cat.CatID, cat.LocalCatName }
                             into catgroup
                             select new CategoryAttrCate
                             {
                                 Id = catgroup.Key.CatID,
                                 Name = catgroup.Key.LocalCatName,
                                 Values = (from attr in catgroup
                                           select new CategoryAttr
                                           {
                                               Id = attr.AttrID,
                                               Name = attr.LocalAttributeName,
                                               ParentId = catgroup.Key.CatID
                                           }).Distinct(new EquAttr<CategoryAttr>()).ToList()
                             }).Take(7).ToList();
                System.Text.RegularExpressions.Regex rgObj = new System.Text.RegularExpressions.Regex(@"^[-+±]?[0-9]{1,3}(?:,?[0-9]{3})*(?:\.[0-9]{1,})?");
                foreach (var cat in specs)
                {
                    foreach (var attr in cat.Values)
                    {
                        var atributeValues = (from attrvalue in psr._specrules
                                              where attrvalue.CatID == cat.Id && attrvalue.AttrID == attr.Id && attrvalue.productcount > 0
                                              select new CategoryAttrValue
                                              {
                                                  Id = attrvalue.AttrValueID,
                                                  Name = System.Text.RegularExpressions.Regex.Replace(attrvalue.LocalValueName, "</?[a-zA-Z][a-zA-Z0-9]*[^<>]*/?>", " "),
                                                  Seq = (rgObj.IsMatch(attrvalue.LocalValueName) && esUtilities.StringUtility.IsNumeric(rgObj.Match(System.Text.RegularExpressions.Regex.Replace(attrvalue.LocalValueName, @"^[-+±\.]+", "")).Value)) ?
                                                      Convert.ToDouble(rgObj.Match(System.Text.RegularExpressions.Regex.Replace(attrvalue.LocalValueName, @"^[-+±\.]+", "")).Value) : -1
                                              }).ToList();
                        if (atributeValues.Where(p => p.Seq >= 0).Count() > atributeValues.Where(p => p.Seq == -1).Count())
                            attr.Values = atributeValues.OrderBy(p => p.Seq).ToList();
                        else
                            attr.Values = atributeValues.OrderBy(p => p.Name).ToList();
                        attr.Values.Insert(0, new CategoryAttrValue() { Id = -1, Name = "-", Seq = 0 });
                    }
                }
            }
            return specs;
        }

        private List<CategoryAttrCate> margCtosSpec(POCOS.ProductCategory category)
        {
            List<CategoryAttrCate> specs = new List<CategoryAttrCate>();
            var parts = category.productList;
            var categoryspec = eStore.Presentation.eStoreContext.Current.Store.getCTOSSpecMask(category.CategoryPath);
            if (parts.Any() == false)
                return new List<CategoryAttrCate>();

            var specificationAttributes = (from spec in parts[0].specs
                                          select new
                                          {
                                              CatID = spec.CatID,
                                              Category = spec.LocalCatName,
                                              ID = spec.AttrID,
                                              Name = spec.LocalAttributeName,
                                              Seq = spec.seq
                                          }).ToList();

            for (int i = 1; i < parts.Count; i++)
            {
                specificationAttributes = (specificationAttributes.Union(
                     from spec in parts[i].specs
                     orderby spec.seq, spec.LocalAttributeName
                     select new
                     {
                         CatID = spec.CatID,
                         Category = spec.LocalCatName,
                         ID = spec.AttrID,
                         Name = spec.LocalAttributeName,
                         Seq = spec.seq
                     })).ToList();
            }

            var specCategories = (from s in specificationAttributes
                                  group s by new { s.CatID, s.Category } into g
                                  select new
                                  {
                                      g.Key.CatID,
                                      g.Key.Category,
                                      Attributes = (from a in g
                                                    group a by new { a.ID } into ag
                                                    select new
                                                    {
                                                        ag.Key.ID,
                                                        Name = ag.Select(x => x.Name).FirstOrDefault(),
                                                        Seq = ag.Select(x => x.Seq).FirstOrDefault()
                                                    }).ToList()
                                  }).ToList();


            
            foreach (var spec in categoryspec)
            {
                CategoryAttrCate attrCate = new CategoryAttrCate { Id = spec.Attrid2.GetValueOrDefault(), Name = " ", Values = new List<CategoryAttr>() };
                CategoryAttr attr = new CategoryAttr { Id = spec.Attrid2.GetValueOrDefault(), Name = spec.Name, Values = new List<CategoryAttrValue>(), ParentId = attrCate.Id };
                // add attr values
                var sc = specCategories.FirstOrDefault(c => c.CatID == attr.Id);
                if (sc != null)
                {
                    foreach (var sa in sc.Attributes)
                    {
                        foreach (var p in parts)
                        {
                            var sp = p.specs.FirstOrDefault(x => x.CatID == sc.CatID && x.AttrID == sa.ID);
                            if (sp != null && attr.Values.FirstOrDefault(c => c.Id == sp.AttrValueID) == null)
                                attr.Values.Add(new CategoryAttrValue { Id = sp.AttrValueID, Name = sp.AttrValueName, Seq = (double)sp.seq });
                        }
                    }   
                }
                attr.Values.Insert(0, new CategoryAttrValue() { Id = -1, Name = "-", Seq = 0 });
                attrCate.Values.Add(attr);

                specs.Add(attrCate);
            }
            return specs;
        }
    }

    


    public class EquAttr<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            if (x == null || y == null)
                return false;
            if (x is CategoryAttr && y is CategoryAttr)
            {
                CategoryAttr xa = x as CategoryAttr;
                CategoryAttr ya = y as CategoryAttr;
                return (xa.Name == ya.Name && xa.Id == ya.Id);
            }
            return false;
        }

        public int GetHashCode(T obj)
        {
            if (obj == null)
                return 0;
            return 1;
        }
    }

    public class CategoryAttrCate
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<CategoryAttr> Values { get; set; }
    }

    public class CategoryAttr
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<CategoryAttrValue> Values { get; set; }
        public int SelectId { get; set; }
        public int ParentId { get; set; }
    }

    public class CategoryAttrValue
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public double Seq { get; set; }
    }

    public class ProductMatrix
    {
        public int CatID { get; set; }
        public int AttrId { get; set; }
        public int AttrValueId { get; set; }
        public string AttrValueName { get; set; }

        public ProductMatrix() { }
        public ProductMatrix(POCOS.VProductMatrix vma)
        {
            this.CatID = vma.CatID;
            this.AttrId = vma.AttrID;
            this.AttrValueId = vma.AttrValueID;
            this.AttrValueName = vma.AttrValueName;
        }
    }
}