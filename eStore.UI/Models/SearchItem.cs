using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class SearchItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string StatusIcon { get; set; }
        public string Price { get; set; }
        public List<string> Status { get; set; }
        public string Url { get; set; }

        public SearchGroup searchgroup { get; set; }
         
        public SearchItem() { }
        public SearchItem(POCOS.Part part)
        {
            if (part is POCOS.Product)
            {
                POCOS.Product product = part as POCOS.Product;
                Id = product.SProductID;
                Name = product.name;
                Description = product.productDescX;

                Image = product.thumbnailImageX;
                //StatusIcon = esUtilities.CommonHelper.ResolveUrl(string.Format("~/images/{0}.gif", product.status.ToString()));
                Status = product.marketingstatus.Select(c => c.ToString()).ToList();
                Price = Presentation.Product.ProductPrice.getPrice(product);

                Url = esUtilities.CommonHelper.ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(product));
            }
            else if (part is POCOS.PStoreProduct)
            {
                POCOS.PStoreProduct product = part as POCOS.PStoreProduct;
                Id = product.SProductID;
                Name = product.name;
                Description = product.productDescX;

                Image = product.thumbnailImageX;
                //Status = product.status.ToString();

                Price = Presentation.Product.ProductPrice.getPrice(product);

                Url = esUtilities.CommonHelper.ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(product));
            }
            else
            {
                Id = part.SProductID;
                Name = part.name;
                Description = part.productDescX;

                Image = part.thumbnailImageX;
                //Status = product.status.ToString();

                Price = Presentation.Product.ProductPrice.getPrice(0);

                Url = esUtilities.CommonHelper.ResolveUrl("~/");
            }
            Image = string.IsNullOrEmpty(Image) ? "" : Image.Replace("~/resource/", "/resource/");
        }

    }
}