using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using System.Xml;
using System.IO;

namespace eStore.BusinessModules
{
    class ShippingCarrierAEUUPS : Carrier //可考虑继承 ShippingCarrierUPS
    {

        private int maxWeight = 70; //ups最大配送重量 70 kg

        public ShippingCarrierAEUUPS(eStore.POCOS.Store store, ShippingCarier shippingCarrier)
            : base(store, shippingCarrier)
        { }

        /// <summary>
        /// thie method is to find aeu ups shipping method
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="shipFromAddress"></param>
        /// <returns></returns>
        public override List<ShippingMethod> getFreightEstimation(POCOS.Cart cart, POCOS.Address shipFromAddress)
        {
            List<ShippingMethod> _upsShippingMethods = new List<ShippingMethod>();
            
            var country = this.store.getCountry(cart.ShipToContact.countryCodeX);
            if (country != null && country.EUUPSZones != null)
            {
                PackingManager packingManager = new PackingManager(_store);
                PackingList packingList = packingManager.getPackingList(cart, 0);
                float weight = 0f;
                //List<float> weights = new List<float>();
                foreach (PackagingBox pb in packingList.PackagingBoxes)
                {
                    MeasureUnit measure = pb.Measure;
                    measure.Convert(this.MeasureUnitType);
                    //weight += (float)measure.Weight;
                    //decimal volumetricWeight = (measure.Height*measure.Width*measure.Length)/5000;
                    weight += (float) (measure.Height*measure.Width*measure.Length)/5000;
                    //weights.AddRange(getIntList((float)measure.Weight));
                    //weights.AddRange(getIntList((volumetricWeight > measure.Weight) ? (float)volumetricWeight : (float)measure.Weight));
                    //weights.AddRange(getIntList((float)volumetricWeight));
                }

                List<EUUPSZone> ls = country.getEUUPSByZipCode(cart.ShipToContact.getSAPZipCode());
                    //if (weights != null && weights.Count>0 && ls != null && ls.Count > 0)
                      //_upsShippingMethods = getEUUPS(weights, ls);
                if (ls != null && ls.Count > 0)
                    _upsShippingMethods = getEUUPS(weight, ls);    
  
            }
            else
            {
                //doesn't support
                ShippingMethod errormethod = new ShippingMethod();

                ShippingMethodError err = new ShippingMethodError();
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.EndUser;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidShipToCountryCode;
                errormethod.Error = err;
                _upsShippingMethods.Add(errormethod);
            }

            //Save log as XML to file
            saveXMLlog(cart, shipFromAddress, cart.PackingLists.FirstOrDefault() ?? new PackingList(), _upsShippingMethods);

            return _upsShippingMethods;
        }

        /// <summary>
        /// this function is change no to ls<int>
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        protected List<float> getIntList(float no)
        {
            if (no <= maxWeight)
                return new List<float>() { no };

            int  i =(int) no / maxWeight;
            float n = no % maxWeight;
            List<float> ls = new List<float>();
            for (int m = 0; m < i; m++)
                ls.Add(maxWeight);
            if(n > 0)
                ls.Add(n);
            return ls;
        }

        /// <summary>
        /// this function is to get shippingmethod by weights and zones
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="zonelist"></param>
        /// <returns></returns>
        protected List<ShippingMethod> getEUUPS(float weight, List<EUUPSZone> zonelist)
        {
            if (zonelist == null || zonelist.Count < 0 )
                return new List<ShippingMethod>();

            float finalWeight;
            EUUPSZoneHelper helper = new EUUPSZoneHelper();
            List<ShippingMethod> ls = new List<ShippingMethod>();
            foreach (var zone in zonelist.OrderBy(x=>x.type))
            {
               
                var priceList = helper.getEUUPSPriceByZone(zone);
                if (priceList != null && priceList.Count > 0) // zone price 中有价格
                {
                    ShippingMethod _sm = new ShippingMethod();
                    _sm.ShippingMethodDescription = zone.MethodX;
                    if (_sm.ShippingMethodDescription == "TNT Economy")
                        finalWeight = weight * 1.25f; // volume weight定義 ( = L cm * W cm * H cm / 4000 for economy service  OR = L cm * W cm * H cm / 5000 for global service)
                    else
                        finalWeight = weight;

                    var price = priceList.FirstOrDefault(c => c.StartKG <= finalWeight && c.EndKG >= finalWeight);
                    if (price != null)
                        _sm.ShippingCostWithPublishedRate += (float) Math.Round(price.Price,2);

                    ls.Add(_sm);
                }
              
            }
            return ls;
        }



        /// <summary>
        /// This method will get shipping methods log as XML then save to C:\eStoreResources3C\Logs\Shipping.
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="shipFromAddress"></param>
        /// <param name="packingList"></param>
        /// <param name="shippingMethod"></param>
        private void saveXMLlog(Cart cart, Address shipFromAddress, PackingList packingList, List<ShippingMethod> shippingMethod)
        {
            XmlDocument doc = getXMLlog(cart, shipFromAddress, packingList, shippingMethod);

            //Save XML file to C:\eStoreResources3C\Logs\Shipping
            StringBuilder filePath = new StringBuilder();
            string folderName = "AEUUPS";
            DateTime now = DateTime.Now;
            int year = now.Year;
            int month = now.Month;
            string yearMonth = year.ToString() + "_" + month.ToString();
            filePath.Append(System.Configuration.ConfigurationManager.AppSettings.Get("Log_Path")).Append("/").Append("Shipping").Append("/").Append(yearMonth).Append("/").Append(folderName).Append("/");
            // filename is order number
            string filename = "";
            filename = cart.CartID + ".xml";

            //Check saving directory existent
            if (!Directory.Exists(@filePath.ToString()))
            {
                //Create default saving folder
                Directory.CreateDirectory(@filePath.ToString());
            }

            //Save
            doc.Save(filePath + filename);
        }
    }
}
