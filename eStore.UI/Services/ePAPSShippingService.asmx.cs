using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using eStore.BusinessModules;

namespace eStore.UI.Services
{
    /// <summary>
    /// Summary description for ePAPSShippingService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ePAPSShippingService : System.Web.Services.WebService
    {                
        [WebMethod(EnableSession = true)]
        public PAPSResponse getFreightEstimation(List<ProductShippingDimensionX> productShippingDimensions, string fromZipCode, string toZipCode)
        {
            var response = new PAPSResponse();
            var shippingMethods = new List<ShippingMethodX>();
            List<ShippingInstructionX> shippingInstructions = new List<ShippingInstructionX>();
            var shippingDimensions = new List<ProductShippingDimension>();
            var service = new eStore.BusinessModules.ePAPSShippingService();

            foreach (var p in productShippingDimensions)
            {
                shippingDimensions.Add(new ProductShippingDimension
                {
                    ID = p.ID,
                    ItemNo = p.ItemNo,
                    PartNo = p.PartNo,
                    Width = p.Width,
                    Depth = p.Depth,
                    Height = p.Height,
                    FreeShipping = p.FreeShipping,
                    Quantity = p.Quantity,
                    ShipFrom = p.ShipFrom,
                    ShipWeightKG = p.ShipWeightKG
                });
            }

            POCOS.Cart cart = service.prepareCart(Presentation.eStoreContext.Current.Store.profile, shippingDimensions);
            POCOS.ShippingCarier carrier = new POCOS.ShippingCarier();
            POCOS.Store store = cart.storeX;
            
            var result = service.getFreightEstimationX(store, shippingDimensions, fromZipCode, toZipCode);

            if (result.Status == "OK")
            {
                foreach (var s in result.ShippingInstructions)
                {
                    shippingInstructions.Add(new ShippingInstructionX
                    {
                        ShippingMethodId = s.ShippingMethodId,
                        ServiceCode = s.ServiceCode,
                        ShippingCarrier = s.ShippingCarrier,
                        ShipVia = s.ShipVia,
                        ItemNo = s.ItemNo,
                        PartNo = s.PartNo,
                        Level = s.Level,
                        ContainerNo = s.ContainerNo,
                        ContainerFreight = s.ContainerFreight,
                        Rate = s.Rate
                    });
                }
                foreach (var m in result.ShippingMethods)
                {
                    shippingMethods.Add(new ShippingMethodX
                    {
                        ServiceCode = m.ServiceCode,
                        ShippingCarrier = m.ShippingCarrier,
                        ShippingMethodDescription = m.ShippingMethodDescription,
                        ShippingCostWithPublishedRate = m.ShippingCostWithPublishedRate
                    });
                }
                response.Status = "OK";
                response.Message = "Service Completed!";
                response.ShippingMethods = shippingMethods;
                response.ShippingInstructions = shippingInstructions;
            }
            else
            {
                response.Status = result.Status;
                response.Message = result.Message;
                response.ShippingInstructions = new List<ShippingInstructionX>();
                response.ShippingMethods = new List<ShippingMethodX>();
            }
            return response;
        }        
    }

    [Serializable]
    public class PAPSResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<ShippingMethodX> ShippingMethods { get; set; }
        public List<ShippingInstructionX> ShippingInstructions { get; set; }
    }

    [Serializable]
    public class ShippingMethodX
    {
        public string ShippingCarrier { get; set; }
        public string ServiceCode { get; set; }
        public string ShippingMethodDescription { get; set; }
        public float ShippingCostWithPublishedRate { get; set; }
    }

    [Serializable]
    public class ShippingInstructionX
    {
        public int ShippingMethodId { get; set; }
        public string ShippingCarrier { get; set; }
        public string ServiceCode { get; set; }
        public float Rate { get; set; }
        public string ContainerNo { get; set; }
        public float ContainerFreight { get; set; }
        public string ShipVia { get; set; }
        public int ItemNo { get; set; }
        public string PartNo { get; set; }
        public int Level { get; set; }
    }

    [Serializable]
    public class USPSPackage
    {
        public int ID { get; set; }
        public string Service { get; set; }
        public string Description { get; set; }
        public string Container { get; set; }
        public decimal? Rate { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Depth { get; set; }
        public int? WeightLimitationInPound { get; set; }
        public decimal? WeightLimitationInOunce { get; set; }
    }

    [Serializable]
    public class ProductShippingDimensionX
    {
        public int ID { get; set; }
        public int ItemNo { get; set; }
        public string PartNo { get; set; }
        public decimal Width { get; set; }
        public decimal Depth { get; set; }
        public decimal Height { get; set; }
        public decimal? ShipWeightKG { get; set; }
        public decimal Surface { get { return Width * Depth; } }
        public string ShipFrom { get; set; }
        public bool? FreeShipping { get; set; }
        public int Quantity { get; set; }
    } 
}
