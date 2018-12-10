using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using eStore.BusinessModules;

namespace eStore.UI.Services
{
    //yest
    /// <summary>
    /// Summary description for shippingrate
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class shippingrate : System.Web.Services.WebService
    {

        [WebMethod]
        public shippingrate.Response getShippingRate(Order order)
        {
            eStore.BusinessModules.StoreSolution soltion = eStore.BusinessModules.StoreSolution.getInstance();
            BusinessModules.Store store=null;
            if (!string.IsNullOrEmpty(order.StoreId))
                store = soltion.getStore(order.StoreId);
            if (store == null)
               store= soltion.getStore("AUS");

            shippingrate.Response Response = new shippingrate.Response();
            List<ShippingMethod> sms;
            sms = null;
            try
            {
                POCOS.User user = new POCOS.User();
                user.UserID = "shippingrateWebService";
                POCOS.Cart eCart = order.convertToeStoreCart(store, user);
                if (eCart.error_message !=null && eCart.error_message.Count > 0)
                {
                    Response.ShippingRates = new List<ShippingRate>();
                    Response.Status = "0";
                    Response.message = "No availabe rates";
                    Response.DetailMessages =  eCart.error_message.Select(c =>  c.columname + "(" + c.message + ")").ToList();
                }
                else
                {
                    sms = store.getAvailableShippingMethods(eCart, false);
                    List<ShippingRate> rates = new List<ShippingRate>();
                    if (sms != null)
                    {
                        foreach (ShippingMethod sm in sms)
                        {
                            var errorMessage = "";
                            if (sm.Error != null)
                                errorMessage = sm.Error.Code.ToString();
                            rates.Add(new ShippingRate(sm.ShippingMethodDescription, sm.ShippingCostWithPublishedRate, errorMessage));
                        }
                        Response.ShippingRates = rates;
                        Response.Status = "1";
                        Response.message = "";

                        List<POCOS.PackagingBox> eboxes = new List<POCOS.PackagingBox>();
                        try
                        {
                            eStore.POCOS.PackingList packinglist = new POCOS.PackingList();
                            eStore.BusinessModules.PackingManager packingManager = new BusinessModules.PackingManager(store.profile);
                            packinglist = packingManager.getPackingList(eCart, 0m);
                            eboxes = packinglist.PackagingBoxes.ToList();
                            List<Box> boxes = new List<Box>();
                            foreach (POCOS.PackagingBox ebox in eboxes)
                            {
                                Box box = new Box();
                                box.Weight = ebox.Weight;
                                box.Width = ebox.Width;
                                box.Height = ebox.Height;
                                box.Length = ebox.Length;
                                List<Item> details = new List<Item>();
                                foreach (POCOS.PackingBoxDetail edetails in ebox.PackingBoxDetails)
                                    details.Add(new Item(edetails.SProductID, edetails.Qty));
                                box.Details = details;
                                boxes.Add(box);
                            }
                            Response.Boxex = boxes;
                        }
                        catch (Exception)
                        {


                        }
                    }
                    else
                    {
                        Response.ShippingRates = new List<ShippingRate>();
                        Response.Status = "0";
                        Response.message = "No availabe rates";
                    }
                }
            }
            catch (Exception ex)
            {
                Response.ShippingRates = new List<ShippingRate>();
                Response.Status = "0";
                Response.message = ex.Message;
            }
            return Response;
        }
        [Serializable]
        public class Response
        {
            public string Status { get; set; }
            public string message { get; set; }
            public List<ShippingRate> ShippingRates { get; set; }
            public List<Box> Boxex { get; set; }
            public List<string> DetailMessages { get; set; }

        }
        public class ShippingRate
        {
            public string Nmae { get; set; }
            public float Rate { get; set; }
            public string ErrorMessage { get; set; }
            public ShippingRate() { }
            internal ShippingRate(string name, float rate, string errorMessage)
            {
                this.Nmae = name;
                this.Rate = rate;
                this.ErrorMessage = errorMessage;
            }
        }

        [Serializable]
        public class Item
        {
            public Item() { }
            public Item(string id, int qty)
            { ProductID = id; Qty = qty; }
            public string ProductID { get; set; }
            public int Qty { get; set; }
            public decimal Price { get; set; }
        }
        public class Box {
            public decimal Width { get; set; }
            public decimal Length { get; set; }
            public decimal Height { get; set; }
            public decimal Weight { get; set; }
            public List<Item> Details { get; set; }
        }

        public class ConfigSystem : Item
        {
            public ConfigSystem() { }
            public ConfigSystem(string id, int qty) : base(id, qty) { }

            public List<Item> Details { get; set; }
        }
        public class Address
        {
            public string Countrycode { get; set; }
            public string StateCode { get; set; }
            public string Zipcode { get; set; }

            internal POCOS.CartContact convertToeStoreCartContact()
            {
                POCOS.CartContact contact = new POCOS.CartContact();
                contact.countryCodeX = Countrycode;
                contact.ZipCode = Zipcode;
                contact.State = StateCode;
                return contact;
            }
        }
        [Serializable]
        public class Order
        {
            public String  StoreId { get; set; }
            public Address Shipto { get; set; }
            public Address Billto { get; set; }
            public List<Item> Items { get; set; }
            public List<ConfigSystem> Systems { get; set; }
            internal POCOS.Cart convertToeStoreCart(BusinessModules.Store store, POCOS.User user)
            {
                POCOS.Cart cart = new POCOS.Cart(store.profile, user);
                cart.ShipToContact = Shipto.convertToeStoreCartContact();
                cart.BillToContact = Billto.convertToeStoreCartContact();
                if (Items != null && Items.Count > 0)
                {
                    List<string> sproductIdList = Items.Select(p => p.ProductID).ToList();
                    //获取已存在的 part
                    List<POCOS.Part> partList = new List<POCOS.Part>();
                    //获取已存在的 sapproduct
                    List<POCOS.SAPProduct> sapProductList = new List<POCOS.SAPProduct>();
                    getPrefetchPartInfo(store, sproductIdList, ref partList, ref sapProductList);

                    foreach (Item item in Items)
                    {
                        POCOS.Part part = partList.FirstOrDefault(p => p.SProductID.ToUpper() == item.ProductID.ToUpper());
                        if (part == null)
                        {
                            POCOS.SAPProduct sapProduct = sapProductList.FirstOrDefault(p => p.PART_NO.ToUpper() == item.ProductID.ToUpper());
                            if (sapProduct != null)
                                part = new POCOS.Part(sapProduct, store.profile);
                        }
                        if (part == null)
                            throw new Exception(item.ProductID + " is not available");
                        else
                        {
                            decimal price = 1;
                            if (item.Price > 0)
                                price = item.Price;
                            else if (part.VendorSuggestedPrice.HasValue && part.VendorSuggestedPrice > 0)
                                price = part.VendorSuggestedPrice.Value;
                            cart.addItem(part, item.Qty, null, price);
                        }
                    }
                }
                
                foreach (ConfigSystem sys in Systems)
                {
                    POCOS.Product_Ctos ctos = store.getProduct("SBC-BTO") as POCOS.Product_Ctos;
                    List<POCOS.BundleItem> bundle = new List<POCOS.BundleItem>();

                    List<string> sproductIdList = sys.Details.Select(p => p.ProductID).ToList();
                    //获取已存在的 part
                    List<POCOS.Part> partList = new List<POCOS.Part>();
                    //获取已存在的 sapproduct
                    List<POCOS.SAPProduct> sapProductList = new List<POCOS.SAPProduct>();
                    getPrefetchPartInfo(store, sproductIdList, ref partList, ref sapProductList);

                    foreach (Item detail in sys.Details)
                    {
                        POCOS.Part part = partList.FirstOrDefault(p => p.SProductID.ToUpper() == detail.ProductID.ToUpper());
                        if (part == null)
                        {
                            POCOS.SAPProduct sapProduct = sapProductList.FirstOrDefault(p => p.PART_NO.ToUpper() == detail.ProductID.ToUpper());
                            if (sapProduct != null)
                                part = new POCOS.Part(sapProduct, store.profile);
                        }
                        if (part != null)
                        {
                            decimal price = 1;
                            if (detail.Price > 0)
                                price = detail.Price;
                            else if (part.VendorSuggestedPrice.HasValue && part.VendorSuggestedPrice > 0)
                                price = part.VendorSuggestedPrice.Value;
                            bundle.Add(new POCOS.BundleItem(part, detail.Qty, 1, price));
                        }
                    }
                    eStore.POCOS.BTOSystem btos = ctos.getDefaultBTOS();
                    btos.addNoneCTOSBundle(bundle, 1);
                    cart.addItem(ctos, sys.Qty, btos);

                }
                return cart;
            }

            internal void getPrefetchPartInfo(BusinessModules.Store store, List<string> sproductIdList, ref List<POCOS.Part> partList, ref List<POCOS.SAPProduct> sapProductList)
            {
                //获取存在的 part
                partList = store.getPartList(string.Join(",", sproductIdList));
                //获取不在part中的sproductId
                foreach (var item in partList.Select(p => p.SProductID).ToList())
                {
                    if (sproductIdList.Contains(item))
                        sproductIdList.Remove(item);
                }
                //获取存在的 sapProduct
                if (sproductIdList.Count > 0)
                    sapProductList = store.getSAPProductList(sproductIdList);
            }
        }
    }
}
