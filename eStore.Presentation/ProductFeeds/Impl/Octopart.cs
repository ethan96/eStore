using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.BusinessModules;
using System.IO;

namespace eStore.Presentation.ProductFeeds.Impl
{
    public class Octopart : FeedSubscriptions
    {
        BusinessModules.Store store;
        public override OutputType outputType
        {
            get
            {
                return OutputType.csv;
            }
        }
        public override System.IO.MemoryStream Subscribe()
        {
            List<string> rlt = new List<string>();
            store = StoreSolution.getInstance().getStore("AUS");
            string header = "manufacturer\tmpn\tsku\tdistributor-url\teligible-region\tquantity\tprice-break-1\tprice-usd-1\tdescription\tattributes\tcategory\tmanufacturer-url\tdatasheet-url\timage-url\t3d-model-url\tinstruction-sheet-url\tdatecode";



            List<POCOS.ProductCategory> categories = store.getTopLevelCTOSProductCategories(null);
            categories.AddRange(store.getTopLevelStandardProductCategories(null));
            foreach (var category in categories)
            {
                var filter = filters.FirstOrDefault(x => x.CategoryPath.Equals(category.CategoryPath, StringComparison.OrdinalIgnoreCase));
                if (filter != null && filter.status)
                {
                    if (filter.ExcludeOnlys.Any())
                    {
                        foreach (var sc in category.childCategoriesX)
                        {
                            if (filter.ExcludeOnlys.Any(x => x.Equals(sc.CategoryPath, StringComparison.OrdinalIgnoreCase)) == false)
                                rlt.AddRange(getProductFeed(sc));
                        }
                    }
                    else if (filter.IncludeOnlys.Any())
                    {
                        foreach (var sc in category.childCategoriesX)
                        {
                            if (filter.ExcludeOnlys.Any(x => x.Equals(sc.CategoryPath, StringComparison.OrdinalIgnoreCase)) == true)
                                rlt.AddRange(getProductFeed(sc));
                        }
                    }
                    else
                    {
                        rlt.AddRange(getProductFeed(category));
                    }
                    
                }

            }
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                System.Text.UTF8Encoding encoding = new UTF8Encoding();
                System.IO.StreamWriter sw = new System.IO.StreamWriter(stream, encoding);
                try
                {


                    sw.WriteLine(header);
                    foreach (string data in rlt)
                        sw.WriteLine(data);
                    sw.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                }
                finally
                {
                    sw.Dispose();
                }
                return stream;
            }

        }
        List<string> getProductFeed(POCOS.ProductCategory category)
        {
            List<string> rlt = new List<string>();
            foreach (var p in category.productList.ToList())
            {
                string feed = getProductFeed(p);
                if (!string.IsNullOrEmpty(feed))
                    rlt.Add(feed);
            }
            return rlt;
        }

