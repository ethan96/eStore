using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace eStore.Presentation.AJAX.Function
{
    public class getSearchKeyV2 : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {

            int maxRows = 10;
            string keyword = context.Request["keyword"] ?? "";
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 2)
                return "";
            int.TryParse(context.Request["maxRows"].ToString(), out maxRows);

            if (maxRows > 20)
                maxRows = 20;   //limited amount of hint search, this is to prevent hacking crash
                                //POCOS.ProductSpecRules psr = eStoreContext.Current.Store.getMatchProducts(keyword,false, maxRows);
                                //  public List<ProductSearchOptimizedResult> getMatchProducts2(String keyword, Int32 maxCount = 9999, bool includeCategory = false)

            List<POCOS.ProductSearchOptimizedResult> result = eStoreContext.Current.Store.getMatchProducts2(keyword, maxRows, false);

            var data = (from item in result
                        group item by item.Group into g
                        select new
                        {
                            Group = g.Key,
                            Items = g.OrderByDescending(x => x.RANK).Select((v, i) => new
                            {
                                Index = i,
                                SearchResult = v
                            })
                        }).SelectMany(x => x.Items).OrderBy(x => x.Index).Take(12)
                        .Select(x => x.SearchResult)
                        .OrderByDescending(x => x.Group).ThenByDescending(x => x.RANK)
                        .ToList();
            if (data != null && data.Any())
            {
                var trems = eStoreContext.Current.Store.getftsparser(keyword);
                foreach (var item in trems)
                {
                    data.ForEach((c) => {
                        c.DisplayPartno = esUtilities.StringUtility.ReplaceString(item, "<span>{0}</span>", c.DisplayPartno);
                    });
                }
            }            

            var rlt = (from p in data
                       select new JObject {
                            new JProperty("GroupId",p.Group),
                             new JProperty("GroupName", eStore.Presentation.eStoreLocalization.Tanslation("eStore_search_"+ p.Group)),
                             new JProperty("Id", p.Group=="Product"?p.SProductID:p.CategoryID .ToString()),
                            new JProperty("Name", p.DisplayPartno),
                            new JProperty("Url",esUtilities.CommonHelper.ResolveUrl( p.StoreUrl)),
                            new JProperty("ProCount", p.Group=="Category" ? p.ProCount : 0),
                       });
           return JsonConvert.SerializeObject(rlt);
        }
    }
}