using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eStore.Presentation.Product
{
    public class ProductResource
    {
        public static string getJsonResourceSetting(POCOS.Part part, bool isSelectConsumingQty = true)
        {
            if (part.ProductLimitedResources != null && part.ProductLimitedResources.Count > 0)
            {
                var resourceSetting = from plr in part.ProductLimitedResources
                                      select new JObject {
                                                       
                                                        new JProperty("ResourceName",plr.LimitedResource.Name),
                                                        new JProperty("AvaiableQty",plr.AvaiableQty.HasValue ? plr.AvaiableQty.GetValueOrDefault() : 0),
                                                        new JProperty("ConsumingQty",plr.ConsumingQty.HasValue && isSelectConsumingQty ? plr.ConsumingQty.GetValueOrDefault() : 0) };
                return JsonConvert.SerializeObject(resourceSetting);
            }
            else
                return string.Empty;
        }

        public static string getJsonResourceSetting(List<POCOS.Part> partList, bool isSelectConsumingQty = true)
        {
            List<POCOS.ProductLimitedResource> limitedResourceList = new List<POCOS.ProductLimitedResource>();
            foreach (var part in partList)
            {
                if (part.ProductLimitedResources != null && part.ProductLimitedResources.Count > 0)
                    limitedResourceList.AddRange(part.ProductLimitedResources);
            }
            if (limitedResourceList.Count > 0)
            {
                //合并相同的name qty相加
                var resurceList = from l in limitedResourceList
                         group l by l.LimitedResource.Name
                             into g
                             select new
                             {
                                 ResourceName = g.Key,
                                 AvaiableQty = g.Sum(p => p.AvaiableQty.HasValue ? p.AvaiableQty.GetValueOrDefault() : 0),
                                 ConsumingQty = g.Sum(p => p.ConsumingQty.HasValue && isSelectConsumingQty ? p.ConsumingQty.GetValueOrDefault() : 0)
                             };
                var resourceSetting = from plr in resurceList
                                      select new JObject {
                                                        new JProperty("ResourceName",plr.ResourceName),
                                                        new JProperty("AvaiableQty",plr.AvaiableQty),
                                                        new JProperty("ConsumingQty",plr.ConsumingQty) };
                return JsonConvert.SerializeObject(resourceSetting);
            }
            else
                return string.Empty;
        }

        public static string getJsonResourceSetting(POCOS.CTOSBOM option)
        {
            if (option.type==POCOS.CTOSBOM.COMPONENTTYPE.OPTION&&option.limitedResources != null && option.limitedResources.Count > 0)
            {
                var resourceSetting = from plr in option.limitedResources 
                                      select new JObject { 
                                                        new JProperty("ResourceName",plr.LimitedResource.Name),
                                                        new JProperty("AvaiableQty",plr.AvaiableQty.HasValue ? plr.AvaiableQty.GetValueOrDefault() : 0),
                                                        new JProperty("ConsumingQty",plr.ConsumingQty.HasValue ? plr.ConsumingQty.GetValueOrDefault() : 0)};
                return JsonConvert.SerializeObject(resourceSetting);
            }
            else
                return string.Empty;
        }

        public static string getResourceCheckingList(POCOS.Product product)
        {

            List<JObject> ResourceCheckingList = new List<JObject>();
            List<POCOS.Part> parts = getProductResourceCheckingList(product);
            foreach (POCOS.Part p in parts)
            {
                if (p.ProductLimitedResources != null)
                {
                    ResourceCheckingList.AddRange(from plr in p.ProductLimitedResources
                                                  select new JObject {
                                                        new JProperty("associatedID", p.SProductID),
                                                        new JProperty("associatedName", p.SProductID),
                                                        new JProperty("ResourceID", plr.ResourceID),
                                                        new JProperty("ResourceName",plr.LimitedResource.Name),
                                                        new JProperty("AvaiableQty",plr.AvaiableQty.HasValue ? plr.AvaiableQty.GetValueOrDefault() : 0),
                                                        new JProperty("ConsumingQty",plr.ConsumingQty.HasValue ? plr.ConsumingQty.GetValueOrDefault() : 0),
                                                        new JProperty("Qty", 0)

                       });
                }
            }
            return JsonConvert.SerializeObject(ResourceCheckingList);
        }

        public static List<POCOS.Part> getProductResourceCheckingList(POCOS.Product product)
        {
            List<POCOS.Part> parts = new List<POCOS.Part>();
            if (product is POCOS.Product_Ctos)
            {
                POCOS.Product_Ctos productCTOS = product as POCOS.Product_Ctos;
                parts.Add(productCTOS);
                foreach (var bom in productCTOS.CTOSBOMs)
                {
                    if (bom.type == POCOS.CTOSBOM.COMPONENTTYPE.OPTION && bom.Show)
                        parts.AddRange(bom.componentDetails.Select(x => x.part));
                }
            }
            else
            {
                parts.Add(product);
                parts.AddRange(product.RelatedProductsX.Select(x => x.RelatedPart));
                var peripheralProductList = product.PeripheralCompatibles.Where(x => x.PeripheralProduct.Publish == true).Select(x => x.PeripheralProduct).ToList();
                foreach (var itemPeripheral in peripheralProductList)
                    parts.AddRange(itemPeripheral.partsX);
            }
            return parts;
        }

        /// <summary>
        /// for qualify Products By Resource, addationalResource only supply resource but will not in return list
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="additionalResource"></param>
        /// <returns></returns>
        public static List<POCOS.Product> qualifyProductsByResource(List<POCOS.Product> candidates,Dictionary<POCOS.Part,int> additionalResource=null)
        {
            List<POCOS.Product> qualifiedProducts=new List<POCOS.Product>();
            Dictionary<string, int> availableResource=new Dictionary<string,int>();


            foreach (POCOS.Product product in candidates)
            {

                foreach (var resource in product.availableResource)
                    if (availableResource.ContainsKey(resource.Key))
                        availableResource[resource.Key] += resource.Value;
                    else
                        availableResource.Add(resource.Key, resource.Value);
            }
            if (additionalResource.Any())
            {
                foreach (var part in additionalResource)
                {

                    foreach (var resource in part.Key.availableResource)
                        if (availableResource.ContainsKey(resource.Key))
                            availableResource[resource.Key] += resource.Value*part.Value;
                        else
                            availableResource.Add(resource.Key, resource.Value * part.Value);
                }
            }

            foreach (POCOS.Product product in candidates)
            {
                bool valid = true;
                if (product.consumingResource.Any())
                {
                    foreach (var resource in product.consumingResource)
                    {
                        var matchedItem = (from ar in availableResource
                                           from r in resource.Key.Split(',')
                                           where ar.Key.Split(',').Contains(r)
                                           && ar.Value>=resource.Value 
                                           select ar);
                        valid = matchedItem.Any();
                    }
                }
                if (valid)
                    qualifiedProducts.Add(product);
            }

            return qualifiedProducts;
        }

    }
}