        List<string> processedproducts = new List<string>();
        /// <summary>
        /// manufacturer	mpn	sku	distributor-url	eligible-region	quantity	price-break-1	price-{CUR}-1	description	attributes	category
        /// manufacturer-url	datasheet-url	image-url	3d-model-url	instruction-sheet-url	moq	order-multiple	datecode
        ///1	2	3	4	5	6	7	8	9	10	11	12	13	14	15	16	17	18	19
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        string getProductFeed(POCOS.Product product)
        {
            if (processedproducts.Any(x => x == product.SProductID))
                return "";
            processedproducts.Add(product.SProductID);

            string atp = "1";
            if (product.isOrderable() && product.getListingPrice().value > 0 )
            { }
            else
                return "";

            //if (!(product is POCOS.Product_Ctos || product is POCOS.Product_Bundle))
            //{
            //    if (product.atpX.availableQty > 0)
            //    {
            //        atp = product.atpX.availableQty.ToString();

            //    }
            //    else
            //        return "";
            //}
            //else
            //    atp = "1";

            string attributes = string.Empty, category = string.Empty;
            attributes = string.Join(";;", product.specs.Select(x => string.Format("{0}::{1}", x.AttrName, x.AttrValueName)).ToArray());
            category = string.Join(">>", product.productCategories.Where(x=>x.MiniSite == null).Select(x => x.localCategoryNameX).ToArray());
            string datecode = DateTime.Now.ToString("yyyy-MM-dd");
            string url = "http://" + store.profile.StoreURL + esUtilities.CommonHelper.ResolveUrl(UrlRewriting.MappingUrl.getMappingUrl(product)) + "?utm_source=octopart&utm_medium=cpc&utm_campaign=octopart";
            string _3durl = product.productResourcesX.Where(x => x.ResourceType == "3DModel").Select(x => x.ResourceURL).FirstOrDefault();
            string manual = product.productResourcesX.Where(x => x.ResourceType == "Manual").Select(x => x.ResourceURL).FirstOrDefault();
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}",
             "Advantech", //manufacturer
product.name, //mpn
product.name, //sku
url, //distributor-url
"US", //eligible-region
"1", //quantity
"1", //price-break-1
product.getListingPrice().value, //price-{CUR}-1
cleanup(product.productDescX), //description
cleanup(attributes), //attributes
cleanup(category), //category
url, //manufacturer-url
product.dataSheetX, //datasheet-url
product.thumbnailImageX, //image-url
_3durl, //3d-model-url
manual, //instruction-sheet-url
                //product.SProductID, //moq
                //product.SProductID, //order-multiple
datecode //datecode

                );

        }
        List<OctopartFilter> _filters;
        List<OctopartFilter> filters
        {
            get
            {
                if (_filters == null)
                {
                    _filters = new List<OctopartFilter>();


                    _filters.Add(new OctopartFilter() { CategoryPath = "PAC", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "ICC", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "SBC", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "IA", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "MstrCATE_EAPRO_RSCOMM", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "IM", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "data-acquisition-module", IncludeOnly = "DELETE", ExcludeOnly = "DELETE" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "MstrCATE_EAPRO_AUTOSOFT", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "IPB", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "Software", IncludeOnly = "DELETE", ExcludeOnly = "DELETE" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "Indus_Video_Sulotion", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "Remote_IO_Module", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "Operator_interface", IncludeOnly = "DELETE", ExcludeOnly = "DELETE" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS0937cc", IncludeOnly = "DELETE", ExcludeOnly = "DELETE" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS33c584", IncludeOnly = " ", ExcludeOnly = " " });
                    _filters.Add(new OctopartFilter() { CategoryPath = "EPPEZDS_DVS", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "EPPEZDS_DSA", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "EPPEZEC_Embeddedauto", IncludeOnly = " ", ExcludeOnly = "AUSfc5d1b" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "vehiclePC", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "EPPEZIPC", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS_30003", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS_30016", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS_30020", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS_30024", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS_30035", IncludeOnly = " ", ExcludeOnly = "AUS_30053" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS_30047", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS_30130", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS_30134", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS_31354", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS_31356", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS_31386", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS_31402", IncludeOnly = "", ExcludeOnly = "" });
                    _filters.Add(new OctopartFilter() { CategoryPath = "AUS_31428", IncludeOnly = "DELETE", ExcludeOnly = "DELETE" });


                }
                return _filters;
            }
        }
        public class OctopartFilter
        {
            public string CategoryPath { get; set; }

            string includeOnly;
            public string IncludeOnly
            {
                get { return includeOnly; }
                set
                {

                    includeOnly = value;
                    if (includeOnly.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                    {
                        status = false;
                    }

                    if (includeOnly.Contains(","))
                        IncludeOnlys = includeOnly.Split(',').ToList();
                    else
                    {
                        IncludeOnlys = new List<string>();
                        if(!string.IsNullOrEmpty(includeOnly))
                        IncludeOnlys.Add(includeOnly);
                    }
                }
            }
            string excludeOnly;
            public string ExcludeOnly
            {
                get { return excludeOnly; }
                set
                {

                    excludeOnly = value;
                    if (excludeOnly.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                    {
                        status = false;
                    }

                    if (excludeOnly.Contains(","))
                        ExcludeOnlys = excludeOnly.Split(',').ToList();
                    else
                    {
                        ExcludeOnlys = new List<string>();
                        if (!string.IsNullOrEmpty(excludeOnly))
                            ExcludeOnlys.Add(excludeOnly);
                    }
                }
            }
            public List<string> IncludeOnlys { get; set; }
            public List<string> ExcludeOnlys { get; set; }
            
            private bool _stauts=true;
            public bool status { get{return _stauts;} set{_stauts=value;} }
        }
    }
}
