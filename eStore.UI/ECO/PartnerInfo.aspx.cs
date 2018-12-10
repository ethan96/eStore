using eStore.POCOS;
using eStore.POCOS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace eStore.UI.ECO
{
    //test
    public partial class PartnerInfo :  Presentation.eStoreBaseControls.eStoreBasePage
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PartnerInfoHelper helper = new PartnerInfoHelper();
            ddlCountries.Items.Insert(0, new ListItem("Select Country", "All"));
            int i = 1;
            var storeId = eStore.Presentation.eStoreContext.Current.Store.storeID;
            foreach (var country in helper.getAllPartnersInfoByStoreId(storeId).Select(p => p.Country).Distinct())
            {
                ddlCountries.Items.Insert(i, new ListItem(country, country));
                i++;
            }

            //check lat lng value, if null, try to get the value by address, then update to db
            //if (!IsPostBack)
            //{
            //    List<PartnersInfo> partnersInfoList = helper.getAllPartnersInfoByStoreId(storeId).Where(p => p.Latitude == null || p.Longitude == null).ToList();
            //    var requestCount = 1;
            //    foreach (var partner in partnersInfoList)
            //    {
            //        try
            //        {
            //            updateLatLng(partner);
            //        }
            //        catch { }

            //        if (requestCount == 10)
            //        {
            //            System.Threading.Thread.Sleep(1);
            //            requestCount = 0;
            //        }
            //        requestCount++;

            //    }
            //}
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static CurrentPagePartnerInfoViewModel RenderPartnerInfoByCountriesAndPageNum(string country, int pageNumber)
        {
            return PartnersInfoListByCurrentPage(country, pageNumber);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static CurrentPagePartnerInfoViewModel PageNumber_Changed(string country, int pageNumber)
        {
            return PartnersInfoListByCurrentPage(country, pageNumber);

        }

        private static CurrentPagePartnerInfoViewModel PartnersInfoListByCurrentPage(string country, int currentPage)
        {
            CurrentPagePartnerInfoViewModel currentPagePartnerInfoViewModel = new CurrentPagePartnerInfoViewModel();

            PartnerInfoHelper helper = new PartnerInfoHelper();
            List<PartnersInfo> partnersInfoList;
            var storeId = eStore.Presentation.eStoreContext.Current.Store.storeID;
            if (country == "All")
                partnersInfoList = helper.getAllPartnersInfoByStoreId(storeId);
            else
                partnersInfoList = helper.getPartnersInfoByCountryAndStoreId(country, storeId);


            
            var pageSize = 10; // set your page size, which is number of records per page
            var totalPage = partnersInfoList.Count()/pageSize + 1;

            var skip = pageSize * (currentPage - 1);

            var partialPartnersInfoList = partnersInfoList
                         .Skip(skip)
                         .Take(pageSize).ToList();

            currentPagePartnerInfoViewModel.PartnerInfoList = partialPartnersInfoList;
            currentPagePartnerInfoViewModel.PageNumber = totalPage;
            return currentPagePartnerInfoViewModel;
        }

        private static void updateLatLng(PartnersInfo part)
        {
                var fulladdress = part.Address + ", " + part.City + ", " + part.Country;
                var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false&?key=AIzaSyDKq4X_ZjRHYr38AWR__zQwCo5aQnjA7VM", Uri.EscapeDataString(fulladdress));

                var geocodeRequest = WebRequest.Create(requestUri);
                var geocodeResponse = geocodeRequest.GetResponse();
                if (geocodeResponse != null)
                {
                    var xdoc = XDocument.Load(geocodeResponse.GetResponseStream());

                    var geocodeResult = xdoc.Element("GeocodeResponse").Element("result");
                    if (geocodeResult != null)
                    {
                        var locationElement = geocodeResult.Element("geometry").Element("location");
                        decimal lat;
                        decimal lng;
                        if (decimal.TryParse(locationElement.Element("lat").Value, out lat))
                        {
                            part.Latitude = lat;
                            if (decimal.TryParse(locationElement.Element("lng").Value, out lng))
                            {
                                part.Longitude = lng;
                                part.save();
                            }
                        }
                    }
                }                              
                            
        }


    }

    public class CurrentPagePartnerInfoViewModel
    {
        public List<eStore.POCOS.PartnersInfo> PartnerInfoList { get;set;}

        public int PageNumber { get; set; }
    }
}