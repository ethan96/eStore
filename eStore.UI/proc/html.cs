using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using eStore.Presentation;

namespace eStore.UI.proc
{
    public class html : IHttpHandler, IReadOnlySessionState
    {
        public bool IsReusable
        {
            get { return true; }
        }
        
        //Replace empty to +
        private string ReplaceEmptyToPlus(string id)
        {
            if (!string.IsNullOrEmpty(id))
                return id.Replace(" ", "+");
            else
                return "";
        }

        public void ProcessRequest(HttpContext context)
        {
            string outhtml = string.Empty;
            if (context.Request["type"] != null)
            {
                switch (context.Request["type"])
                {

                    case "Product3DModel":
                        {
                            string ProductId = context.Request["ProductID"];
                            ProductId = ReplaceEmptyToPlus(ProductId);
                            POCOS.Part part = Presentation.eStoreContext.Current.Store.getPart(ProductId); if (part != null)
                            {
                                ViewManager<Modules.Product3DModel> viewProduct3DModelManager = new ViewManager<Modules.Product3DModel>();
                                Modules.Product3DModel ucProduct3DModel = viewProduct3DModelManager.LoadViewControl("~/Modules/Product3DModel.ascx");
                                ucProduct3DModel.part = part;
                                outhtml = viewProduct3DModelManager.RenderView(ucProduct3DModel);
                            }
                            break;
                        }
                    case "ProductSpecList":
                        {
                            string ProductId = context.Request["ProductID"];
                            ProductId = ReplaceEmptyToPlus(ProductId);
                            POCOS.Part part = Presentation.eStoreContext.Current.Store.getPart(ProductId);
                            if (part != null)
                            {
                                ViewManager<Modules.ProductSpecList> viewProductSpecListManager = new ViewManager<Modules.ProductSpecList>();
                                Modules.ProductSpecList ucProductSpecList = viewProductSpecListManager.LoadViewControl("~/Modules/ProductSpecList.ascx");
                                ucProductSpecList.vProductMatrixs = part.specs;
                                //if (part is POCOS.Product && ((POCOS.Product)part).isEPAPS())
                                //    ucProductSpecList.ShowAttribute = false;
                                outhtml = viewProductSpecListManager.RenderView(ucProductSpecList);
                            }
                            break;
                        }
                    case "ProductDetailTip":
                        {
                            string ProductId = context.Request["ProductID"];
                            ProductId = ReplaceEmptyToPlus(ProductId);
                            bool showimage = true;
                            if (!string.IsNullOrEmpty(context.Request["showimage"]))
                                showimage = false;
                            POCOS.Part part = Presentation.eStoreContext.Current.Store.getPart(ProductId);
                            if (part == null)
                                part = Presentation.eStoreContext.Current.Store.getVendorPartForOrderbyPartNo(ProductId);
                            if (part == null)
                                part = Presentation.eStoreContext.Current.Store.getPartbyDisplayName(ProductId);
                            if (part != null)
                            {
                                ViewManager<Modules.ProductDetailTip> viewProductDetailTipManager = new ViewManager<Modules.ProductDetailTip>();
                                Modules.ProductDetailTip ucProductDetailTip = viewProductDetailTipManager.LoadViewControl("~/Modules/ProductDetailTip.ascx");
                                ucProductDetailTip.part = part;
                                ucProductDetailTip.ShowImage = showimage;
                                outhtml = viewProductDetailTipManager.RenderView(ucProductDetailTip);
                            }
                            break;
                        }
                    case "QuantityDiscountRequest":
                        {
                            string ProductId = context.Request["ProductID"];
                            ProductId = ReplaceEmptyToPlus(ProductId);
                            POCOS.Part part = Presentation.eStoreContext.Current.Store.getPart(ProductId);

                            if (part != null && part is POCOS.Product )
                            {
                                ViewManager<Modules.QuantityDiscountRequest> viewQuantityDiscountRequestManager = new ViewManager<Modules.QuantityDiscountRequest>();
                                Modules.QuantityDiscountRequest ucQuantityDiscountRequest = viewQuantityDiscountRequestManager.LoadViewControl("~/Modules/QuantityDiscountRequest.ascx");
                                ucQuantityDiscountRequest._product =( POCOS.Product ) part;
                                ucQuantityDiscountRequest.NeedLogin = false;
                                outhtml = viewQuantityDiscountRequestManager.RenderView(ucQuantityDiscountRequest);
                            }
                            break;
                        }
                    case "SAPContatDetailTip":
                        {
                            string companyid = context.Request["companyid"];
                            companyid = ReplaceEmptyToPlus(companyid);
                            POCOS.VSAPCompany sapcomany = Presentation.eStoreSAPCompanies.SAPCompanies.getVSAPCompany(companyid);
                            if (sapcomany != null)
                            {
                                ViewManager<Modules.SAPContatDetailTip> viewProductDetailTipManager = new ViewManager<Modules.SAPContatDetailTip>();
                                Modules.SAPContatDetailTip ucSAPContatDetailTip = viewProductDetailTipManager.LoadViewControl("~/Modules/SAPContatDetailTip.ascx");
                                ucSAPContatDetailTip.vSAPCompany = sapcomany;
                                outhtml = viewProductDetailTipManager.RenderView(ucSAPContatDetailTip);
                            }
                            break;
                        }

                    case "SuggestingProducts":
                        {
                            string ProductId = context.Request["ProductID"];
                            ProductId = ReplaceEmptyToPlus(ProductId);
                            POCOS.Part part = Presentation.eStoreContext.Current.Store.getPart(ProductId); 
                            if (part != null&& part is POCOS.Product)
                            {
                                ViewManager<Modules.SuggestingProducts> viewSuggestingProductsManager = new ViewManager<Modules.SuggestingProducts>();
                                Modules.SuggestingProducts ucSuggestingProducts = viewSuggestingProductsManager.LoadViewControl("~/Modules/SuggestingProducts.ascx");
                                ucSuggestingProducts.SuggestingProductsList = part .SuggestingProductsDictionary;
                                outhtml = viewSuggestingProductsManager.RenderView(ucSuggestingProducts);
                            }
                            break;

                        }
                    case "eStoreHelper":
                        {
                            string helperID = context.Request["helperID"];
                            outhtml = Presentation.eStoreContext.Current.Store.Tanslation(string.Format("eStoreHelper_{0}", helperID));
                            break;

                        }
                    case "ChannelParter":
                        if (Presentation.eStoreContext.Current.Order != null && Presentation.eStoreContext.Current.getBooleanSetting("Support_Channel_Partner"))
                        {
                            int channelID = -1;
                            if (!int.TryParse(context.Request["channelIDStr"], out channelID))
                                channelID = -1;

                            string channelName = context.Request["channelNameStr"];
                            if (channelID != -1 && !string.IsNullOrEmpty(channelName))
                            {
                                Presentation.eStoreContext.Current.Order.ChannelID = channelID;
                                Presentation.eStoreContext.Current.Order.ChannelName = channelName;
                                outhtml = "success";
                            }
                        }
                        break;
                    case "ScenarioProducts":
                        string ScenarioID = context.Request["ScenarioID"];
                        string Categoryid = context.Request["CategoryID"];
                        POCOS.ProductCategory scenario = Presentation.eStoreContext.Current.Store.getProductCategory(ScenarioID);

                        if (scenario != null && scenario is POCOS.Application)
                        {
                            POCOS.ScenarioCategory sc = (scenario as POCOS.Application).scenarioCategoriesX.FirstOrDefault(x => x.ProductCategory.CategoryPath == Categoryid);
                           
                            if (sc != null && sc.productList.Any())
                            {
                                ViewManager<Modules.ProductCompare> viewProductCompare = new ViewManager<Modules.ProductCompare>();
                                Modules.ProductCompare ucProductCompare = viewProductCompare.LoadViewControl("~/Modules/ProductCompare.ascx");
                                ucProductCompare.showHeaderButtons = true;
                                ucProductCompare.useHtmlControlForAdd2Cart  = true;
                                ucProductCompare.showPrintButton = false;
                                ucProductCompare.showRemoveButton = false;
                                List<eStore.POCOS.CTOSSpecMask> mask = new List<eStore.POCOS.CTOSSpecMask>();

                                POCOS.ProductSpecRules psr = new POCOS.ProductSpecRules();
                                psr = Presentation.eStoreContext.Current.Store.getMatchProducts(Categoryid, string.Empty);
                                if (psr._specrules != null)
                                {
                                    foreach (POCOS.VProductMatrix s in psr._specrules)
                                    {
                                        if (mask.Any(x => x.AttrID == s.AttrID.ToString()) == false)
                                        {
                                            mask.Add(new eStore.POCOS.CTOSSpecMask
                                            {
                                                AttrID = s.AttrID.ToString()
                                                ,
                                                Attrid2 = s.AttrID
                                                ,
                                                CategoryPath = Categoryid
                                                ,
                                                Name = s.AttrName
                                                ,
                                                Sequence = s.seq
                                             ,
                                                CatID = s.CatID

                                            });
                                        }
                                    }
                                }

                                ucProductCompare.CTOSSpecMask = mask;
                                ucProductCompare.compareProducts = sc.productList;

                                outhtml = viewProductCompare.RenderView(ucProductCompare);
                            }
                        }
                        break;
                    default:

                        break;
                }
            }
            context.Response.Write(outhtml);

        }
    }
}
