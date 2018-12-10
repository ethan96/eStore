using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

 namespace eStore.POCOS{ 

public partial class User { 
 
#region Extension Attributes
    private Boolean _hasCompleteRegistration = false;
    private Cart _shoppingCart = null;
    private Quotation _openingQuotation = null;
    private Role _actingRole = Role.Default;
    private User _actingUser = null;
    public enum ACToken { ATP, InStock, Price_V, Price_M, SwitchRole, SimulateTransaction, ViewCTOSOptionParts, ModifyConfirmedDeal, AccessSAPContact };
    public enum Role { Employee, Customer, Default, Anonymous };
    /// <summary>
    /// This property is a temporary property used only by OM for reporting purpose
    /// </summary>
    public decimal revenue;
    private string country = string.Empty;
    public string Country 
    {
        get
        {
            return this.country;
        }
        set
        {
            this.country = value;
        }
    }

#endregion

	#region Extension Methods 

    //default constructor
    public User()
    {
        timeSpan = new TimeSpan();
        UserLanguages = new String[0]; //empty set
    }

    /// <summary>
    /// This is runtime only property.
    /// </summary>
    public Store store
    {
        get;
        set;
    }

    /**** this method is reserved to be implemented once PaymentAccount is definied in DB and entity
    public virtual List<PaymentAccount> getPaymentAccount()
    {
    }
    ***********/

    /// <summary>
    /// This method is to add a new contact to a user and return new contact instance to the caller.  
    /// The parameters for this method are all required fields.
    /// To setup optional fields in contact, caller can assign or overwrite those fields when it
    /// receive contact instance from this method call.
    /// </summary>
    /// <param name="companyName"></param>
    /// <param name="FirstName"></param>
    /// <param name="LastName"></param>
    /// <param name="country"></param>
    /// <param name="state"></param>
    /// <param name="city"></param>
    /// <param name="zip"></param>
    /// <param name="Address1"></param>
    /// <param name="Telno"></param>
    /// <param name="TelExt"></param>
    /// <returns></returns>
    public Contact addContact(String companyName, String firstName, String lastName, String country, String state, String city, String zip,
                                String address1, String telno, String address2 = "", String telEx = "")
    {
        Contact newContact;
        if (this.CompanyID == null) //companyId has to be initialized before adding additonal contact to user
            return null;

        try
        {
            newContact = new Contact(this);

            newContact.AddressID = getNextAddressId();
            if (String.IsNullOrEmpty(newContact.AddressID)) //problem happened at generating new address Id.
                return null;

            newContact.Address1 = address1;
            newContact.AttCompanyName = companyName;
            newContact.FirstName = firstName;
            newContact.LastName = lastName;
            newContact.City = city;
            newContact.countryX = country;
            newContact.State = state;
            newContact.TelNo = telno;
            newContact.ZipCode = zip;
            newContact.Address2 = address2;
            newContact.TelExt = telEx;

            this.Contacts.Add(newContact);
        }
        catch (Exception ex)
        {
            eStoreLoger.Error("Problem at creating new user contact", UserID, "", "", ex);
            newContact = null;
        }

        return newContact;    
    }

    /// <summary>
    /// This method is to update existing user contact.  It returns the updated contact
    /// The parameters for this method are all required fields.
    /// To setup optional fields in contact, caller can assign or overwrite those fields when it
    /// receive contact instance from this method call.
    /// </summary>
    /// <param name="companyName"></param>
    /// <param name="FirstName"></param>
    /// <param name="LastName"></param>
    /// <param name="country"></param>
    /// <param name="state"></param>
    /// <param name="city"></param>
    /// <param name="zip"></param>
    /// <param name="Address1"></param>
    /// <param name="Telno"></param>
    /// <param name="address2">optional</param>
    /// <param name="TelExt">optional</param>
    /// <returns></returns>
    public Contact updateContact(String addressID, String companyName, String firstName, String lastName, String country, String state, String city, String zip,
                                String address1, String telno, String address2 = "", String telEx = "")
    {
        Contact contact;
        if (this.CompanyID == null) //companyId has to be initialized before adding additonal contact to user
            return null;

        try
        {
            contact = Contacts.FirstOrDefault(ct => ct.AddressID.Equals(addressID));
            if (contact != null)
            {
                contact.Address1 = address1;
                contact.AttCompanyName = companyName;
                contact.FirstName = firstName;
                contact.LastName = lastName;
                contact.City = city;
                contact.countryX = country;
                contact.State = state;
                contact.TelNo = telno;
                contact.ZipCode = zip;
                contact.Address2 = contact.Address2;
                contact.TelExt = contact.TelExt;
            }
        }
        catch (Exception ex)
        {
            eStoreLoger.Error("Problem at updating user contact", UserID, addressID, "", ex);
            contact = null;
        }

        return contact;
    }

    public Contact mainContact
    {
        get
        {
            Contact match = null;
            match = (from contact in Contacts
                    where !string.IsNullOrEmpty(contact.AddressID) && contact.AddressID.Equals(CompanyID)
                    select contact).FirstOrDefault();
            return match;
        }

    }

    /// <summary>
    /// Contacts in the same User account follows a particular naming conversion
    ///     user.companyID + alphabetic suffix
    /// </summary>
    /// <returns></returns>
    public String getNextAddressId()
    {
        String companyId = null;
        String nextId = null;

        try
        {
            StoreHelper storeHelper = new StoreHelper();
            String prefix = this.CompanyID;

            //regular process
            int nextSeq = this.Contacts.Count();
            if (nextSeq > 0)
                nextSeq--;
            do
            {
                nextId = formatingSeq(nextSeq);
                companyId = prefix + nextId;
                nextSeq++;
            } while (storeHelper.isCompanyIDExist(companyId));
        }
        catch (Exception ex)
        {
            eStoreLoger.Error("Exception at generating new company Id", UserID, "", "", ex);
            companyId = null;
        }

        return companyId;
    }

    /// <summary>
    /// This method converts input seq to the format of XXXXX format.  X ranges from A to Z descendingly
    /// </summary>
    /// <param name="seq"></param>
    /// <returns></returns>
    private String formatingSeq(int seq)
    {
        StringBuilder result = new StringBuilder();

        int power = (int)Math.Floor(Math.Log(seq, 26));

        while (power > 0)
        {
            int divider = (int)Math.Pow(26, power);
            //range from 0 to 25
            int digit = seq / divider;
            result.Append(Convert.ToChar('Z' - digit));
            seq = seq - (digit * divider);
            power--;
        }

        result.Append(Convert.ToChar('Z' - seq));

        return result.ToString();
    }

    /// <summary>
    /// 是否为eStore Team人员
    /// </summary>
    public bool IsEstoreManager
    {
        get 
        {
            if (isEstoreManager == null)
                isEstoreManager = System.Configuration.ConfigurationManager.AppSettings.Get("eStoreItEmailGroup").ToUpper().Contains(this.UserID.ToUpper());
            return isEstoreManager.Value; 
        }
        set { isEstoreManager = value; }
    }
    private bool? isEstoreManager = null;

    /// <summary>
    /// When authKey is null, it means current user hasn't be authenticated yet. Application shall make sure user is authenticated before
    /// applying user authorization to access control.
    /// </summary>
    public String authKey
    {
        get;
        set;
    }

    /// <summary>
    /// Some old registration has incomplete required user information. To allow user continuing on eStore activities,
    /// eStore application should redirect user to membership web site to complete the entire registration. Then returns
    /// back to eStore.
    /// </summary>
    /// <returns></returns>
    public Boolean hasCompleteRegistration()
    {
        return _hasCompleteRegistration;
    }

    /// <summary>
    /// This method is only for internal use.  eStore application shall not invoke this method at all.
    /// </summary>
    public Boolean completeRegistration
    {
        set {_hasCompleteRegistration = value;}
    }

    /// <summary>
    /// isAuthenticated is used to validate if current user has successfully sign in to eStore via SSO
    /// </summary>
    /// <returns></returns>
    public Boolean isAuthenticated()
    {
        if (authKey == null)
            return false;
        else
            return true;
    }

    /// <summary>
    /// This is a runtime property indicating current user is a new user to eStore
    /// </summary>
    public Boolean newUser
    {
        get;
        set;
    }

    public Contact getContact(int contactId)
    {
        Contact match = (from contact in Contacts
                         where contact.ContactID.Equals(contactId)
                         select contact).FirstOrDefault();

        return match;
    }

    /// <summary>
    /// This shall be a read-only property. It returns current user shopping cart item.
    /// </summary>
    public Cart shoppingCart
    {
        get 
        {
            if (_shoppingCart == null)
            {
                _shoppingCart = getShoppingCart();
                //_shoppingCart.initValidation(); //validating cart content and remove phased out or inorderable items
                _shoppingCart.refresh();
            }

            return _shoppingCart; 
        }
    }

    public void reOrder(POCOS.Order order)
    {
        POCOS.Cart reordercart = new POCOS.Cart(order.storeX, this);
        order.cartX.copyTo(reordercart);
        reordercart.refresh(true);
        _shoppingCart = reordercart;
    }

    /// <summary>
    /// This method is to retrieve user shopping cart.  In current design, user will only have one shopping cart per store.
    /// User shall only different shopping cart for different store since the currency will be different.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private Cart getShoppingCart()
    {
        CartHelper cartHelper = new CartHelper();
        Cart cart = cartHelper.getCartMastersbyUserID(UserID, store.StoreID);

        if (cart == null)
        {
            //create a new cart for itself
            cart = new Cart(store, this);
            cart.CartID = this.UserID;
        }
        else
        {
            cart.Currency = store.defaultCurrency.CurrencyID;
            cart.LocalCurrency = null;
            cart.LocalCurExchangeRate = null;
        }

        return cart;
    }


    /// <summary>
    /// This is a read-only property to retrive current opening quotation for "Add to quotation" 
    /// purpose.
    /// </summary>
    public Quotation openingQuote
    {
        get
        {
            if (_openingQuotation == null ||
                !_openingQuotation.isModifiable())
            {
                QuotationHelper helper = new QuotationHelper();
                _openingQuotation = helper.getLastestOpenQuote(UserID, store.StoreID);

                if (_openingQuotation == null ||
                    !_openingQuotation.isModifiable())  //still not modifiable
                {
                    _openingQuotation = new Quotation(store, this);
                }
                else
                {
                    _openingQuotation.cartX.refresh();
                }
            }

            return _openingQuotation;
        }
    }

    public Quotation createQuote
    {
        get
        {
            return new Quotation(store, this);
        }
    }
    /// <summary>
    /// This property returns list of confirmed quotations
    /// </summary>
    public IList<Quotation> quotations
    {
        get
        {
            QuotationHelper helper = new QuotationHelper();
            
            IList<Quotation> quotes = helper.getUserQuotes(UserID, store.StoreID);
            if (quotes == null)    //no match
                quotes = new List<Quotation>();

            if (openingQuote.totalAmountX > 0)
                quotes.Insert(0, openingQuote);

            return quotes;
        }
    }

    /// <summary>
    /// This property returns list of confirmed orders
    /// </summary>
    public IList<Order> ordersX
    {
        get
        {
            OrderHelper helper = new OrderHelper();

            IList<Order> _orders = helper.getUserOrders(UserID, store.StoreID);
            if (_orders == null)    //no match
                _orders = new List<Order>();

            return _orders;
        }
    }

    /// <summary>
    /// This property indicates which role current user is.
    /// </summary>
    public Role role
    {
        get
        {
            if (isAuthenticated())
            {
                if (isEmployee())
                    return Role.Employee;
                else
                    return Role.Customer;
            }
            else
                return Role.Anonymous;
        }
    }

    /// <summary>
    /// ActingRole serves different purpose from ActingUser. ActingRole is used to simulate customer page view to understand
    /// what information customer can only see.   ActingUser is for Advantech Employee to act as actingUser, so they can
    /// create quotation, order and do possibly check out on behalf of customer.
    /// 
    /// ActingRole has no correlation with ActingUser.
    /// </summary>
    public Role actingRole
    {
        get 
        {
            if (_actingRole == Role.Default) //has initialized yet
                _actingRole = role;

            return _actingRole; 
        }
    }


    /// <summary>
    /// This property returns which user the current user is acting as.
    /// 
    /// ActingUser serves different purpose from ActingRole. ActingUser is for Advantech Employee to act as actingUser, so they can
    /// create quotation, order and do possibly check out on behalf of customer.  ActingRole is used to simulate customer page view to understand
    /// what information customer can only see.   
    /// 
    /// ActingUser has no correlation with ActingRole.
    /// </summary>
    public User actingUser
    {
        get
        {
            if (_actingUser == null)
                _actingUser = this;

            return _actingUser;
        }
    }

        public UserGrade userGradeX { get; set; } = null;

    /// <summary>
    /// Only employee has right to change role. actingUserID only takes effiect when newRols is Customer
    /// </summary>
    /// <param name="newRole"></param>
    public Boolean switchRole(Role newRole, String actingUserID = "")
    {
        Boolean result = false;

        if (isEmployee())
        {
            if (newRole == Role.Default)
            {
                _actingRole = role;
                _actingUser = this;
                result = true;
            }
            else
            {
                if (newRole == Role.Employee  && !String.IsNullOrWhiteSpace(actingUserID))
                {
                    if (actingUserID == this.UserID)
                    {
                        _actingUser = null;
                    }
                    else
                    {
                        User newUser = (new UserHelper()).getUserbyID(actingUserID);

                        if (newUser == null)    //not existing
                            result = false;
                        else
                        {
                            //The following two assignments are to make sure it's consistent with Store.getUser method logic
                            newUser.store = this.store;
                            newUser.newUser = false;
                            newUser.UserLanguages = this.UserLanguages; //inherit current user browser language settings

                            _actingUser = newUser;
                            _actingRole = newRole;
                            result = true;
                        }
                    }
                }
                else
                {
                    _actingRole = newRole;
                    _actingUser = this;
                    result = true;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// This method returns user authorization status at the specifying access (right).  Currently only very basic concept is 
    /// implemented.  This method requires later attention to complete it.  *******
    /// </summary>
    /// <param name="rightName"></param>
    /// <returns></returns>
    public Boolean hasRight(ACToken rightToken)
    {
        Boolean access = false;

        switch (rightToken)
        {
            case ACToken.ATP:
            case ACToken.InStock:
            case ACToken.Price_M:
            case ACToken.ModifyConfirmedDeal:
            case ACToken.AccessSAPContact:
                if (role == Role.Employee && actingRole == Role.Employee)
                    access = true;
                break;
            case ACToken.Price_V:
                if (role != Role.Anonymous)
                    access = true;
                break;
            case ACToken.SwitchRole:
                if (role == Role.Employee)
                    access = true;
                break;
            case ACToken.SimulateTransaction:
                if (actingRole == Role.Employee)
                    access = true;
                break;
            case ACToken.ViewCTOSOptionParts:
                if (actingRole == Role.Employee)
                    access = true;
                break;
        }

        return access;
    }
    private bool? _isEmployee;
    public Boolean isEmployee()
    {
        if (_isEmployee == null)
        {
            if (UserID.ToLower().IndexOf("@advantech") >= 0)
            {
                _isEmployee = true;
                /*** temporarily comment out due to validation logic flaw
                try
                {
                    EZWS.EZ_WS ezws = new EZWS.EZ_WS();
                    string firstname = string.Empty; string lastname = string.Empty; string fullname = string.Empty; string birthday = string.Empty; string gender = string.Empty;
                    string address = string.Empty; string hPhone = string.Empty; string mPhone = string.Empty;
                    ezws.getEmployeeInfo(UserID, ref  firstname, ref  lastname, ref fullname, ref birthday, ref gender, ref address, ref hPhone, ref mPhone);

                    if (string.IsNullOrEmpty(fullname))
                        _isEmployee= false;
                    else
                        _isEmployee = true;
                }
                catch (Exception)
                {

                    _isEmployee = true;
                }
                ****/
            }
            else
                _isEmployee = false;
        }
        return _isEmployee??false;
    }

    public ICollection<UserShortCut> myShortcuts
    {
        get
        {
            return UserShortCuts;
        }
    }

    public void addShortcut(UserShortCut shortcut)
    {
        shortcut.save();
        if (UserShortCuts == null)
            UserShortCuts = new List<UserShortCut>();
        
        //remove shortcut from shortcut list first if it already exists
        deleteShortcut(shortcut.ShortCutID);
        //add new or update shortcut to the list
        UserShortCuts.Add(shortcut);
    }

    public void deleteShortcut(int shortcutId)
    {
        UserShortCut exist = findShortcut(shortcutId);
        if (exist != null)
            deleteShortcut(exist);
    }

    private UserShortCut findShortcut(int shortcutId)
    {
        UserShortCut exist = (from item in UserShortCuts
                              where item.ShortCutID == shortcutId
                              select item).FirstOrDefault();
        return exist;
    }

    private void deleteShortcut(UserShortCut shortcut)
    {
        if (UserShortCuts != null && shortcut !=null)
            UserShortCuts.Remove(shortcut);

        shortcut.delete();
    }

    //Following runtime property will be assigned with real value when user sign in.  The default value will be initialized in user constructor
    public TimeSpan timeSpan { get; set; }
    public string[] UserLanguages { get; set; }

    /// <summary>
    /// user member
    /// </summary>
    private Member _member;
    public Member MemberX
    {
        get 
        {
            if (_member == null)
                _member = new MemberHelper().getMemberUserById(this.UserID);
            return _member; 
        }
        set { _member = value; }
    }

    public decimal IABLEpoint
    {
        get
        {
            try
            {
                return (new RewardRecordHelper()).GetCurrentPoint(this.UserID);
            }
            catch
            {
                return 0m;
            }
        }
    }

        private SiteBuilder _SiteBuilder;
        public SiteBuilder getSiteBuilder(bool reload = false)
        {
            if (_SiteBuilder == null || reload)
            {
                var helper = new SiteBuilderHelper();
                _SiteBuilder = helper.getSiteBuilder(this.UserID);
                if (_SiteBuilder == null)
                {
                    _SiteBuilder = new SiteBuilder(this);
                    _SiteBuilder.save();
                }
                _SiteBuilder.helper = helper;
            }

            return _SiteBuilder;
        }


        #endregion
    }
}