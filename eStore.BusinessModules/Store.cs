using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Globalization;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.POCOS.PocoX;
using eStore.BusinessModules.SSO.Advantech;
using eStore.Utilities;
using eStore.BusinessModules.Templates;
using eStore.BusinessModules.TaxService;
using eStore.BusinessModules.SiebelWS4EMKT;
using eStore.BusinessModules.Task;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eStore.BusinessModules
{
    public class Store
    {
        private eStore.POCOS.Store _profile;
        private ShippingManager _shippingManager = null;
        private MembershipWebservice _sso = null;
        private String _companyIdPrefix = null;
        private PaymentManager _paymentManager = null;
        private Dictionary<String, TimeZoneInfo> timezones = new Dictionary<String, TimeZoneInfo>();
        private Dictionary<String, CultureInfo> locales = new Dictionary<string, CultureInfo>();
        private List<ShippingCarier> scaries = new List<ShippingCarier>();
        private List<RewardActivity> _rewardActivities = null;
        public string HeaderMenu { get; set; }
        public Dictionary<string, string> Chats = new Dictionary<string, string>();        

        /// <summary>
        /// constructor with storeId specified
        /// </summary>
        /// <param name="storeId"></param>
        public Store(eStore.POCOS.Store profile)
        {
            _profile = profile;
            //缺少StoreMgt中的静态变量初始化，需要改进
        }

        //attributes

        //properties
        /// <summary>
        /// To retrieve store by store ID
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns>
        ///     Store instance if found
        ///     null if not exist
        /// </returns>
        public String storeID
        {
            get { return _profile.StoreID; }
        }

        public eStore.POCOS.Store profile
        {
            get { return _profile; }
        }

        /// <summary>
        /// This is a safe property which garantee the return shipping manager won't be null
        /// </summary>
        public ShippingManager shippingManager
        {
            get
            {
                if (_shippingManager == null)
                {
                    //create a new shipping manager to get available shipping methods
                    _shippingManager = new ShippingManager(this.profile);
                }

                return _shippingManager;
            }
        }

        public ICollection<ShippingCarier> ShippingCariers
        {
            get
            {

                foreach (StoreCarier s in _profile.StoreCariers)
                {
                    scaries.Add(s.ShippingCarier);

                }
                return scaries;
            }
        }

        public Address shipFromAddress
        {
            get { return _profile.ShipFromAddress; }
        }

        /// <summary>
        /// Return SSO implementation
        /// </summary>
        public MembershipWebservice sso
        {
            get
            {
                if (_sso == null)
                    _sso = new MembershipWebservice();

                return _sso;
            }
        }

        public List<RewardActivity> getRewardActivities(MiniSite minisite = null)
        {
            //需要加入对minisite和country的筛选
            //对 getBooleanSetting 重构 如eStore.Presentation中的function
            if (_rewardActivities == null)
                _rewardActivities = (new RewardActivityHelper()).getActivity_RewardActivityByStoreid(this.storeID);
            return _rewardActivities;
        }

        //methods
        /// <summary>
        /// This method will only return Product.  If the caller inputs Part only ID, it'll return null
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public Product getProduct(String productId, bool includeInvalidStatus = true)
        {
            Part part = new PartHelper().getPart(productId, _profile, includeInvalidStatus);
            if (part is Product)
            {
                //find and associate campaign strategy (promotion) to product
                Product product = (Product)part;

                return product;
            }
            else
                return null;
        }

        public bool isPartNumber(string PartNumber, string storeid)
        {
            Part part = new PartHelper().getPart(PartNumber, storeid, false);
            if (part is Product && part.isOrderable())
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// This method returns part instance available in eStore.  The part can be part-only, product or CTOS
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        public Part getPart(String partId, bool includeInvalidStatus = false)
        {
            return new PartHelper().getPart(partId, _profile, includeInvalidStatus);
        }

        public Part getPart(String partId, string storeId, bool includeInvalidStatus = false)
        {
            return new PartHelper().getPart(partId, storeId, includeInvalidStatus);
        }

        /// <summary>
        /// get sapproduct
        /// </summary>
        /// <param name="partno"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public SAPProduct getSAPProduct(string partno, eStore.POCOS.Store store = null)
        {
            if (store == null)
                store = this.profile;
            return new PartHelper().getSAPProduct(partno, store);
        }
        //返回sapproduct list
        public List<SAPProduct> getSAPProductList(List<string> partNoList)
        {
            return new PartHelper().prefetchSAPProductList(this.profile, partNoList);
        }

        /// <summary>
        /// this method returns part list
        /// </summary>
        /// <param name="partids">sproduct ids like 'a,b,c'</param>
        /// <returns></returns>
        public List<Part> getPartList(string partids)
        {
            return new PartHelper().prefetchPartList(this.profile.StoreID, partids);
        }

        public List<Product> getProductList(string partids)
        {
            var ls = getPartList(partids);
            List<Product> ll = new List<Product>();
            if (ls.Any())
            {
                foreach (var c in ls)
                {
                    if (c is Product && (c as Product).isOrderable())
                        ll.Add(c as Product);
                }
            }
            return ll;
        }

        /// <summary>
        /// This method returns product instance available in eStore by displayname
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        public Part getPartbyDisplayName(String partId)
        {
            return new PartHelper().getPartbyDisplayName(partId, _profile.StoreID);
        }

        /// <summary>
        /// This method is to retrieve user account
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User getUser(String userId)
        {
            User user = (new UserHelper()).getUserbyID(userId);

            if (user == null)   //not existing in eStore DB
            {
                user = new User();
                user.UserID = userId;
                user.newUser = true;
                user.store = this.profile;
                user.CreatedDate = DateTime.Now;
            }
            else
            {
                user.newUser = false;
                user.store = this.profile;
            }

            return user;
        }

        /// <summary>
        /// user是否已经被注册
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckUser(string userId)
        {
            return (new UserHelper()).getSimpleUserbyID(userId) == null;
        }

        /// <summary>
        /// if tax provider is fixed rate and haven't set taxconfig for the store, then return ture, UI and mail content shouldn't display tax area
        /// </summary>
        public Boolean hasTaxCalculator
        {
            get
            {
                if (this.profile.TaxProvider == "FixRate" && this.profile.TaxConfigs.Any(x => x.StoreID == this.storeID) == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// hasTaxCalculator but return 0
        /// </summary>
        /// <returns></returns>
        public string getZeroTaxDispayString()
        {
            return this.profile.getStringSetting("ZeroTaxDispayString");
        }

        /// <summary>
        /// offerShippingService but return 0
        /// </summary>
        /// <returns></returns>
        public string getZeroFreightDispayString()
        {
            return this.profile.getStringSetting("ZeroFreightDispayString");
        }


        /// <summary>
        /// This property indicates whether current store provides shipping service online or not.
        /// </summary>
        public Boolean offerShippingService
        {
            get { return this.shippingManager.offerShippingService; }
        }

        public Boolean onlyLocalFixRate
        {
            get { return this.shippingManager.onlyLocalFixRate; }
        }

        /// <summary>
        /// This property indicates whether current store provides drop shipping service online or not.
        /// </summary>
        public Boolean offerDropShipmentService
        {
            get { return this.shippingManager.offerDropShipmentService; }
        }

        /// <summary>
        /// This property is to mark store is support order no shipping mothed or not
        /// if support and has no shipping mothed will send a ConfirmdButNeedFreightReview mail fx: aeu store
        /// </summary>
        public bool offerNoShippingMothed
        {
            get
            {
                bool result = true;
                if (bool.TryParse(profile.getStringSetting("ProceedWithProblematicFreight"), out result))
                    return result;
                return true;
            }
        }


        //*** this method is subjected to be changed after ShippingManager implement getAvailableShippingMethods funciton
        public List<ShippingMethod> getAvailableShippingMethods(Cart cart, Boolean forDropOff = false)
        {
            List<ShippingMethod> methods = new List<ShippingMethod>();
            if (cart.isPStoreOrder())
            {
                // get paps shipping methods -- eric
                var standardShippingMethods = new List<ShippingMethod>();
                var standardShippingInstructions = new List<ShippingInstruction>();
                // TODO: create getStandardShippingMethods to return Standard Shipping Methods
                var ePAPSShippingService = new ePAPSShippingService();
                var response = ePAPSShippingService.getStandardShippingMethods(profile, cart);
                if (response.Status == "OK")
                {
                    standardShippingMethods = response.ShippingMethods;
                    standardShippingInstructions = response.ShippingInstructions;
                    standardShippingMethods[0].ShippingMethodDescription = standardShippingMethods[0].ServiceCode;
                    methods.Add(standardShippingMethods[0]);
                    // OM needs to transfer standard shipping instructions to SAP whenever PAPS Standard Shipping Method chosen
                }
                var baseMethods = shippingManager.getAvailableShippingMethods(cart, forDropOff);
                methods.AddRange(baseMethods);
            }
            else
            {
                methods = shippingManager.getAvailableShippingMethods(cart, forDropOff);

                Cart freeGroupShipmentCart = cart.getFreeGroupShipmentEligibleCartItems();
                if (freeGroupShipmentCart != null)
                {
                    if (freeGroupShipmentCart.cartItemsX.Count() == cart.cartItemsX.Count())
                    {
                        //find ground shipment methods
                        foreach (ShippingMethod m in methods)
                        {
                            if (m.ServiceCode == "FEDEX_GROUND" || m.ServiceCode == "03" /*UPS ground*/)
                            {
                                m.Discount = m.ShippingCostWithPublishedRate;
                            }
                        }

                    }
                    else
                    {
                        List<ShippingMethod> freeShipmentApplicableMethods = shippingManager.getAvailableShippingMethods(freeGroupShipmentCart, forDropOff);
                        //find ground shipment methods
                        foreach (ShippingMethod m in methods)
                        {
                            if (m.ServiceCode == "FEDEX_GROUND" || m.ServiceCode == "03" /*UPS ground*/)
                            {
                                //find ground shipment methods in freeGroupShipmentApplicableMethods
                                ShippingMethod mc = freeShipmentApplicableMethods.FirstOrDefault(x => x.ServiceCode == m.ServiceCode);
                                if (mc != null)
                                {
                                    //if disount > actual value then use actual
                                    m.Discount = m.ShippingCostWithPublishedRate < mc.ShippingCostWithPublishedRate ? m.ShippingCostWithPublishedRate : mc.ShippingCostWithPublishedRate;
                                }
                            }
                        }
                    }

                }
            }
            return methods;
        }

        /// <summary>
        /// This method is for application to authenticate user and retrieve user account and profile. It returns user profile
        /// after successfully authenticated by SSO service providers.  Even this method return User profile, it doesn't mean user
        /// has complete required registration information.  Caller shall call user.hasCompleteRegistration() method to confirm user's
        /// registration status before move on.  Some early registered user doesn't fill up complete registration
        /// information due to less restrictions were applied.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="userHostIP"></param>
        /// <returns></returns>
        public User login(String userId, String password, String userHostIP)
        {
            User user = null;

            try
            {
                //Perform SSO authentication
                String authKey = SSOAuthenticate(userId, password, userHostIP);
                if (String.IsNullOrWhiteSpace(authKey))    //authentication fails
                {
                    //login sign in failure
                    eStoreLoger.Warn("User signs in failure", userId, password, userHostIP);
                    return null;
                }

                user = syncSSOUserProfile(userId, authKey);
                checkUserGrade(ref user);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Login exception", userId, userHostIP, this.storeID, ex);
            }

            return user;
        }

        /// <summary>
        /// This is a method allowed SSO sign in from other Advantech platform
        /// </summary>
        /// <param name="userHostIP"></param>
        /// <param name="authKey"></param>
        /// <returns></returns>
        public User ssoLogin(String userHostIP, String authKey, String userId)
        {
            User user = null;
            try
            {
                if (sso.validateTemidEmail(userHostIP, authKey, this.profile.StoreMembershippass, userId))
                {
                    //validate authKey and User ID
                    user = syncSSOUserProfile(userId, authKey);
                    checkUserGrade(ref user);
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("SSO login exception", userId, userHostIP, this.storeID, ex);
            }

            return user;
        }

        private void checkUserGrade(ref User user)
        {
            if (user != null && !string.IsNullOrEmpty(user.UserGrade))
            {
                string grade = user.UserGrade;
                user.userGradeX = this.profile.userGradeX.FirstOrDefault(x => x.Grade == grade);
            }
        }
        /// <summary>
        /// This method is invoked only after user is successfully authenticated.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="authKey"></param>
        /// <returns></returns>
        private User syncSSOUserProfile(String userId, String authKey)
        {
            User user = null;

            try
            {
                //Perform SSO authentication
                if (String.IsNullOrWhiteSpace(authKey))    //authentication fails
                {
                    //invalid authKey
                    return null;
                }

                //retrieve user profile from SSO service provider
                SSOUSER ssoUserProfile = SSOGetUserProfile(userId);
                if (ssoUserProfile == null)
                {
                    //log error, this type of error shall not happen
                    eStoreLoger.Error("User Profile missing in membership platform", userId, "", this.storeID);
                    return null;
                }

                //retrieve user profile
                user = getUser(userId);

                user.completeRegistration = SSOCheckRegistrationCompletion(userId);
                user.authKey = authKey.Trim();

                Boolean validCompanyId_SSOUser = isValidCompanyId(ssoUserProfile.company_id);
                Boolean validCompanyId_StoreUser = isValidCompanyId(user.CompanyID);
                String newCompanyId = null;

                //there are two conditions needing new company Id
                //1. Company Ids in both SSO user profile and in eStore user profile are invalid
                //2. (1) Company Id in SSO user profile is valid and
                //   (2) company Id in eStore user profile is invalid and company Id in SSO user profile has already taken in
                //   eStore or SAP contacts.
                if (!validCompanyId_StoreUser &&
                    (!validCompanyId_SSOUser || isCompanyIdExist(ssoUserProfile.company_id)))
                {
                    //current user doesn't have valid company account created yet.  Hereby generate a new company ID
                    newCompanyId = generateValidCompanyId();
                    if (String.IsNullOrWhiteSpace(newCompanyId))
                    {
                        eStoreLoger.Error("Failed at generating new company Id");
                        return null;    //has problem at generating company Id due to unknow reason
                    }
                }

                //if company id in eStore user profile is invalid, assign valid company Id back to eStore
                if (!validCompanyId_StoreUser)
                {
                    if (newCompanyId != null)
                        user.CompanyID = newCompanyId;
                    else if (validCompanyId_SSOUser)
                        user.CompanyID = ssoUserProfile.company_id;
                    else
                    {
                        //Ideally this condition shall never happen.  However, for safety, we'll still handle exception case here
                        eStoreLoger.Error("Critical issue at user ERPId handling");
                        return null;
                    }
                }

                //update user profile in membership if company ID in SSO user profile is invalid
                if (!isValidCompanyId(ssoUserProfile.company_id))
                {
                    ssoUserProfile.company_id = user.CompanyID;
                    //inform SSO service provider new company Id
                    sso.updProfile(ssoUserProfile, this.profile.StoreMembershippass);
                }

                //Sync up user information retrieved from SSO with eStore user profile
                //1. first match contact for modification. Create a new contact if no existing contact is found
                //2. Update eStore user contact with user profile information from SSO
                //3. Save current eStore
                Contact mainContact = null;
                foreach (Contact contact in user.Contacts)
                {
                    if (!string.IsNullOrEmpty(contact.AddressID) && contact.AddressID.Equals(user.CompanyID))
                    {   //main contact is found
                        mainContact = contact;
                        break;
                    }
                }

                if (mainContact == null)
                {
                    mainContact = new Contact(user);
                    mainContact.AddressID = user.CompanyID;
                    user.Contacts.Add(mainContact);
                }

                //checkSSOUserInformation if some infor is empty will get from my Advantech WS
                checkSsoUserProfile(ref ssoUserProfile);

                //sync user info
                user.FederalID = ssoUserProfile.vat_number;
                user.CompanyName = ssoUserProfile.company_name;
                user.Department = ssoUserProfile.department;
                user.LastName = ssoUserProfile.last_name;
                user.FirstName = ssoUserProfile.first_name;
                user.JobTitle = ssoUserProfile.job_title;
                user.JobFunction = ssoUserProfile.job_function;
                user.LoginPassword = ssoUserProfile.login_password;
                user.User_Field_TXT1 = ssoUserProfile.cnpj;
                user.User_Field_TXT2 = ssoUserProfile.insc;
                user.UserStatus = ssoUserProfile.user_status;
                user.BusinessApplicationArea = ssoUserProfile.business_type;
                user.BusinessType = ssoUserProfile.business_type;
                user.InterestedProduct = ssoUserProfile.in_product;
                if (String.IsNullOrEmpty(ssoUserProfile.reseller_id) || string.IsNullOrEmpty(user.ResellerID) ||
                    !user.ResellerID.Equals(ssoUserProfile.reseller_id))
                    user.ResellerID = ssoUserProfile.reseller_id;
                user.GroupID = ssoUserProfile.promotion_code;
                user.eMailSubscription = ssoUserProfile.notify;
                user.Country = ssoUserProfile.country; //Add country

                //sync contact info
                //mainContact.AddressType = "UserAddress";  //this field is missing..*****
                mainContact.AttCompanyName = ssoUserProfile.company_name;
                //mainContact.Attention = getUserFullName(user);
                mainContact.FirstName = user.FirstName;
                mainContact.LastName = user.LastName;
                mainContact.Address1 = ssoUserProfile.address;
                mainContact.Address2 = ssoUserProfile.address2;
                mainContact.FaxNo = ssoUserProfile.fax_no;
                mainContact.TelNo = ssoUserProfile.tel_areacode + "-" + ssoUserProfile.tel_no;
                mainContact.TelExt = ssoUserProfile.tel_ext;
                mainContact.Mobile = ssoUserProfile.mobile;
                mainContact.City = ssoUserProfile.city;

                Country currentCountry = getCountryByName(ssoUserProfile.country);
                if (currentCountry == null && this.profile.Country != null)
                    currentCountry = this.profile.Country;
                if (currentCountry != null)
                {
                    mainContact.countryX = string.IsNullOrEmpty(currentCountry.CountryName) ? ssoUserProfile.country : currentCountry.CountryName.Trim();
                    string state = getStateShort(currentCountry, ssoUserProfile.state);
                    if (!string.IsNullOrEmpty(state))
                        mainContact.State = state;
                }
                else
                {
                    mainContact.countryX = ssoUserProfile.country.Trim();
                    mainContact.State = ssoUserProfile.state.Trim();
                }

                mainContact.ZipCode = ssoUserProfile.zip;

                int result = user.save();
                if (result != 0) //problem at creating or update user profile
                    eStoreLoger.Error("Failed at updating user profile error code : " + result, userId, "", storeID);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("SSO Account sync exception", userId, "", this.storeID, ex);
            }

            return user;
        }


        private void checkSsoUserProfile(ref SSOUSER ssoUserProfile)
        {
            eStore.POCOS.MySAPDAL.MYSAPDAL helper = new POCOS.MySAPDAL.MYSAPDAL();
            helper.Timeout = 3000;
            try
            {
                var sapUserInfor = helper.GetSAPPartnerAddressesTableByKunnr(ssoUserProfile.erpid);
                if (sapUserInfor != null && sapUserInfor.Rows.Count > 0)
                {
                    System.Data.DataRow dr = sapUserInfor.Rows[0];
                    //ssoUserProfile.company_name = string.IsNullOrEmpty(ssoUserProfile.company_name) ? dr[""].ToString() : ssoUserProfile.company_name;
                    //mainContact.FirstName = user.FirstName;
                    //mainContact.LastName = user.LastName;
                    ssoUserProfile.address = string.IsNullOrEmpty(ssoUserProfile.address) ? dr["Street"].ToString() : ssoUserProfile.address;
                    ssoUserProfile.address2 = string.IsNullOrEmpty(ssoUserProfile.address2) ? dr["Str_Suppl3"].ToString() : ssoUserProfile.address2;
                    ssoUserProfile.fax_no = string.IsNullOrEmpty(ssoUserProfile.fax_no) ? dr["Fax_Number"].ToString() : ssoUserProfile.fax_no;
                    //ssoUserProfile.tel_areacode = string.IsNullOrEmpty(ssoUserProfile.tel_areacode) ? dr["Fax_Number"].ToString() : ssoUserProfile.tel_areacode;
                    ssoUserProfile.tel_no = string.IsNullOrEmpty(ssoUserProfile.tel_no) ? dr["Tel1_Numbr"].ToString() : ssoUserProfile.tel_no;
                    ssoUserProfile.tel_ext = string.IsNullOrEmpty(ssoUserProfile.tel_ext) ? dr["Tel1_Ext"].ToString() : ssoUserProfile.tel_ext;
                    //ssoUserProfile.mobile = string.IsNullOrEmpty(ssoUserProfile.mobile) ? dr["Tel1_Ext"].ToString() : ssoUserProfile.mobile;
                    ssoUserProfile.city = string.IsNullOrEmpty(ssoUserProfile.city) ? dr["City"].ToString() : ssoUserProfile.city;
                    ssoUserProfile.country = string.IsNullOrEmpty(ssoUserProfile.country) ? dr["Country"].ToString() : ssoUserProfile.country;
                    ssoUserProfile.zip = string.IsNullOrEmpty(ssoUserProfile.zip) ? dr["Postl_Cod1"].ToString() : ssoUserProfile.zip;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Get Account Information from SAP failed", "", "", this.storeID, ex);
            }

        }


        public enum SSO_Update_Type { Contact, ResellerID, CompanyID };
        /// <summary>
        /// This method is invoked only after user is successfully authenticated.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="authKey"></param>
        /// <returns></returns>
        public Boolean syncStoreUserToSSOUserProfile(User user, SSO_Update_Type updateType, Boolean isFromOrder = false)
        {
            Boolean success = true;
            try
            {
                if (!isFromOrder && String.IsNullOrWhiteSpace(user.authKey))    //User is not authenticated
                    return false;

                //retrieve user profile from SSO service provider
                SSOUSER ssoUserProfile = SSOGetUserProfile(user.UserID);
                if (ssoUserProfile == null)
                {
                    //log error, this type of error shall not happen
                    eStoreLoger.Error("User Profile missing in membership platform", user.UserID, "", this.storeID);
                    return false;
                }

                switch (updateType)
                {
                    case SSO_Update_Type.CompanyID:
                        ssoUserProfile.company_id = user.CompanyID;
                        break;
                    case SSO_Update_Type.Contact:
                        if (isFromOrder)
                            ssoUserProfile.company_id = user.CompanyID;
                        //sync user info
                        Contact mainContact = user.mainContact;
                        ssoUserProfile.company_name = mainContact.AttCompanyName;
                        ssoUserProfile.first_name = mainContact.FirstName;
                        ssoUserProfile.last_name = mainContact.LastName;
                        ssoUserProfile.address = mainContact.Address1;
                        ssoUserProfile.address2 = mainContact.Address2;
                        int iIndex = mainContact.TelNo.IndexOf("-");
                        if (iIndex != -1)
                        {
                            ssoUserProfile.tel_areacode = mainContact.TelNo.Substring(0, iIndex);
                            ssoUserProfile.tel_no = mainContact.TelNo.Substring(iIndex + 1);
                        }
                        else
                            ssoUserProfile.tel_no = mainContact.TelNo;
                        ssoUserProfile.fax_no = mainContact.FaxNo;
                        ssoUserProfile.mobile = mainContact.Mobile;
                        ssoUserProfile.city = mainContact.City;
                        ssoUserProfile.state = mainContact.State;
                        ssoUserProfile.country = mainContact.countryX;
                        ssoUserProfile.zip = mainContact.ZipCode;
                        break;
                    case SSO_Update_Type.ResellerID:
                        ssoUserProfile.reseller_id = user.ResellerID;
                        break;
                }
                sso.updProfile(ssoUserProfile, this.profile.StoreMembershippass);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("SSO Account sync exception", user.UserID, "", this.storeID, ex);
                success = false;
            }

            return success;
        }


        /// <summary>
        /// eStore shall call this function to logout current user from both eStore and SSO service provider
        /// </summary>
        /// <param name="user"></param>
        public void logout(User user)
        {
            try
            {
                if (user != null && user.isAuthenticated())
                    sso.logout(user.authKey, this.profile.StoreMembershippass);
            }
            catch (Exception ex)
            {
                eStoreLoger.Warn("Failed to log out", user.UserID, "", storeID, ex);
            }
        }

        /// <summary>
        /// Edward , This is  auto registration for SAP shipping notice, let the customer can check orders status in estore.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        public Boolean registeredUsertoSSO(SSOUSER user, string siteid = "SAP Ship")
        {
            try
            {
                sso.register(siteid, ref user);
                return true;
            }
            catch (Exception ex)
            {
                eStoreLoger.Warn("Failed to log out", user.email_addr, "", storeID, ex);
                return false;
            }


        }


        /// <summary>
        /// This method is to revalidate user authentication key status.  The auth key is from SSO server after user successfully sign in.
        /// In case the auth key expires, auth key shall be renew.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userHostIP"></param>
        /// <returns></returns>
        public Boolean validateUserAuth(User user, String userHostIP)
        {
            if (user.isAuthenticated())
            {
                if (sso.validateTempid(userHostIP, user.authKey, this.profile.StoreMembershippass))
                    return true;    //the auth key is still valid and good
                else
                {
                    user.authKey = null;
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 根据userId 同步member信息到eStore User表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User syncEstoreUserBySSOUser(String userId)
        {
            MemberHelper memberHelper = new MemberHelper();
            Member member = memberHelper.getMemberUserById(userId);

            User user = null;
            //Member表中有数据, 才去SSO同步
            if (member != null)
                user = syncSSOUserProfile(userId, userId);
            return user;
        }

        public enum TEMPLATE_TYPE
        {
            AccountEstablish,
            PayByBankConfirmation, PayByCreditCardConfirmation, PayByNetTermConfirmation,
            PrivacyPolicy, ProductEvaluationRequest, ProductReferral, Glossary,
            QuotationConfirmation, TransferredQuotationNotice, QuotationInPDF,
            ReturnPolicy, ShippingDamageTerm, UserAgreement, VolumnDiscountRequest,
            WarrantyPolicy, ProductEvaluation, EstablishAccount, BasicEmailTemplate, CallMeNow, EdiOrder,
            OrderSyncConfirmation, PayByDirectly, RateGift, WidgetRequest, ForgotPassWord, Register, SubscribeUs
        };

        /// <summary>
        /// This method returns template content of the specifying template in String format from C:\eStoreResources3C\Templates
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public String getTemplate(TEMPLATE_TYPE type, Language language = null, MiniSite ministe = null)
        {
            StringBuilder filePath = new StringBuilder();
            String fullPath = null;

            //the following file path is only for temporary purpose.  It requires further refinement
            //filePath.Append(AppDomain.CurrentDomain.BaseDirectory);
            filePath.Append(ConfigurationManager.AppSettings.Get("Template_Path")).Append("\\").Append(storeID).Append("\\");

            if (ministe != null && !string.IsNullOrEmpty(ministe.getStringSetting("EmailFilePath")))
                filePath.Append(ministe.getStringSetting("EmailFilePath")).Append("\\");

            //如果有language, 读取language 文件夹下面的html
            if (language != null && !string.IsNullOrEmpty(language.Code))
                filePath.Append(language.Code).Append("\\");

            switch (type)
            {
                case TEMPLATE_TYPE.AccountEstablish:
                    filePath.Append("AccountEstablish_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.PayByBankConfirmation:
                    filePath.Append("PayByBankConfirmation_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.PayByCreditCardConfirmation:
                    filePath.Append("PayByCardConfirmation_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.PayByNetTermConfirmation:
                    filePath.Append("PayByNetTermConfirmation_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.PayByDirectly:
                    filePath.Append("PayByDirectlyConfirmation_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.PrivacyPolicy:
                    filePath.Append("PrivacyPolicy_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.ProductEvaluationRequest:
                    filePath.Append("ProductEvaluation_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.ProductReferral:
                    filePath.Append("ProductReferral_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.QuotationConfirmation:
                    filePath.Append("QuotationConfirmation_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.TransferredQuotationNotice:
                    filePath.Append("TransferredQuotationNotice_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.QuotationInPDF:
                    filePath.Append("QuotationInPDF_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.ReturnPolicy:
                    filePath.Append("ReturnPolicy_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.ShippingDamageTerm:
                    filePath.Append("ShippingDamage_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.UserAgreement:
                    filePath.Append("UserAgreement_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.VolumnDiscountRequest:
                    filePath.Append("VolumnDiscountRequest_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.WarrantyPolicy:
                    filePath.Append("WarrantyPolicy_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.ProductEvaluation:
                    filePath.Append("ProductEvaluation_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.EstablishAccount:
                    filePath.Append("EstablishAccount_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.Glossary:
                    filePath.Append("Glossary_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.BasicEmailTemplate:
                    filePath.Append("BasicEmailTemplate_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.CallMeNow:
                    filePath.Append("CallMeNow_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.EdiOrder:
                    filePath.Append("EdiOrder_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.OrderSyncConfirmation:
                    filePath.Append("OrderSyncConfirmation_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.RateGift:
                    filePath.Append("RateGift_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.WidgetRequest:
                    filePath.Append("WidgetRequest_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.ForgotPassWord:
                    filePath.Append("ForgotPassWord_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.Register:
                    filePath.Append("Register_").Append(storeID).Append(".htm");
                    break;
                case TEMPLATE_TYPE.SubscribeUs:
                    filePath.Append("SubscribeUs_").Append(storeID).Append(".htm");
                    break;
            }

            //open template file and read it in 
            String content = "";
            try
            {
                fullPath = Path.GetFullPath(filePath.ToString());

                if (language != null && !string.IsNullOrEmpty(language.Code))
                {
                    //如果language下面文件不存在.  读取Store下面的文件
                    if (!File.Exists(fullPath))
                        fullPath = fullPath.Replace(language.Code + "\\", "");
                }

                //create an instance of StreamReader to read from a file.
                //The using statement also closes the StreamReader
                using (StreamReader reader = new StreamReader(fullPath))
                {
                    content = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Warn("File could not be read", fullPath.ToString(), "", "", ex);
            }

            return content;
        }

        /// <summary>
        /// The method returns matched order instance.  If nothing is found, it returns null.
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public Order getOrder(String orderNo)
        {
            OrderHelper orderHelper = new OrderHelper();
            Order order = orderHelper.getOrderbyOrderno(orderNo);

            return order;
        }
        //根据contact 生成sap erp
        public bool saveSAPCompanyByContact(CartContact _cartContact, System.Data.DataSet myDS)
        {
            if (_cartContact != null)
            {
                System.Data.DataTable master = myDS.Tables["Master"];
                System.Data.DataTable parner = myDS.Tables["partner"];

                SAPCompany sapCompany = new SAPCompany();
                sapCompany.CompanyID = _cartContact.AddressID;
                sapCompany.ParentCompanyID = _cartContact.AddressID;
                sapCompany.CompanyName = _cartContact.AttCompanyName;
                sapCompany.Address = _cartContact.Address1;
                sapCompany.FaxNo = _cartContact.FaxNo;
                sapCompany.TelNo = _cartContact.TelNo;
                sapCompany.Country = _cartContact.countryCodeX;
                sapCompany.ZipCode = _cartContact.ZipCode;
                sapCompany.City = _cartContact.City;
                sapCompany.CreatedBy = _cartContact.UpdatedBy;
                sapCompany.CreatedDate = DateTime.Now.AddDays(-20);
                sapCompany.LastUpdated = DateTime.Now.AddDays(-20);

                SAPCompanySalesarea sapCompanySale = new SAPCompanySalesarea();
                sapCompanySale.COMPANY_ID = _cartContact.AddressID;
                sapCompanySale.CHANNEL = "00";
                sapCompanySale.DIVISION = "00";//默认为00

                if (master != null && master.Rows.Count > 0)
                {
                    System.Data.DataRow masterRow = master.Rows[0];
                    sapCompany.OrgID = masterRow["ORG_ID"] == null ? "EU01" : masterRow["ORG_ID"].ToString();
                    sapCompany.ParentCompanyID = masterRow["ParentCompanyID"] == null ? "" : masterRow["ParentCompanyID"].ToString();
                    sapCompany.Currency = masterRow["CURRENCY"] == null ? "EUR" : masterRow["CURRENCY"].ToString();
                    sapCompany.CompanyType = masterRow["COMPANY_TYPE"] == null ? "" : masterRow["COMPANY_TYPE"].ToString();
                    sapCompany.PriceClass = masterRow["PRICE_CLASS"] == null ? "" : masterRow["PRICE_CLASS"].ToString();

                    sapCompanySale.PRICE_CLASS = masterRow["PRICE_CLASS"] == null ? "" : masterRow["PRICE_CLASS"].ToString();
                    sapCompany.PtradePriceClass = masterRow["Ptrade_Price_Class"] == null ? "" : masterRow["Ptrade_Price_Class"].ToString();
                    sapCompany.Region = masterRow["REGION"] == null ? "" : masterRow["REGION"].ToString();
                    sapCompany.Attention = masterRow["Attention"] == null ? "" : masterRow["Attention"].ToString();
                    sapCompany.CreditLimit = masterRow["Credit_Limit"] == null ? 0 : Decimal.Parse(masterRow["Credit_Limit"].ToString());
                    sapCompany.CreditTerm = masterRow["Credit_Term"] == null ? "" : masterRow["Credit_Term"].ToString();

                    sapCompanySale.CREDIT_TERM = masterRow["Credit_Term"] == null ? "" : masterRow["Credit_Term"].ToString();
                    sapCompany.ShipVia = masterRow["Ship_Via"] == null ? "" : masterRow["Ship_Via"].ToString();
                    sapCompany.Url = masterRow["Url"] == null ? "" : masterRow["Url"].ToString();
                    sapCompany.CompanyPriceType = masterRow["Company_Price_Type"] == null ? "" : masterRow["Company_Price_Type"].ToString();

                    sapCompanySale.COMPANY_PRICE_TYPE = masterRow["Company_Price_Type"] == null ? "" : masterRow["Company_Price_Type"].ToString();
                    sapCompany.SalesUserID = masterRow["Sales_UserID"] == null ? "" : masterRow["Sales_UserID"].ToString();
                    sapCompany.ShipCondition = masterRow["Ship_Condition"] == null ? "" : masterRow["Ship_Condition"].ToString();
                    sapCompany.Attribute4 = masterRow["Attribute4"] == null ? "" : masterRow["Attribute4"].ToString();
                    sapCompany.SalesOffice = masterRow["SalesOffice"] == null ? "" : masterRow["SalesOffice"].ToString();
                    sapCompany.SalesGroup = masterRow["SalesGroup"] == null ? "" : masterRow["SalesGroup"].ToString();

                    sapCompanySale.SalesOffice = masterRow["SalesOffice"] == null ? "" : masterRow["SalesOffice"].ToString();
                    sapCompanySale.SalesGroup = masterRow["SalesGroup"] == null ? "" : masterRow["SalesGroup"].ToString();

                    sapCompanySale.ORG_ID = masterRow["ORG_ID"].ToString();
                    sapCompanySale.CURRENCY = masterRow["CURRENCY"].ToString();
                }

                if (parner != null && parner.Rows.Count > 0)
                {
                    System.Data.DataRow parnerRow = parner.Rows[0];
                    sapCompanySale.CHANNEL = parnerRow["DIST_CHANN"] == null ? "00" : parnerRow["DIST_CHANN"].ToString();
                    sapCompanySale.DIVISION = parnerRow["DIVISION"] == null ? "00" : parnerRow["DIVISION"].ToString();
                    sapCompanySale.Sales_Person1_Code = parnerRow["Sales_Code"] == null ? "00" : parnerRow["Sales_Code"].ToString();
                }

                if (sapCompany.save() == 0 && sapCompanySale.save() == 0)
                    return true;
            }
            return false;
        }

        public bool isWeek2DeliveryProducts(POCOS.Part part)
        {
            if (part is POCOS.Product)
            {
                POCOS.Product pro = part as POCOS.Product;
                return pro.isIncludeSatus(Product.PRODUCTMARKETINGSTATUS.EXPRESS)
                        || pro.isIncludeSatus(Product.PRODUCTMARKETINGSTATUS.TwoWeeksFastDelivery);
            }
            else
                return false;
        }

        public bool isFastDeliveryProducts(POCOS.Product product)
        {
            if (storeID != "AUS")
                return product.deliveryMarketings.Any();
            else if (product is POCOS.Product_Ctos)
                return getFastDeliveryProducts().Any(x => x == ((POCOS.Product_Ctos)product).BTONo);
            else
                return getFastDeliveryProducts().Any(x => x == product.fillupPartnoasSAP());
        }
        public bool isFastDeliveryOrder(POCOS.Order order)
        {
            if (order != null && order.cartX != null)
                return order.cartX.CartItems.Any(x => (x.partX != null && x.partX is Product && isFastDeliveryProducts((Product)x.partX)));
            else
                return false;
        }
        private List<string> _fastDeliveryProducts;
        public List<string> getFastDeliveryProducts()
        {
            if (_fastDeliveryProducts == null)
            {
                _fastDeliveryProducts = new List<string>();
                try
                {
                    if (storeID == "AUS")
                        _fastDeliveryProducts = (new DeliveryCategoryMGT()).Get("AUS").getFastDeliveryProductIds(new POCOS.ProductCategory() { Storeid = "AUS" });
                }
                catch (Exception ex)
                {
                    eStoreLoger.Error("Failed to get getFastDeliveryProducts", "", "", "", ex);
                }
            }

            return _fastDeliveryProducts;
        }


        public void fixSpecialProductCategory(POCOS.ProductCategory pc, POCOS.MiniSite site = null)
        {
            if (pc.simpleProductList.Any())
                return;

            List<string> partids = null;
            List<Product> spls = new List<Product>();
            if (pc.categoryTypeX == ProductCategory.Category_Type.Delivery)
            {
                if (pc.CategoryPath.Equals(Product.PRODUCTMARKETINGSTATUS.TwoDaysFastDelivery.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    IDeliveryCategory deliveryhelp = new DeliveryCategoryOther();
                    if (pc.Storeid == "AUS")
                        deliveryhelp = (new DeliveryCategoryMGT()).Get("AUS");
                    partids = deliveryhelp.getFastDeliveryProductIds(pc);
                }

            }
            if (partids == null)
            {
                if (!string.IsNullOrEmpty(pc.CategoryPath))
                {
                    Product.PRODUCTMARKETINGSTATUS item;
                    if (Enum.TryParse(pc.CategoryPath, out item))
                        spls = (new PartHelper()).getProductsByState(item, this.profile, null, 1000).Where(p => p.isOrderable()).ToList();
                }
            }
            else
            {
                spls = new POCOS.DAL.PartHelper().prefetchProductList(this.storeID, partids).Where(p => p.isOrderable()).ToList();
            }
            if (pc.childCategoriesX.Any())
            {
                foreach (var item in pc.childCategoriesX)
                {
                    fixSpecialProductCategory(item, site);
                    pc.resetSimpleProductList();
                }
            }
            else
            {
                fixSpecialCategorys(ref pc, spls, site);
            }

            foreach (var p in spls)
            {
                if (pc.simpleProductList.FirstOrDefault(c => c.SProductID == p.SProductID) == null)
                    pc.simpleProductList.Add(new SimpleProduct(p));
            }
        }

        /// <summary>
        /// 根据产品分组category
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="pros"></param>
        protected void fixSpecialCategorys(ref ProductCategory pc, List<Product> pros, POCOS.MiniSite site = null)
        {
            foreach (var p in pros)
            {
                foreach (var c in p.productCategories.Where(c => c.MiniSite == site && !c.childCategoriesX.Any()))
                {
                    var root = c.getRootCategory();
                    var item = pc.childCategoriesX.FirstOrDefault(t => t.CategoryID == root.CategoryID);
                    if (item == null)
                    {
                        item = root.copyToTemp();
                        item.CreatedBy = "Temp_" + pc.CategoryPath;//parent category 
                        item.addSimpleProduct(new SimpleProduct(p));
                        pc.childCategoriesX.Add(item);
                    }
                    else
                    {
                        if (item.simpleProductList.FirstOrDefault(t => t.SProductID == p.SProductID) == null)
                            item.addSimpleProduct(new SimpleProduct(p));
                    }
                }
            }
        }

        public void fixProductQtyByCategory(ProductCategory pc, ref List<POCOS.SimpleProduct> sls)
        {
            if (pc == null || sls == null || !sls.Any())
                return;
            var ds = resetCategoryQty(pc).OrderBy(c => c.Value);
            if (ds.Any())
            {
                List<POCOS.SimpleProduct> ls = new List<SimpleProduct>();
                foreach (var item in ds)
                {
                    var p = sls.FirstOrDefault(c => c.SProductID == item.Key);
                    if (p != null)
                        ls.Add(p);
                }
                foreach (var pp in sls)
                {
                    if (ls.FirstOrDefault(c => c.SProductID == pp.SProductID) == null)
                        ls.Add(pp);
                }
                sls = ls;
            }
        }

        public void fixProductQtyByCategory(ProductCategory pc, ref List<POCOS.Product> sls)
        {
            if (pc == null || sls == null || !sls.Any())
                return;
            var ds = resetCategoryQty(pc).OrderBy(c => c.Value);
            if (ds.Any())
            {
                List<POCOS.Product> ls = new List<Product>();
                foreach (var item in ds)
                {
                    var p = sls.FirstOrDefault(c => c.SProductID == item.Key && c.isOrderable()); // call for price will at last 
                    if (p != null)
                        ls.Add(p);
                }
                foreach (var pp in sls)
                {
                    if (ls.FirstOrDefault(c => c.SProductID == pp.SProductID) == null)
                        ls.Add(pp);
                }
                sls = ls;
            }
        }

        protected Dictionary<string, int> resetCategoryQty(ProductCategory pc)
        {
            Dictionary<string, int> ds = new Dictionary<string, int>();
            if (pc == null)
                return ds;
            if (pc.childCategoriesX.Any())
            {
                foreach (var cate in pc.childCategoriesX)
                {
                    var tempds = resetCategoryQty(cate);
                    if (tempds.Any())
                    {
                        foreach (var item in tempds)
                        {
                            if (ds.Keys.Contains(item.Key))
                            {
                                if (ds[item.Key] > item.Value)
                                    ds[item.Key] = item.Value;
                            }
                            else
                                ds.Add(item.Key, item.Value);
                        }
                    }
                }
            }
            else
            {
                foreach (var mp in pc.ProductCategroyMappings)
                {
                    if (ds.Keys.Contains(mp.SProductID))
                    {
                        if (ds[mp.SProductID] > mp.Seq)
                            ds[mp.SProductID] = mp.Seq;
                    }
                    else
                        ds.Add(mp.SProductID, mp.Seq);
                }
            }
            return ds;
        }


        /// <summary>
        /// The method returns matched quotation instance.  If nothing is found, it returns null.
        /// </summary>
        /// <param name="quotationNo"></param>
        /// <returns></returns>
        public Quotation getQuotation(String quotationNo)
        {
            QuotationHelper quotationHelper = new QuotationHelper();
            Quotation quotation = quotationHelper.getQuoteByQuoteno(quotationNo);

            return quotation;
        }

        /// <summary>
        /// Get Quotes list by Storeid and Date Range
        /// </summary>
        public IList<Quotation> getQuotesbyDateRange(string DMF, DateTime startdate, DateTime enddate, string Company = null, string email = null)
        {
            DMFHelper DMFhelper = new DMFHelper();
            DMF dmf = DMFhelper.getDMFbyID(DMF);

            QuotationHelper helper = new QuotationHelper();

            IList<Quotation> quotes = null;
            quotes = helper.getQuotations(dmf, startdate, enddate, Company, email);

            return quotes;
        }

        /// <summary>
        /// This method is to retreive cross-sell product items based on shopping cart content.
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public List<Product> getCustomersAlsoBought(Cart cart)
        {
            Dictionary<string, int> productsAlsoBought = new Dictionary<string, int>();
            foreach (var ci in cart.CartItems)
            {
                //get products that other customers also bought in the same orders of the buying standard products in their shopping cart.
                //The information will be based on AOnline orders only.
                if (!(ci.partX is POCOS.Product_Bundle) && !(ci.partX is POCOS.Product_Ctos) && ci.partX.SuggestingProductsDictionary != null)
                {
                    //part's suggesting products will be from SAP order records
                    foreach (var s in ci.partX.SuggestingProductsDictionary)
                    {
                        if (productsAlsoBought.ContainsKey(s.Key))
                            productsAlsoBought[s.Key] += s.Value;
                        else
                            productsAlsoBought.Add(s.Key, s.Value);
                    }
                }
            }

            //Following is to retrieve products that other customer also bought in the same order.  The statistics will be 
            //from eStore online orders only.   This will be the only source providing CTOS products that other customer also bought
            //in the same order.
            CartHelper helper = new CartHelper();
            Dictionary<string, int> estorealsobought = helper.getCustomersAlsoBought(cart);
            foreach (var s in estorealsobought)
            {
                if (productsAlsoBought.ContainsKey(s.Key))
                {
                    //avoid double count standard product weight since they shall already be weighted.
                    //allalsobought[s.Key] += s.Value; 
                }
                else
                {
                    //CTOS products or standard products that are not counted in earlier process
                    productsAlsoBought.Add(s.Key, s.Value);
                }
            }

            List<Product> products = applyAlsoBoughtProductWeight(productsAlsoBought);

            return products;
        }

        /// <summary>
        /// This method is to retreive cross-sell product items based on shopping cart content.
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public List<Product> getCustomersAlsoBoughtX(List<POCOS.Product> productList)
        {
            Dictionary<string, int> productsAlsoBought = new Dictionary<string, int>();
            foreach (var ci in productList)
            {
                Part part = ci as POCOS.Part;
                //get products that other customers also bought in the same orders of the buying standard products in their shopping cart.
                //The information will be based on AOnline orders only.
                if (!(part is POCOS.Product_Bundle) && !(part is POCOS.Product_Ctos) && part.SuggestingProductsDictionary != null)
                {
                    //part's suggesting products will be from SAP order records
                    foreach (var s in part.SuggestingProductsDictionary)
                    {
                        if (productsAlsoBought.ContainsKey(s.Key))
                            productsAlsoBought[s.Key] += s.Value;
                        else
                            productsAlsoBought.Add(s.Key, s.Value);
                    }
                }
            }

            //Following is to retrieve products that other customer also bought in the same order.  The statistics will be 
            //from eStore online orders only.   This will be the only source providing CTOS products that other customer also bought
            //in the same order.
            CartHelper helper = new CartHelper();
            Dictionary<string, int> estorealsobought = helper.getCustomersAlsoBoughtX(productList);
            foreach (var s in estorealsobought)
            {
                if (productsAlsoBought.ContainsKey(s.Key))
                {
                    //avoid double count standard product weight since they shall already be weighted.
                    //allalsobought[s.Key] += s.Value; 
                }
                else
                {
                    //CTOS products or standard products that are not counted in earlier process
                    productsAlsoBought.Add(s.Key, s.Value);
                }
            }

            List<Product> products = applyAlsoBoughtProductWeight(productsAlsoBought);

            return products;
        }

        /// <summary>
        /// This method will apply additional weight to product list based on product type.  The weighting priority
        /// will be CTOS, Bundle, Advantech mainstream product and others
        /// </summary>
        /// <param name="alsoBoughtProducts"></param>
        /// <returns></returns>
        public List<Product> applyAlsoBoughtProductWeight(Dictionary<string, int> alsoBoughtProducts)
        {
            string[] exceptPN = { "BTO-", "AGS-", "ES-", "SBC-BTO", "LCD-KIT" };
            string[] partIDs = (from b in alsoBoughtProducts
                                where exceptPN.FirstOrDefault(x => b.Key.ToUpper().StartsWith(x)) == null
                                select b.Key).ToArray();
            List<Part> alsoboughtproducts = this.getPartList(string.Join(",", partIDs));

            Dictionary<Product, int> productsWights = new Dictionary<Product, int>();
            foreach (Part p in alsoboughtproducts)
            {
                if (string.IsNullOrEmpty(p.SProductID)) //sproductid 为空的是否考虑删除？
                    continue;
                if (p.isOrderable() == false)
                    continue;
                if (p is Product_Ctos)
                {
                    productsWights.Add((Product)p, alsoBoughtProducts[p.SProductID] * 20);
                }
                else if (p is Product_Bundle)
                {
                    productsWights.Add((Product)p, alsoBoughtProducts[p.SProductID] * 10);
                }
                else if (p is Product)
                {
                    if (p.isMainStream())
                        productsWights.Add((Product)p, alsoBoughtProducts[p.SProductID] * 5);
                    else
                        productsWights.Add((Product)p, alsoBoughtProducts[p.SProductID] * 1);
                }
            }
            return productsWights.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
        }

        /// <summary>
        /// This method is to retreive cross-sell product items based on shopping cart content.
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public List<Product> getCrossSellProductByCart(Cart cart)
        {
            Dictionary<Product, int> productsTotalWights = new Dictionary<Product, int>();
            //排除特定的产品
            //string[] exceptPN = { "BTO-", "AGS-", "ES-", "SBC-BTO" };
            //var productCrossSellList = from c in cart.cartItemsX
            //         where exceptPN.FirstOrDefault(x => c.SProductID.ToUpper().StartsWith(x)) == null                      
            //         select c.partX;
            var productCrossSellList = from c in cart.cartItemsX
                                       where c.partX is Product
                                       select c.partX;
            foreach (Product crossSellItem in productCrossSellList)//循环cart中的产品
            {
                //循环权重
                foreach (var pWightKey in crossSellItem.categoryCrossSellProducts.Keys)
                {
                    //if already exists, then use the higher one, else, add new cross sell product
                    if (!productsTotalWights.Any(x => x.Key.SProductID == pWightKey.SProductID))
                        productsTotalWights.Add(pWightKey, crossSellItem.categoryCrossSellProducts[pWightKey]);
                    else if (productsTotalWights.First(x => x.Key.SProductID == pWightKey.SProductID).Value < crossSellItem.categoryCrossSellProducts[pWightKey])
                    {
                        productsTotalWights[productsTotalWights.First(x => x.Key.SProductID == pWightKey.SProductID).Key] = crossSellItem.categoryCrossSellProducts[pWightKey];
                    }
                }
            }
            return productsTotalWights.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
        }

        /// <summary>
        /// This method is to retreive cross-sell product items based on shopping cart content.
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public List<Product> getCrossSellProductByProductList(List<POCOS.Product> productNos)
        {
            Dictionary<Product, int> productsTotalWights = new Dictionary<Product, int>();

            foreach (Product crossSellItem in productNos)//循环cart中的产品
            {
                //循环权重
                foreach (var pWightKey in crossSellItem.categoryCrossSellProducts.Keys)
                {
                    //if already exists, then use the higher one, else, add new cross sell product
                    if (!productsTotalWights.Any(x => x.Key.SProductID == pWightKey.SProductID))
                        productsTotalWights.Add(pWightKey, crossSellItem.categoryCrossSellProducts[pWightKey]);
                    else if (productsTotalWights.First(x => x.Key.SProductID == pWightKey.SProductID).Value < crossSellItem.categoryCrossSellProducts[pWightKey])
                    {
                        productsTotalWights[productsTotalWights.First(x => x.Key.SProductID == pWightKey.SProductID).Key] = crossSellItem.categoryCrossSellProducts[pWightKey];
                    }
                }
            }
            return productsTotalWights.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
        }

        /// <summary>
        /// This method returns list of eStore active products
        /// </summary>
        /// <returns></returns>
        public IList<Product> getStandardProductList()
        {
            PartHelper helper = new PartHelper();
            //enable cache management
            return helper.getActiveStandardProducts(storeID);
        }

        /// <summary>
        /// This method return list of CTO systems
        /// </summary>
        /// <returns></returns>
        public IList<Product_Ctos> getCTOSystemList()
        {
            PartHelper helper = new PartHelper();
            return helper.getCTOSProducts(storeID);
        }

        /// <summary>
        /// This method execute payment transaction and return the payment status
        /// </summary>
        /// <param name="order"></param>
        /// <param name="paymentProviderInfo"></param>
        /// <param name="paymentInfo"></param>
        /// <returns>Payment</returns>
        public Payment makePayment(Order order, StorePayment paymentProviderInfo, Payment paymentInfo, Boolean simulation = false)
        {
            IPaymentSolutionProvider solutionProvider = paymentManager.getPaymentSolutionProvider(paymentProviderInfo);

            paymentInfo.Order = order;
            if (simulation)
                paymentInfo.Comment2 = "Simulation";

            Payment transaction = solutionProvider.makePayment(order, paymentInfo, simulation);

            //Update order status based on payment transaction result
            transaction.PaymentType = paymentProviderInfo.PaymentMethod;
            transaction.TranxType = Payment.TransactionType.Authorization.ToString();
            if (order.Payments == null)
                order.Payments = new List<Payment>();
            order.addPayment(transaction);  //order status will be changed in this method too
            order.PaymentID = transaction.PaymentID;
            order.PaymentType = transaction.PaymentType;

            //Send payment status to eStore administration  ****** 
            sendPaymentDetails(paymentInfo);

            return transaction;
        }

        public bool ServiceByChannelPartner(Order order)
        {
            order.statusX = Order.OStatus.Confirmed;
            return true;
        }

        public IDictionary<String, String> prepareIndirectPayment(Order order, StorePayment paymentProviderInfo, Payment paymentInfo, Boolean simulation = false, Boolean savePayment = true)
        {
            IPaymentSolutionProvider solutionProvider = paymentManager.getPaymentSolutionProvider(paymentProviderInfo);
            IDictionary<String, String> form = new Dictionary<String, String>();

            if (!solutionProvider.supportDirectAccess())
            {
                paymentInfo.Order = order;
                try
                {
                    form = solutionProvider.getIndirectPaymentRequestForm(order, paymentInfo, simulation);
                    if (savePayment)
                    {
                        //Update order status based on payment transaction result
                        paymentInfo.PaymentType = paymentProviderInfo.PaymentMethod;
                        order.addPayment(paymentInfo);  //order status will be changed in this method too
                        order.PaymentID = paymentInfo.PaymentID;
                        order.PaymentType = paymentInfo.PaymentType;
                    }
                }
                catch (Exception)
                {
                }
            }

            return form;
        }
        public string getIndirectPaymentOrderResponseNO(NameValueCollection response)
        {
            StorePayment paymentProviderInfo = this.profile.StorePayments.FirstOrDefault(sp => sp.PaymentType == "Redirect");
            IPaymentSolutionProvider solutionProvider = paymentManager.getPaymentSolutionProvider(paymentProviderInfo);
            return solutionProvider.getIndirectPaymentOrderResponseNO(response);
        }

        public Payment completeIndirectPayment(Order order, StorePayment paymentProviderInfo, NameValueCollection paymentResponse)
        {
            IPaymentSolutionProvider solutionProvider = paymentManager.getPaymentSolutionProvider(paymentProviderInfo);
            Payment transaction = solutionProvider.processIndirectPaymentResponse(paymentResponse, order);

            transaction.PaymentType = paymentProviderInfo.PaymentMethod;
            order.addPayment(transaction);
            order.PaymentID = transaction.PaymentID;
            order.PaymentType = transaction.PaymentType;

            //Send payment status to eStore administration  ****** 
            sendPaymentDetails(transaction);

            return transaction;
        }


        public string getBankInformation(Order order)
        {
            /*this logical is removed as user requirement
            if (this.storeID == "ABR")
            {
                if (order.totalAmountX < 5000)
                {
                    return this.getSettings()["DOCBankInformation"];
                }
                else
                {
                    return this.getSettings()["TEDBankInformation"];
                }
            }
            */
            POCOS.Address _storeAddress;
            if (order.cartX.minisiteX != null && order.cartX.minisiteX.getIntegerSetting("AddressId") != null)
                _storeAddress = getAddressById(order.cartX.minisiteX.getIntegerSetting("AddressId").GetValueOrDefault());
            else if (order != null && order.cartX != null && order.cartX.BillToContact != null)
                _storeAddress = this.getAddressByCountry(order.cartX.BillToContact.Country, order.cartX.businessGroup);
            else
                return string.Empty;


            if (_storeAddress != null)
                return _storeAddress.BankInformation;
            else
                return this.Tanslation(POCOS.Store.TranslationKey.eStore_Bank_Information_Context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payment"></param>
        private void sendPaymentDetails(Payment payment)
        {
            //prepare messageInfo
            Dictionary<String, String> messageInfo = new Dictionary<string, string>();
            if (payment.responseValues != null)
                messageInfo = payment.responseValues.ToDictionary(pair => pair.Key, pair => pair.Value);

            messageInfo.Add("EmailSubject", profile.StoreName + " Payment Result");
            messageInfo.Add("UserID", payment.Order.UserID);
            messageInfo.Add("OrderNo", payment.OrderNo);
            messageInfo.Add("PaymentType", payment.PaymentType);
            messageInfo.Add("PaymentID", payment.PaymentID);

            EmailBasicTemplate emailContext = new EmailBasicTemplate();
            emailContext.sendEmailInBasicTemplate(this, messageInfo, false, @"<meta http-equiv='Content-Type' content='text/html; charset=gb2312'> UserID : [/UserID]<br> OrderNo : [/OrderNo]<br> PaymentType : [/PaymentType]<br> PaymentID : [/PaymentID]<br><br><br> [/EmailDictionaryList]");
        }

        public StorePayment getStorePayment(String paymentMethod)
        {
            StorePayment storePayment = (from pm in this.storePayments
                                         where pm.PaymentMethod.Equals(paymentMethod)
                                         select pm).FirstOrDefault();
            return storePayment;
        }

        public ICollection<StorePayment> storePayments
        {
            get
            {
                return this.profile.StorePayments.OrderBy(pm => pm.Preference.GetValueOrDefault()).ToList();
            }
        }

        public ICollection<StorePayment> getStorePayments(Country BilltoCountry, bool WireTransferOnly = false)
        {
            if (WireTransferOnly)
                return this.profile.StorePayments.Where(sp => sp.PaymentMethod == "WireTransfer").ToList();
            else if (BilltoCountry != null)
                return this.profile.StorePayments.Where(sp => string.IsNullOrWhiteSpace(sp.AppliedRegion)
                    || sp.AppliedRegion.Contains(BilltoCountry.CountryName)
                    || sp.AppliedRegion.Contains(BilltoCountry.Region))
                    .OrderBy(pm => pm.Preference.GetValueOrDefault()).ToList();
            else
                return this.profile.StorePayments.Where(sp => sp.PaymentMethod == "WireTransfer").ToList();
        }



        /// <summary>
        /// This property retains available menu items in current store.  The value will be cached as long as store instance exist.
        /// To drop the cache a maintenance point needs to be implemented.
        /// </summary>
        private List<Menu> _menuItems = null;
        public IList<Menu> getMenuItems(POCOS.MiniSite miniSite)
        {

            if (_menuItems == null) //first time -- need initialization
            {
                _menuItems = this.getMenus(this.storeID, Menu.MenuPosition.Header).Where(x => x.MiniSite == miniSite).ToList();
            }
            return _menuItems;

        }

        private List<Menu> _landing = null;
        public IList<Menu> getLandingPages(POCOS.MiniSite miniSite)
        {
            if (_landing == null) //first time -- need initialization
            {
                _landing = this.getMenus(this.storeID, Menu.MenuPosition.Landing).Where(x => x.MiniSite == miniSite).ToList();
            }

            return _landing;

        }

        private List<Menu> _footerLinks = null;
        public IList<Menu> getFooterLinks(POCOS.MiniSite miniSite)
        {

            if (_footerLinks == null) //first time -- need initialization
            {
                _footerLinks = this.getMenus(this.storeID, Menu.MenuPosition.Footer).Where(x => x.MiniSite == miniSite).ToList();
            }

            return _footerLinks;

        }

        public List<Menu> getMenus(string Store, POCOS.Menu.MenuPosition position)
        {
            List<Menu> ms;
            MenuHelper helper = new MenuHelper();
            List<Menu> menus = helper.getMenusByStoreid(this.storeID, position, false);

            if (menus == null)
                ms = new List<Menu>();  //create empty menu list
            else
            {
                //filtering out non-published menu item
                var items = from menu in menus
                            where menu.Publish == true
                            select menu;
                if (items == null)
                    ms = new List<Menu>();
                else
                    ms = items.ToList<Menu>();
            }
            return ms;
        }

        private List<VSAPCompany> _sapContacts = null;
        public IList<VSAPCompany> sapContacts
        {
            get
            {
                if (_sapContacts == null)
                {
                    StoreHelper helper = new StoreHelper();
                    _sapContacts = helper.getSAPCompanies(this.profile);
                }

                return _sapContacts;
            }
        }

        public List<VSAPCompany> getMatchVSAPCompanies(String keyword, int maxCount, bool isgetFromSAP = false)
        {
            StoreHelper helper = new StoreHelper();
            List<VSAPCompany> sapCompanies = helper.getSAPCompaniesbyKeyword(this.profile, keyword, maxCount);

            if (isgetFromSAP && !sapCompanies.Any())
            {
                var vcom = getSAPCompanyFrowWS(keyword);
                if (vcom != null)
                    sapCompanies.Add(vcom);
            }

            return sapCompanies;
        }

        public VSAPCompany getSAPCompanyFrowWS(String companyId)
        {
            if (!string.IsNullOrEmpty(companyId))
            {
                try
                {
                    MyAdvantechSAPCustomerNew.MAMigration mahelper = new MyAdvantechSAPCustomerNew.MAMigration();
                    string res = "";
                    System.Data.DataSet ds = mahelper.SyncSingleSAPCustomer(companyId, false, ref res);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables["Master"] != null && ds.Tables["Master"].Rows.Count > 0)
                    {
                        VSAPCompany company = new VSAPCompany()
                        {
                            CompanyID = ds.Tables["Master"].Rows[0]["COMPANY_ID"].ToString(),
                            Address = ds.Tables["Master"].Rows[0]["ADDRESS"].ToString(),
                            CHANNEL = "",
                            City = ds.Tables["Master"].Rows[0]["CITY"].ToString(),
                            CompanyName = ds.Tables["Master"].Rows[0]["COMPANY_NAME"].ToString(),
                            Country = ds.Tables["Master"].Rows[0]["COUNTRY_NAME"].ToString(), //myAdvantech发布新版后可看到。
                            Currency = ds.Tables["Master"].Rows[0]["CURRENCY"].ToString(),
                            DIVISION = "",
                            FaxNo = ds.Tables["Master"].Rows[0]["FAX_NO"].ToString(),
                            OrgID = ds.Tables["Master"].Rows[0]["ORG_ID"].ToString(),
                            PRICE_CLASS = ds.Tables["Master"].Rows[0]["PRICE_CLASS"].ToString(),
                            Region = ds.Tables["Master"].Rows[0]["REGION"].ToString(),
                            Sales_Person1_Code = "",
                            SalesDistrict = "SAP_COMPANY_SALES_DEF",
                            SalesGroup = ds.Tables["Master"].Rows[0]["SALESGROUP"].ToString(),
                            SalesOffice = ds.Tables["Master"].Rows[0]["SALESOFFICE"].ToString(),
                            TelNo = ds.Tables["Master"].Rows[0]["TEL_NO"].ToString(),
                            ZipCode = ds.Tables["Master"].Rows[0]["ZIP_CODE"].ToString()
                        };
                        if (ds.Tables["Partner"] != null && ds.Tables["Partner"].Rows.Count > 0)
                        {
                            //string salesCode = ds.Tables["Master"].Rows[0]["SAP_SALESCODE"].ToString();
                            //if (string.IsNullOrEmpty(salesCode))
                            //    salesCode = ds.Tables["Partner"].Rows[0]["SALES_CODE"].ToString();

                            var parterdrs = ds.Tables["Partner"].Select("SALES_CODE <> '00000000' AND PARTNER_FUNCTION = 'VE'");
                            if (parterdrs != null && parterdrs.Length > 0)
                            {
                                company.Sales_Person1_Code = parterdrs[0]["SALES_CODE"].ToString();
                                company.DIVISION = parterdrs[0]["DIVISION"].ToString();
                                company.CHANNEL = parterdrs[0]["DIST_CHANN"].ToString();
                            }
                        }
                        if (ds.Tables["SalesDef"] != null && ds.Tables["SalesDef"].Rows.Count > 0)
                            company.SalesDistrict = ds.Tables["SalesDef"].Rows[0]["SALES_DISTRICT"].ToString();
                        return company;
                    }
                    eStoreLoger.Info(string.Format("Get SAP Company from MyAdvantech web service. Company ID: {0}", companyId), "", "", this.storeID);
                }
                catch (Exception ex)
                {
                    //ignore exception and move on
                    eStoreLoger.Error(string.Format("Get SAP Company from MyAdvantech web service exception. Company ID: {0}", companyId), "", "", this.storeID, ex);
                }
            }
            return null;
        }

        /// <summary>
        /// This method is to find match SAPCompany information based on ERPID
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public VSAPCompany findSAPContact(String companyId, bool isgetFromSAP = false)
        {
            /*
            VSAPCompany sapContact = (from item in sapContacts
                                       where item.CompanyID.Equals(companyId)
                                       select item).FirstOrDefault();
             */

            StoreHelper helper = new StoreHelper();
            VSAPCompany sapContact = helper.getSAPCompany(this.profile, companyId);

            if (sapContact == null && isgetFromSAP)
            {
                var vcom = getSAPCompanyFrowWS(companyId);
                if (vcom != null)
                    return vcom;
            }

            return sapContact;
        }
        /// <summary>
        /// Adjust order/cart/cartitem time by store time zone
        /// </summary>
        /// <param name="order"></param>
        public void AdjustOrderAndCartTime(Order order)
        {
            //Update order's time
            DateTime localTime = getLocalDateTime(DateTime.Now);
            order.OrderDate = localTime;
            order.LastUpdated = localTime;
            order.DueDate = localTime;
            order.RequiredDate = localTime;
            order.ConfirmedDate = localTime;

            //Update cart's time
            order.cartX.CreatedDate = localTime;
            order.cartX.LastUpdateDate = localTime;

            //Update cart item's time
            foreach (var item in order.cartX.cartItemsX) //We also have to update line item's required date
            {
                item.DueDate = order.RequiredDate.Value;
                item.RequiredDate = order.RequiredDate.Value;
                item.SupplierDueDate = order.RequiredDate.Value;
            }
        }

        public DateTime getLocalDateTime(DateTime sourceDateTime, User user = null, bool isUseUserTimeZone = false)
        {
            TimeSpan targetOffset = new TimeSpan();
            try
            {
                DateTime utcTime = sourceDateTime.ToUniversalTime();

                if (isUseUserTimeZone && user != null)
                    targetOffset = user.timeSpan;
                else
                    targetOffset = this.timezone.GetUtcOffset(utcTime);
                DateTime newTime = sourceDateTime;

                TimeSpan sourceOffset = sourceDateTime.Subtract(utcTime);
                newTime = sourceDateTime.Subtract(sourceOffset - targetOffset);
                return newTime;
            }
            catch (Exception ex)
            {
                return sourceDateTime;
            }
        }


        /// <summary>
        /// This function is used to convert date time in localization
        /// </summary>
        /// <param name="sourceDateTime"></param>
        /// <param name="store"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public string getLocalTime(DateTime sourceDateTime, User user = null, bool isUseUserTimeZone = false)
        {
            string localDateTime = "";
            string targetCulture;
            TimeSpan targetOffset = new TimeSpan();

            try
            {
                // Get CultureCode
                if (user != null && user.UserLanguages != null && user.UserLanguages.Length > 0)
                    targetCulture = user.UserLanguages[0];
                else
                    targetCulture = this.profile.CultureCode;

                if (isUseUserTimeZone && user != null)
                    targetOffset = user.timeSpan;
                else
                {
                    //时间显示为Store Team 所在区域时间(而非User所在区域时间)
                    DateTime utcTime = sourceDateTime.ToUniversalTime();
                    targetOffset = this.timezone.GetUtcOffset(utcTime);
                }

                if (targetOffset.Hours == 0 && targetOffset.Minutes == 0)
                    localDateTime = this.convertTimeAndFormat(sourceDateTime, targetCulture);
                else
                    localDateTime = this.convertTimeAndFormat(sourceDateTime, targetCulture, targetOffset);

                return localDateTime;
            }
            catch (Exception ex)
            {
                eStoreLoger.Warn("Failed to get local time in email template", "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// this method is to convert input time to the time of targeting timezone with targeting country time format
        /// </summary>
        /// <param name="sourceTime">sourceTime is Pacific Time Zone in default</param>
        /// <param name="targetCulture">language-Country, for example en-US</param>
        /// <param name="offset">This is UTC offset time</param>
        /// <returns></returns>
        public String convertTimeAndFormat(DateTime sourceTime, String targetCulture, TimeSpan targetOffset, Boolean dateOnly = false)
        {
            String result = "";

            try
            {
                DateTime newTime = sourceTime;

                if (sourceTime.Year < 3000 && sourceTime.Year > 1000) //only convert meaningful date
                {
                    DateTime utcTime = sourceTime.ToUniversalTime();
                    TimeSpan sourceOffset = sourceTime.Subtract(utcTime);
                    newTime = sourceTime.Subtract(sourceOffset - targetOffset);
                }

                result = getTimeInLocaleFormat(newTime, targetCulture, dateOnly);
            }
            catch (Exception ex)
            {
                //log error
                eStoreLoger.Error("Exception in Time convertion", sourceTime.ToString(), targetOffset.ToString(), "", ex);

                //use store default timezone
                DateTime newTime = TimeZoneInfo.ConvertTime(sourceTime, this.timezone);

                //use store default culture for output
                result = getTimeInLocaleFormat(newTime, null, dateOnly);    //use local culture
            }

            return result;
        }

        /// <summary>
        /// This method shall be used to convert source time to eStore local time
        /// </summary>
        /// <param name="sourceTime"></param>
        /// <param name="targetCulture"></param>
        /// <returns></returns>
        public String convertTimeAndFormat(DateTime sourceTime, String targetCulture, Boolean dateOnly = false)
        {
            String result = "";

            try
            {
                DateTime newTime = TimeZoneInfo.ConvertTime(sourceTime, this.timezone);  //use local time zone

                result = getTimeInLocaleFormat(newTime, targetCulture, dateOnly);
            }
            catch (Exception ex)
            {
                //log error
                eStoreLoger.Error("Exception in Time convertion", "", "", "", ex);

                //use store default timezone
                DateTime newTime = TimeZoneInfo.ConvertTime(sourceTime, this.timezone);

                //use store default culture for output
                result = getTimeInLocaleFormat(newTime, null, dateOnly);    //use local culture
            }

            return result;
        }

        /// <summary>
        /// This method is to convert the source value in store currency to user preferred currency.  If the user specified currency
        /// is not supported in current store, it returns 0.  Otherwise, it returns the exchanged value.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="targetCurrency"></param>
        /// <returns></returns>
        public Decimal getCurrencyExchangeValue(Decimal sourceValue, String targetCurrency)
        {


            if (SupportCurrencies.ContainsKey(targetCurrency))
                return getCurrencyExchangeValue(sourceValue, _supportCurrencies[targetCurrency]);
            else
                return 0;   //not supported currency
        }

        private Dictionary<String, Currency> _supportCurrencies = null;
        public Dictionary<String, Currency> SupportCurrencies
        {
            get
            {
                if (_supportCurrencies == null) //not initialized yet
                {
                    _supportCurrencies = new Dictionary<String, Currency>();
                    foreach (StoreCurrency storeCurrency in this.profile.StoreCurrencies)
                    {
                        if (storeCurrency != null && !String.IsNullOrEmpty(storeCurrency.CurrencyID))
                        {
                            Currency currency = CurrencyHelper.getCurrencybyID(storeCurrency.CurrencyID);
                            if (currency != null && !_supportCurrencies.ContainsKey(currency.CurrencyID)) //not in dictionary yet
                                _supportCurrencies.Add(currency.CurrencyID, currency);
                        }
                    }
                }
                return _supportCurrencies;
            }
        }
        private Currency _defaultCurrency = null;

        /// <summary>
        /// This method is to convert the source value in store currency to user preferred currency.  If the user specified currency
        /// is not supported in current store, it returns 0.  Otherwise, it returns the exchanged value.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="targetCurrency"></param>
        /// <returns></returns>
        public Decimal getCurrencyExchangeValue(Decimal sourceValue, Currency targetCurrency)
        {

            if (_defaultCurrency == null)
                _defaultCurrency = SupportCurrencies[this.profile.defaultCurrency.CurrencyID];

            if (targetCurrency != null)
            {
                Decimal converted = (sourceValue * _defaultCurrency.ToUSDRate.GetValueOrDefault()) / targetCurrency.ToUSDRate.GetValueOrDefault();
                return Converter.round(converted, this.storeID);
            }
            else
                return 0;   //not valid
        }

        /// <summary>
        /// This method is to retrieve products which are in a certain status, for example promotion / hot / new or esc...
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public IList<Product> getProducts(Product.PRODUCTSTATUS status, POCOS.MiniSite site, int take = 10, bool isOrderByLastUpdate = false)
        {
            if (isOrderByLastUpdate)
                return (new PartHelper()).getProductsByStateOrderByLastUpdate(status, this.profile, site, take);
            return (new PartHelper()).getProductsByState(status, this.profile, site, take);

        }

        public IList<Product> getProducts(Product.PRODUCTMARKETINGSTATUS status, POCOS.MiniSite site, int take = 10, bool isOrderByLastUpdate = false)
        {
            var ls = (new PartHelper()).getProductsByState(status, this.profile, site, take);
            if (isOrderByLastUpdate)
                return ls.OrderByDescending(c => c.LastUpdated).ToList();
            return ls;
        }

        public IList<Product> getProducts(List<Product.PRODUCTMARKETINGSTATUS> status, POCOS.MiniSite site, int take = 10, bool isOrderByLastUpdate = false)
        {
            List<Product> ls = new List<Product>();
            foreach (var item in status)
                ls.AddRange((new PartHelper()).getProductsByState(item, this.profile, site, take));
            return ls.Take(take).ToList();
        }

        public List<Product> getProducts(Product.PRODUCTSTATUS status, POCOS.MiniSite site, POCOS.ProductCategory pc, int take = 10)
        {
            var ls = (new PartHelper()).getCategoryOptionByState(status, this.profile, site, pc).OrderByDescending(c => c.ProductLastUpdated).Take(take).ToList();
            return ls;
        }

        public List<Product> getProducts(Product.PRODUCTMARKETINGSTATUS status, POCOS.MiniSite site, POCOS.ProductCategory pc, int take = 10)
        {
            var ls = (new PartHelper()).getCategoryOptionByState(status, this.profile, site, pc).OrderByDescending(c => c.ProductLastUpdated).Take(take).ToList();
            return ls;
        }

        /// <summary>
        /// This method is to retrieve products which is promototion and date between start date and end date
        /// </summary>
        /// <returns></returns>
        public List<Product> getPromotionProducts(POCOS.MiniSite site, int take = 10)
        {
            var ls = (new PartHelper()).getProductsByState(Product.PRODUCTMARKETINGSTATUS.PROMOTION, this.profile, site, take > 100 ? take : 100);
            return ls.Where(c => c.PromoteStart < DateTime.Now && c.promotionEnd > DateTime.Now).Take(take).ToList();
        }

        /// <summary>
        /// This method will find the matched productCategory using indicating categoryPath and return product list
        /// of this product category
        /// </summary>
        /// <param name="categoryPath"></param>
        /// <param name="products"></param>
        /// <returns></returns>
        public ProductCategory getProducts(string categoryPath, out List<Product> products)
        {
            CachePool cachePool = CachePool.getInstance();
            ProductCategory category = cachePool.getProductCategory(this.storeID, categoryPath);
            String productListKey = String.Format("{0}.Category.{1}.ProductList", storeID, categoryPath);
            Object list = cachePool.getObject(productListKey);
            if (category != null && list != null)
            {
                //find informaiton in cache pool
                products = (List<Product>)list;
                return category;
            }

            //not found in cache
            ProductCategoryHelper helper = new ProductCategoryHelper();
            category = helper.getProductCategories(storeID, categoryPath, out products);

            //cache product list and category
            cachePool.cacheProductCategory(category);
            cachePool.cacheObject(productListKey, products);

            return category;
        }

        /// <summary>
        /// This function will perform product search by keyword with an optional parameter indicating how many records in max
        /// the caller needs.  If the search is invoked from "product search hints", it comes with maxCount.   Otherwise, there won't
        /// be max count specified.
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public ProductSpecRules getMatchProducts(String keyword, bool includeinvalid = false, Int32 maxCount = 9999)
        {
            SpecHelper helper = new SpecHelper();

            if (maxCount != 9999)
                maxCount = maxCount * 2;    //increase buffer in case there is unexpected filtering

            ProductSpecRules specRules = helper.searchOnlybyKeywords(keyword.Trim(), storeID, maxCount, includeinvalid);

            return specRules;
        }
        public List<spProductSearch_Result> getMatchProducts(String keyword, Int32 maxCount = 9999, bool includeCategory = false)
        {
            if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < 2)
                return new List<spProductSearch_Result>();
            SpecHelper helper = new SpecHelper();

            if (maxCount != 9999)
                maxCount = maxCount * 2;    //increase buffer in case there is unexpected filtering

            List<spProductSearch_Result> result;
            if (includeCategory)
                result = helper.searchOnlybyKeywordsWithCategory(keyword.Trim(), storeID, maxCount);
            else
                result = helper.searchOnlybyKeywords(keyword.Trim(), storeID, maxCount);

            return result;
        }

        public List<ProductSearchOptimizedResult> getMatchProducts2(String keyword, Int32 maxCount = 9999, bool includeCategory = false)
        {
            if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < 2)
                return new List<ProductSearchOptimizedResult>();
            SpecHelper helper = new SpecHelper();

            if (maxCount != 9999)
                maxCount = maxCount * 2;    //increase buffer in case there is unexpected filtering

            List<ProductSearchOptimizedResult> result;

            result = helper.searchbyKeywords(keyword.Trim(), storeID, maxCount, includeCategory);

            return result;
        }
        public List<string> getftsparser(string keyword)
        {
            //int language = this.profile.getIntegerSetting("fulltext_languages", 1033);
            int language = 1033; // use en
            SpecHelper helper = new SpecHelper();

            var ls = helper.getftsparser(keyword, language);
            if (!ls.Contains(keyword))
                ls.Add(keyword);
            return ls.OrderByDescending(c => c.Length).ToList();
        }

        public ProductSpecRules getMatchProducts(String keyword, MiniSite minisite, bool includeinvalid = false, Int32 maxCount = 9999)
        {
            SpecHelper helper = new SpecHelper();

            if (maxCount != 9999)
                maxCount = maxCount * 2;    //increase buffer in case there is unexpected filtering

            ProductSpecRules specRules = helper.searchOnlybyKeywords(keyword.Trim(), minisite, maxCount, includeinvalid);

            return specRules;
        }

        public ProductSpecRules getMatchProducts(String categoryPath, String keyword)
        {
            SpecHelper helper = new SpecHelper();
            ProductSpecRules specRules = helper.getProductSpecRules(this.getProductCategory(categoryPath), keyword);

            return specRules;
        }

        public ProductSpecRules getMatchProducts(String categoryPath, List<VProductMatrix> specCriteria, String keyword)
        {
            SpecHelper helper = new SpecHelper();
            ProductSpecRules specRules = helper.getProductSpecRules(this.getProductCategory(categoryPath), keyword, specCriteria);

            return specRules;
        }

        /// <summary>
        /// Get productspec according to product type
        /// </summary>
        /// <param name="p"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<VProductMatrix> getProductSpec(Part p, string storeId = "")
        {
            PartHelper helper = new PartHelper();
            IList<VProductMatrix> matrix = p.specs;
            var attrls = getCTOSAttributes();
            return matrix.OrderBy(c => c.seq).ThenBy(c => getDefaultAttributelang(attrls.FirstOrDefault(a => a.AttrID == c.AttrID)).Sequence)
                .ThenBy(c => c.LocalAttributeName).ToList();
        }

        /// <summary>
        ///  列出CTOS所有System Spec選項用。
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<CTOSAttribute> getCTOSAttributes()
        {
            CTOSComparisionHelper componenthelper = new CTOSComparisionHelper();
            List<CTOSAttribute> ctoscomponent = componenthelper.getCtosAttribute(profile.StoreLangID);
            return ctoscomponent;
        }

        /// <summary>
        /// 获取Store对应的 Attributelang
        /// </summary>
        /// <param name="ctosAttr"></param>
        /// <returns></returns>
        public CTOSAttributelang getDefaultAttributelang(CTOSAttribute ctosAttr)
        {
            CTOSAttributelang alt = null;
            if (ctosAttr != null && ctosAttr.CTOSAttributelangs != null && ctosAttr.CTOSAttributelangs.Count > 0)
                alt = ctosAttr.CTOSAttributelangs.FirstOrDefault(c => c.LangID.Trim() == profile.StoreLangID);
            return alt ?? new CTOSAttributelang
            {
                AttrID = ctosAttr.AttrID,
                LangID = profile.StoreLangID,
                LangValue = "",
                Sequence = null
            };
        }

        public IList<Part> getMatchParts(String keyword)
        {
            PartHelper helper = new PartHelper();
            List<Part> matchParts = helper.searchPartsOrderbyPN(keyword, this.profile);

            return matchParts;
        }

        public List<Product_Ctos> searchCTOSbyPN(string partno)
        {
            PartHelper helper = new PartHelper();
            List<Product_Ctos> matchParts = helper.searchCTOSbyPN(partno, this.profile);
            return matchParts;
        }

        /// <summary>
        /// For WWW website, to identify if this partno is a part of Product CTOS
        /// </summary>
        /// <param name="partno"></param>
        /// <returns></returns>
        public bool iseStoreCTOSPart(string storeid, string partno)
        {
            PartHelper helper = new PartHelper();

            bool rlt = false;
            try
            {
                rlt = helper.iseStoreCTOSPart(partno.ToUpper(), storeid.ToUpper());
            }
            catch (Exception ex)
            {

                eStoreLoger.Error("iseStoreproduct Error", "", "", "", ex);
                rlt = false;
            }

            return rlt;
        }

        /// <summary>
        /// For WWW website, return the estore link 
        /// </summary>
        /// <param name="partno"></param>
        /// <returns></returns>
        public void geteStoreProductLink(string storeid, string partno, out List<String> products, out List<String> ctoss)
        {
            PartHelper helper = new PartHelper();
            List<String> _part;
            List<String> _ctos;
            try
            {
                helper.iseStoreproduct(partno, storeid, out _part, out _ctos);
                ctoss = _ctos;
                products = _part;

            }
            catch (Exception ex)
            {
                eStoreLoger.Error(ex.Message, "iseStoreproduct Error", "", "", ex);
                products = null;
                ctoss = null;

            }


        }


        /// <summary>
        /// Form WWW website use, use it identify if the component partno is sell on estore.
        /// </summary>
        /// <param name="partno"></param>
        /// <returns></returns>
        public bool iseStoreproduct(string storeid, string partno)
        {
            PartHelper helper = new PartHelper();

            bool rlt = false;
            try
            {
                rlt = helper.iseStoreproduct(partno, storeid);
            }
            catch (Exception ex)
            {

                eStoreLoger.Error("iseStoreproduct Error", "", "", "", ex);
                rlt = false;
            }

            return rlt;
        }

        public Dictionary<string, string> getOrderbyPNHint(String keyword)
        {
            PartHelper helper = new PartHelper();
            Dictionary<string, string> matchParts = helper.getOrderbyPNHint(keyword, this.profile, false);

            return matchParts;
        }

        public bool isValidOrderbyPN(string partno)
        {
            if (this.profile.OrderByPartnoExcludeRules == null && this.profile.OrderByPartnoExcludeRules.Any() == false)
                return true;

            List<string> starwith = (from or in this.profile.OrderByPartnoExcludeRules
                                     where or.MatchCriteria.Equals("STARTWITH")
                                     select or.ExcludePartno).ToList<string>();

            if (starwith.Any(x => partno.StartsWith(x)))
                return false;

            List<string> endwith = (from or in this.profile.OrderByPartnoExcludeRules
                                    where or.MatchCriteria.Equals("ENDWITH")
                                    select or.ExcludePartno).ToList<string>();
            if (starwith.Any(x => partno.EndsWith(x)))
                return false;

            return true;

        }

        /// <summary>
        /// This method is to retrieve ProductCategory by categoryPath. Make sure you are not confused and misuse it with category Id
        /// </summary>
        /// <param name="categoryPath"></param>
        /// <returns></returns>
        public ProductCategory getProductCategory(String categoryPath)
        {
            CachePool cachePool = CachePool.getInstance();
            ProductCategory category = CachePool.getInstance().getProductCategory(storeID, categoryPath);

            if (category == null)
            {
                ProductCategoryHelper helper = new ProductCategoryHelper();
                category = helper.getStoreProductCategories(storeID).FirstOrDefault(x => x.CategoryPath.ToLower() == categoryPath.ToLower());

                if (category != null)
                    cachePool.cacheProductCategory(category);
            }

            return category;
        }

        public ProductCategory getProductCategoryForCategoryPathSEO(String categoryPathSEO)
        {
            CachePool cachePool = CachePool.getInstance();
            ProductCategory category = CachePool.getInstance().getProductCategory(storeID, categoryPathSEO);

            try
            {
                if (category == null)
                {
                    ProductCategoryHelper helper = new ProductCategoryHelper();
                    category = helper.getStoreProductCategories(storeID).FirstOrDefault(x => x.CategoryPathSEO.ToLower() == categoryPathSEO.ToLower());

                    if (category != null)
                        cachePool.cacheProductCategory(category);
                }
            }
            catch (Exception ex)
            {
            }

            return category;
        }


        /// <summary>
        /// get product category by storeid and path ,
        /// can include delete category.
        /// </summary>
        /// <param name="categorypath"></param>
        /// <param name="includeDelete"></param>
        /// <returns></returns>
        public ProductCategory getProductCateogryWithNonFilterForCategoryPathSEO(string categorypathSEO, bool includeDelete = false)
        {
            return (new ProductCategoryHelper()).getProductCategoryForProductCategorySEO(categorypathSEO, this.profile.StoreID, true);
        }

        public ProductCategory getProductCateogryWithNonFilter(string categorypath, bool includeDelete = false)
        {
            return (new ProductCategoryHelper()).getProductCategory(categorypath, this.profile.StoreID, true);
        }

        public List<ProductCategory> GetSearchDiffSiteCategory(string fromStore, string toStore, string categorypath)
        {
            return (new ProductCategoryHelper()).GetSearchDiffSiteCategory(fromStore, toStore, categorypath);
        }

        public ProductCategory getProductCategory(int categoryId)
        {
            ProductCategoryHelper helper = new ProductCategoryHelper();
            return helper.getProductCategory(categoryId, this.storeID);
        }

        public Dictionary<string, string> getSettings()
        {
            return getSettings(this.profile.Country);
        }

        public Dictionary<string, string> getSettings(POCOS.Country country)
        {
            if (country == null)
                return this.profile.Settings;
            else
                return country.getSettings(this.profile);
        }

        public List<ChannelPartner> getAvailableChannelPartners(string countryName, Cart cart)
        {
            if (cart != null)
            {
                string businessGroup = cart.businessGroup != POCOS.Store.BusinessGroup.None ? cart.businessGroup.ToString() : "eP";
                return ChannelPartnerHelper.getChannelPartners(countryName, businessGroup);
            }
            return new List<ChannelPartner>();
        }

        /// <summary>
        /// get channel parthner by channel id except Advantech
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public ChannelPartner getChannelPartner(int channelId)
        {
            ChannelPartner ChannelAccount = ChannelPartnerHelper.getChannelPartner(channelId);
            return ChannelAccount;
        }

        public List<ProductCategory> getRootProductCategories(string storeid)
        {
            ProductCategoryHelper _hepler = new ProductCategoryHelper();
            return _hepler.getProductCategories(storeid).Where(c => (string.IsNullOrEmpty(c.CategoryStatus) || !c.CategoryStatus.Equals("delete", StringComparison.OrdinalIgnoreCase))).ToList();
        }

        public List<ProductCategory> getTopLevelStandardProductCategories(POCOS.MiniSite site)
        {
            ProductCategoryHelper helper = new ProductCategoryHelper();
            return helper.getStandardRootProductCategory(storeID).Where(x => x.MiniSite == site).OrderBy(x => x.Sequence).ThenBy(x => x.localCategoryNameX).ToList();
        }

        public List<ProductCategory> getTopLevelCategories()
        {
            ProductCategoryHelper helper = new ProductCategoryHelper();
            return helper.getTopLevelCategories(storeID);

        }

        public List<ProductCategory> getTopLevelCTOSProductCategories(POCOS.MiniSite site)
        {
            ProductCategoryHelper helper = new ProductCategoryHelper();
            return helper.getCTOSRootProductCategory(storeID).Where(x => x.MiniSite == site).OrderBy(x => x.Sequence).ThenBy(x => x.localCategoryNameX).ToList();
        }

        public List<POCOS.Application> getTopLevelApplications(POCOS.MiniSite site)
        {
            ProductCategoryHelper helper = new ProductCategoryHelper();
            return helper.getRootApplication(storeID).Where(x => x.MiniSite == site).ToList();
        }
        public List<POCOS.ProductCategory> getTopLeveluStoreCategories(POCOS.MiniSite site)
        {
            ProductCategoryHelper helper = new ProductCategoryHelper();
            return helper.getuStoreRootCategory(storeID).Where(x => x.MiniSite == site).ToList(); ;
        }
        public IList<POCOS.Menu> getTopLevelSolutionMenus(POCOS.MiniSite site)
        {
            POCOS.Menu solutionMenu = this.getMenuItems(site).FirstOrDefault(m => m.MenuID == 1341);
            if (solutionMenu != null)
                return solutionMenu.subMenusX;
            return new List<POCOS.Menu>();
        }

        /********************************
         * Following section has the implementation of order by part no related methods
         ******************************/

        /// <summary>
        /// This method will try to fetch vendor part information directly from vendor communication and create product instance
        /// for it.  The product instance created by this method does not exist in eStore DB.  If user would like to order this part
        /// , the part would need to be stored in eStore before preceed.  The method to add vendor part to store is addVendorPartToStore
        /// </summary>
        /// <param name="partNo"></param>
        /// <returns></returns>
        public Part getVendorPartForOrderbyPartNo(String partNo)
        {
            PartHelper helper = new PartHelper();
            return helper.getPartOrderbyPartno(partNo, this.profile);
        }

        /// <summary>
        /// This method will fetech part from vendor communication interface and store new prodcut into Store DB.
        /// </summary>
        /// <param name="partNo"></param>
        /// <returns></returns>
        public Product addVendorPartToStore(String partNo)
        {
            PartHelper helper = new PartHelper();
            return helper.AddtoStore(partNo, this.profile);
        }

        /// <summary>
        /// add vendor part to part not product
        /// </summary>
        /// <param name="partNo"></param>
        /// <returns></returns>
        public Part addVendorPartToPart(string partNo)
        {
            PartHelper helper = new PartHelper();
            return helper.AddParttoStore(partNo, this.profile);
        }
        public List<Advertisement> getiServiceHomeBanners(POCOS.MiniSite site)
        {
            try
            {
                AdvertisementHelper helper = new AdvertisementHelper();
                List<Advertisement> ads = new List<Advertisement>();
                ads.AddRange(helper.getAdsByAdType(storeID, Advertisement.AdvertisementType.TodayHighLight));
                ads.AddRange(helper.getAdsByAdType(storeID, Advertisement.AdvertisementType.HomeBanner));
                return ads.Where(x => x.MiniSite == site).OrderByDescending(x => x.weight).ToList();
            }
            catch (Exception)
            {

                return new List<Advertisement>();
            }
        }

        public List<Advertisement> getAds(ProductCategory _pc, Advertisement.AdvertisementType adType = Advertisement.AdvertisementType.NotSpecified)
        {
            AdvertisementHelper helper = new AdvertisementHelper();
            return helper.getAds(_pc, adType);
        }


        ///
        /// Following section has definition of Ad banner management methods
        ///

        /// <summary>
        /// This method is to provide Ad banners based on the input keyword criteria
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public IList<Advertisement> getAdBanner(POCOS.MiniSite site, Dictionary<String, String> keywords)
        {
            AdvertisementHelper helper = new AdvertisementHelper();
            List<Advertisement> ads = new List<Advertisement>();
            Dictionary<String, Advertisement> adList = new Dictionary<String, Advertisement>();

            try
            {
                if (keywords == null || keywords.Count == 0)
                {
                    //default page
                    mergeAdList(adList, getQualifiedDefaultAds());
                }
                else
                {
                    if (keywords.ContainsKey("HomePage"))
                    {
                        //only TodayHighLight Ads are applicable to Home page
                        ads.AddRange(helper.getAdsByAdType(storeID, Advertisement.AdvertisementType.TodayHighLight));
                        ads.AddRange(helper.getAdsByAdType(storeID, Advertisement.AdvertisementType.CenterPopup, "homepage"));
                        ads.AddRange(helper.getAdsByAdType(storeID, Advertisement.AdvertisementType.Floating));
                        ads.AddRange(helper.getAdsByAdType(storeID, Advertisement.AdvertisementType.Expanding));
                        return ads.Where(x => x.MiniSite == site).OrderByDescending(x => x.weight).ToList();
                    }
                    //find product matching Ads

                    //Add banner after finished order
                    if (keywords.ContainsKey("CompleteOrder"))
                    {
                        ads.AddRange(helper.getAdsByAdType(storeID, Advertisement.AdvertisementType.CenterPopup, "CompleteOrder"));
                        return ads.Where(x => x.MiniSite == site).OrderByDescending(x => x.weight).ToList();
                    }
                    //Add banner after finished quote
                    if (keywords.ContainsKey("CompleteQuote"))
                    {
                        ads.AddRange(helper.getAdsByAdType(storeID, Advertisement.AdvertisementType.CenterPopup, "CompleteQuote"));
                        return ads.Where(x => x.MiniSite == site).OrderByDescending(x => x.weight).ToList();
                    }

                    if (keywords.ContainsKey("ProductID"))
                    {
                        String productId = keywords["ProductID"];
                        //ads.AddRange(getQualifiedProductAds(productId));
                        mergeAdList(adList, getQualifiedProductAds(productId));
                        keywords.Remove("ProductID");
                    }
                    //find category matching Ads
                    if (keywords.ContainsKey("CategoryID"))
                    {
                        String categoryId = keywords["CategoryID"];
                        //ads.AddRange(getQualifiedCategoryAds(categoryId));
                        mergeAdList(adList, getQualifiedCategoryAds(categoryId));
                        keywords.Remove("CategoryID");
                    }
                    //find keyword match ******** reserved for further extension
                    if (keywords.ContainsKey("Keywords"))
                    {
                        String Keywords = keywords["Keywords"];
                        //ads.AddRange(getQualifiedCategoryAds(categoryId));
                        mergeAdList(adList, getQualifiedKeywordAds(Keywords));
                        keywords.Remove("Keywords");
                    }
                    if (keywords.Count > 0)//for other keys
                    {
                        //ads.AddRange(getQualifiedKeywordAds(keywords));
                        //mergeAdList(adList, getQualifiedKeywordAds(keywords));
                    }
                }

                ads = (from ad in adList.Values
                       where ad.weight > -100 && !String.IsNullOrWhiteSpace(ad.Imagefile)
                       && ad.MiniSite == site//(ad.MiniSite == null || ad.MiniSite == site)
                       orderby ad.weight descending
                       select ad).ToList();

                if (keywords.ContainsKey("GoldEnggs"))
                {
                    if (isEnableSetUp("EableGoldEnggsGift", this.storeID, site))
                        ads.AddRange(helper.getAdsByAdType(storeID, Advertisement.AdvertisementType.GoldenEggs));
                }
            }
            catch (Exception)
            {
                //ignore
            }

            return ads;
        }

        public List<Advertisement> getOrderOrQuoteCompleteBanner(string storeID, Advertisement.AdvertisementType advtype, string type, MiniSite site = null)
        {
            AdvertisementHelper helper = new AdvertisementHelper();
            try
            {
                return helper.getAdsByAdType(storeID, advtype, type);
            }
            catch
            {
                return new List<Advertisement>();
            }
        }

        /// <summary>
        /// This method is to merge new Advertisement items to existing list.  
        /// The return from this method will contain unique advertisement item list.
        /// </summary>
        /// <param name="adList"></param>
        /// <param name="newList"></param>
        protected void mergeAdList(Dictionary<String, Advertisement> adList, List<Advertisement> newList, int weightedPoint = 1)
        {
            if (adList != null && newList != null)
            {
                foreach (Advertisement ad in newList)
                {
                    ad.weight = ad.weight * weightedPoint;
                    String key = String.Format("{0}.{1}", ad.segmentType.ToString(), ad.id);
                    if (adList.ContainsKey(key))
                    {
                        Advertisement existAd = adList[key];
                        if (existAd == null || existAd.weight < ad.weight)
                        {
                            adList.Remove(key);
                            adList.Add(key, ad);
                        }
                    }
                    else
                        adList.Add(key, ad);
                }
            }
        }

        virtual protected List<Advertisement> getQualifiedProductAds(String productId)
        {
            AdvertisementHelper helper = new AdvertisementHelper();
            //Dictionary to filter duplicate entries
            Dictionary<String, Advertisement> adList = new Dictionary<String, Advertisement>();

            //below ad type can be dispayed in product page
            List<Advertisement.AdvertisementType> adtypes = new List<Advertisement.AdvertisementType>();
            adtypes.Add(Advertisement.AdvertisementType.StoreAds);
            adtypes.Add(Advertisement.AdvertisementType.Resources);
            adtypes.Add(Advertisement.AdvertisementType.Floating);
            adtypes.Add(Advertisement.AdvertisementType.Expanding);
            try
            {
                if (!String.IsNullOrEmpty(productId))
                {
                    Product product = getProduct(productId);
                    if (product != null)
                    {
                        //in present version eStore will only support StoreAds type
                        List<Advertisement> ads = helper.getAds(product, adtypes);

                        mergeAdList(adList, ads, 2000);


                        if (product.productCategories != null)
                        {
                            foreach (ProductCategory pc in product.productCategories)
                            {
                                List<Advertisement> categoryAds = helper.getAds(pc, adtypes);
                                mergeAdList(adList, categoryAds, 1000);
                            }
                        }
                    }
                    //find default Ads
                    mergeAdList(adList, helper.getDefaultAdv(product.StoreID, Advertisement.AdvertisementType.StoreAds));
                }
            }
            catch (Exception)
            {
                //ignore
            }

            return adList.Values.OrderByDescending(ad => ad.weight).ToList();
        }

        virtual protected List<Advertisement> getQualifiedCategoryAds(String categoryId)
        {
            AdvertisementHelper helper = new AdvertisementHelper();
            //Dictionary to filter duplicate entries
            Dictionary<String, Advertisement> adList = new Dictionary<String, Advertisement>();
            try
            {
                List<Advertisement> ads = new List<Advertisement>();
                if (!String.IsNullOrEmpty(categoryId))
                {
                    //below ad type can be dispayed in product category page
                    List<Advertisement.AdvertisementType> adtypes = new List<Advertisement.AdvertisementType>();
                    adtypes.Add(Advertisement.AdvertisementType.StoreAds);
                    adtypes.Add(Advertisement.AdvertisementType.Resources);
                    adtypes.Add(Advertisement.AdvertisementType.Floating);
                    adtypes.Add(Advertisement.AdvertisementType.Expanding);
                    adtypes.Add(Advertisement.AdvertisementType.CenterPopup);
                    ProductCategory category = getProductCategory(categoryId);

                    ads = helper.getAds(category, adtypes);
                    mergeAdList(adList, ads, 1000);
                }

                //find default Ads
                mergeAdList(adList, helper.getDefaultAdv(storeID, Advertisement.AdvertisementType.StoreAds));
            }
            catch (Exception)
            {
                //ignore
            }

            return adList.Values.OrderByDescending(ad => ad.weight).ToList();
        }

        public string[] getLiveEngageSections(POCOS.Product product)
        {
            CachePool cachePool = CachePool.getInstance();
            string key = $"{this.storeID}.LiveEngageSections.Product.{product.SProductID}";

            string[] rlt = (string[])cachePool.getObject(key);
            if (rlt == null)
            {
                AdvertisementHelper helper = new AdvertisementHelper();
                List<Advertisement> ads = new List<Advertisement>();
                List<Advertisement.AdvertisementType> adtypes = new List<Advertisement.AdvertisementType>();
                adtypes.Add(Advertisement.AdvertisementType.LiveEngageSection);
                ads = helper.getAds(product, adtypes);
                if (ads != null && ads.Any())
                {
                    rlt = ((Advertisement.LiveEngageSection)ads.First().complexAdvertisementContent).Sections.ToArray();
                }
                else
                    rlt = new string[] { };
                cachePool.cacheObject(key, rlt, CachePool.CacheOption.Hour2);

            }
            
            return rlt;
        }
        public string[] getLiveEngageSections(POCOS.ProductCategory productcategory)
        {
            CachePool cachePool = CachePool.getInstance();
            string key = $"{this.storeID}.LiveEngageSections.ProductCategory.{productcategory.CategoryID}";

            string[] rlt = (string[])cachePool.getObject(key);
            if (rlt == null)
            {
                AdvertisementHelper helper = new AdvertisementHelper();
                List<Advertisement> ads = new List<Advertisement>();
                List<Advertisement.AdvertisementType> adtypes = new List<Advertisement.AdvertisementType>();
                adtypes.Add(Advertisement.AdvertisementType.LiveEngageSection);
                ads = helper.getAds(productcategory, adtypes);
                if (ads != null && ads.Any())
                {
                    rlt = ((Advertisement.LiveEngageSection)ads.First().complexAdvertisementContent).Sections.ToArray();
                }
                else
                    rlt = new string[] { };
                cachePool.cacheObject(key, rlt, CachePool.CacheOption.Hour2);

            }

            return rlt;
        }

        /// <summary>
        /// This method is not implemented yet and will be reserved for further Ad expension
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        private List<Advertisement> getQualifiedKeywordAds(string keywords)
        {
            AdvertisementHelper helper = new AdvertisementHelper();
            List<Advertisement> ads = new List<Advertisement>();

            Dictionary<String, Advertisement> adList = new Dictionary<String, Advertisement>();
            try
            {
                if (!String.IsNullOrEmpty(keywords))
                {

                    ads = helper.getAds(keywords, this.storeID, true);
                    mergeAdList(adList, ads, 1500);
                }

                //find default Ads
                mergeAdList(adList, helper.getDefaultAdv(storeID, Advertisement.AdvertisementType.StoreAds));
            }
            catch (Exception)
            {
                //ignore
            }

            return adList.Values.OrderByDescending(ad => ad.weight).ToList();
        }

        /// <summary>
        /// This method is not implemented yet and will be reserved for further Ad expension
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        private List<Advertisement> getQualifiedDefaultAds()
        {
            AdvertisementHelper helper = new AdvertisementHelper();
            //Dictionary to filter duplicate entries
            Dictionary<String, Advertisement> adList = new Dictionary<String, Advertisement>();
            try
            {
                List<Advertisement> ads = new List<Advertisement>();
                //find default Ads
                mergeAdList(adList, helper.getDefaultAdv(storeID, Advertisement.AdvertisementType.StoreAds));
            }
            catch (Exception)
            {
                //ignore
            }

            return adList.Values.OrderByDescending(ad => ad.weight).ToList();
        }

        /// <summary>
        /// This property retains list of education advertisement ad
        /// </summary>
        public IList<Advertisement> educationColumns
        {
            get
            {
                if (_educationColumns == null)
                    setHomePageStaticAdItems();

                return _educationColumns;
            }
        }
        private List<Advertisement> _educationColumns = null;

        /// <summary>
        /// This property retains list of education advertisement ad
        /// </summary>
        public IList<Advertisement> todaysDealsColumns
        {
            get
            {
                if (_todaysDealsColumns == null)
                    setHomePageStaticAdItems();

                return _todaysDealsColumns;
            }
        }
        private List<Advertisement> _todaysDealsColumns = null;

        /// <summary>
        /// This property return adam forum advertisement
        /// </summary>
        public Advertisement adamForum
        {
            get
            {
                if (_adamForum == null)
                    setHomePageStaticAdItems();

                return _adamForum;
            }
        }
        private Advertisement _adamForum = null;

        /// <summary>
        /// This property return home banners
        /// </summary>
        public List<Advertisement> getHomeBanners(POCOS.MiniSite miniSite, bool isFullSize = false)
        {

            if (_homeBanners == null)
                setHomePageStaticAdItems();
            if (isFullSize)
                return _homeBanners.Where(x => x.MiniSite == miniSite && x.segmentType == Advertisement.AdvertisementType.FullColumnBanner).ToList();
            else
                return _homeBanners.Where(x => x.MiniSite == miniSite && x.segmentType == Advertisement.AdvertisementType.HomeBanner).ToList();

        }
        private List<Advertisement> _homeBanners = null;

        /// <summary>
        /// The following section has methods for cache management
        /// </summary>
        /// <returns></returns>
        public List<String> getCacheItemList()
        {
            return CachePool.getInstance().getStoreCacheList(this.storeID);
        }

        #region cache management

        public void releaseEntireCache()
        {
            CachePool.getInstance().releaseStoreCacheItems(this.storeID);
        }

        public void releaseCacheItem(String key)
        {
            CachePool.getInstance().releaseCacheItem(key);
        }

        public void releaseCacheItems(List<String> items)
        {
            CachePool.getInstance().releaseCacheItems(items);
        }

        public void releaseStoreCacheItems(String storeID)
        {
            CachePool.getInstance().releaseStoreCacheItems(this.storeID);
        }

        public void releaseStoreCacheProducts()
        {
            CachePool.getInstance().releaseStoreCacheProducts(this.storeID);
        }
        //remove product cache by SproductId
        public void releaseStoreCacheProduct(String SProductID)
        {
            CachePool.getInstance().releaseStoreCacheProduct(this.storeID, SProductID);
        }
        public void resetProductStoreUrl(string sproductid)
        {
            Product product = getProduct(sproductid);
            product.StoreUrl = eStore.BusinessModules.UrlRewrite.UrlRewriteModel.getMappingUrl(product);
            product.save();
        }
        public void releaseStoreCacheProductCategories()
        {
            CachePool.getInstance().releaseStoreCacheProductCategories(this.storeID);
        }
        public void releaseStoreCacheProductCategory(String CategoryPath, string CategoryID = null)
        {
            CachePool.getInstance().releaseStoreCacheProductCategory(this.storeID, CategoryPath, CategoryID);
        }
        public void releaseStoreCacheCampaign()
        {
            CachePool.getInstance().releaseStoreCacheCampaign(this.storeID);
        }

        public void releaseStoreCacheStore(string storeID, String storeURL)
        {
            CachePool.getInstance().releaseStoreCacheStore(this.storeID, storeID, storeURL);
        }

        public void releaseStoreCacheCart()
        {
            CachePool.getInstance().releaseStoreCacheCart(this.storeID);
        }

        public void releaseStoreCacheQuotation()
        {
            CachePool.getInstance().releaseStoreCacheQuotation(this.storeID);
        }
        #endregion



        public List<Advertisement> sliderBanner(string keywords)
        {
            var lis = getQualifiedKeywordAds(keywords);
            return lis.Where(c => c.segmentOMType == Advertisement.AdvertisementOMType.SliderBanner).ToList();
        }
        public List<Advertisement> sliderBanner(POCOS.ProductCategory category, bool fullsize = false, bool includeparentsAds=false)
        {
            AdvertisementHelper helper = new AdvertisementHelper();
            //Dictionary to filter duplicate entries
            Dictionary<String, Advertisement> adList = new Dictionary<String, Advertisement>();
            try
            {
                List<Advertisement> ads = new List<Advertisement>();
                if (category != null)
                {
                    //below ad type can be dispayed in product category page
                    List<Advertisement.AdvertisementType> adtypes = new List<Advertisement.AdvertisementType>();
                    //If paps == true, use FullBanner
                    if (fullsize == true)
                        adtypes.Add(Advertisement.AdvertisementType.FullColumnBanner);
                    else
                        adtypes.Add(Advertisement.AdvertisementType.SliderBanner);
                    ads = helper.getAds(category, adtypes);
                    mergeAdList(adList, ads, 1000);
                  
                    if (includeparentsAds && category.parentCategoryX != null)
                    {
                        int maxlevel = 5;
                        var parentCategory = category.parentCategoryX;
                        do
                        {
                            var parentads = helper.getAds(parentCategory, adtypes);
                            mergeAdList(adList, parentads, 1005-maxlevel);
                            parentCategory = parentCategory.parentCategoryX;
                            maxlevel--;
                        }
                        while (parentCategory != null && maxlevel>0);
                    }
                 
                }
            }
            catch (Exception)
            {
                //ignore
            }

            return adList.Values.OrderByDescending(ad => ad.weight).ToList();
        }
        private void setHomePageStaticAdItems()
        {
            try
            {
                AdvertisementHelper helper = new AdvertisementHelper();
                Dictionary<String, Advertisement> adList = new Dictionary<string, Advertisement>();
                mergeAdList(adList, helper.getHomeAdv(this.storeID));
                List<Advertisement> ads = adList.Values.ToList();

                if (ads != null)
                {
                    _educationColumns = (from ad in ads
                                         where ad.segmentType == Advertisement.AdvertisementType.EducationColumn
                                         orderby ad.weight descending
                                         select ad).ToList();
                    _adamForum = (from ad in ads
                                  where ad.segmentType == Advertisement.AdvertisementType.SeeWhatsNew
                                    && ad.Title == "ADAM Forum"
                                  select ad).FirstOrDefault();
                    _homeBanners = (from ad in ads
                                    where ad.segmentType == Advertisement.AdvertisementType.FullColumnBanner
                                    || ad.segmentType == Advertisement.AdvertisementType.HomeBanner
                                    orderby ad.weight descending
                                    select ad).ToList();

                    _todaysDealsColumns = (from ad in ads
                                           where ad.segmentType == Advertisement.AdvertisementType.TodaysDeals
                                           orderby ad.weight descending
                                           select ad).ToList();
                }
            }
            catch (Exception)
            {
                _educationColumns = new List<Advertisement>();
            }
        }

        /// <summary>
        /// This method is to convert input time to the targeting culture format
        /// </summary>
        /// <param name="sourceTime"></param>
        /// <param name="targetCulture"></param>
        /// <returns></returns>
        private String getTimeInLocaleFormat(DateTime sourceTime, String targetCulture, Boolean dateOnly = false)
        {
            String result = "";

            try
            {
                CultureInfo culture = this.cultureInfo;
                if (!String.IsNullOrEmpty(targetCulture))
                    culture = new CultureInfo(targetCulture);

                if (dateOnly)
                    result = sourceTime.ToString(culture.DateTimeFormat.ShortDatePattern);
                else
                    result = sourceTime.ToString(culture.DateTimeFormat);
            }
            catch (Exception ex)
            {
                //log error
                eStoreLoger.Error("Exception in Time convertion", "", "", "", ex);

                //use store default culture for output
                if (dateOnly)
                    result = sourceTime.ToString(this.cultureInfo.DateTimeFormat.ShortDatePattern);
                else
                    result = sourceTime.ToString(this.cultureInfo.DateTimeFormat);
            }

            return result;
        }

        private CultureInfo _defaultCulture = null;
        private CultureInfo cultureInfo
        {
            get
            {
                if (_defaultCulture == null)
                    _defaultCulture = new CultureInfo(this.profile.CultureCode); //this shall be replace with store default culture setting ******

                return _defaultCulture;
            }

            set
            {
                _defaultCulture = value;
            }
        }

        private TimeZoneInfo _defaultTimeZone = null;
        private TimeZoneInfo timezone
        {
            get
            {
                if (_defaultTimeZone == null)
                    _defaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById(this.profile.TimeZone);

                return _defaultTimeZone;
            }
        }

        /// <summary>
        /// This method is to performance authentication via SSO connection. It returns authentication key returned from SSO service provider
        /// if the provided credential is valid.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="userHostIP"></param>
        /// <returns></returns>
        public String SSOAuthenticate(String userId, String password, String userHostIP)
        {
            if (String.IsNullOrWhiteSpace(userId) || String.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(userHostIP))
                return null;

            userId = userId.Trim();
            password = password.Trim();
            userHostIP = userHostIP.Trim();

            String authKey = null;
            try
            {
                authKey = sso.login(userId, password, this.profile.StoreMembershippass, userHostIP);
                if (String.IsNullOrWhiteSpace(authKey))
                    return null;    //authentication fails
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at SSOAuthentication", userId, userHostIP, storeID, ex);
            }

            return authKey;
        }

        /// <summary>
        /// This method is to retrieve user profile from SSO service provider
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SSOUSER SSOGetUserProfile(String userId)
        {
            SSOUSER profile = null;

            try
            {
                profile = sso.getProfile(userId, this.profile.StoreMembershippass);
            }
            catch (Exception)
            {
            }

            return profile;
        }

        /// <summary>
        /// This method is to validate user registion completion.  Some early registerred user doesn't fill
        /// up complete registration information due to less restrictions in early registration process.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private Boolean SSOCheckRegistrationCompletion(String userId)
        {
            Boolean complete = false;

            try
            {
                complete = sso.chkReqFields(userId, this.profile.StoreMembershippass);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Registration status check failure", userId, "", this.profile.StoreID, ex);
            }

            return complete;
        }

        private Boolean isValidCompanyId(String companyId)
        {
            Boolean validity = true;

            if (String.IsNullOrWhiteSpace(companyId))
                return false;

            if (companyId.Equals("WB000000"))
                return false;

            return validity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public bool isValidatedShiptoAddress(string countrycode, User user = null)
        {

            if (user != null && user.actingRole == User.Role.Employee)
            {
                return true;
            }
            else
            {
                string addtionalShipToCountry = this.profile.getStringSetting("Ship_To_Country");

                if (!string.IsNullOrEmpty(countrycode)
                    && (
                        this.profile.Countries.FirstOrDefault(c => c.Shorts == countrycode) != null
                            || (
                            string.IsNullOrEmpty(addtionalShipToCountry) == false
                            && addtionalShipToCountry.Split(',').Contains(countrycode)
                            )
                        )
                    )
                {
                    return true;
                }
                else
                    return false;
            }
        }

        public AddressValidator.ShippingAddressValidationResult isValidateUSAShiptoAddress(CartContact contact, string shippingmethod, User user)
        {
            AddressValidator.ValidatationProvider provider = AddressValidator.ValidatationProvider.UPS;
            AddressValidator.ShippingAddress orderAddress = new AddressValidator.ShippingAddress(contact);
            orderAddress.ERPID = user.CompanyID;

            if (shippingmethod.StartsWith("UPS"))
                provider = AddressValidator.ValidatationProvider.UPS;
            else if (shippingmethod.ToUpper().StartsWith("FEDEX"))
                provider = AddressValidator.ValidatationProvider.Fedex;

            AddressValidator addressValidator = new AddressValidator();
            AddressValidator.ShippingAddressValidationResult addressResult =
                                                            addressValidator.validate(orderAddress, provider, user.UserID);
            return addressResult;
        }

        public bool isValidatedBilltoAddress(string countrycode, User user = null)
        {

            if (user != null && user.actingRole == User.Role.Employee)
            {
                return true;
            }
            else
            {
                if (this.profile.Countries.FirstOrDefault(c => c.Shorts == countrycode) != null)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// This method is to retrieve country particular business information
        /// </summary>
        /// <param name="countryName"></param>
        /// <param name="businessGroup"></param>
        /// <returns></returns>
        public POCOS.Address getAddressByCountry(String countryName, eStore.POCOS.Store.BusinessGroup businessGroup)
        {
            return getAddressByCountry(this.profile.getCountry(countryName), businessGroup);
        }

        public POCOS.Address getAddressByCountry(Country country, eStore.POCOS.Store.BusinessGroup businessGroup)
        {
            POCOS.Address tmpstoreAddress = null;
            POCOS.Address storeAddress = this.profile.getStoreAddress(businessGroup);

            if (country != null && country.CountryAddresses != null)
            {
                tmpstoreAddress = country.CountryAddresses
                     .Where(x => x.Division.ToUpper() == businessGroup.ToString().ToUpper())
                     .Select(x => x.Address).FirstOrDefault();
                if (tmpstoreAddress == null)
                    tmpstoreAddress = storeAddress;
                else
                {
                    if (String.IsNullOrWhiteSpace(tmpstoreAddress.BankInformation))
                        tmpstoreAddress.BankInformation = storeAddress.BankInformation;

                    //no country wise contact address
                    if (String.IsNullOrWhiteSpace(tmpstoreAddress.Address1) && String.IsNullOrWhiteSpace(tmpstoreAddress.Address2))
                    {
                        tmpstoreAddress.Address1 = storeAddress.Address1;
                        tmpstoreAddress.Address2 = storeAddress.Address2;
                        tmpstoreAddress.City = storeAddress.City;
                        tmpstoreAddress.Country = storeAddress.Country;
                        tmpstoreAddress.State = storeAddress.State;
                        tmpstoreAddress.Tel = storeAddress.Tel;
                        tmpstoreAddress.ZipCode = storeAddress.ZipCode;
                    }

                    if (String.IsNullOrWhiteSpace(tmpstoreAddress.ServiceTime))
                        tmpstoreAddress.ServiceTime = storeAddress.ServiceTime;
                }
            }
            else
            {
                tmpstoreAddress = storeAddress;
            }
            return tmpstoreAddress;
        }

        /// <summary>
        /// This method return company ID prefix for new account creation
        /// </summary>
        /// <returns></returns>
        private String getCompanyIdPrefix()
        {
            if (_companyIdPrefix == null)   //to be initialized
            {
                _companyIdPrefix = "UWE";  //default prefix
                StoreParameter param = null;

                //retrive 
                param = (from item in profile.StoreParameters
                         where item.SiteParameter.Equals("CompanyIDPrefix")
                         select item).FirstOrDefault();

                if (param != null && !String.IsNullOrEmpty(param.ParaValue))
                    _companyIdPrefix = param.ParaValue;
            }

            return _companyIdPrefix;
        }

        /// <summary>
        /// This method is to generate a valid company ID based on store rules
        /// </summary>
        /// <returns></returns>
        private String generateValidCompanyId()
        {
            String companyId = null;
            String nextId = null;

            //company ID sequence will be from a central place.  The definition will be from AUS store.  This is to prevent
            //competition situation.
            try
            {
                StoreHelper storeHelper = new StoreHelper();
                String prefix = getCompanyIdPrefix();
                Store aus = StoreSolution.getInstance().getStore("AUS");

                do
                {
                    //nextId = AllSequenceHelper.GetNewSeq(this.profile, prefix);
                    nextId = AllSequenceHelper.GetNewSeq(aus.profile, prefix);
                    companyId = prefix + nextId;
                } while (storeHelper.isCompanyIDExist(companyId));
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at generating new company Id", "UWE", nextId, this.profile.StoreID, ex);
                companyId = null;
            }

            return companyId;
        }

        /// <summary>
        /// This method checks company existance at both eStore and SAP
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        private Boolean isCompanyIdExist(String companyId)
        {
            //check eStore to see company ID existing status
            return (new StoreHelper()).isCompanyIDExist(companyId);
        }

        private String getUserFullName(User user)
        {
            return getCultureFullName(user.FirstName, user.LastName);
        }

        public String getCultureFullName(CartContact contact)
        {
            return getCultureFullName(contact.FirstName, contact.LastName);
        }

        public String getCultureFullName(Contact contact)
        {
            return getCultureFullName(contact.FirstName, contact.LastName);
        }

        private String _cultureNamingConversion = null;
        public String getCultureFullName(String firstName, String lastName)
        {
            String fullname = null;
            if (_cultureNamingConversion == null)   //to be initialized
            {
                var param = (from item in profile.StoreParameters
                             where item.SiteParameter.Equals("NameConvention")
                             select item).FirstOrDefault();
                if (param != null)
                    _cultureNamingConversion = param.ParaValue;

                if (String.IsNullOrEmpty(_cultureNamingConversion))
                {
                    //Ideally this naming format shall be defined as store attributes in store profile kept in DB
                    if (storeID.Equals("ACN") || storeID.Equals("AMY") || storeID.Equals("AKR") || storeID.Equals("AJP") || storeID.Equals("ATW"))
                        _cultureNamingConversion = "{1} {0}";
                    else
                        _cultureNamingConversion = "{0} {1}";
                }
            }

            fullname = String.Format(_cultureNamingConversion, firstName, lastName);

            return fullname;
        }

        /// <summary>
        /// 格式化名字和尊称
        /// </summary>
        /// <param name="name"></param>
        /// <param name="honorific"></param>
        /// <returns></returns>
        public string FormatHonorific(string name, string honorific)
        { 
            if(string.IsNullOrEmpty(name))
                return "";
            else if (string.IsNullOrEmpty(honorific))
                return name;
            return string.Format("{0}{1}", name, honorific);
        }

        /// <summary>
        /// This method will return greeting name per culture.  For example in US, the greating name will be
        /// Firstname only.  Thus the entire greeting title will be "Dear xFirstnamex".  In Japan, the greeting name
        /// may be customer's last name, like "xLastnamex san,"
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public String getCultureGreetingName(String firstName, String lastName)
        {
            string nicknameformat = profile.getStringSetting("NickNameFormat");
            if (string.IsNullOrWhiteSpace(nicknameformat))
                nicknameformat = "{0}";
            return string.Format(nicknameformat, firstName, lastName);
        }

        public string Tanslation(POCOS.Store.TranslationKey key, Boolean returnUSValueIfNull = true, Language language = null, MiniSite minisite = null)
        {
            string rlt;
            String resourceName = key.ToString();
            rlt = this.Tanslation(resourceName, returnUSValueIfNull, language, minisite);
            return rlt;
        }
        public string Tanslation(string key, Boolean returnUSValueIfNull = true, Language language = null, MiniSite minisite = null)
        {
            Dictionary<string, int> keys = new Dictionary<string, int>();

            keys.Add(key, 1);
            if (language != null)
            {
                keys.Add($"{key}_{language.Id.ToString()}", 2);
            }

            if (minisite != null)
            {
                keys.Add($"{key}_{minisite.SiteName}", 3);
                if (language != null)
                {
                    keys.Add($"{key}_{minisite.SiteName}_{language.Id.ToString()}", 4);
                }
            }
            string rlt  = (from t in this.profile.Localization
                          from k in keys
                          where t.Key == k.Key
                          orderby k.Value descending
                          select t.Value
                        ).FirstOrDefault();


            if (string.IsNullOrEmpty(rlt))
            {
                if (returnUSValueIfNull && USStore != null && USStore.profile.Localization.ContainsKey(key))
                    rlt = USStore.profile.Localization[key];
            }
            if (rlt == null) //设置为‘’时不显示
                rlt = key.Replace("_", "");
            return rlt;
        }

        private Store _usStore = null;
        private Store USStore
        {
            get
            {
                if (_usStore == null)
                    _usStore = StoreSolution.getInstance().getStore("AUS");

                return _usStore;
            }
        }

        //暂时用不到
        //private String getCountryCode(String name)
        //{
        //    String code = null;

        //    code = (from country in profile.Countries
        //            where country.CountryName.ToUpper().Equals(name) || country.Shorts.ToUpper().Equals(name)
        //            select country.Shorts).FirstOrDefault();

        //    return code;
        //}

        private Country getCountryByName(String name)
        {
            Country country = null;
            if (name.Equals("US") || name.Equals("USA") || name.Equals("United States", StringComparison.OrdinalIgnoreCase))
                name = "US";
            country = (from c in profile.Countries
                       where c.CountryName.Trim().ToUpper().Equals(name.Trim().ToUpper()) || c.Shorts.Trim().ToUpper().Equals(name.Trim().ToUpper())
                       select c).FirstOrDefault();
            return country;
        }

        //根据country 和 state, 返回对应的short state
        private string getStateShort(POCOS.Country country, string stateName)
        {
            string shortCode = "";
            if (country != null && country.CountryStates.Count > 0)
            {
                POCOS.CountryState countryState = country.CountryStates.FirstOrDefault(p => p.StateName.Trim().ToLower() == stateName.Trim().ToLower() || p.StateShorts.Trim().ToLower() == stateName.Trim().ToLower());
                if (countryState != null)
                    shortCode = string.IsNullOrEmpty(countryState.StateShorts) ? countryState.StateName.Trim() : countryState.StateShorts.Trim();
            }
            return shortCode;
        }

        private PaymentManager paymentManager
        {
            get
            {
                if (_paymentManager == null)
                    _paymentManager = new PaymentManager();

                return _paymentManager;
            }
        }

        /// <summary>
        /// This method is used to get the value of site parameter. Programmer must provide parameter name to get particular value of parameter.
        /// If failed, then return empty string.
        /// </summary>
        /// <param name="siteParameter"></param>
        /// <returns></returns>
        public string getStoreGlossary(string siteParameter)
        {
            if (string.IsNullOrEmpty(siteParameter))
                return "";
            StoreParameter param = null;

            //retrive 
            param = (from item in profile.StoreParameters
                     where item.SiteParameter.Equals(siteParameter)
                     select item).FirstOrDefault();

            if (!string.IsNullOrEmpty(param.ParaValue))
                return param.ParaValue;
            else
                return "";
        }

        public WidgetPage getWidgetPage(int widgetpageid)
        {
            WidgetPageHelper _help = new WidgetPageHelper();

            POCOS.WidgetPage widgetPage = _help.getWidgetPageByID(widgetpageid);
            return widgetPage;
        }

        public WidgetPage getWidgetPage(String widgetName)
        {
            WidgetPageHelper _help = new WidgetPageHelper();

            POCOS.WidgetPage widgetPage = _help.getWidgetPageByName(widgetName, this.profile.StoreID);
            return widgetPage;
        }
        public List<POCOS.Translation> getSeriesTranslation(string prefix)
        {
            return this.profile.translationX.Where(x => x.Key.StartsWith(prefix)).ToList();
        }
        private int _maxYouAreHereLinks = 0;
        public int maxYouAreHereLinks
        {
            get
            {
                if (_maxYouAreHereLinks == 0)
                {
                    _maxYouAreHereLinks = profile.getIntegerSetting("MaxYouAreHereLinks");
                    _maxYouAreHereLinks = (_maxYouAreHereLinks == 0) ? 1 : _maxYouAreHereLinks;
                }

                return _maxYouAreHereLinks;
            }
        }

        public List<RateServiceName> getSupportedShipping(string country)
        {
            StoreHelper sh = new StoreHelper();
            List<RateServiceName> r = sh.getSupportedShipping(country, this.storeID);

            return r;


        }

        /// <summary>
        /// Return CTOS spec mask.
        /// </summary>
        /// <param name="categorypath"></param>
        /// <returns></returns>
        public List<CTOSSpecMask> getCTOSSpecMask(string categorypath)
        {
            SpecMaskHelper sphelper = new SpecMaskHelper();
            return sphelper.getCTOSSpecMask(categorypath, this.storeID);

        }

        /// <summary>
        /// Return roles by role name, for windows service sending schedule emails.
        /// </summary>
        /// <param name="rolenames"></param>
        /// <returns></returns>
        public List<AdminRole> getRolebyName(string rolenames)
        {
            AdminRoleHelper adminRoleHelper = new AdminRoleHelper();
            List<AdminRole> adminRoles = adminRoleHelper.getRolebyName(rolenames);
            return adminRoles;
        }

        /// <summary>
        /// get published models from PIS by date
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public Dictionary<string, DateTime> getPublishedModelByDateFromPIS(DateTime startdate, DateTime enddate)
        {
            PISHelper phelper = new PISHelper();
            return phelper.getPublishedModelByDateFromPIS(startdate, enddate);
        }

        /// <summary>
        /// get CMS by cms type
        /// </summary>
        /// <param name="cmsTpye"></param>
        /// <returns></returns>
        public CMSManager.DataModle getCms(CMSType cmsTpye, string baa = "")
        {
            if (string.IsNullOrEmpty(baa))
                return CMSManager.Instance.getCms(cmsTpye, this.profile);
            else
                return CMSManager.Instance.getCmsByBAA(cmsTpye, this.profile, baa);
        }

        public CMSManager.DataModle getCms(string cmsTpye, string baa = "")
        {
            CMSType _cmsType = CMSType.NA;
            Enum.TryParse(cmsTpye.Trim().Replace(" ", "_").Replace("/", "Slash"), out _cmsType);
            return getCms(_cmsType, baa);
        }


        public CMSManager.DataModle getCms(CMSType cmsTpye, List<string> categoryLs)
        {
            if (categoryLs != null && categoryLs.Count > 0)
                return CMSManager.Instance.getCmsCategoryList(cmsTpye, this.profile, categoryLs);
            else
                return new CMSManager.DataModle();
        }

        public CMSManager.DataModle getCms(string cmsTpye, List<string> categoryLs)
        {
            CMSType _cmsType = CMSType.NA;
            Enum.TryParse(cmsTpye.Trim().Replace(" ", "_").Replace("/", "Slash"), out _cmsType);
            return getCms(_cmsType, categoryLs);
        }

        public POCOS.CMS getCMSByID(string id)
        {
            return CMSManager.Instance.getCMSByID(this.profile, id);
        }
        /// <summary>
        /// get store events
        /// </summary>
        /// <returns></returns>
        public List<Events> getEvents()
        {
            WWWCMSHelper helper = new WWWCMSHelper();
            return helper.getEventsByRBU(this.profile.StoreID);
        }

        public System.Data.DataSet getCmsByPartNo(string keyWords)
        {
            WWWCMSHelper helper = new WWWCMSHelper();
            return helper.getCMSByPartNumber(keyWords, this.profile.StoreID);
        }

        public System.Data.DataSet getNewCmsByModels(string[] Models, string cmsType, int rowCount = 20, string storeid = "")
        {
            WWWCmsNew helper = new WWWCmsNew();
            if (string.IsNullOrEmpty(storeid))
                storeid = profile.StoreID;
            return helper.getCMSNewByModel(Models, storeid, cmsType, rowCount);
        }

        public List<string> getStoreAllProductModel(MiniSite minisite = null)
        {
            PartHelper helper = new PartHelper();
            return helper.getStoreProductAllModel(profile, minisite);
        }

        public bool getLandedCost(POCOS.Order order)
        {
            LandedCostManager lcMgr = new LandedCostManager();
            LandedCost lc = lcMgr.getLandedCost(this.profile, order.cartX);
            if (lc != null)
            {
                order.TaxAndFees = lc.TaxAndFees;
                order.OtherTaxAndFees = lc.OtherTaxAndFees;
                order.VATTax = lc.VAT;
                order.DutyAndTax = lc.Duties + lc.TaxAndFees + lc.OtherTaxAndFees + lc.VAT;
                return true;
            }
            return false;
        }
        public bool getLandedCost(POCOS.Quotation quotation)
        {
            LandedCostManager lcMgr = new LandedCostManager();
            LandedCost lc = lcMgr.getLandedCost(this.profile, quotation.cartX);
            if (lc != null)
            {
                quotation.TaxAndFees = lc.TaxAndFees;
                quotation.OtherTaxAndFees = lc.OtherTaxAndFees;
                quotation.VATTax = lc.VAT;
                quotation.DutyAndTax = lc.Duties + lc.TaxAndFees + lc.OtherTaxAndFees + lc.VAT;
                return true;
            }
            return false;
        }


        private TaxManager _taxManager;
        private TaxManager taxManager
        {
            get
            {
                if (_taxManager == null)
                    _taxManager = new TaxManager();

                return _taxManager;
            }
        }

        /// <summary>
        /// calculateTax is to calculate tax of input sales order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public TaxCalculator calculateTax(POCOS.Order order)
        {
            TaxCalculator taxCalculator = taxManager.getTaxProvider(this.profile.TaxProvider);
            taxCalculator.Freight = order.Freight.GetValueOrDefault() - order.FreightDiscount.GetValueOrDefault();
            taxCalculator.CartDiscount = order.cartDiscountX.GetValueOrDefault() + order.cartX.cartItemsX.Sum(x => x.DiscountAmount).GetValueOrDefault();
            taxCalculator.VATNumber = order.VATNumbe;
            taxCalculator.calculateTax(order, this);

            if (taxCalculator.Status)
            {
                order.Tax = taxCalculator.Amount;
                order.TaxRate = taxCalculator.Rate;

            }
            return taxCalculator;
        }

        /// <summary>
        /// calculateTax is a method for calculating tax of input quotation
        /// </summary>
        /// <param name="quotation"></param>
        /// <returns></returns>
        public TaxCalculator calculateTax(POCOS.Quotation quotation)
        {
            TaxCalculator taxCalculator = taxManager.getTaxProvider(this.profile.TaxProvider);
            taxCalculator.Freight = quotation.Freight.GetValueOrDefault() - quotation.FreightDiscount.GetValueOrDefault();
            taxCalculator.CartDiscount = quotation.cartDiscountX.GetValueOrDefault() + quotation.cartX.cartItemsX.Sum(x => x.DiscountAmount).GetValueOrDefault();
            taxCalculator.VATNumber = quotation.VATNumber;
            taxCalculator.calculateTax(quotation, this);

            if (taxCalculator.Status)
            {
                quotation.Tax = taxCalculator.Amount;
                quotation.TaxRate = taxCalculator.Rate;
            }
            return taxCalculator;
        }

        /// <summary>
        /// sync warranty to part ,
        /// </summary>
        /// <param name="warrantyList"></param>
        /// <returns></returns>
        public List<Part> getPartInforByWarrantyList()
        {
            ExtendedWarantyHelper ewhelper = new ExtendedWarantyHelper();
            return ewhelper.getPartInforByWarrantyList(this.profile.StoreID);
        }
        /// <summary>
        /// this method is to get today campaigns
        /// </summary>
        /// <returns></returns>
        public List<Campaign> getTodayPromotionCampaigns()
        {
            CampaignHelper campainhelper = new CampaignHelper();
            List<Campaign> coupons = campainhelper.getCampaigns(this.profile.StoreID, DateTime.Now);

            return coupons;
        }
        //add request to siebel  -contact us, discount request
        public void AddOnlineRequestWithWS(ref OnlineRequest onlineRequest)
        {
            try
            {
                sso.AddOnlineRequestWithWS(ref onlineRequest);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Integrate eStore volumn request  contact us request to Siebel", "", "", "", ex);
            }

        }

        public void postTrackingData(string trackingurl)
        {
            TrackingDataPublisher trackingData = null;
            trackingData = new TrackingDataPublisher(trackingurl);

            if (trackingData != null)
                BusinessModules.Task.EventManager.getInstance(storeID).PublishNewEvent(trackingData);
        }

        public void AddOnlineRequest2Siebel(String eventPage, POCOS.PocoX.FollowUpable followUpableobj)
        {
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings.Get("UseOnlineRequestV2").ToLower() == "true")
                {
                    EventPublisher eventPublisher = null;
                    eventPublisher = new EventPublisher(eventPage, followUpableobj);

                    if (eventPublisher != null)
                        BusinessModules.Task.EventManager.getInstance(storeID).PublishNewEvent(eventPublisher);
                }
                else
                {
                    AdvantechOnlineRequestAdapter adapter = new AdvantechOnlineRequestAdapter();
                    OnlineRequest req = adapter.convert2OnlineRequest(followUpableobj);
                    if (req != null)
                    {
                        req.website = eventPage;
                        sso.AddOnlineRequestWithWS(ref req);
                        followUpableobj.currentFollowupStatus = "N/A";
                        followUpableobj.currentFollowUpAssignee = "N/A";
                        if (!string.IsNullOrEmpty(req.contactRowID) || !string.IsNullOrEmpty(req.requestRowID))
                            followUpableobj.currentFollowupComment = "Siebel Activity Succeeds -- Contact ID : " + req.contactRowID + ", Request ID : " + req.requestRowID
                                + ", Error Message : " + (string.IsNullOrEmpty(req.logMessage) ? "" : req.logMessage);
                        else
                            followUpableobj.currentFollowupComment = "Siebel Activity Fails -- Contact ID : " + (String.IsNullOrEmpty(req.contactRowID) ? "" : req.contactRowID)
                                + ", Request ID : " + (String.IsNullOrEmpty(req.requestRowID) ? "" : req.requestRowID) + ", Error Message : " +
                                (string.IsNullOrEmpty(req.logMessage) ? "" : req.logMessage);
                        followUpableobj.logFollowupActivity("System@eStore", req.subject + " Siebel Activity");
                    }
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Integrate eStore volumn request to Siebel", "", "", "", ex);
            }

        }

        /// <summary>
        /// This method is for UI page to publish user events to other subscribers like UNICA
        /// </summary>
        /// <param name="eventPage"></param>
        /// <param name="user"></param>
        /// <param name="product"></param>
        /// <param name="type"></param>
        public void PublishStoreEvent(String eventPage, POCOS.User user, POCOS.Product product, Task.EventType type)
        {
            if (product == null)
            {
                BusinessModules.Task.EventPublisher eventPublisher = new BusinessModules.Task.EventPublisher(eventPage, user, type);
                BusinessModules.Task.EventManager.getInstance(storeID).PublishNewEvent(eventPublisher);
            }
            else
            {
                BusinessModules.Task.EventPublisher eventPublisher = new BusinessModules.Task.EventPublisher(eventPage, user, product.DisplayPartno, type);
                BusinessModules.Task.EventManager.getInstance(storeID).PublishNewEvent(eventPublisher);
            }
        }

        //add Opportunity to siebel -order question discount request
        public SiebelWS4EMKT.RESULT addOnlineOpportunity(OPPTY opp)
        {
            try
            {
                eStore.BusinessModules.SiebelWS4EMKT.EMPLOYEE emp = new eStore.BusinessModules.SiebelWS4EMKT.EMPLOYEE();
                emp.USER_ID = "EMARKETING";
                emp.PASSWORD = "EMA594";

                SiebelWS4EMKT.WS4EMKT wsemkt = new WS4EMKT();
                return wsemkt.AddOppty(emp, opp);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Integrate eStore volumn order to Siebel", "", "", "", ex);
            }
            return null;
        }
        //get Admin Email By Country
        public string getAdminEmailByCountry(string country)
        {
            try
            {
                return sso.GetAdminEmailByCountry(country);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Get eStore Store AdminEmail By Country", "", "", "", ex);
            }
            return "EMARKETING";
        }

        /// <summary>
        /// get product addon IDK attribute by productid
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public List<IDKAttribute> getProductIDKAttributeBySproductID(string productID)
        {
            //测试数据需要补齐IDKPeripheralAddOnHelper
            List<IDKAttribute> ls = new List<IDKAttribute>();

            ls = (new PartHelper()).getIDKAttributes(this.storeID, productID);

            return ls;
        }

        public object getIDKAddons(string productid)
        {
            return (new eStore.POCOS.DAL.PartHelper()).getIDKAddons(this.storeID, productid);
        }
        public object getIDKCompatibilityEmbeddedBoard(string productid)
        {
            return (new eStore.POCOS.DAL.PartHelper()).getIDKCompatibilityEmbeddedBoard(this.storeID, productid);
        }
        //get PeripheralAddOn by Id
        public PeripheralAddOn getPeripheralAddOnById(int addOnItemID)
        {
            return (new eStore.POCOS.DAL.PeripheralHelper()).getPeripheralAddOnById(this.storeID, addOnItemID);
        }

        /// <summary>
        /// filter product IDK attribute
        /// </summary>
        /// <param name="attributeList">attribute name and attribute value</param>
        /// <returns></returns>
        public List<string> getIDKPeripheralAddOnByAttribute(string productID, Dictionary<string, string> attributeList)
        {
            //测试数据需要补齐IDKPeripheralAddOnHelper
            List<string> ls = new List<string>();
            ls = (new PartHelper()).getMatchedAddonsByAttributes(this.storeID, productID, attributeList);
            return ls;
        }

        // get Advertisement by id 
        public Advertisement getAdByID(int adid)
        {
            AdvertisementHelper AdHelper = new AdvertisementHelper();

            Advertisement ads = null;
            ads = AdHelper.getAdvByID(this.profile.StoreID, adid);
            return ads;

        }

        //根据adv type 获取所有发布的adv
        public List<Advertisement> getAdvByStoreAndType(Advertisement.AdvertisementType adType)
        {
            AdvertisementHelper AdHelper = new AdvertisementHelper();
            List<Advertisement> advList = AdHelper.getAdvByStoreAndType(this.profile, adType);
            if (advList == null)
                advList = new List<Advertisement>();
            return advList;
        }

        public List<Advertisement> getKitThemeAdvertisement(KitThemeType themetype)
        {
            List<string> treeids = new List<string>();
            treeids.Add("KitTheme_" + themetype.KitTheme.Id.ToString());
            treeids.Add("KitThemeTypes_" + themetype.Id.ToString());
            return (new AdvertisementHelper()).getAdvByMenuTreeIds(treeids, profile.StoreID);
        }

        /// <summary>
        /// get address by id
        /// </summary>
        /// <param name="addressid"></param>
        /// <returns></returns>
        public Address getAddressById(int addressid)
        {
            return (new AddressHelper()).getAddressById(addressid);
        }

        public ECOPartner getECOPartnerById(int id)
        {
            return (new ECOPartnerHelper()).getECOPartnerById(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ECOSpecialty> getAllECOSpecialty()
        {
            return (new ECOSpecialtyHelper()).getAllECOSpecialty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ECOIndustry> getAllECOIndustry()
        {
            return (new ECOIndustryHelper()).getAllECOIndustry();
        }

        public List<ECOPartner> getECOPartnerByBaseInfor(List<string> specialties, string country, string state, string industry)
        {
            return (new ECOPartnerHelper()).getECOPartnerByBaseInfor(specialties, country, state, industry);
        }

        public List<ECOPartner> getAllECOPartner()
        {
            return (new ECOPartnerHelper()).getAllECOPartner().Where(ep => ep.Status.Equals("Passing")).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ECOIndustry getIndustryByName(string name)
        {
            return (new ECOIndustryHelper()).getIndustryByName(name);
        }

        /// <summary>
        /// get country by country name
        /// </summary>
        /// <param name="countryname"></param>
        /// <returns></returns>
        public Country getCountrybyCountrynameORCode(string countryname)
        {
            return (new CountryHelper()).getCountrybyCountrynameORCode(countryname);
        }

        public List<Country> getAllCountries()
        {
            return (new CountryHelper()).getAllCountry();
        }

        public List<SiebelContactBAA> getAllStandardSiebelContactBAA()
        {
            return (new SiebelContactBAAHelper()).getAllStandardSiebelContactBAA();
        }

            //get ctos 'Check configured system' link
            public string getbuildsystemlink(string sproductid)
        {
            List<String> _products;
            List<String> _ctoss;
            string buildsystemlink = "";
            geteStoreProductLink(this.storeID, sproductid, out _products, out _ctoss);
            if (_ctoss != null && _ctoss.Count > 0)
                buildsystemlink = esUtilities.CommonHelper.GetStoreLocation() + "Compare.aspx?parts=" + System.Web.HttpUtility.UrlEncode(string.Join(",", _ctoss));
            return buildsystemlink;
        }

        /// <summary>
        /// store getStringSetting 的加强设置，支持store and ministie的bool设置
        /// 例如 ACN，ACNiAutomation 在acn和iautomation上可以使用
        /// </summary>
        /// <param name="SiteParameterName">StoreParameter SiteParameter Name</param>
        /// <param name="storeid">Store id</param>
        /// <param name="minisite">minisite</param>
        /// <returns>bool</returns>
        public bool isEnableSetUp(string SiteParameterName, string storeid, POCOS.MiniSite minisite = null)
        {
            bool iseable = false;
            string StoreAndMinisite = "";
            if (minisite != null)
                StoreAndMinisite = storeid + minisite.SiteName;
            else
                StoreAndMinisite = storeid;
            var eableStores = profile.getStringSetting(SiteParameterName);
            var _app = eableStores.Split(',');
            iseable = _app.Contains(StoreAndMinisite);
            return iseable;
        }

        public void calculateOrderReward(Order order, MiniSite minisite = null)
        {
            foreach (var o in getRewardActivities(minisite))
            {
                Reward.RewardMgt helper = new Reward.RewardMgt(o);
                helper.getRewardHelper().calculateOrderReward(order);
            }            
        }

        public void syncBBorderToSAP(Order order)
        {
            SyncBBorderToMyAdvantech mya = new SyncBBorderToMyAdvantech(this.profile, order);
            if (mya != null)
                EventManager.getInstance(this.profile.StoreID.ToUpper()).QueueCommonTask(mya);
        }

        public MyAdvantechBB.BBCustomer GetBBcustomerdataFromMyAdvantech(string userID)
        {
            MyAdvantechBB.BBorderAPI bbAPI = new MyAdvantechBB.BBorderAPI();

            string url = this.profile.getStringSetting("BBOrderWebServiceURL");
            if (string.IsNullOrEmpty(url))
                url = "http://my.advantech.com/Services/BBorderAPI.asmx";

            bbAPI.Timeout = 30000;
            bbAPI.Url = url;

            try
            {
                MyAdvantechBB.BBCustomer customer = bbAPI.GetERPIDbyEmail(userID);
                return customer;
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Get BB customer data from MyAdvantech ws error", userID, "", this.storeID, ex);
            }
            return null;
        }

        //查询具体用户的 log
        public List<RewardLog> getRewardLogByUserId(string userId = "", MiniSite minisite = null)
        {
            List<RewardLog> ls = new List<RewardLog>();
            foreach (var i in getRewardActivities(minisite))
                ls.AddRange((new RewardLogHelper()).getRewardLogByUserId(this.profile.StoreID, userId,i.Id));
            return ls;
        }
        
        //根据Id 获取RewardLog 对象
        public RewardGiftItem getRewardGiftItemById(int itemNo)
        {
            return (new RewardGiftItemHelper()).getRewardGiftItemById(itemNo);
        }

        //获取所有 publish的兑换产品
        public List<RewardGiftItem> getAllPublishRewardGiftItem(MiniSite minisite = null)
        {
            List<RewardGiftItem> ls = new List<RewardGiftItem>();
            foreach (var i in getRewardActivities(minisite))
                ls.AddRange((new RewardGiftItemHelper()).getAllPublishRewardGiftItem(this.profile.StoreID, i.Id));
            return ls;
        }

        //获取用户可用的积分
        public Dictionary<RewardActivity, decimal> getUserTotalPoint(string userId, MiniSite minisite = null)
        {
            return new Reward.RewardPointMgt(this.profile, getRewardActivities(minisite)).dal.getUserTotalPoint(userId, minisite);
        }

        //获取用户所有的积分
        public Dictionary<RewardActivity, decimal> getUserAllPoint(string userId, MiniSite minisite = null)
        {
            return new Reward.RewardPointMgt(this.profile, getRewardActivities(minisite)).dal.getUserAllPoint(userId, minisite);
        }

        public List<GiftLog> getGiftLogByUserid(string userid, int advid = 0)
        {
            return (new GiftLogHelper()).getGiftLogByUserid(userid, profile.StoreID, advid);
        }

        public List<GiftActivity> getAllGiftActivity(int advid, decimal minProbability = 0)
        {
            return (new GiftActivityHelper()).getAllGiftActivity(profile.StoreID, advid, minProbability);
        }

        public Campaign getCampaignByID(int campid, string storeid = "")
        {
            if (string.IsNullOrEmpty(storeid))
                storeid = this.profile.StoreID;
            return (new CampaignHelper()).getCampaignByID(storeid, campid);
        }
        //根据model 返回关联的model
        public PopularModel getPopulareProductByModel(string modelName)
        {
            return (new PopularModelHelper()).getPopulareProductByModel(modelName, profile.StoreID);
        }

        //根据model 返回关联的model
        public List<PopularModel> getPopulareProductByModel(List<string> modelName)
        {
            return (new PopularModelHelper()).getPopulareProductByModel(modelName, profile.StoreID);
        }
        //根据Model 返回 对应的随机产品
        public Product getProductByModelNo(List<string> modelList)
        {
            return (new PartHelper()).getProductByModelNo(modelList, profile);
        }
        public List<Product> getProductListByModelNo(List<string> modelList)
        {
            return (new PartHelper()).getProductListByModelNo(modelList, profile);
        }
        //设置Popular log的点击次数 增加。
        public void savePopularProductByClicks(string sessionId, string userId, string sourceProduct, string popularProduct)
        {
            PopularModelLog modelLog = new PopularModelLog();
            modelLog.StoreID = this.profile.StoreID;
            modelLog.SessionID = sessionId;
            modelLog.UserId = userId;
            modelLog.SourceProduct = sourceProduct;
            modelLog.PopulareProduct = popularProduct;

            PopularProductReporter eventReporter = null;
            eventReporter = new PopularProductReporter(modelLog, PopularEventType.Click);

            if (eventReporter != null)
                BusinessModules.Task.EventManager.getInstance(storeID).QueueCommonTask(eventReporter);
        }

        //登录时,把15天内的sessionId一样的记录,把userId更新
        public void savePopularModelLogByLogin(string sessionId, string userId)
        {
            PopularModelLog modelLog = new PopularModelLog();
            modelLog.StoreID = this.profile.StoreID;
            modelLog.SessionID = sessionId;
            modelLog.UserId = userId;

            PopularProductReporter eventReporter = null;
            eventReporter = new PopularProductReporter(modelLog, PopularEventType.UserLogin);

            if (eventReporter != null)
                BusinessModules.Task.EventManager.getInstance(storeID).QueueCommonTask(eventReporter);
        }

        //check order的时候,保存log
        public void savePopularModelLogByOrder(Order order)
        {
            PopularModelLog modelLog = new PopularModelLog();
            modelLog.StoreID = this.profile.StoreID;
            modelLog.UserId = order.UserID;

            PopularProductReporter eventReporter = null;
            eventReporter = new PopularProductReporter(order, modelLog, PopularEventType.Order);

            if (eventReporter != null)
                BusinessModules.Task.EventManager.getInstance(storeID).QueueCommonTask(eventReporter);
        }

        //confirm quotation的时候,保存log
        public void savePopularModelLogByQuotation(Quotation quotation)
        {
            PopularModelLog modelLog = new PopularModelLog();
            modelLog.StoreID = this.profile.StoreID;
            modelLog.UserId = quotation.UserID;

            PopularProductReporter eventReporter = null;
            eventReporter = new PopularProductReporter(quotation, modelLog, PopularEventType.Quotation);

            if (eventReporter != null)
                BusinessModules.Task.EventManager.getInstance(storeID).QueueCommonTask(eventReporter);
        }

        /// <summary>
        /// get spec category by category id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Spec_Category getSpecCategoryById(int id)
        {
            return (new Spec_CategoryHelper()).getSpecCategoryById(id);
        }

        public List<Spec_Category> getAllSpecCategoryByRootId(int id)
        {
            return (new Spec_CategoryHelper()).getAllSpecCategoryByRootId(id, profile.StoreID);
        }

        public List<Spec_Category> getAllSpecCategoryByRootId(string name)
        {
            return (new Spec_CategoryHelper()).getAllSpecCategoryByRootId(name, profile.StoreID);
        }

        /// <summary>
        /// get spec category by category display name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Spec_Category getSpecCategoryByName(string name)
        {
            return (new Spec_CategoryHelper()).getSpecCategoryByName(name);
        }

        /// <summary>
        /// mapping part spec by category ids 
        /// </summary>
        /// <param name="categoryids"></param>
        /// <param name="includStoreProduct"></param>
        /// <returns></returns>
        public List<Part_Spec_V3> getSpecByCategoryIds(List<int> categoryids, string includStoreProduct = "")
        {
            return (new Spec_CategoryHelper()).getSpecByCategoryIdsWithMarg(categoryids, includStoreProduct);
        }

        public List<Part> getPartsCategoryIds(List<int> categoryids, string includStoreProduct = "")
        {
            return (new Spec_CategoryHelper()).getPartsByCategoryIds(categoryids, includStoreProduct);
        }

        public List<Part_Spec_V3> getAllPart_Spec_V3()
        {
            return (new Part_Spec_V3Helper()).getAllSpec();
        }

        public List<EasyUITreeNode> getEasyUITreeNodeByCategoryID(int ID)
        {
            List<EasyUITreeNode> nodes = new List<EasyUITreeNode>();
            if (ID > 0)
            {
                List<getHierarchyTree_Result> categoryies = this.GetHierarchyTreeByCategoryID(ID);
                getHierarchyTree_Result root = categoryies.Where(c => c.CATEGORY_LEVEL == 1).FirstOrDefault();

                if (root != null)
                {
                    List<Part_Spec_V3> products = this.GetPartSpecV3ListbyIds(categoryies.Select(c => c.CATEGORY_ID).ToList());
                    EasyUITreeNode node = new EasyUITreeNode(root);
                    node.nodeState = TreeStates.open;
                    this.RecursiveCategory(categoryies, products, node);
                    nodes.Add(node);
                }
            }
            return nodes;
        }

        public List<getHierarchyTree_Result> GetHierarchyTreeByCategoryID(int ID)
        {
            return (new HierarchyTreeHelper()).GetHierarchyTreeByCategoryID(ID);
        }

        public List<Part_Spec_V3> GetPartSpecV3ListbyIds(List<int> ids)
        {
            return (new Part_Spec_V3Helper()).getPartSpecV3ListbyIds(ids);
        }

        public void RecursiveCategory(List<getHierarchyTree_Result> categories, List<Part_Spec_V3> products, EasyUITreeNode currentNode)
        {
            List<getHierarchyTree_Result> subcategories = categories.Where(c => c.ParentHierarchyID == currentNode.hierarchyID).ToList();
            if (subcategories.Count == 0)
            {
                currentNode.nodeState = TreeStates.open;
                return;
            }

            foreach (getHierarchyTree_Result child in subcategories)
            {
                EasyUITreeNode tree = new EasyUITreeNode(child);
                if (tree.nodeType == NodeType.Value)
                {
                    foreach (var p in products)
                    {
                        if (p.CATEGORY_ID == child.CATEGORY_ID && !tree.products.Contains(p.PART_NO))
                            tree.products.Add(p.PART_NO);
                    }
                }
                currentNode.children.Add(tree);
                RecursiveCategory(categories, products, tree);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="minisite"></param>
        /// <returns></returns>
        public string getCurrStoreUrl(POCOS.Store store, POCOS.MiniSite minisite = null)
        {
            if (minisite != null && !string.IsNullOrEmpty(minisite.StoreURL))
                return minisite.StoreURL;

            return (string.IsNullOrEmpty(store.StoreURL)) ? "#" : store.StoreURL;
        }
        /// <summary>
        /// show special tax meaage
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public string specialTaxMessage(CartContact contact)
        {
            if (contact == null)
                return "";
            if (this.profile.StoreID == "SAP" && contact.countryCodeX.ToUpper() != "SG") //sap store 要求配送到sg外的订单税率需要显示特殊文字
                return "It will be subject to duty and tax (borne by receiver at custom)";

            return "";
        }


        /// <summary>
        /// get estore policy url
        /// </summary>
        /// <returns></returns>
        public virtual string policylink()
        {

            if (this.profile.getBooleanSetting("showPolicyOnProduct"))
                return this.profile.getStringSetting("estorePolicyUrl");
            else
                return "";
        }

        /// <summary>
        /// get template by file name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="language"></param>
        /// <param name="ministe"></param>
        /// <returns></returns>
        public String getTemplateByStr(string name, Language language = null, MiniSite ministe = null)
        {
            if (string.IsNullOrEmpty(name))
                return "";

            StringBuilder filePath = new StringBuilder();
            String fullPath = null;

            //the following file path is only for temporary purpose.  It requires further refinement
            //filePath.Append(AppDomain.CurrentDomain.BaseDirectory);
            filePath.Append(ConfigurationManager.AppSettings.Get("Template_Path")).Append("\\").Append(storeID).Append("\\");

            if (ministe != null && !string.IsNullOrEmpty(ministe.getStringSetting("EmailFilePath")))
                filePath.Append(ministe.getStringSetting("EmailFilePath")).Append("\\");

            //如果有language, 读取language 文件夹下面的html
            if (language != null && !string.IsNullOrEmpty(language.Code))
                filePath.Append(language.Code).Append("\\");

            filePath.AppendFormat("{0}_{1}.htm", name, storeID);

            //open template file and read it in 
            String content = "";
            try
            {
                fullPath = Path.GetFullPath(filePath.ToString());

                if (language != null && !string.IsNullOrEmpty(language.Code))
                {
                    //如果language下面文件不存在.  读取Store下面的文件
                    if (!File.Exists(fullPath))
                        fullPath = fullPath.Replace(language.Code + "\\", "");
                }

                //create an instance of StreamReader to read from a file.
                //The using statement also closes the StreamReader
                using (StreamReader reader = new StreamReader(fullPath))
                {
                    content = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Warn("File could not be read", fullPath.ToString(), "", "", ex);
            }

            return content;
        }

        public List<POCOS.PolicyCategory> getRootPolicyCategory(string storeid = null)//取得PolicyCategory的根节点
        {
            List<POCOS.PolicyCategory> policyCategory;
            if (storeid == null)
                storeid = this.profile.StoreID;
            PolicyCategoryHelper helper = new PolicyCategoryHelper();
            policyCategory = helper.getRootPolicyCategory(storeid);
            return policyCategory;
        }
        public POCOS.PolicyCategory getPolicyCategoryByUrl(string  url )//取得指定id的PolicyCategory
        {
            POCOS.PolicyCategory policyCategory;
            POCOS.DAL.PolicyCategoryHelper helper = new PolicyCategoryHelper();
            policyCategory = helper.getPolicyCategoryByUrl(url, this.storeID);
            return policyCategory;
        }
        public POCOS.PolicyCategory getPolicyCategoryById(int id )//取得指定id的PolicyCategory
        {
            POCOS.PolicyCategory policyCategory;
            POCOS.DAL.PolicyCategoryHelper helper = new PolicyCategoryHelper();
            policyCategory = helper.getPolicyCategoryById(id,this.storeID );
            return policyCategory;
        }

        public PolicyGlobalResource getPolicyGlobalResource(int languageId, int policyId)
        {
            return new POCOS.DAL.PolicyGlobalResourceHelper().getPolicyGlobalResource(languageId, policyId);

        }

        public List<Solution> getAllSolution(string storeid = null)
        {
            if (storeid == null)
                storeid = this.profile.StoreID;
            string cacheKey =storeid+ "_Solutions";
            List<Solution> solutions = (List<Solution>)CachePool.getInstance().getObject(cacheKey);
            if (solutions == null)
            {
                solutions = new POCOS.DAL.SolutionHelper().getAllSolution(storeid);
                new POCOS.DAL.PartHelper().prefetchPartList(this.storeID, solutions.SelectMany(x => x.SolutionsAssosociateItems.Select(p => p.SProductID)).ToList());
                CachePool.getInstance().cacheObject(cacheKey, solutions);
            }
            return solutions;
        }

        public SolutionGlobalResource getSolutionGlobalResource(int solutionId, int languageId)
        {
            return new POCOS.DAL.SolutionGlobalResourceHelper().getSolutionGlobalResource(solutionId, languageId);
        }

        public List<Product> getProductsBySolutionId(int solutionId, string storeid = null)
        {
            if (storeid == null)
                storeid = this.profile.StoreID;
            return new POCOS.DAL.PartHelper().getProductsBySolutionId(solutionId, storeid);
        }

        /// <summary>
        /// 根据ProuductID找到这个Product关联的所有ProductCategory
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public List<ProductCategory> getAssociatedProductCategoriesByProductID(string productID)
        {
             Product currentProduct = getProduct(productID);
            List<POCOS.ProductCategory> results = new List<ProductCategory>();
            if (currentProduct != null)
            {
                List<POCOS.ProductCategory> pcs = currentProduct.productCategories;

                foreach (var pc in pcs)
                {
                    var associatedProductCategories = getAssociatedProductCategories(pc);
                    if (associatedProductCategories != null && associatedProductCategories.Any())
                    {
                        foreach (var apc in associatedProductCategories)
                        {
                            if (!results.Any(x => x.CategoryID == apc.CategoryID))
                                results.Add(apc);
                        }
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// 根据CategoryPath获取这个Category关联的category
        /// </summary>
        /// <param name="categoryPath"></param>
        /// <returns></returns>
        public List<ProductCategory> getAssociatedProductCategoriesByCategoryPath(string categoryPath)
        {

            var pc = this.getProductCategory(categoryPath);
            return getAssociatedProductCategories(pc);
        }
        public List<ProductCategory> getAssociatedProductCategories(ProductCategory pc)
        {
            if (pc != null)
            {
                List<ProductCategory> AssociatedProductCategories = new List<ProductCategory>();
                AssociatedProductCategories.AddRange(pc.getAssociatedCategorys());


                var ancestorAssociatedProductCategories =  pc.ancestorX.SelectMany(x => x.getAssociatedCategorys());
                if (ancestorAssociatedProductCategories != null && ancestorAssociatedProductCategories.Any())
                {
                    foreach (var apc in ancestorAssociatedProductCategories)
                    {
                        if (!AssociatedProductCategories.Any(x => x.CategoryID == apc.CategoryID))
                            AssociatedProductCategories.Add(apc);
                    }
                }

                return AssociatedProductCategories;
            }
            else
                return new List<ProductCategory>();
        }

        public string getFullShippingMethodDescription(string shipDes)
        {
            string _SpecialShippingMethodDescription = shipDes;
            if (profile.StoreID == "AEU")
            {
                if (shipDes == "TNT Global" || shipDes == "TNT Express")
                    _SpecialShippingMethodDescription = "TNT Express";
                else if (shipDes == "TNT Economy")
                    _SpecialShippingMethodDescription = "TNT Economy";
            }
            return _SpecialShippingMethodDescription;
        }
        public string getShortShippingMethodDescription(string shipDes)
        {
            return profile.getShortShippingMethodDescription(shipDes);
        }

        public decimal GetiAblePoint(string userid, string tempid)
        {
            //decimal point = 0m;
            //iAbleClub.IMartPointService ws = new iAbleClub.IMartPointService();
            //ws.Timeout = 5000;
            //try
            //{
            //    point = ws.eStoreSSOLoginAndCalPoint(userid, tempid, false);
            //}
            //catch
            //{
            //    return point;
            //}
            //return point;
            decimal point = 0m;
            iAbleClub.MyAdvantech.CreateSAPSoForeStore ws = new iAbleClub.MyAdvantech.CreateSAPSoForeStore();
            ws.Timeout = 5000; //ICC Set timeout to 5 seconds, because we still have to keep SSO function for user to visit iABLE web site from eStore
            try
            {
                point = ws.eStoreSSOLoginAndCalPoint(userid, tempid, false);
            }
            catch
            {
                return point;
            }
            finally
            {
                ws.Dispose();
                ws = null;
            }
            return point;
        }

        /// <summary>
        /// 赛选 固定状态的产品
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="satus"></param>
        public void margerSimpleProduct(ref List<POCOS.SimpleProduct> ls, string satus)
        {
            if (!string.IsNullOrEmpty(satus))
            {
                POCOS.Product.PRODUCTMARKETINGSTATUS status;
                if (Enum.TryParse(satus, out status))
                {
                    foreach (var item in ls)
                    {
                        var pro = item.mappedToProduct();
                        if (isFastDeliveryProducts(pro)) // aus will get product from sap
                        {
                            if (storeID == "AUS")
                            {
                                if (pro is POCOS.Product_Ctos)
                                    pro.addMarketingstatus(POCOS.Product.PRODUCTMARKETINGSTATUS.EXPRESS);
                                else
                                    pro.addMarketingstatus(POCOS.Product.PRODUCTMARKETINGSTATUS.TwoDaysFastDelivery);
                            }
                        }
                    }
                    ls = ls.Where(c => c.mappedToProduct().isIncludeSatus(status)).ToList();
                }
            }
        }

        public decimal CalculateiAblePointAfterOrder(Order order, MiniSite minisite = null, bool testing = true)
        {
            //ICC No longer call iABLE webservice to give point after ordering immediately
            decimal point = 0m;
            //iAbleClub.IMartPointService ws = new iAbleClub.IMartPointService();
            //ws.Timeout = 5000;
            //try
            //{
            //    iAbleClub.AblePointOrder apo = new iAbleClub.AblePointOrder();
            //    apo.CartID = order.CartID;
            //    apo.StoreID = order.StoreID;
            //    apo.OrderNo = order.OrderNo;
            //    apo.UserID = order.UserID;
            //    apo.OrderDate = order.OrderDate.GetValueOrDefault(DateTime.Now);
            //    apo.TotalAmount = order.TotalAmount.GetValueOrDefault(0m);

            //    List<iAbleClub.AblePointCartItem> cartitems = new List<iAbleClub.AblePointCartItem>();
            //    foreach (CartItem item in order.Cart.cartItemsX)
            //    {
            //        iAbleClub.AblePointCartItem ci = new iAbleClub.AblePointCartItem();
            //        ci.ProductID = item.SProductID;
            //        ci.DisplayPartNo = item.productNameX;
            //        ci.Qty = item.Qty;
            //        ci.AdjustedPrice = item.AdjustedPrice;
            //        cartitems.Add(ci);
            //    }
            //    apo.CartItems = cartitems.ToArray();
            //    point = ws.eStorePlaceOrder(apo, testing);
            //}
            //catch
            //{
            //    return point;
            //}
            return point;
        }

        public KitTheme getKitThemeById(int id)
        {
            var kit = (new KitThemeHelper()).getKitThemeById(id);
            if (kit.StoreID != profile.StoreID)
                kit = null;
            return kit;
        }
        public List<KitTheme> GetKitThemeLs()
        {
            return (new KitThemeHelper()).GetKitThemeLs(profile.StoreID);
        }

        public List<spSearchCtosWithDefaultPartNo_Result> SearchCtosWithDefaultPartNo(string sourcestoreID, string sourceSystemNo)
        {
            return (new PartHelper()).SearchCtosWithDefaultPartNo(sourcestoreID, sourceSystemNo, profile.StoreID);
        }

        public KitThemeType getKitThemeTypeById(int id)
        {
            return (new KitThemeTypeHelper()).getKitThemeTypeById(id);
        }

        public void addAperza(string projectName, string productID, string siteID, string URL)
        {
            try
            {
                Affiliate a = (new AffiliateHelper()).getAffiliateBySiteID(siteID);
                if (a == null)
                    a = new Affiliate();
                a.Name = projectName;
                if (!string.IsNullOrEmpty(a.Description))
                    a.Description = a.Description + "|" + productID;
                else
                    a.Description = productID;
                a.SiteID = siteID;
                a.TrackingURL = URL;
                a.CreateDate = DateTime.Now;
                a.save();
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Save Aperza exception", "SiteID:" + siteID, "", this.storeID, ex);
            }
        }

        #region Unit Test

        /// <summary>
        /// This method is for testing purpose. It returns dummy product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productPrice"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public Product getDummyProduct(String productId, Decimal productPrice, Decimal length = 1.0m, Decimal width = 1.0m, Decimal height = 1.0m, Decimal weight = 1.0m)
        {
            Product product = new Product();

            product.StoreID = storeID;
            product.VendorID = "Dummy vendor";

            product.clearanceThreshValue = 0;
            product.CreatedDate = DateTime.Now;
            product.DimensionHeightCM = height;
            product.DimensionLengthCM = length;
            product.DimensionWidthCM = width;  //*** Typo at this property name.  Need to come back to correct it
            product.ShipWeightKG = weight;

            product.DisplayPartno = productId + " Friendly name";
            product.EffectiveDate = DateTime.Now;
            product.ExtendedDesc = productId + " Extended description";
            product.ImageURL = productId + " image URL";
            product.Keywords = productId + " Product keywords";
            product.name = productId;
            product.priceSource = Product.PRICESOURCE.LOCAL;
            product.ProductDesc = productId + " Product description";
            product.ProductFeatures = productId + " Product feature";
            product.ProductGroup = "eP";
            product.productType = Product.PRODUCTTYPE.STANDARD;
            product.publishStatus = Product.PUBLISHSTATUS.PUBLISHED;
            product.ShowPrice = true;
            product.SProductID = productId;
            product.addMarketingstatus(Product.PRODUCTMARKETINGSTATUS.NEW);
            product.StorePrice = productPrice;
            product.LocalPrice = productPrice;
            product.VendorProductDesc = product.ProductDesc;
            product.VendorFeatures = product.ProductFeatures;
            product.VendorSuggestedPrice = productPrice;
            product.VProductID = productId;
            product.dummy = true;

            //save product for later use
            new PartHelper().save(product);

            return product;
        }

        /// <summary>
        /// This method is for testing purpose. It returns dummy product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productPrice"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public Product_Ctos getDummyCTOS(String productId, Decimal productPrice, Decimal length = 1.0m, Decimal width = 1.0m, Decimal height = 1.0m, Decimal weight = 1.0m)
        {
            Product_Ctos product = new Product_Ctos();

            product.StoreID = storeID;
            product.VendorID = "Dummy vendor";
            product.BTONo = "productId";

            product.clearanceThreshValue = 0;
            product.CreatedDate = DateTime.Now;
            product.DimensionHeightCM = height;
            product.DimensionLengthCM = length;
            product.DimensionWidthCM = width;  //*** Typo at this property name.  Need to come back to correct it
            product.ShipWeightKG = weight;

            product.DisplayPartno = productId + " Friendly name";
            product.EffectiveDate = DateTime.Now;
            product.ExtendedDesc = productId + " Extended description";
            product.ImageURL = productId + " image URL";
            product.Keywords = productId + " Product keywords";
            product.name = productId;
            product.priceSource = Product.PRICESOURCE.LOCAL;
            product.ProductDesc = productId + " Product description";
            product.ProductFeatures = productId + " Product feature";
            product.ProductGroup = "eP";
            product.productType = Product.PRODUCTTYPE.CTOS;
            product.publishStatus = Product.PUBLISHSTATUS.PUBLISHED;
            product.ShowPrice = true;
            product.SProductID = productId;
            product.addMarketingstatus(Product.PRODUCTMARKETINGSTATUS.NEW);
            product.StorePrice = productPrice;
            product.LocalPrice = productPrice;
            product.VendorProductDesc = product.ProductDesc;
            product.VendorFeatures = product.ProductFeatures;
            product.VendorSuggestedPrice = productPrice;
            product.VProductID = productId;
            product.dummy = true;

            //save product for later use
            product.save();

            return product;
        }

        /// <summary>
        /// get store hot sale product
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public List<Product> getHotDeals(int interval, string storeid = null, int count = 3)
        {
            if (storeid == null)
                storeid = this.profile.StoreID;
            return (new PartHelper()).getHotDeals(storeid, interval, count);
        }

        public bool SetCategoryMappingDefault(int categoryid, string sproductid)
        {
            return (new ProductCategroyMappingHelper()).UpdateProductMappingDefault(categoryid, sproductid, profile.StoreID);
        }

        public List<MarketingResourceResult> GetMarketingResourceList(string rAWTXT, string nFKEY)
        {
            return (new MarketingResourceHelper()).GetList(rAWTXT,nFKEY);
        }

        #endregion

        public void SaveEDMParameter(string mailLogId)
        {
            try
            {
               CampaignMailLogByEDM_Helper CampaignMailLogByEDM_Helper = new CampaignMailLogByEDM_Helper();
               CampaignMailLogByEDM CampaignMailLogByEDM = CampaignMailLogByEDM_Helper.getCampaignMailLogByEDMByMailLogId(this.storeID, mailLogId);

                if (CampaignMailLogByEDM != null)
                {
                    CampaignMailLogByEDM edm = new CampaignMailLogByEDM()
                    {
                        CampaignID = CampaignMailLogByEDM.CampaignID,
                        UserId = CampaignMailLogByEDM.UserId,
                        CreateDate = DateTime.Now,
                        EdmCode = CampaignMailLogByEDM.EdmCode,
                        PromotionCode = CampaignMailLogByEDM.PromotionCode,
                        Storeid = CampaignMailLogByEDM.Storeid,
                        IsCheck = (int)CampaignMailLogByEDM.MailLogStatus.MailCheck,
                        MailLogId = mailLogId,
                        ID = 0
                    };

                    edm.save();
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
            }
        }
    }
}
