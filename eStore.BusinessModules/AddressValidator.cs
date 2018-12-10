using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.BusinessModules.UPSAddressValidation;
using eStore.BusinessModules.FedExAddressValidation;

namespace eStore.BusinessModules
{
    public class AddressValidator
    {
        public AddressValidator()
        {
 
        }

        //快递类型
        public enum ValidatationProvider { UPS, Fedex }

        //验证 地址
        public ShippingAddressValidationResult validate(ShippingAddress address, ValidatationProvider provider, string confirmedBy, string site="eStore")
        {
            ShippingAddressValidationResult result = null;

            //Check is PO box address
            System.Text.RegularExpressions.Regex regPoBox = new System.Text.RegularExpressions.Regex(@"\b[P|p]?(OST|ost)?\.?\s*[O|o|0]?(ffice|FFICE)?\.?\s*[B|b][O|o|0]?[X|x]?\.?\s+[#]?(\d+)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
            try
            {
                bool isPoxBox = false;
                if (!string.IsNullOrEmpty(address.Street))
                    isPoxBox = regPoBox.IsMatch(address.Street);
                else if (!string.IsNullOrEmpty(address.Street2))
                    isPoxBox = regPoBox.IsMatch(address.Street2);

                string poboxmsg = "Sorry, PO BOXes are not acceptable in the address, please fill in another one. Thank you.";
                if (isPoxBox == true)
                {
                    result = new ShippingAddressValidationResult();
                    result.isValid = false;
                    result.message = poboxmsg;
                    result.Candidate = new List<ShippingAddress>();
                    result.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_POBox;
                    return result;
                }

                //Second shot
                regPoBox = new System.Text.RegularExpressions.Regex(@"[B|b][O|o|0]?[X|x]?[\W_]+?(\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
                if (!string.IsNullOrEmpty(address.Street))
                    isPoxBox = regPoBox.IsMatch(address.Street);
                else if (!string.IsNullOrEmpty(address.Street2))
                    isPoxBox = regPoBox.IsMatch(address.Street2);
                if (isPoxBox == true)
                {
                    result = new ShippingAddressValidationResult();
                    result.isValid = false;
                    result.message = poboxmsg;
                    result.Candidate = new List<ShippingAddress>();
                    result.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_POBox;
                    return result;
                }
            }
            catch (Exception ex)
            {
                Utilities.eStoreLoger.Error("Check address is po box exception. " + ex.Message);
            }

            bool isUPSToFedEx = false;
            //验证时,如果是UPS,但是国家不是USA 、Puerto Rico. 就走FedEx
            if (provider == ValidatationProvider.UPS)
            {
                if (address.Country != "US" && address.Country != "PR")
                {
                    provider = ValidatationProvider.Fedex;
                    isUPSToFedEx = true;
                }
            }

            AddressValidatationProvider avp = null;
            if (provider == ValidatationProvider.UPS)
                avp = new UPSAddressValidatation();
            else
                avp = new FedexAddressValidatation();

            avp.confirmedBy = confirmedBy;
            avp.Site = site;
            avp.isUPSToFedEx = isUPSToFedEx;
            result = avp.validate(address);

            if (isUPSToFedEx && result.isValid)
                result.message = "According to FedEx validation.";

            return result;
        }

        //确认地址是正确的.
        public bool confirmAsValidAddress(ShippingAddress address, ValidatationProvider provider, string confirmedBy, string site = "eStore")
        {
            bool result = false;

            AddressValidatationProvider avp = null;
            if (provider == ValidatationProvider.UPS)
                avp = new UPSAddressValidatation();
            else
                avp = new FedexAddressValidatation();

            avp.confirmedBy = confirmedBy;
            avp.Site = site;
            result = avp.confirmAsValidAddress(address);

            return result;
        }

        //地址 基本信息
        public class ShippingAddress
        {
            public ShippingAddress()
            {

            }

            //SAPCompany
            public ShippingAddress(POCOS.VSAPCompany sapContact)
            {
                ERPID = sapContact.CompanyID;
                Country = sapContact.countryCodeX;
                State = sapContact.Region;
                City = sapContact.City;
                Street = sapContact.Address;
                Street2 = sapContact.Address;
                PostalCode = sapContact.ZipCode;
            }

            //CartContact
            public ShippingAddress(POCOS.CartContact cartContact)
            {
                ERPID = cartContact.AddressID;
                Country = cartContact.countryCodeX;
                State = cartContact.State;
                City = cartContact.City;
                Street = cartContact.Address1;
                Street2 = cartContact.Address2;
                PostalCode = cartContact.ZipCode;
            }

            //AddressValidate
            public ShippingAddress(POCOS.AddressValidate validedAddress)
            {
                ERPID = validedAddress.ERPId;
                Country = validedAddress.Country;
                State = validedAddress.State;
                City = validedAddress.City;
                Street = validedAddress.Address1;
                Street2 = validedAddress.Address2;
                PostalCode = validedAddress.ZipCode;
            }

            #region 基本信息
            public string ERPID { get; set; }

            public string Street { get; set; }

            public string Street2 { get; set; }

            public string PostalCode { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string Country { get; set; }
            #endregion

            #region 重写 比较方法
            public override int GetHashCode()
            {
                return (ERPID + Street + Street2 + PostalCode + City + State + Country).GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is ShippingAddress)
                    return (ShippingAddress)obj == this;
                else
                    return false;
            }

            public static bool operator ==(ShippingAddress a, ShippingAddress b)
            {
                // If both are null, or both are same instance, return true.
                if (System.Object.ReferenceEquals(a, b))
                    return true;

                // If one is null, but not both, return false.
                if (((object)a == null) || ((object)b == null))
                    return false;

                // Return true if the fields match:
                return (a.ERPID == b.ERPID
                        && a.Street == b.Street
                        && a.Street2 == b.Street2
                        && a.PostalCode == b.PostalCode
                        && a.City == b.City
                        && a.State == b.State
                        && a.Country == b.Country);
            }

            public static bool operator !=(ShippingAddress a, ShippingAddress b)
            {
                return !(a == b);
            }
            #endregion


            public POCOS.AddressValidate getPrevalidatedAddress(string addressType)
            {
                //根据ship address生成一个address valiedate
                POCOS.AddressValidate validedAddress = new POCOS.AddressValidate();
                validedAddress.ERPId = ERPID;
                validedAddress.Country = Country;
                validedAddress.State = State;
                validedAddress.City = City;
                validedAddress.Address1 = Street;
                validedAddress.Address2 = Street2;
                validedAddress.ZipCode = PostalCode;

                validedAddress = new eStore.POCOS.DAL.AddressValidateHelper().getPrevalidatedAddress(validedAddress, addressType);
                return validedAddress;
            }
        }

        public class ShippingAddressValidationResult
        {
            public bool isValid { get; set; }

            public string message { get; set; }
            //申请
            public List<ShippingAddress> Candidate { get; set; }

            public POCOS.Store.TranslationKey TranslationKey { get; set; }

            public ShippingAddressValidationResult()
            {
                this.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Unknown;
            }
        }

        protected abstract class AddressValidatationProvider
        {
            //快递类型
            public ValidatationProvider provider { get; set; }

            public string confirmedBy { get; set; }

            public string Site { get; set; }
            //是否是ups 转 FedEx
            public bool isUPSToFedEx { get; set; }
            
            public abstract ShippingAddressValidationResult validate(ShippingAddress address);

            public virtual bool confirmAsValidAddress(ShippingAddress address,bool isValidate = true)
            {
                bool isSaveIt = false;//是否需要保存
                if (isValidate)//需要验证不.  validate里面有验证DB,这里就不需要验证了,直接保存
                {
                    POCOS.AddressValidate validedAddress = address.getPrevalidatedAddress(provider.ToString());
                    //DB中没有确认过Erp地址
                    if (validedAddress == null)
                        isSaveIt = true;
                }
                else
                    isSaveIt = true;
                
                if (isSaveIt)
                {
                    POCOS.AddressValidate newvalidedAddress = new POCOS.AddressValidate();
                    newvalidedAddress.Status = !isValidate;
                    newvalidedAddress.ERPId = address.ERPID;
                    newvalidedAddress.Country = address.Country;
                    newvalidedAddress.State = address.State;
                    newvalidedAddress.City = address.City;
                    newvalidedAddress.Address1 = address.Street;
                    newvalidedAddress.Address2 = address.Street2;
                    newvalidedAddress.ZipCode = address.PostalCode;
                    newvalidedAddress.AddressType = provider.ToString();
                    //标记 是那个站点,  是Confirm 二次确认的地址.
                    if (isValidate)
                        newvalidedAddress.ErrorMessage = Site + " Error:Confirm Address";
                    else
                    {
                        newvalidedAddress.ErrorMessage = Site;
                        if (this.isUPSToFedEx)
                            newvalidedAddress.ErrorMessage += " Error:According to FedEx validation.";
                    }
                    newvalidedAddress.UpdateDate = DateTime.Now;
                    newvalidedAddress.UpdateBy = confirmedBy;

                    if (newvalidedAddress.save() == 0)
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }

            public virtual ShippingAddressValidationResult validateErpId(ShippingAddress address)
            {
                ShippingAddressValidationResult result = new ShippingAddressValidationResult();
                if (address == null || string.IsNullOrEmpty(address.ERPID))
                {
                    result.isValid = false;
                    result.message = "Not a valid ERPID.";
                    result.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                }
                else
                    result.isValid = true;
                return result;
            }
        }

        //去本地DB验证地址
        protected class LocalAddressValidatation : AddressValidatationProvider
        {
            public LocalAddressValidatation(ValidatationProvider provider)
            {
                base.provider = provider;
            }

            public override ShippingAddressValidationResult validate(ShippingAddress address)
            {
                ShippingAddressValidationResult result = new ShippingAddressValidationResult();

                //根据ErpID去DB中 查找是否有确认的属性
                POCOS.AddressValidate validedAddress = address.getPrevalidatedAddress(provider.ToString());
                if (validedAddress == null)
                {
                    result.isValid = false;
                    result.message = "hasn't validated.";
                    result.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                }
                else
                {
                    result.isValid = true;
                    result.message = string.Empty;
                    result.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_CCRConfirmed;
                }
                return result;
            }
        }

        protected class UPSAddressValidatation : AddressValidatationProvider
        {
            public UPSAddressValidatation()
            {
                base.provider = ValidatationProvider.UPS;
            }

            public override ShippingAddressValidationResult validate(ShippingAddress address)
            {
                //验证ERPId
                ShippingAddressValidationResult upsresult = base.validateErpId(address);
                if (!upsresult.isValid)
                    return upsresult;

                //验证DB中是否验证过
                LocalAddressValidatation lav = new LocalAddressValidatation(base.provider);
                upsresult = lav.validate(address);
                if (upsresult.isValid)
                    return upsresult;
                else
                {
                    upsresult = new ShippingAddressValidationResult();
                    try
                    {
                        //Service
                        XAVService xavService = new XAVService();
                        //客服端
                        XAVRequest xavRequest = new XAVRequest();
                        //安全机制
                        UPSSecurity upsSecurity = new UPSSecurity();
                        //访问口令
                        UPSSecurityServiceAccessToken upsAccessToken = new UPSSecurityServiceAccessToken();
                        //帐号 密码
                        UPSSecurityUsernameToken upsUserToken = new UPSSecurityUsernameToken();

                        //UPS帐号
                        upsAccessToken.AccessLicenseNumber = "CC640DFEAA3FFCD8";
                        upsSecurity.ServiceAccessToken = upsAccessToken;

                        upsUserToken.Username = "eStore.Advantech";
                        upsUserToken.Password = "eStoreUPS";
                        upsSecurity.UsernameToken = upsUserToken;

                        xavService.UPSSecurityValue = upsSecurity;

                        //create 客服端
                        string[] requestOption = new string[] { "1" };
                        RequestType userRequest = new RequestType();
                        userRequest.RequestOption = requestOption;
                        xavRequest.Request = userRequest;

                        string[] addressLine = new string[] { "", "" };
                        addressLine[0] = address.Street;
                        addressLine[1] = address.Street2;

                        AddressKeyFormatType userAddressKey = new AddressKeyFormatType();
                        userAddressKey.AddressLine = addressLine;
                        userAddressKey.PoliticalDivision1 = address.State;
                        userAddressKey.PoliticalDivision2 = address.City;
                        userAddressKey.PostcodePrimaryLow = address.PostalCode;
                        userAddressKey.CountryCode = address.Country;
                        xavRequest.AddressKeyFormat = userAddressKey;

                        //发送请求
                        XAVResponse xavResponse = xavService.ProcessXAV(xavRequest);
                        if (xavResponse.Response.ResponseStatus.Code == "1")
                        {

                            if (xavResponse.ItemElementName == ItemChoiceType.ValidAddressIndicator)
                                upsresult.isValid = true;
                            else if (xavResponse.ItemElementName == ItemChoiceType.AmbiguousAddressIndicator)
                            {
                                //模糊的也算过.  
                                upsresult.isValid = false;
                                upsresult.message = "It's Ambiguous Address Indicator.";
                                upsresult.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                            }
                            else
                            {
                                upsresult.isValid = false;
                                upsresult.message = "It's invalid address.";
                                upsresult.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                            }

                            if (xavResponse.Candidate != null && xavResponse.Candidate.Length > 0 && xavResponse.Candidate[0].AddressKeyFormat.AddressLine.Length > 0)
                            {
                                List<AddressValidator.ShippingAddress> candidate = new List<ShippingAddress>();
                                foreach (var c in xavResponse.Candidate)
                                {
                                    if (c.AddressKeyFormat.AddressLine.Length > 0)
                                    {
                                        AddressValidator.ShippingAddress sa = new AddressValidator.ShippingAddress();
                                        userAddressKey = xavResponse.Candidate[0].AddressKeyFormat;
                                        sa.Street = userAddressKey.AddressLine[0];
                                        sa.State = userAddressKey.PoliticalDivision1;
                                        sa.City = userAddressKey.PoliticalDivision2;
                                        sa.PostalCode = userAddressKey.PostcodePrimaryLow;
                                        sa.Country = userAddressKey.CountryCode;
                                        candidate.Add(sa);
                                    }
                                }
                                //测试遇到返回15条相同数据
                                upsresult.Candidate = candidate.Distinct().ToList();
                            }

                            if (upsresult.isValid)
                            {
                                //保存数据到DB
                                if (!base.confirmAsValidAddress(address, false))
                                {
                                    //如果保存失败
                                    upsresult.isValid = false;
                                    upsresult.message = "Save failed.";
                                    upsresult.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                                }
                            }
                        }
                        else
                        {
                            upsresult.isValid = false;
                            upsresult.message = xavResponse.Response.ResponseStatus.Description;
                            upsresult.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                        }
                    }
                    catch (Exception ex)
                    {
                        upsresult.isValid = false;
                        upsresult.message = ex.Message;
                        upsresult.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                    }
                    return upsresult;
                }
            }

            public override bool confirmAsValidAddress(ShippingAddress address, bool isValidate = true)
            {
                return base.confirmAsValidAddress(address, isValidate);
            }
        }

        protected class FedexAddressValidatation : AddressValidatationProvider
        {
            public FedexAddressValidatation()
            {
                base.provider = ValidatationProvider.Fedex;
            }

            public override ShippingAddressValidationResult validate(ShippingAddress address)
            {
                //验证ERPId
                ShippingAddressValidationResult fedexresult = base.validateErpId(address);
                if (!fedexresult.isValid)
                    return fedexresult;
                //验证DB中是否验证过
                LocalAddressValidatation lav = new LocalAddressValidatation(base.provider);
                fedexresult = lav.validate(address);
                if (fedexresult.isValid)
                    return fedexresult;
                else
                {
                    fedexresult = new ShippingAddressValidationResult();
                    try
                    {
                        AddressValidationRequest request = CreateAddressValidationRequest(address);
                        AddressValidationService service = new AddressValidationService();
                        AddressValidationReply reply = service.addressValidation(request);
                        if (reply != null)
                        {
                            if (reply.HighestSeverity != NotificationSeverityType.ERROR && reply.HighestSeverity != NotificationSeverityType.FAILURE)
                            {
                                if (reply.AddressResults.Length > 0 && reply.AddressResults[0].ProposedAddressDetails.Length > 0)
                                {
                                    int Score = !string.IsNullOrEmpty(reply.AddressResults[0].ProposedAddressDetails[0].Score) ? int.Parse(reply.AddressResults[0].ProposedAddressDetails[0].Score) : 0;
                                    if (Score == 100)
                                        fedexresult.isValid = true;
                                    else if (Score > 0 && Score < 100)
                                    {
                                        //模糊的也算过.  
                                        fedexresult.isValid = false;
                                        fedexresult.message = "It's Ambiguous Address Indicator.";
                                        fedexresult.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                                    }
                                    else
                                    {
                                        fedexresult.isValid = false;
                                        fedexresult.message = "It's invalid address.";
                                        fedexresult.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                                    }

                                    List<AddressValidator.ShippingAddress> candidate = new List<ShippingAddress>();
                                    AddressValidator.ShippingAddress sa = new AddressValidator.ShippingAddress();
                                    sa.Street = reply.AddressResults[0].ProposedAddressDetails[0].Address.StreetLines[0];
                                    sa.PostalCode = reply.AddressResults[0].ProposedAddressDetails[0].Address.PostalCode;
                                    sa.City = reply.AddressResults[0].ProposedAddressDetails[0].Address.City;
                                    sa.State = reply.AddressResults[0].ProposedAddressDetails[0].Address.StateOrProvinceCode;
                                    sa.Country = reply.AddressResults[0].ProposedAddressDetails[0].Address.CountryCode;

                                    candidate.Add(sa);
                                    fedexresult.Candidate = candidate;

                                    if (fedexresult.isValid)
                                    {
                                        //保存数据到DB
                                        if (!base.confirmAsValidAddress(address, false))
                                        {
                                            //如果保存失败
                                            fedexresult.isValid = false;
                                            fedexresult.message = "Save failed.";
                                            fedexresult.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                                        }
                                    }
                                }
                                else
                                {
                                    fedexresult.isValid = false;
                                    fedexresult.message = "It's invalid address.";
                                    fedexresult.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                                }
                            }
                            else
                            {
                                fedexresult.isValid = false;
                                if (reply.Notifications.Any())
                                    fedexresult.message = string.Join("<br />", reply.Notifications.Select(x => x.Message).ToArray());
                                fedexresult.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                            }
                        }

                    }
                    catch (System.Web.Services.Protocols.SoapException ex)
                    {
                        fedexresult.isValid = false;
                        fedexresult.message = ex.Detail.InnerText;
                        fedexresult.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                    }
                    catch (Exception ex)
                    {
                        fedexresult.isValid = false;
                        fedexresult.message = ex.Message;
                        fedexresult.TranslationKey = POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Invalid;
                    }
                    return fedexresult;
                }
            }

            public override bool confirmAsValidAddress(ShippingAddress address, bool isValidate = true)
            {
                return base.confirmAsValidAddress(address, isValidate);
            }

            private AddressValidationRequest CreateAddressValidationRequest(ShippingAddress address)
            {
                AddressValidationRequest request = new AddressValidationRequest();
                request.WebAuthenticationDetail = new WebAuthenticationDetail();
                request.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();
                request.WebAuthenticationDetail.UserCredential.Key = "usvAyuZI2Hbz2QFv";
                request.WebAuthenticationDetail.UserCredential.Password = "YmqDX8cnYSMdMEEg0AGyQhKNe";

                request.ClientDetail = new ClientDetail();
                request.ClientDetail.AccountNumber = "229182811";
                request.ClientDetail.MeterNumber = "102086661";

                request.TransactionDetail = new TransactionDetail();
                request.TransactionDetail.CustomerTransactionId = "Address Validation";

                request.Version = new VersionId();
                request.RequestTimestamp = DateTime.Now;

                SetOptions(request);
                SetAddress(request, address);
                return request;
            }

            private void SetOptions(AddressValidationRequest request)
            {
                request.Options = new AddressValidationOptions();
                request.Options.CheckResidentialStatus = true;
                request.Options.MaximumNumberOfMatches = "5";
                request.Options.StreetAccuracy = AddressValidationAccuracyType.LOOSE;
                request.Options.DirectionalAccuracy = AddressValidationAccuracyType.LOOSE;
                request.Options.CompanyNameAccuracy = AddressValidationAccuracyType.LOOSE;
                request.Options.ConvertToUpperCase = true;
                request.Options.RecognizeAlternateCityNames = true;
                request.Options.ReturnParsedElements = true;
            }

            private void SetAddress(AddressValidationRequest request, ShippingAddress address)
            {
                request.AddressesToValidate = new AddressToValidate[] { new AddressToValidate() };
                request.AddressesToValidate[0].AddressId = address.ERPID;
                request.AddressesToValidate[0].Address = new Address();
                request.AddressesToValidate[0].Address.StreetLines = new string[] { "", "" };
                request.AddressesToValidate[0].Address.StreetLines[0] = address.Street;
                request.AddressesToValidate[0].Address.StreetLines[1] = address.Street2;
                request.AddressesToValidate[0].Address.PostalCode = address.PostalCode;
                request.AddressesToValidate[0].Address.City = address.City;
                request.AddressesToValidate[0].Address.StateOrProvinceCode = address.State;
                request.AddressesToValidate[0].Address.CountryCode = address.Country;
            }
        }
    }
}
