using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace eStore.UI.APIControllers
{
    public class HomeController : ApiController
    {
        [HttpGet]

        public IEnumerable<Models.Menu> Menus()
        {
            List<Models.Menu> menus = new List<Models.Menu>();
            menus = eStore.Presentation.eStoreContext.Current.Store.getMenuItems(eStore.Presentation.eStoreContext.Current.MiniSite).Select(x => new Models.Menu(x)).ToList();
            return menus;
        }
        [HttpGet]
        public IEnumerable<Models.Advertisement> Banners(bool fullsize=true)
        {

            List<Models.Advertisement> advertisements = new List<Models.Advertisement>();
            advertisements = Presentation.eStoreContext.Current.Store.getHomeBanners(Presentation.eStoreContext.Current.MiniSite, fullsize).Select(x => new Models.Advertisement(x)).ToList();

            if (Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("hasRegionBanner") == true && Request.Properties.ContainsKey("MS_HttpContext"))
            {
                var ctx = Request.Properties["MS_HttpContext"] as System.Web.HttpContextWrapper;
                if (ctx != null)
                {
                    string country = esUtilities.CommonHelper.IPtoNation(eStore.Presentation.eStoreContext.Current.getUserIP());
                    string region = string.Empty;
                    switch (country)
                    {
                        case "MY":
                            region = "AMY";
                            advertisements = advertisements.Where(a => a.Map == null || a.Map == string.Empty || a.Map == region).ToList();
                            break;
                        case "ID":
                            region = "AID";
                            advertisements = advertisements.Where(a => a.Map == null || a.Map == string.Empty || a.Map == region).ToList();
                            break;
                        case "SG":
                            region = "ASG";
                            advertisements = advertisements.Where(a => a.Map == null || a.Map == string.Empty || a.Map == region || a.Map == "ROA").ToList();
                            break;
                        default:
                            region = "XX";
                            advertisements = advertisements.Where(a => a.Map == null || a.Map == string.Empty || a.Map == region || a.Map == "ROA").ToList();
                            break;
                    }
                    //if(region != "XX")
                    //    advertisements = advertisements.Where(a => a.Map == region).ToList();
                }
            }
            return advertisements;
        }

        [HttpGet]
        public Models.VContextTodaysHighlight TodaysHighlights()
        {

            IEnumerable<Models.TodaysHighlight> todaysHighlights = null;

            List<POCOS.ProductCategory> pcs = new List<POCOS.ProductCategory>();
            List<POCOS.Menu> menus = Presentation.eStoreContext.Current.Store.getMenus(Presentation.eStoreContext.Current.Store.storeID, POCOS.Menu.MenuPosition.TodayHighLight).OrderBy(m => m.Sequence).ToList();
            foreach (var menu in menus)
            {
                pcs.Add(menu.productCategory);
            }
            if (pcs != null)
            {
                if (pcs.Any())
                {
                    todaysHighlights = pcs.Where(p => p != null).Select(p => new Models.TodaysHighlight(p)).Take(9).ToList();
                }
                //else
                //{
                //    todaysHighlights = (from product in pc.productList
                //                        select new Models.TodaysHighlight(product)).Take(6).ToList();
                //}

            }
            else
            { }
            return new Models.VContextTodaysHighlight() { TodaysHighlights = todaysHighlights };
        }


        [HttpGet]
        public string[] TodaysHighlightsV2()
        {

           string[]todaysHighlights = null;

            List<POCOS.ProductCategory> pcs = new List<POCOS.ProductCategory>();
            List<POCOS.Menu> menus = Presentation.eStoreContext.Current.Store.getMenus(Presentation.eStoreContext.Current.Store.storeID, POCOS.Menu.MenuPosition.TodayHighLight).OrderBy(m => m.Sequence).ToList();
            foreach (var menu in menus)
            {
                pcs.Add(menu.productCategory);
            }
            if (pcs != null)
            {
                if (pcs.Any())
                {
                    todaysHighlights = pcs.Where(p => p != null).Select(p =>p.CategoryPath).ToArray();
                }
                //else
                //{
                //    todaysHighlights = (from product in pc.productList
                //                        select new Models.TodaysHighlight(product)).Take(6).ToList();
                //}

            }
            else
            { }
            return todaysHighlights;
        }


        [HttpGet]
        public Models.VContextTodaysHighlight TodaysHighlights(string id)
        {
            Models.VContextTodaysHighlight vcontext = new Models.VContextTodaysHighlight();
            IEnumerable<Models.TodaysHighlight> todaysHighlights = null;
            POCOS.ProductCategory pc = Presentation.eStoreContext.Current.Store.getProductCategory(id);
            if (pc != null)
            {
                if (pc.childCategoriesX.Any())
                {
                    todaysHighlights = pc.childCategoriesX.Select(s => new  Models.TodaysHighlight(s, s.Sequence.GetValueOrDefault())).ToList();
                }
                else
                {
                    todaysHighlights = (from product in pc.productList
                                        orderby
                                        (product.ShowPrice ? 0 : 1)
                                        , (product.StorePrice > 0 ? 0 : 1)
                                        , product.CategorySeq, product.StorePrice
                                        select new Models.TodaysHighlight(product)).Take(9).ToList();
                    if (pc.productList.Count > 9) {
                        vcontext.BtnMore = eStore.Presentation.eStoreLocalization.Tanslation("eStore_More");
                        vcontext.Url = esUtilities.CommonHelper.ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(pc));
                    }
                }

            }
            else
            { }
            vcontext.TodaysHighlights = todaysHighlights;

            return vcontext;
        }


        [HttpGet]
        public IEnumerable<Models.Solution> Solutions()
        {

            List<Models.Solution> solutions = new List<Models.Solution>();
            solutions = Presentation.eStoreContext.Current.Store.getAllSolution().Where(s => s.PublishStatus == true).Select(s => new Models.Solution(s)).ToList();
            return solutions;
        }
    }
}