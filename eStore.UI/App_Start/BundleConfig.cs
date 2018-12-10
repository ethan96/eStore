using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace eStore.UI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            ScriptBundle js = new ScriptBundle("~/Scripts/V4/eStoreScripts");
            js.Include("~/Scripts/V4/jquery-1.11.2.min.js")
            .Include("~/Scripts/V4/jquery-ui.min.js")
            //.Include("~/Scripts/V4/jquery-migrate-1.2.1.js")
            .Include("~/Scripts/V4/json3.min.js")
            .Include("~/Scripts/V4/knockout-3.3.0.js")
            .Include("~/Scripts/V4/knockout.mapping.js")
            .Include("~/Scripts/V4/underscore.js")
            .Include("~/Scripts/V4/jquery.easing.1.3.js")
            .Include("~/Scripts/V4/custom-form-elements.js")
            .Include("~/Scripts/V4/device.min.js")
            .Include("~/Scripts/V4/jquery.cookie.js")
            .Include("~/Scripts/V4/ContactPanel.js")
             .Include("~/Scripts/V4/jquery.tabSlideOut.v1.3.js")
            .Include("~/Scripts/V4/jquery.fancybox.js")                      
            .Include("~/Scripts/V4/jquery.carouFredSel-6.1.0-packed.js")
            .Include("~/Scripts/V4/jquery.cycle2.min.js")
            .Include("~/Scripts/jquery.jtip.js")
            .Include("~/Scripts/V4/default.js")
            .Include("~/Scripts/storeutilities.js");
            BundleTable.Bundles.Add(js);

            StyleBundle css = new StyleBundle("~/App_Themes/V4/eStoreCSS");
            css.Include("~/App_Themes/V4/reset.css")
            .Include("~/App_Themes/V4/jquery-ui.min.css")
            .Include("~/App_Themes/V4/template.css")
            .Include("~/App_Themes/V4/base.css")
            .Include("~/App_Themes/V4/jquery.fancybox.css")
            .Include("~/App_Themes/V4/jquery.cycle2.min.css")
            .Include("~/App_Themes/V4/contact-panel.css");

            BundleTable.Bundles.Add(css);

            ScriptBundle homejs = new ScriptBundle("~/Scripts/V4/home");
            homejs.Include("~/Scripts/V4/byIndex.js");
            BundleTable.Bundles.Add(homejs);

            StyleBundle homecss = new StyleBundle("~/App_Themes/V4/home");
            homecss.Include("~/App_Themes/V4/byIndex.css");
            BundleTable.Bundles.Add(homecss);

            ScriptBundle homejs2016 = new ScriptBundle("~/Scripts/V4/home2016");
            homejs2016.Include("~/Scripts/V4/byIndex2016.js").Include("~/Scripts/V4/jquery.loadmask.js");
            BundleTable.Bundles.Add(homejs2016);

            StyleBundle homecss2016 = new StyleBundle("~/App_Themes/V4/home2016");
            homecss2016.Include("~/App_Themes/V4/byIndex2016.css");
            BundleTable.Bundles.Add(homecss2016);

            ScriptBundle homejs2018 = new ScriptBundle("~/Scripts/V4/home2018");
            homejs2018.Include("~/Scripts/V4/byIndex2018.js");
            BundleTable.Bundles.Add(homejs2018);

            StyleBundle homecss2018 = new StyleBundle("~/App_Themes/V4/home2018");
            homecss2018.Include("~/App_Themes/V4/byIndex.css")
                    .Include("~/App_Themes/V4/font-awesome/css/font-awesome.min.css")
                    .Include("~/App_Themes/V4/byIndex2018.css");
            BundleTable.Bundles.Add(homecss2018);

            StyleBundle homecss2018V2 = new StyleBundle("~/App_Themes/V4/home2018V2");
            homecss2018V2.Include("~/App_Themes/V4/byIndex2016.css")
                    .Include("~/App_Themes/V4/byIndex2018V2.css");
            BundleTable.Bundles.Add(homecss2018V2);

            ScriptBundle categoryjs = new ScriptBundle("~/Scripts/V4/category");
            categoryjs.Include("~/Scripts/V4/knockout.simpleGrid.3.0.js")
                .Include("~/Scripts/V4/byCategory.js")
                .Include("~/Scripts/V4/jquery.loadmask.js");
            BundleTable.Bundles.Add(categoryjs);

            ScriptBundle categoryV4Sjs = new ScriptBundle("~/Scripts/V4/categoryV4S");
            categoryV4Sjs.Include("~/Scripts/V4/byCategoryV4S.js")
                .Include("~/Scripts/V4/knockout.simpleGrid.3.0.js")
                .Include("~/Scripts/V4/jquery.loadmask.js")
                .Include("~/Scripts/V4/jquery.lazy.min.js");
            BundleTable.Bundles.Add(categoryV4Sjs);

            StyleBundle categorycss = new StyleBundle("~/App_Themes/V4/category");
            categorycss.Include("~/App_Themes/V4/byCategory.css");
            BundleTable.Bundles.Add(categorycss);

            ScriptBundle accountjs = new ScriptBundle("~/Scripts/V4/account");
            accountjs.Include("~/Scripts/V4/byAccount.js");
            BundleTable.Bundles.Add(accountjs);

            StyleBundle accountcss = new StyleBundle("~/App_Themes/V4/account");
            accountcss
                .Include("~/App_Themes/V4/byAccount.css");
            BundleTable.Bundles.Add(accountcss);

            StyleBundle productcss = new StyleBundle("~/App_Themes/V4/product");
            productcss.Include("~/App_Themes/V4/byProduct.css");
            BundleTable.Bundles.Add(productcss);

            ScriptBundle productjs = new ScriptBundle("~/Scripts/V4/product");
            productjs.Include("~/Scripts/V4/byProduct.js")
                        .Include("~/Scripts/V4/byZoom.js")
                        .Include("~/Scripts/jquery.jtip.js")
                        .Include("~/Scripts/productaddons.js");
            BundleTable.Bundles.Add(productjs);

            StyleBundle systemcss = new StyleBundle("~/App_Themes/V4/system");
            systemcss.Include("~/App_Themes/V4/bySystem.css");
            BundleTable.Bundles.Add(systemcss);

            ScriptBundle systemjs = new ScriptBundle("~/Scripts/V4/system");
            systemjs.Include("~/Scripts/V4/bySystem.js")
                        .Include("~/Scripts/V4/byZoom.js");
            BundleTable.Bundles.Add(systemjs);


            StyleBundle comparecss = new StyleBundle("~/App_Themes/V4/compare");
            comparecss.Include("~/App_Themes/V4/byCompare.css") ;
            BundleTable.Bundles.Add(comparecss);

            ScriptBundle comparejs = new ScriptBundle("~/Scripts/V4/compare");
            comparejs
                        .Include("~/Scripts/V4/byCompare.js");
            BundleTable.Bundles.Add(comparejs);
     
            StyleBundle ordercss = new StyleBundle("~/App_Themes/V4/order");
            ordercss.Include("~/App_Themes/V4/byOrder.css");
            BundleTable.Bundles.Add(ordercss);

            ScriptBundle orderjs = new ScriptBundle("~/Scripts/V4/order");
            orderjs.Include("~/Scripts/V4/byOrder.js");
            BundleTable.Bundles.Add(orderjs);

            StyleBundle articlecss = new StyleBundle("~/App_Themes/V4/article");
            articlecss.Include("~/App_Themes/V4/bytxtPage.css");
            BundleTable.Bundles.Add(articlecss);

            StyleBundle contactUsCss = new StyleBundle("~/App_Themes/V4/ContactUSCSS");
            contactUsCss.Include("~/App_Themes/V4/bytxtPage.css") ;
            BundleTable.Bundles.Add(contactUsCss);

            StyleBundle fontAwesome = new StyleBundle("~/App_Themes/V4/font-awesome");
            fontAwesome.Include("~/App_Themes/V4/font-awesome/css/font-awesome.min.css");
            BundleTable.Bundles.Add(fontAwesome);

            StyleBundle subscribeUsCss = new StyleBundle("~/App_Themes/V4/subscribeUsCss");
            subscribeUsCss.Include("~/App_Themes/V4/subscribeUs.css");
            BundleTable.Bundles.Add(subscribeUsCss);
            

            ScriptBundle contactUsJs = new ScriptBundle("~/Scripts/V4/ContactUSJS");
            contactUsJs.Include("~/Scripts/V4/byContactUs.js")
                .Include("~/Scripts/V4/bytxtPage.js");
            BundleTable.Bundles.Add(contactUsJs);

            StyleBundle orderbyPNcss = new StyleBundle("~/App_Themes/V4/orderbyPNcss");
            orderbyPNcss.Include("~/App_Themes/V4/default.css")
                .Include("~/App_Themes/V4/byOrder.css") ;
            BundleTable.Bundles.Add(orderbyPNcss);

            ScriptBundle policyJs = new ScriptBundle("~/Scripts/V4/policyJs");
            policyJs.Include("~/Scripts/V4/byPolicy.js");
            BundleTable.Bundles.Add(policyJs);


            ScriptBundle searchJs = new ScriptBundle("~/Scripts/V4/search");
            searchJs.Include("~/Scripts/V4/bySearch.js")
                .Include("~/Scripts/V4/jquery.loadmask.js")
                .Include("~/Scripts/V4/jquery.lazy.min.js");
            BundleTable.Bundles.Add(searchJs);
            StyleBundle searchCss = new StyleBundle("~/App_Themes/V4/search");
            searchCss.Include("~/App_Themes/V4/font-awesome/css/font-awesome.min.css")
                    .Include("~/App_Themes/V4/bySearch.css");
            BundleTable.Bundles.Add(searchCss);

            StyleBundle allProductcss = new StyleBundle("~/App_Themes/V4/allProductcss");
            allProductcss.Include("~/App_Themes/V4/byallList.css");
            BundleTable.Bundles.Add(allProductcss);

            ScriptBundle categoryjsPAPS = new ScriptBundle("~/Scripts/V4/categoryjsPAPS");
            categoryjsPAPS.Include("~/Scripts/V4/byCategoryNew.js");
            BundleTable.Bundles.Add(categoryjsPAPS);

            StyleBundle categorycssPAPS = new StyleBundle("~/App_Themes/V4/categorycssPAPS");
            categorycssPAPS.Include("~/App_Themes/V4/common.css");
            BundleTable.Bundles.Add(categorycssPAPS);

            ScriptBundle peculiarCategoryJs = new ScriptBundle("~/Scripts/V4/peculiarCategoryJs");
            peculiarCategoryJs.Include("~/Scripts/jquery.simplePagination.js");
            BundleTable.Bundles.Add(peculiarCategoryJs);

            StyleBundle peculiarCategoryCss = new StyleBundle("~/App_Themes/V4/peculiarCategoryCss");
            peculiarCategoryCss.Include("~/Styles/custom-Marianna.css");
            peculiarCategoryCss.Include("~/Styles/simplePagination.css");
            BundleTable.Bundles.Add(peculiarCategoryCss);

            StyleBundle KitThemecss = new StyleBundle("~/App_Themes/V4/KitTheme");
            KitThemecss.Include("~/App_Themes/V4/byKitTheme.css");
            BundleTable.Bundles.Add(KitThemecss);

            ScriptBundle KitThemeJs = new ScriptBundle("~/Scripts/V4/KitThemeJs");
            KitThemeJs.Include("~/Scripts/V4/byKitTheme.js");
            BundleTable.Bundles.Add(KitThemeJs);
            
            ScriptBundle ScoTooltipJs = new ScriptBundle("~/Scripts/scojs");
            ScoTooltipJs.Include("~/Scripts/sco.tooltip.js");
            BundleTable.Bundles.Add(ScoTooltipJs);


            StyleBundle MenuCssV1 = new StyleBundle("~/App_Themes/V4/byMenuV1");
            MenuCssV1.Include("~/App_Themes/V4/byMenuV1.css");
            BundleTable.Bundles.Add(MenuCssV1);

            ScriptBundle MenuJsV1 = new ScriptBundle("~/Scripts/V4/eStoreMenuV1");
            MenuJsV1.Include("~/Scripts/V4/eStoreMenuV1.js");
            BundleTable.Bundles.Add(MenuJsV1);

            StyleBundle MenuCssV2 = new StyleBundle("~/App_Themes/V4/byMenuV2");
            MenuCssV2.Include("~/App_Themes/V4/font-awesome/css/font-awesome.min.css")
                    .Include("~/App_Themes/V4/byMenuV2.css");
            BundleTable.Bundles.Add(MenuCssV2);

            ScriptBundle MenuJsV2 = new ScriptBundle("~/Scripts/V4/eStoreMenuV2");
            MenuJsV2.Include("~/Scripts/V4/eStoreMenuV2.js");
            BundleTable.Bundles.Add(MenuJsV2);


        }
    }
}