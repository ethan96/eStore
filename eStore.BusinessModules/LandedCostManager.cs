using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.BusinessModules.UPSLandedCostWS;
namespace eStore.BusinessModules
{
    public class LandedCostManager
    {

        public LandedCost getLandedCost(POCOS.Store store, POCOS.Cart cart)
        {
            if (!store.getBooleanSetting("EnableLandedCost") || store.ShipFromAddress.countryCodeX == cart.ShipToContact.countryCodeX)
                return null;
            try
            {
                LC lc = new LC();
                LandedCostRequest lcRequest = new LandedCostRequest();
                QueryRequestType queryRequest = new QueryRequestType();
                eStore.BusinessModules.UPSLandedCostWS.ShipmentType shipType = new eStore.BusinessModules.UPSLandedCostWS.ShipmentType();

                shipType.OriginCountryCode = store.ShipFromAddress.countryCodeX;
                shipType.OriginStateProvinceCode = store.ShipFromAddress.State;

                shipType.DestinationCountryCode = cart.ShipToContact.countryCodeX;
                shipType.DestinationStateProvinceCode = cart.ShipToContact.State;
                shipType.TransportationMode = "1";
                shipType.ResultCurrencyCode = store.defaultCurrency.CurrencyID;

                eStore.BusinessModules.UPSLandedCostWS.ChargesType freightChargeType = new eStore.BusinessModules.UPSLandedCostWS.ChargesType();
                freightChargeType.CurrencyCode = store.defaultCurrency.CurrencyID;
                freightChargeType.MonetaryValue = "0";  //??
                shipType.FreightCharges = freightChargeType;

                eStore.BusinessModules.UPSLandedCostWS.ChargesType additionalInsurance = new eStore.BusinessModules.UPSLandedCostWS.ChargesType();
                additionalInsurance.CurrencyCode = store.defaultCurrency.CurrencyID;
                additionalInsurance.MonetaryValue = "0";
                shipType.AdditionalInsurance = additionalInsurance;
                shipType.TariffCodeAlert = "1";

                shipType.Product = getProducts(store,cart).ToArray<ProductType>();
                shipType.ResultCurrencyCode = store.defaultCurrency.CurrencyID;
                queryRequest.Shipment = shipType;

                RequestTransportType requestTransType = new RequestTransportType();
                requestTransType.RequestAction = "LandedCost";
                lcRequest.Request = requestTransType;
                lcRequest.Item = queryRequest;


                AccessRequest accessRequest = new AccessRequest();
                accessRequest.UserId = "eStore.Advantech";
                accessRequest.Password = "eStoreUPS";
                accessRequest.AccessLicenseNumber = "CC640DFEAA3FFCD8";
                lc.AccessRequestValue = accessRequest;

                System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
                Console.WriteLine(lcRequest);
                LandedCostResponse landedCostResponse = lc.LCRequest(lcRequest);

                //Please note:
                //Below type casting/iteration/extraction of required data is for sample. Type, null and length checks are recommended.
                QueryResponseType queryResponse = (QueryResponseType)landedCostResponse.Item;
                String questionName = queryResponse.Shipment.Product[0].Question[0].Name;
                String questionText = queryResponse.Shipment.Product[0].Question[0].Text;

                string t = queryResponse.TransactionDigest;


                Console.WriteLine("The landedCostResponse : ");
                Console.WriteLine("Question name : " + questionName);
                Console.WriteLine("Question Text: " + questionText);
                LandedCost landedcost = ShipmentEstimate(queryResponse);

                return landedcost;
            }

            catch (System.Web.Services.Protocols.SoapException ex)
            {
                 eStore.Utilities.eStoreLoger.Error(ex.Message, "", "", "", ex);
                
                return null;
            }

            catch (Exception ex)
            {
                eStore.Utilities.eStoreLoger.Error(ex.Message, "", "", "", ex);
                //Console.WriteLine(ex.Message);
                return null;
            }

        }


