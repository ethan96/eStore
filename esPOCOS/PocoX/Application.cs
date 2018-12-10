using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class Application : ProductCategory
    {


        public Size BackgroundImageSize
        {
            get
            {
                return new Size(this.ImageWidth.GetValueOrDefault(), this.ImageHeight.GetValueOrDefault());
            }
            set
            {
                this.ImageWidth = value.Width;
                this.ImageHeight = value.Height;
            }
        }
        public class Size
        {
            public Size() { }
            public Size(int width, int height)
            {
                this.Width = width;
                this.Height = height;
            }

            public int Width { get; set; }
            public int Height { get; set; }
        }
        private List<ScenarioCategory> _scenarioCategoriesX;
        public List<ScenarioCategory> scenarioCategoriesX
        {
            get
            {
                if (_scenarioCategoriesX == null)
                {
                    if (this.childCategoriesX.Count == 0)
                    {
                        _scenarioCategoriesX = this.ScenarioCategories.ToList();
                    }
                    else
                    {
                        _scenarioCategoriesX = new List<ScenarioCategory>();
                        _scenarioCategoriesX.AddRange(this.ScenarioCategories.ToList());
                        foreach (var item in this.childCategoriesX)
                        {
                            if (item is Application)
                            {
                                var app = item as Application;
                                if (app.scenarioCategoriesX != null && app.scenarioCategoriesX.Any())
                                {
                                    foreach (ScenarioCategory sc in app.scenarioCategoriesX)
                                    {

                                        if (_scenarioCategoriesX.Any(x => x.CategoryID == sc.CategoryID) == false)
                                        {
                                            _scenarioCategoriesX.Add(sc);

                                        }
                                        else
                                        {
                                            var category = _scenarioCategoriesX.First(x => x.CategoryID == sc.CategoryID);
                                            foreach (POCOS.Product p in sc.productList)
                                            {
                                                if (category.productList.Any(x => x.SProductID == p.SProductID) == false)
                                                {
                                                    category.productList.Add(p);
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                return _scenarioCategoriesX;
            }
        }
        public override List<SimpleProduct> simpleProductList
        {
            get
            {
                if (_simpleProductList == null)
                {
                    var items = new List<SimpleProduct>();


                    if (this.childCategoriesX.Count > 0)
                    {
                        foreach (ProductCategory pc in this.childCategoriesX)
                        {
                            foreach (var item in pc.simpleProductList)
                            {
                                if (items.FirstOrDefault(c => c.SProductID.Equals(item.SProductID, StringComparison.OrdinalIgnoreCase)) == null)
                                    items.Add(item);
                            }
                        }
                    }
                    else
                    {
                        foreach (var sc in scenarioCategoriesX)
                        {
                           var  scitems = helper.getSimpleProducts(sc.ProductCategory);
                           foreach (var item in scitems)
                           {
                               if (items.FirstOrDefault(c => c.SProductID.Equals(item.SProductID, StringComparison.OrdinalIgnoreCase)) == null)
                                   items.Add(item);
                           }
                        }
                     
                    }
                  

                    _simpleProductList = items;
                }
                return _simpleProductList; 
            }
            set
            {
                base.simpleProductList = value;
            }
        }
    
    }
}
