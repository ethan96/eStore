using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.IO;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules
{
    class AddressValidation
    {
        public enum ValidateType {Zipcode,City };

        private bool _validationResult;
        public bool ValidationResult
        {
            get { return _validationResult; }
        }

        private string _validationResultDescription = null;
        public string ValidationResultDescription
        {
            get { return _validationResultDescription; }
        }


        private string _zipCode = null;
        private string _city = null;
        private string _state = null;
        private string _country = null;
        private string _upsHTTP = "HTTPS://www.ups.com/ups.app/xml/AV";

        // Constructor
        public AddressValidation(eStore.POCOS.Address addr, AddressValidation.ValidateType type)
        {
            _zipCode = addr.ZipCode;
            _city = addr.City;
            _state = addr.State;
            _country = addr.Country;
            if (_zipCode.Length == 5 && _country == "US")
                validateAddress(type);
            else if (_zipCode.Length != 5 && _country == "US")
            {
                _validationResult = false;
                _validationResultDescription = "Zip code format is wrong.";
            }
            // it can only validate usa.
            else
            {
                _validationResult = false;
                _validationResultDescription = "The address validation is only for US.";
            }
        }

        private void validateAddress(AddressValidation.ValidateType type)
        {
            string _responseString = "";
            XmlDocument _xmlAccessRequest = getAccessRequest();
            XmlDocument _xmlValidationRequest;

            switch (type.ToString())
            { 
                case "Zipcode":
                    _xmlValidationRequest = getXMLRequest(_zipCode, "", "", _country);
                    break;
                
                case "City":
                    _xmlValidationRequest = getXMLRequest("", _city, "", _country);
                    break;

                default:
                    _xmlValidationRequest = getXMLRequest("", "", "", "");
                    break;
            }
 
            //validateAddressByUPS(_upsHTTP, _xmlAccessRequest, _xmlValidationRequest);
            _responseString = UPSRequest(_upsHTTP, _xmlAccessRequest, _xmlValidationRequest);
            processXmlResult(_responseString);
        }

        private XmlDocument getAccessRequest()
        {
            string shippingcariername = "UPS_US";       //Using US account
            ShippingCarier sc = ShippingCarierHelper.getShippingCarrier(shippingcariername);
            XmlDocument xmlAccessRequest = new XmlDocument();
            XmlDeclaration xmlARDeclaration = xmlAccessRequest.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlAccessRequest.InsertBefore(xmlARDeclaration, xmlAccessRequest.DocumentElement);
            XmlElement accessRequest = xmlAccessRequest.CreateElement("AccessRequest");
            accessRequest.SetAttribute("xml:lang", "en-us");
            xmlAccessRequest.AppendChild(accessRequest);
            XmlElement accessLicenseNumber = xmlAccessRequest.CreateElement("AccessLicenseNumber");
            accessLicenseNumber.InnerText = sc.AccessKey;
            accessRequest.AppendChild(accessLicenseNumber);
            XmlElement userId = xmlAccessRequest.CreateElement("UserId");
            userId.InnerText = sc.ShipperAccount;
            accessRequest.AppendChild(userId);
            XmlElement password = xmlAccessRequest.CreateElement("Password");
            password.InnerText = sc.Password;
            accessRequest.AppendChild(password);
            return xmlAccessRequest;
        }

        private XmlDocument getXMLRequest(string zipcode, string city, string state, string country)
        {
            // Address Validation Request
            XmlDocument _xmlRequest = new XmlDocument();

            XmlDeclaration _xmlRDeclaration = _xmlRequest.CreateXmlDeclaration("1.0", "utf-8", null);
            _xmlRequest.InsertBefore(_xmlRDeclaration, _xmlRequest.DocumentElement);
            XmlElement _addressValidationRequest = _xmlRequest.CreateElement("AddressValidationRequest");
            _addressValidationRequest.SetAttribute("xml:lang", "en-us");
            _xmlRequest.AppendChild(_addressValidationRequest);

            XmlElement _request = _xmlRequest.CreateElement("Request");
            _addressValidationRequest.AppendChild(_request);

            XmlElement _transactionReference = _xmlRequest.CreateElement("TransactionReference");
            _request.AppendChild(_transactionReference);

            XmlElement _customerContext = _xmlRequest.CreateElement("CustomerContext");
            _customerContext.InnerText = "Customer Data";
            _transactionReference.AppendChild(_customerContext);

            XmlElement _xpciVersion = _xmlRequest.CreateElement("XpciVersion");
            _xpciVersion.InnerText = "1.0001";
            _transactionReference.AppendChild(_xpciVersion);

            XmlElement _requestAction = _xmlRequest.CreateElement("RequestAction");
            _requestAction.InnerText = "AV";
            _request.AppendChild(_requestAction);

            //XmlElement _requestOption = _xmlRequest.CreateElement("RequestOption");
            //_requestOption.InnerText = "1"; //
            //_request.AppendChild(_requestOption);

            XmlElement _address = _xmlRequest.CreateElement("Address");
            _addressValidationRequest.AppendChild(_address);

            XmlElement _city = _xmlRequest.CreateElement("City");
            _city.InnerText = city;
            _address.AppendChild(_city);

            XmlElement _stateProvinceCode = _xmlRequest.CreateElement("StateProvinceCode");
            _stateProvinceCode.InnerText = state;
            _address.AppendChild(_stateProvinceCode);

            XmlElement _countryCode = _xmlRequest.CreateElement("CountryCode");
            _countryCode.InnerText = country;
            _address.AppendChild(_countryCode);

            XmlElement _zipCode = _xmlRequest.CreateElement("PostalCode");
            _zipCode.InnerText = zipcode;
            _address.AppendChild(_zipCode);

            return _xmlRequest;
        }

        private string UPSRequest(string url, XmlDocument access, XmlDocument request)
        {
            string result = "";

            ASCIIEncoding encodedData = new ASCIIEncoding();
            byte[] byteArray = encodedData.GetBytes(access.InnerXml + request.InnerXml);

            // open up da site
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.Method = "POST";
            wr.KeepAlive = false;
            wr.UserAgent = "Advantech";
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.ContentLength = byteArray.Length;
            try
            {
                // send xml data
                Stream SendStream = wr.GetRequestStream();
                SendStream.Write(byteArray, 0, byteArray.Length);
                SendStream.Close();

                // get da response
                HttpWebResponse WebResp = (HttpWebResponse)wr.GetResponse();
                using (StreamReader sr = new StreamReader(WebResp.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                    sr.Close();
                }


                WebResp.Close();
            }
            catch (Exception ex)
            {
                // Unhandle exception occure
                result = ex.Message;
            }

            return result;
        }

        //public void validateAddressByUPS(string url, XmlDocument access, XmlDocument request)
        //{
        //    //AddressValidationResponse result = new AddressValidationResponse();
        //    ASCIIEncoding encodedData = new ASCIIEncoding();
        //    byte[] byteArray = encodedData.GetBytes(access.InnerXml + request.InnerXml);

        //    HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
        //    wr.Method = "POST";
        //    wr.KeepAlive = false;
        //    wr.UserAgent = "Advantech";
        //    wr.ContentType = "application/x-www-form-urlencoded";
        //    wr.ContentLength = byteArray.Length;
        //    try
        //    {
        //        // send xml data
        //        Stream SendStream = wr.GetRequestStream();
        //        SendStream.Write(byteArray, 0, byteArray.Length);
        //        SendStream.Close();

        //        // get  response
        //        HttpWebResponse WebResp = (HttpWebResponse)wr.GetResponse();
        //        Stream _receiveStream = WebResp.GetResponseStream();
                
        //        Encoding _encode = System.Text.Encoding.GetEncoding("utf-8");
        //        StreamReader _readStream = new StreamReader(_receiveStream, _encode);
        //        //TextReader reader = _readStream;
        //        string _stringResult = _readStream.ReadToEnd();
                
        //        //StringReader sr = new StringReader(_stringResult);

        //        XmlSerializer serializer = new XmlSerializer(typeof(AddressValidationResponse));
                
        //        XmlReader reader = XmlReader.Create(new StringReader(_stringResult));
        //        AddressValidationResponse address = (AddressValidationResponse)serializer.Deserialize(reader);
        //        _readStream.Close();

        //        WebResp.Close();
        //        //return _xmlWriter;

        //        Console.WriteLine("ResponseStatusCode: " + address.Response.ResponseStatusCode);
        //        Console.WriteLine("ResponseStatusDescription: " + address.Response.ResponseStatusDescription);
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //}

        private void processXmlResult(string responseResult)
        {
            string str1 = "";
            string str2 = "";
            int foundStart = 0;
            int foundEnd = 0;
            if (string.IsNullOrEmpty(responseResult))
            { }
            else
            { 
                if(responseResult.Contains("Success"))
                {
                    str2 = "Success";
                    _validationResult = true;
                    _validationResultDescription = "Validated successful";
                }
                else if(responseResult.Contains("Failure"))
                {
                    foundStart = responseResult.IndexOf("<ErrorDescription>");
                    str1 = responseResult.Substring(foundStart+18);
                    foundEnd = str1.IndexOf("</ErrorDescription>");
                    str2 = str1.Remove(foundEnd);
                    _validationResult = false;
                    _validationResultDescription = str2;
                }
            }
           
        }
    }


    





    //[XmlRoot(ElementName = "AddressValidationResponse")]
    [XmlRootAttribute("AddressValidationResponse")]
    public class AddressValidationResponse
    {
        //[XmlElement(ElementName = "Response")]
        [XmlElement("Response")]
        public Response Response
        {
            get;
            set;
        }

        
        //public List<AddressValidationResult> AddressValidationResult
        //{
        //    get;
        //    set;
        //}
    }


    public class Response
    {
        
        //[XmlElement(ElementName = "TransactionReference")]
        [XmlElement("TransactionReference")]
        public TransactionReference TransactionReference
        {
            get;
            set;
        }

        [XmlAttribute("ResponseStatusCode")]
        public string ResponseStatusCode
        {
            get;
            set;
        }

        [XmlAttribute("ResponseStatusDescription")]
        public string ResponseStatusDescription
        {
            get;
            set;
        }
    }

    public class TransactionReference
    {
        [XmlAttribute("CustomerContext")]
        public string CustomerContext
        {
            get;
            set;
        }

        [XmlAttribute("XpciVersion")]
        public string XpciVersion
        {
            get;
            set;
        }
    }

    //[XmlElement(ElementName = "AddressValidationResult")]
    //public class AddressValidationResult
    //{
    //    [XmlAttribute("Rank")]
    //    public string Rank
    //    {
    //        get;
    //        set;
    //    }

    //    [XmlAttribute("Quality")]
    //    public string Quality
    //    {
    //        get;
    //        set;
    //    }

    //    [XmlElement(ElementName="Address")]
    //    public UPSAddress Address
    //    {
    //        get;
    //        set;
    //    }

    //    [XmlAttribute("PostalCodeLowEnd")]
    //    public string PostalCodeLowEnd
    //    {
    //        get;
    //        set;
    //    }

    //    [XmlAttribute("PostalCodeHighEnd")]
    //    public string PostalCodeHighEnd
    //    {
    //        get;
    //        set;
    //    }
    //}

    //public class UPSAddress
    //{
    //    [XmlAttribute("City")]
    //    public string City
    //    {
    //        get;
    //        set;
    //    }

    //    [XmlAttribute("StateProvinceCode")]
    //    public string StateProvinceCode
    //    {
    //        get;
    //        set;
    //    }
    //}
}