        private ProductType[] getProducts(POCOS.Store store, POCOS.Cart cart)
        {

            try
            {
                List<ProductType> producttypes = new List<ProductType>();

                foreach (POCOS.CartItem ci in cart.CartItems)
                {

                    ProductType prodType = new ProductType();
                    prodType.ProductCountryCodeOfOrigin = store.ShipFromAddress.countryCodeX;
                    TariffInfoType teriffType = new TariffInfoType();

                    //from Part object
                    if (string.IsNullOrEmpty(ci.partX.HTZCode))
                        teriffType.TariffCode = "8471.50.20.000";
                    else
                        teriffType.TariffCode = ci.partX.HTZCode;

                    prodType.TariffInfo = teriffType;
                    eStore.BusinessModules.UPSLandedCostWS.ChargesType unitPrice = new eStore.BusinessModules.UPSLandedCostWS.ChargesType();
                    unitPrice.MonetaryValue = ci.AdjustedPrice.ToString();
                    unitPrice.CurrencyCode = store.defaultCurrency.CurrencyID;
                    prodType.UnitPrice = unitPrice;

                    ValueWithUnitsType vwt = new ValueWithUnitsType();
                    ValueWithUnitsTypeUnitOfMeasure unit = new ValueWithUnitsTypeUnitOfMeasure();
                    unit.UnitCode = "kg";
                    vwt.UnitOfMeasure = unit;
                    vwt.Value = "2";

                    prodType.Weight = vwt;
                    ValueWithUnitsType quantity = new ValueWithUnitsType();
                    quantity.Value = ci.Qty.ToString();
                    prodType.Quantity = quantity;

                    producttypes.Add(prodType);
                }
                return producttypes.ToArray<ProductType>();
            }

            catch (Exception e)
            {
                throw e;
            }


        }


        /// <summary>
        ///  This method will return landed cost, extract Duty, Tax , otherTax&Fees and Tax&Fees.
        /// </summary>
        /// <param name="queryResponse"></param>

        private LandedCost ShipmentEstimate(QueryResponseType queryResponse)
        {
            LandedCost landedcost = new LandedCost();

            LC lc = new LC();
            LandedCostRequest lcRequest = new LandedCostRequest();
            EstimateRequestType queryRequest = new EstimateRequestType();
            ShipmentAnswerType shipType = new ShipmentAnswerType();
            ProductAnswerType[] product = new ProductAnswerType[queryResponse.Shipment.Product.Length];

            //Set products information
            for (int i = 0; i < queryResponse.Shipment.Product.Length; i++)
            {
                product[i] = new ProductAnswerType();
                product[i].TariffCode = queryResponse.Shipment.Product[i].TariffCode;
            }
            shipType.Product = product;


            queryRequest.Shipment = shipType;
            queryRequest.TransactionDigest = queryResponse.TransactionDigest;
            RequestTransportType requestTransType = new RequestTransportType();
            requestTransType.RequestAction = "LandedCost";
            lcRequest.Request = requestTransType;
            lcRequest.Item = queryRequest;

            AccessRequest accessRequest = new AccessRequest();
            accessRequest.UserId = "eStore.Advantech";
            accessRequest.Password = "eStoreUPS";
            accessRequest.AccessLicenseNumber = "CC640DFEAA3FFCD8";
            lc.AccessRequestValue = accessRequest;

            System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            Console.WriteLine(lcRequest);
            LandedCostResponse landedCostResponse = lc.LCRequest(lcRequest);

            EstimateResponseType estimateResponse = (EstimateResponseType)landedCostResponse.Item;

            //Calcuate Total Tax, Duties, fees  for all products
            decimal duties = 0m;
            decimal vats = 0m;
            decimal OtherTaxFees = 0m;

            foreach (ProductEstimateType p in estimateResponse.ShipmentEstimate.ProductsCharges.Product)
            {
                decimal duty;
                decimal vat;
                decimal taxesfee;
                if (decimal.TryParse(p.Charges.Duties, out duty))
                    duties = duties + duty;

                if (decimal.TryParse(p.Charges.VAT, out vat))
                    vats = vats + vat;

                if (decimal.TryParse(p.Charges.TaxesAndFees, out taxesfee))
                    OtherTaxFees = OtherTaxFees + taxesfee;
            }

            landedcost.Duties = duties;
            landedcost.VAT = vats;
            landedcost.OtherTaxAndFees = OtherTaxFees;
            decimal taxfees = 0m;
            decimal.TryParse(estimateResponse.ShipmentEstimate.ShipmentCharges.TaxesAndFees, out taxfees);
            landedcost.TaxAndFees = taxfees;

            //Console.WriteLine("CurrrencyCode" + estimateResponse.ShipmentEstimate.CurrencyCode);
            Console.WriteLine("Duties" + duties);
            Console.WriteLine("VAT " + vats);
            Console.WriteLine("Other Tax&Fees" + OtherTaxFees);
            Console.WriteLine("TaxesAndFees" + estimateResponse.ShipmentEstimate.ShipmentCharges.TaxesAndFees);

            return landedcost;
        } 
    }
}
