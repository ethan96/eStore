using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace eStore.POCOS.Sync
{
    public class PAPSSync
    {
        public static PAPSSync instance;
        public static  PAPSSync getInstance()
        {
            if (instance == null)
                instance = new PAPSSync();
            return instance;
        
        }

        public   string Sync(ref POCOS.Part part)
        {
            POCOS.DAL.PartHelper helper = new DAL.PartHelper();
           var specs= helper.getProductEPAPSSpec(part, part.StoreID, true);

           string desc = specs.Where(x => x.AttrName == "Product Description").Select(x => x.LocalValueName).FirstOrDefault();
           if (!string.IsNullOrEmpty(desc))
           {
               part.VendorExtendedDesc = desc;
           }
           string productfeature =  specs.Where(x => x.AttrName == "Features").Select(x => x.LocalValueName).FirstOrDefault();
           if (!string.IsNullOrEmpty(productfeature))
           {
               var _features = productfeature.Split('•').Where(x => string.IsNullOrEmpty(x) == false).ToList();
               part.VendorFeatures = string.Join("", _features.Select(x => string.Format("<li>{0}</li>", x.Trim())).ToArray()); 
           }
            string largeImage=string.Format("{0}_Main.jpg", part.SProductID);
            if (getpapsresourceslist().Any(x => x.Equals(largeImage, StringComparison.OrdinalIgnoreCase)))
               part.TumbnailImageID = string.Format("https://wfcache.advantech.com/www/certified-peripherals/documents/{0}_main.jpg", part.SProductID);
           else
               part.TumbnailImageID = "";
   
            StringBuilder errors = new StringBuilder();
            return errors.ToString();
        }
        public   List<POCOS.ProductResource> getPAPSProductResource(POCOS.Part part)
        {
            string[] files = getpapsresourceslist().Where(x => x.StartsWith(part.SProductID, StringComparison.OrdinalIgnoreCase)).ToArray();

            List<ProductResource> resources = new List<ProductResource>();
            foreach (string file in files)
            {
                string ext = file.Remove(0, part.SProductID.Length).ToLower();
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
                        StoreID = part.StoreID,
                        SProductID = part.SProductID,
                        ResourceName = type,
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
            string key = "ePapsResourcesCachePool";


            if (papsresourceslist == null)
            {
                papsresourceslist = (string[])CachePool.getInstance().getObject(key);
                if (papsresourceslist == null)
                {
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

            string[] files = reader.ReadToEnd().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

            return files;
        }

    }
}
