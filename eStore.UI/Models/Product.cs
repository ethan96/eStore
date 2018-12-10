using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class Product
    {
        public Product() { }
        public Product(POCOS.SimpleProduct simproduct)
        {
            var product = simproduct.mappedToProduct();
            mappedProduct = product;
            Id = product.SProductID;
            Name = product.name;
            Description = product.productDescX;
            Fetures = product.productFeatures;
            Image = esUtilities.CommonHelper.ResolveUrl(esUtilities.CommonHelper.fixLocalImgPath(product.thumbnailImageX));
            Status = product.marketingstatus.Select(c=>c.ToString()).ToList();
            SalePrice = product.getListingPrice().value.ToString();
            Price = product.isIncludeSatus(POCOS.Product.PRODUCTMARKETINGSTATUS.Inquire) ? "" : Presentation.Product.ProductPrice.getPrice(product);
            CurrencySign = "";
            Url = esUtilities.CommonHelper.ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(product));
            FactorIds = null;// product.specs.Select(x => "V" + x.AttrValueID.ToString()).ToList();
            MarketingStatus = product.marketingstatus;
        }

        private POCOS.Product mappedProduct { get; set; }
        public Product(POCOS.Part part)
        {
            if (part is POCOS.Product)
            {
                POCOS.Product product = part as POCOS.Product;
                mappedProduct = product;
                Id = product.SProductID;
                Name = product.name;
                Description = product.productDescX;
                Fetures = product.productFeatures;
                Image =esUtilities.CommonHelper.ResolveUrl( product.thumbnailImageX);
                Status = product.marketingstatus.Select(c => c.ToString()).ToList();
                SalePrice = product.getListingPrice().value.ToString();
                Price = Presentation.Product.ProductPrice.getPrice(product);
                CurrencySign = "";
                Url = esUtilities.CommonHelper.ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(product));
                FactorIds = null;// product.specs.Select(x => "V" + x.AttrValueID.ToString()).ToList();
                MarketingStatus = product.marketingstatus;
            }
        }

        public Product(POCOS.Product product,string[] properties){
            foreach (string p in properties)
            {
                switch (p) {
                    case "Id":
                        Id = product.SProductID;
                        break;
                    case "Name":
                        Name = product.name;
                        break;
                    case "FactorIds":
                        FactorIds = product.specs.Select(x => x.AttrValueID.ToString()).ToList();
                        break;
                    default:
                        break;
                }
            }
        }

        public void loadMatrix(List<POCOS.SimpleProduct> simpProductList)
        {
            var mappedProduct = simpProductList.FirstOrDefault(c => c.SProductID.Equals(this.Id)).mappedToProduct();
            Matrix = mappedProduct.specs.Select(c => new ProductMatrix(c)).ToList();
        }
        public void loadMatrix()
        {
            if (mappedProduct != null)
                Matrix = mappedProduct.specs.Select(c => new ProductMatrix(c)).ToList();
        }


        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Fetures { get; set; }
        public string Image { get; set; }
        public List<string> Status { get; set; }
        public string SalePrice { get; set; }
        public string Price { get;set; }
        public string CurrencySign { get; set; }
        public string Url { get; set; }
        public int Sequence { get; set; }
        public List<string> FactorIds { get; set; }
        public List<ProductMatrix> Matrix { get; set; }
        public List<POCOS.Product.PRODUCTMARKETINGSTATUS> MarketingStatus { get; set; }
        private List<string> marketingstatusicons;
        public List<string> MarketingStatusIcons
        {
            get
            {
                if (this.marketingstatusicons == null)
                {
                    this.marketingstatusicons = new List<string>();
                    if (this.MarketingStatus != null)
                    {
                        foreach (var ms in this.MarketingStatus)
                        {
                            BusinessModules.ProductDelivery delivery = eStore.Presentation.ProductDeliveryBss.getDeliveryInfor(ms);
                            this.marketingstatusicons.Add(string.Format("<img src='{0}' title='{1}' />", delivery.Ico, delivery.Message));
                        }
                    }
                }
                return this.marketingstatusicons;
            }
        }
    }
}