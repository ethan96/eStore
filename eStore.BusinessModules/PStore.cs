using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using FtpLib;
using System.Net;
using System.IO;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules
{
    public class PStore : Store
    {
      
        public POCOS.PStore pstore;
        public PStore(eStore.POCOS.Store profile)
            : base(profile)
        {
            initDate = DateTime.Now;
            POCOS.DAL.PStoreHelper helper = new PStoreHelper();
            pstore = helper.getPStoreByStore(profile);
        }

        private static Dictionary<string, PStore> _instances=new Dictionary<string,PStore>();

        public static PStore getInstance(eStore.POCOS.Store profile)
        {
            lock (_instances)
            {
                if (!_instances.ContainsKey(profile.StoreID) )
                {
                    _instances.Add(profile.StoreID, new PStore(profile));
                }
                else if (_instances[profile.StoreID].isExpired())
                {
                    _instances[profile.StoreID] = new PStore(profile);
                }
                return _instances[profile.StoreID];
            }
        }

        public bool isActive() {
            return pstore != null;
        }

        private DateTime? initDate;
        public bool isExpired()
        {
            return initDate == null || initDate.GetValueOrDefault().AddHours(2) < DateTime.Now;
        }
        public List<PStoreProductCategory> getTopLevelPStoreCategory()
        {
            eStore.POCOS.DAL.PStoreProductCategoryHelper helper = new POCOS.DAL.PStoreProductCategoryHelper();
            return helper.getTopLevelCategory(pstore);
        }
        public Dictionary<eStore.POCOS.StoreDeal.PromotionType, List<PStoreProduct>> getPromotionPStoreProducts(List<eStore.POCOS.StoreDeal.PromotionType> types)
        {
            POCOS.DAL.PStoreProductHelper helper = new POCOS.DAL.PStoreProductHelper();
            return helper.getPromotionPStoreProducts(this.storeID, types);
        }
        public List<PStoreProduct> getPromotionPStoreProducts(eStore.POCOS.StoreDeal.PromotionType type)
        {
            POCOS.DAL.PStoreProductHelper helper = new POCOS.DAL.PStoreProductHelper();
            return helper.getPromotionPStoreProducts(this.storeID, type);
        }
        public List<StoreProductBundleList> getSolutionList()
        {
            if (!isActive())
                return new List<StoreProductBundleList>();
            POCOS.DAL.PStoreProductHelper helper = new POCOS.DAL.PStoreProductHelper();
            return helper.getSolutionList(this.pstore.Id, this.storeID);
        }
        public new List<PStoreProduct> getMatchProducts(String keyword, bool includeinvalid = false, Int32 maxCount = 9999)
        {
            POCOS.DAL.PStoreProductHelper helper = new POCOS.DAL.PStoreProductHelper();
            if (maxCount != 9999)
                maxCount = maxCount * 2;    //increase buffer in case there is unexpected filtering

            List<PStoreProduct> products = helper.searchOnlybyKeywords(keyword.Trim(), storeID, maxCount, includeinvalid);

            return products;
        }

     protected   override List<Advertisement> getQualifiedProductAds(String productId)
        {
            AdvertisementHelper helper = new AdvertisementHelper();
            //Dictionary to filter duplicate entries
            Dictionary<String, Advertisement> adList = new Dictionary<String, Advertisement>();

            //below ad type can be dispayed in product page
            List<Advertisement.AdvertisementType> adtypes = new List<Advertisement.AdvertisementType>();
            adtypes.Add(Advertisement.AdvertisementType.StoreAds);
            adtypes.Add(Advertisement.AdvertisementType.Resources);
            adtypes.Add(Advertisement.AdvertisementType.Floating);
            adtypes.Add(Advertisement.AdvertisementType.Expanding);
            try
            {
                if (!String.IsNullOrEmpty(productId))
                {
                    Part part = getPart(productId);
                    if (part != null && part is PStoreProduct)
                    {
                        Product product = new Product();
                        product.StoreID = part.StoreID;
                        product.SProductID = part.SProductID;
                        //in present version eStore will only support StoreAds type
                        List<Advertisement> ads = helper.getAds(product, adtypes);

                        mergeAdList(adList, ads, 2000);

                        //finding category matches
                        PStoreProduct pstoreproduct = (PStoreProduct)part;
                        if (pstoreproduct.category != null)
                        {
                            List<Advertisement> categoryAds = getQualifiedCategoryAds(pstoreproduct.category.Id.ToString());
                            mergeAdList(adList, categoryAds, 1000);
                        }
                        //find default Ads
                        mergeAdList(adList, helper.getDefaultAdv(part.StoreID, Advertisement.AdvertisementType.StoreAds));
                    }


                }
            }
            catch (Exception)
            {
                //ignore
            }

            return adList.Values.OrderByDescending(ad => ad.weight).ToList();
        }

     protected override List<Advertisement> getQualifiedCategoryAds(String categoryId)
        {
            AdvertisementHelper helper = new AdvertisementHelper();
            //Dictionary to filter duplicate entries
            Dictionary<String, Advertisement> adList = new Dictionary<String, Advertisement>();
            try
            {
                List<Advertisement> ads = new List<Advertisement>();
                int cid;
                if (!String.IsNullOrEmpty(categoryId)&& int.TryParse(categoryId,out cid))
                {
                    //below ad type can be dispayed in product category page
                    List<Advertisement.AdvertisementType> adtypes = new List<Advertisement.AdvertisementType>();
                    adtypes.Add(Advertisement.AdvertisementType.StoreAds);
                    adtypes.Add(Advertisement.AdvertisementType.Resources);
                    adtypes.Add(Advertisement.AdvertisementType.Floating);
                    adtypes.Add(Advertisement.AdvertisementType.Expanding);
                    PStoreProductCategory category = (new PStoreProductCategoryHelper()).get(pstore,cid);
                    if (storeID == "AKR")
                        adtypes.Add(Advertisement.AdvertisementType.CenterPopup);
                    if (category != null) {
                        ProductCategory tmpcategory = new ProductCategory();
                        tmpcategory.Storeid = this.storeID;
                        tmpcategory.CategoryPath = categoryId;
                        ads = helper.getAds(tmpcategory, adtypes);
                        mergeAdList(adList, ads, 1000);
                    }
              
                }

                //find default Ads
                mergeAdList(adList, helper.getDefaultAdv(storeID, Advertisement.AdvertisementType.StoreAds));
            }
            catch (Exception)
            {
                //ignore
            }

            return adList.Values.OrderByDescending(ad => ad.weight).ToList();
        }

     public Advertisement getTodaysDeals(PStoreProductCategory category)
     {
         AdvertisementHelper helper = new AdvertisementHelper();
         List<Advertisement.AdvertisementType> adtypes = new List<Advertisement.AdvertisementType>();
         adtypes.Add(Advertisement.AdvertisementType.TodaysDeals);
         List<Advertisement> ads = new List<Advertisement>();
         if (category != null)
         {
            
             ProductCategory tmpcategory = new ProductCategory();
             tmpcategory.Storeid = this.storeID;
             tmpcategory.CategoryPath = category.Id.ToString();
             ads = helper.getAds(tmpcategory, adtypes);
           
         }
         return ads.FirstOrDefault();
     }
     public List<Advertisement> getTodaysDeals(PStoreProduct product)
     {
         AdvertisementHelper helper = new AdvertisementHelper();
         List<Advertisement.AdvertisementType> adtypes = new List<Advertisement.AdvertisementType>();
         adtypes.Add(Advertisement.AdvertisementType.TodaysDeals);
         List<Advertisement> ads = new List<Advertisement>();
         if (product != null)
         {
             Product tmpproduct = new Product();
             tmpproduct.StoreID = this.storeID;
             tmpproduct.SProductID = product.SProductID.ToString();
             ads = helper.getAds(tmpproduct, adtypes);
             if (product.category != null)
             {
                 List<Advertisement> cads = new List<Advertisement>();
                 ProductCategory tmpcategory = new ProductCategory();
                 tmpcategory.Storeid = this.storeID;
                 tmpcategory.CategoryPath = product.category.Id.ToString();
                 cads = helper.getAds(tmpcategory, adtypes);
                 foreach (var cad in cads)
                 {
                     if (ads.Any(x => x.id == cad.id) == false)
                         ads.Add(cad);
                 }
             }
         }
         return ads;
     }


        public string[] syncProductImages()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://webftp.advantech.com.tw/0212papsdoc1147");
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("apiagent", "thu0731","aus");
            request.Proxy = null;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string[] files = reader.ReadToEnd().Split('\n');
            return files;

        }

        public List<POCOS.ProductResource> getPAPSProductResource(string storeid,string partno)
        {
            string[] files = getpapsresourceslist().Where(x => x.StartsWith(partno, StringComparison.OrdinalIgnoreCase)).ToArray();

            List<ProductResource> resources = new List<ProductResource>();
            foreach (string file in files)
            {
                string ext = file.Remove(0, partno.Length).ToLower();
                string type = string.Empty;
                switch (ext)
                { 
                    case "_b.jpg":
                    case "_b1.jpg":
                    case "_b2.jpg":
                    case "_b3.jpg":
                    case "_b4.jpg":
                    case "_b5.jpg":
                    case "_b6.jpg":
                    case "_b7.jpg":
                        type = "LargeImages";
                        break;
                    case "_datasheet.pdf"://ftp://aus\apiagent@webftp.advantech.com.tw/0212papsdoc1147/96PS-A400W2U_datasheet.pdf
                        type = "Datasheet";
                        break;
                    case "_main.jpg"://ftp://aus\apiagent@webftp.advantech.com.tw/0212papsdoc1147/96PS-A350WFX_main.jpg
                        type = "LargeImage";
                        break;
                    default:
                        type = string.Empty;
                        break;
                }
                if (!string.IsNullOrEmpty(type))
                {
                    ProductResource resource = new ProductResource
                    {
                        StoreID = storeid,
                        SProductID = partno,
                        ResourceType = type,
                        IsLocalResource = false,
                        ResourceURL = "https://wfcache.advantech.com/www/certified-peripherals/documents/" + file
                    };
                    resources.Add(resource);
                }
            }
            return resources;

        }
        string[] papsresourceslist;
        private string[] getpapsresourceslist()
        {
            string key = this.profile.StoreID + "ePapsResources";
         

           if (papsresourceslist == null)
           {
               papsresourceslist = (string[])CachePool.getInstance().getObject(key);
               if (papsresourceslist == null) {
                   papsresourceslist = downloadpapsresourceslistfromftp();
                   CachePool.getInstance().cacheObject(key, papsresourceslist, CachePool.CacheOption.Hour6);
               }
               
           }
            return papsresourceslist;
        }

        private string[] downloadpapsresourceslistfromftp()
        {
         
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://webftp.advantech.com.tw/0212papsdoc1147");
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("apiagent", "thu0731", "aus");
                request.Proxy = null;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string[] stringSeparators = new string[] { "\r\n" };
             
                string[] files = reader.ReadToEnd().Split(stringSeparators,StringSplitOptions.RemoveEmptyEntries);

                return files;
        }

        //get policy
        public override string policylink()
        {
            if (this.profile.getBooleanSetting("showPolicyOnProduct"))
                return this.profile.getStringSetting("pstorePolicyUrl");
            else
                return "";
        }

    }
}
