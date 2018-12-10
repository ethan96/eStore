using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using eStore.BusinessModules.Task;

namespace eStore.Presentation.AJAX.Function
{


    class getPopularProduct : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {
            string type = context.Request["type"].ToLower();
            string keyword = context.Request["keyword"].ToUpper();

            JObject result = null;
            eStore.POCOS.Product popularProduct = null;
            eStore.POCOS.PopularModel popularModel = null;
            List<string> productModelArr = new List<string>();//原产品的model list(product:是一个. category可能是多个)
            if (!string.IsNullOrEmpty(keyword))
            {
                if (type == "product")
                {
                    POCOS.Product productItem = eStoreContext.Current.Store.getProduct(keyword, true);
                    if (productItem != null && !string.IsNullOrEmpty(productItem.ModelNo))
                        productModelArr.Add(productItem.ModelNo);
                }
                else
                    productModelArr = keyword.Split(';').Distinct().ToList();

                if (productModelArr.Count > 0)
                {
                    //根据ModelNo 获取对应的 Popular Model
                    List<eStore.POCOS.PopularModel> popularModelList = eStoreContext.Current.Store.getPopulareProductByModel(productModelArr);
                    List<string> modelList = popularModelList.Select(p => p.PopularProductModel).Distinct().ToList();
                    if (modelList.Count > 0)
                    {
                        //根据Model 返回 对应的随机产品
                        popularProduct = eStoreContext.Current.Store.getProductByModelNo(modelList);

                        //如果取到 Popular Product, 去把PopularModel 拿出来。
                        if (popularProduct != null)
                            popularModel = popularModelList.FirstOrDefault(p => p.PopularProductModel.ToUpper() == popularProduct.ModelNo.ToUpper());
                    }
                }
            }

            if (popularProduct != null && popularModel != null)
            {
                //生成 推送log
                POCOS.PopularModelLog modelLog = new POCOS.PopularModelLog();
                modelLog.StoreID = eStoreContext.Current.Store.profile.StoreID;
                modelLog.SessionID = context.Session.SessionID;
                if (eStoreContext.Current.User != null)
                    modelLog.UserId = eStoreContext.Current.User.UserID;
                modelLog.PopularModelId = popularModel.Id;//popular model 
                modelLog.SourceProduct = context.Request["sourceKey"];
                modelLog.PopulareProduct = popularProduct.SProductID;//推送的产品
                modelLog.Impression = 1;
                modelLog.Click = 0;
                modelLog.CreatedDate = DateTime.Now;

                PopularProductReporter eventReporter = null;
                eventReporter = new PopularProductReporter(modelLog, PopularEventType.Impression);
                if (eventReporter != null)
                    BusinessModules.Task.EventManager.getInstance(eStoreContext.Current.Store.profile.StoreID).QueueCommonTask(eventReporter);

                string priceMessage = Presentation.Product.ProductPrice.FormartPriceWithDecimal(popularProduct.getListingPrice().value, eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign);

                result = new JObject 
                            {
                                new JProperty("SproductId", popularProduct.SProductID),
                                new JProperty("TumbnailImage", popularProduct.thumbnailImageX),
                                //js使用productDescX <sup>会直接显示在页面上
                                new JProperty("Description", string.IsNullOrEmpty(popularProduct.ProductDesc) ? popularProduct.VendorProductDesc : popularProduct.ProductDesc),
                                new JProperty("Price", priceMessage),
                                new JProperty("Link", eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(popularProduct).Replace("~", "") + "?SourceLog=" + modelLog.SourceProduct)
                            };
            }

            if (result == null)
                return "";
            else
                return JsonConvert.SerializeObject(result);
        }
    }
}
