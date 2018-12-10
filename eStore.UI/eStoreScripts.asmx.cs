using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text.RegularExpressions;

namespace eStore.UI
{
    /// <summary>
    /// Summary description for eStoreScripts
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class eStoreScripts : System.Web.Services.WebService
    {

        [WebMethod(enableSession:true,Description="product only")]
        public bool addProducttoCart(string productid, int qty)
        {
            if (eStore.Presentation.eStoreContext.Current.User != null)
            {
                POCOS.Part part = Presentation.eStoreContext.Current.Store.getPart(productid);
                if (part != null && qty > 0)
                {
                    eStore.Presentation.eStoreContext.Current.UserShoppingCart.addItem(part, qty);
                    return true;
                }
            }
            return false;
     
        }

        [WebMethod(enableSession: true, Description = "product only")]
        public bool addProducttoQuotation(string productid, int qty)
        {
            if (eStore.Presentation.eStoreContext.Current.User != null)
            {
                POCOS.Part part = Presentation.eStoreContext.Current.Store.getPart(productid);
                if (part != null && qty > 0)
                {
                    eStore.Presentation.eStoreContext.Current.Quotation.addItem(part, qty);
                    return true;
                }
            }
            return false;     
        }
        [WebMethod(enableSession: true, Description = "get ProductPage Link")]
        public string getProductPageLink(string productid)
        {
            string url="";
            POCOS.Part part = Presentation.eStoreContext.Current.Store.getPart(productid);
            if (part != null)
            {
                url = Presentation.UrlRewriting.MappingUrl.getMappingUrl(part);
                if (url.StartsWith("~"))
                    url = url.Remove(0, 1);
            }
            return url;

        }
        [WebMethod(enableSession: true, Description = "get Object URL")]
        public string getObjectURL(string ID, ObjectType oty)
        {
            string url = string.Empty;
            if (string.IsNullOrEmpty(ID))
                return url;

            ID = ID.Trim();
            switch (oty)
            {
                case ObjectType.Category:
                    int cid = 0;
                    if (int.TryParse(ID, out cid) == true && cid > 0)
                    {
                        var category = Presentation.eStoreContext.Current.Store.getProductCategory(cid);
                        if (category != null)
                            url = Presentation.UrlRewriting.MappingUrl.getMappingUrl(category);
                    }
                    break;
                case ObjectType.Menu:
                    int mid = 0;
                    if (int.TryParse(ID, out mid) == true && mid > 0)
                    {
                        var menu = new POCOS.DAL.MenuHelper().getMenusByid(Presentation.eStoreContext.Current.Store.storeID, mid);
                        if (menu != null)
                            url = Presentation.UrlRewriting.MappingUrl.getMappingUrl(menu);
                    }
                    break;
                case ObjectType.Widget:
                    int wid = 0;
                    if (int.TryParse(ID, out wid) == true && wid > 0)
                    {
                        var widget = Presentation.eStoreContext.Current.Store.getWidgetPage(wid);
                        if (widget != null)
                            url = Presentation.UrlRewriting.MappingUrl.getMappingUrl(widget);
                    }
                    break;
                case ObjectType.Product:
                default:
                    url = getProductPageLink(ID);
                    break;
            }

            return url;
        }
        [WebMethod(enableSession: true, Description = "get getCustomersAlsoBought and CrossSellProducts")]
        public object getSuggestingProducts(string type)
        {
            POCOS.Cart cart;
            if (type == "cart_cart_aspx")
            {
                cart = Presentation.eStoreContext.Current.UserShoppingCart;
            }
            else if (type == "quotation_quote_aspx" && Presentation.eStoreContext.Current.Quotation != null)
            {
                cart = Presentation.eStoreContext.Current.Quotation.cartX;
            }
            else if (type == "widget_aspx")
            {
                object obj= getSuggestingProducts();
                return obj;
            }
            else
                return null;

            if (cart == null || cart.CartItems.Count == 0)
                return null;

            Dictionary<string, List<POCOS.Product>> suggestingProductsgroups = new Dictionary<string, List<POCOS.Product>>();
            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("Cart_Cross_Sell_Products"))
            {
                List<POCOS.Product> crossSellProducts = Presentation.eStoreContext.Current.Store.getCrossSellProductByCart(cart);
                if (crossSellProducts != null && crossSellProducts.Any())
                    suggestingProductsgroups.Add(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Cross_Sell_Product), crossSellProducts);
            }

            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("Cart_Suggesting_Products") )
            {

                List<POCOS.Product> suggestingProducts = Presentation.eStoreContext.Current.Store.getCustomersAlsoBought(cart);
                if (suggestingProducts != null && suggestingProducts.Any())
                    suggestingProductsgroups.Add(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Suggesting_Products), suggestingProducts);
            }


            Presentation.Product.PriceStyle style = Presentation.Product.PriceStyle.productprice;
            var storeurl = esUtilities.CommonHelper.GetStoreLocation();
            storeurl = storeurl.Substring(0,storeurl.Length -1);
            var rlt = (from g in suggestingProductsgroups
                       select new
                       {
                           Title = g.Key,
                           Products = (from p in g.Value
                                       where p.isOrderable()
                                       select new
                                        {
                                            ProductID = p.name,
                                            Desc = ((POCOS.Product)p).productDescX ?? p.name,
                                            Hyperlink = storeurl + eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(p).Replace("~", ""),
                                            image = p.thumbnailImageX ?? string.Empty,
                                            price = Presentation.Product.ProductPrice.getSimplePrice(p, style)
                                        }).Take(4)
                       });
            return rlt;
        }

        public object getSuggestingProducts()
        {
            Dictionary<string, List<POCOS.Product>> suggestingProductsgroupsX = new Dictionary<string, List<POCOS.Product>>();
            List<POCOS.Product> productList = eStore.Presentation.Widget.WidgetHandler.WigetCrossSellProduct;
            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("Cart_Cross_Sell_Products"))
            {
                List<POCOS.Product> crossSellProducts = Presentation.eStoreContext.Current.Store.getCrossSellProductByProductList(productList);
                if (crossSellProducts != null && crossSellProducts.Any())
                    suggestingProductsgroupsX.Add(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Cross_Sell_Product), crossSellProducts);
            }

            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("Cart_Suggesting_Products"))
            {
                List<POCOS.Product> suggestingProducts = Presentation.eStoreContext.Current.Store.getCustomersAlsoBoughtX(productList);
                if (suggestingProducts != null && suggestingProducts.Any())
                    suggestingProductsgroupsX.Add(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Suggesting_Products), suggestingProducts);
            }

            Presentation.Product.PriceStyle style = Presentation.Product.PriceStyle.productprice;
            var rlt = (from g in suggestingProductsgroupsX
                       select new
                       {
                           Title = g.Key,
                           Products = (from p in g.Value
                                       where p.isOrderable()
                                       select new
                                       {
                                           ProductID = p.name,
                                           Desc = ((POCOS.Product)p).productDescX ?? p.name,
                                           Hyperlink = eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(p).Replace("~", ""),
                                           image = p.thumbnailImageX ?? string.Empty,
                                           price = Presentation.Product.ProductPrice.getSimplePrice(p, style)
                                       }).Take(4)
                       });
            return rlt;
        }

        [WebMethod(enableSession: true, Description = "get ustore application")]
        public Presentation.ApplicationModel.Application getApplication(string path)
        {
            Presentation.ApplicationModel.Application app= new Presentation.ApplicationModel.Application();
            app.load(eStore.Presentation.eStoreContext.Current.Store, path);
            if (app.isValid)
                return app;
            else
                return null;

        }

        #region Scripts settings
        /// <summary>
        /// support showing multiline banners in eStore Homepage. 
        /// </summary>
        /// <returns></returns>
        [WebMethod(enableSession: true, CacheDuration=7200, Description = "support showing multiline banners in eStore Homepage. Make this to be a store parameter and set its default to 1 if this value is missing and set AEU default value to 2. Name the parameter as “HomeBannerLineCap")]
        public int getHomeBannerLineCap()
        {
            int line  =eStore.Presentation.eStoreContext.Current.getIntegerSetting("HomeBannerLineCap", 1);
            return line;
        }

        /// <summary>
        /// 返回前台所需的Json数据 (addon product dropdown list)
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        [WebMethod(enableSession: true, Description = "返回前台所需的Json数据")]
        public object getIDKAttributeByProductID(string productid)
        {
            var list = Presentation.eStoreContext.Current.Store.getProductIDKAttributeBySproductID(productid);
            if (list != null && list.Any())
            {
                Regex regexObj = new Regex(@"(\d+)");
                return (from attibute in list
                        group attibute by attibute.AttributeName into g
                        select new
                        {
                            Name = g.Key,
                            NameX = g.Key.Replace(' ', '_'),
                            Items =(from v in g
                                    select new 
                                        {
                                            Display = v.AttributeValue.Replace("INCH", "''"),
                                            Value = v.AttributeValue
                                        }).OrderBy(x => g.Key == "Size" ?  regexObj.Match(x.Display).Groups[1].Value.PadLeft(2,'0'): x.Value).Distinct()
                        });
            }
            else
                return null;
        }

        [WebMethod(enableSession: true, Description = "")]
        public object getIDKAddons(string productid)
        {
            return Presentation.eStoreContext.Current.Store.getIDKAddons(productid);
        }

        [WebMethod(enableSession: true, Description = "")]
        public object getIDKCompatibilityEmbeddedBoard(string productid)
        {
            return Presentation.eStoreContext.Current.Store.getIDKCompatibilityEmbeddedBoard(productid);
        }

        /// <summary>
        /// 通过塞选条件 获取Addon itemid list提供前台塞选
        /// </summary>
        /// <param name="filterstr"></param>
        /// <returns></returns>
        [WebMethod(enableSession: true, Description = "通过塞选条件 获取Addon itemid list提供前台")]
        public object getPeripheralAddOnIDSByFilterstring(string productid, string filterstr)
        {
            if (string.IsNullOrEmpty(filterstr))
                return string.Empty;
            Dictionary<string, string> cc = new Dictionary<string, string>();
            var aa = filterstr.Split(';');
            if (aa != null && aa.Length > 0)
            {
                foreach (var a in aa)
                {
                    if (string.IsNullOrEmpty(a))
                        continue;
                    var c = a.Split('|');
                    cc.Add(c[0], c[1]);
                }
            }
            if (cc.Any())
            {
                List<string> list = Presentation.eStoreContext.Current.Store.getIDKPeripheralAddOnByAttribute(productid,cc);
                return list;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取Product的所有Addons
        /// </summary>
        /// <param name="sproductid"></param>
        /// <returns></returns>
        [WebMethod(enableSession: true, Description = "获取Product的所有Addons")]
        public string getProductAddonsBySproductid(string sproductid)
        {
            Presentation.IDKProductUI.IDKUIHelper helper = new Presentation.IDKProductUI.IDKUIHelper();
            return helper.getAddonList(sproductid);
        }

        [WebMethod(enableSession: true, Description = "get all countries",CacheDuration=120)]
        public object getAllCountries()
        {
            List<POCOS.Country> countries = new List<POCOS.Country>();
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            countries = eSolution.countries.OrderBy(c => c.CountryName).ToList();
            var rlt = (from c in countries
                       select new
                       {
                           countryName = c.CountryName
                       });
            return rlt;
        }

        [WebMethod(enableSession: true, Description = "change Current Country")]
        public string changeCurrentCountry(string countryName)
        {
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            List<POCOS.Country> countries = eSolution.countries.ToList();
            POCOS.Country country = countries.FirstOrDefault(c => c.CountryName == countryName);

            BusinessModules.Store currentStore = Presentation.eStoreContext.Current.Store;
            HttpContext _context = HttpContext.Current;
            string redirectUrl = "http://" + _context.Request.Url.Authority;
            if (country == null)
                return redirectUrl;
            else if (country.StoreID != currentStore.storeID)
            {
                Boolean isTestMode = false;
                if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["TestingMode"]))
                    isTestMode = false;
                else
                    isTestMode= System.Configuration.ConfigurationManager.AppSettings["TestingMode"] == "true";

                if (isTestMode)
                {
                    _context.Session.Clear();
                    //_context.Session.Abandon();
                    _context.Session["CurrentStore"] = country.StoreID;
                    redirectUrl = string.Format("http://{3}/?storeid={0}&country={1}&token={2}", country.StoreID, country.CountryName, DateTime.Now.Ticks.ToString(), _context.Request.Url.Authority);
                }
                else
                    redirectUrl = string.Format("http://{0}?country={1}&token={2}", country.storeX.StoreURL, country.CountryName, DateTime.Now.Ticks.ToString());
            }
            else if (Presentation.eStoreContext.Current.CurrentCountry == null)
            {
                //这里好像要重新赋值下
                //_currentCountry = country;
                _context.Session["seletedcountry"] = country;
                //默认返回 首页
            }
            else if (country.StoreID == currentStore.storeID && Presentation.eStoreContext.Current.CurrentCountry.CountryID != country.CountryID)
            {
                //这里好像要重新赋值下
                //_currentCountry = country;
                _context.Session["seletedcountry"] = country;
                //默认返回 首页
            }
            else
            {
                //这里好像要重新赋值下
                //_currentCountry = country;
                _context.Session["seletedcountry"] = country;
            }
            return redirectUrl;
        }

        [WebMethod(enableSession: true, Description = "检验验证码是否正确")]
        public string checkVerification(string vCode)
        {
            if (string.IsNullOrEmpty(vCode))
                return "false";
            return (vCode.ToUpper() == eStore.Presentation.eStoreContext.Current.verificationCode.ToUpper()).ToString().ToLower();
        }

        [WebMethod(enableSession: true, Description = "check bank information")]
        public object getsafeBankStr(string bankcode, bool cbsimulation = false)
        {
            var user = Presentation.eStoreContext.Current.User;
            if (cbsimulation && (user == null || !user.isEmployee()))
                cbsimulation = false;

            string bankCodes = Presentation.eStoreContext.Current.getStringSetting("BankCodes");
            if (!string.IsNullOrEmpty(bankCodes))
            { 
                var bankCodeList = bankCodes.Split(',');
                if (bankCodeList.Contains(bankcode))
                {
                    POCOS.StorePayment storePayment = Presentation.eStoreContext.Current.Store.getStorePayment(Presentation.eStoreContext.Current.Order.PaymentType);
                    POCOS.Payment paymentInfo = new POCOS.Payment() { CCAuthCode = bankcode};
                    IDictionary<String, String> paras = Presentation.eStoreContext.Current.Store.prepareIndirectPayment(Presentation.eStoreContext.Current.Order, storePayment, paymentInfo, cbsimulation, false);
                    if (paras != null && paras.Count > 0)
                        return new { signMsg = paras["signMsg"], orderTime = paras["orderTime"] };
                }
            }
            return null;
        }

        [WebMethod(enableSession: true, Description = "根据国家和省份获取 供应商信息")]
        public object getPartnerList(string countryCode, string stateCode, string webSite = "eStore")
        {
            try
            {
                BusinessModules.ServicePartner.AdminWebService partnerService = new BusinessModules.ServicePartner.AdminWebService();
                BusinessModules.ServicePartner.Solutions_Company_VIEW[] companyViewArray = partnerService.GetPartnerList(countryCode, stateCode, webSite);

                return companyViewArray;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [WebMethod(enableSession: true, Description = "砸金蛋")]
        public object smashGoldEggs(int advid)
        {
            var giftls = Presentation.eStoreContext.Current.Store.getAllGiftActivity(advid);
            if (giftls == null) //没有礼物
                return null;

            string userid = "";

            Guid id = Guid.NewGuid();
            if (Presentation.eStoreContext.Current.User != null)
                userid = Presentation.eStoreContext.Current.User.UserID;
            else if (HttpContext.Current.Request.Cookies["LogTemId"] != null)
                userid = HttpContext.Current.Request.Cookies["LogTemId"].Value;
            else
                userid = id.ToString();
            
            POCOS.GiftLog giftlog;
            var ls = Presentation.eStoreContext.Current.Store.getGiftLogByUserid(userid, advid);
            if (ls != null && ls.Any())
                giftlog = ls.FirstOrDefault();
            else
            {
                Dictionary<int, decimal> round = new Dictionary<int, decimal>();
                foreach (var c in giftls)
                    round.Add(c.GiftId, c.Probability.Value * 100);
                int giftid = esUtilities.StringUtility.lottery(round);
                POCOS.GiftActivity gift = giftls.FirstOrDefault(c => c.GiftId == giftid);
                
                giftlog = new POCOS.GiftLog()
                {
                    CreateDate = DateTime.Now,
                    IP = HttpContext.Current.Request.ServerVariables["Remote_Addr"],
                    Storeid = Presentation.eStoreContext.Current.Store.storeID,
                    LogStatuse = false,
                    UserId = userid,
                    LogId = id,
                    GiftId = gift.GiftId,
                    HasSend = false
                };
                if (giftlog.save() == 0 && Presentation.eStoreContext.Current.User == null)
                {
                    HttpCookie cookie = new HttpCookie("LogTemId");
                    cookie.Value = id.ToString();
                    cookie.Expires = DateTime.Now.AddDays(10); ; //10 day
                    HttpContext.Current.Response.AppendCookie(cookie);
                }
            }
            return new { Id = giftlog.LogId, ImageUrl = giftlog.GiftActivityX.ImageUrl, smashed = giftlog.LogStatuse.Value, Desc = giftlog.GiftActivityX.GiftDescription };
        }

        [WebMethod(enableSession: true, Description = "砸金蛋")]
        public object modifyGoldEggsGiftUser(string temId)
        {
            if (string.IsNullOrEmpty(temId) || Presentation.eStoreContext.Current.User == null
                    || HttpContext.Current.Request.Cookies["LogTemId"] == null
                    || HttpContext.Current.Request.Cookies["LogTemId"].Value.ToUpper() != temId.ToUpper())
                return false;
            var ls = Presentation.eStoreContext.Current.Store.getGiftLogByUserid(temId);
            var userGls = Presentation.eStoreContext.Current.Store.getGiftLogByUserid(Presentation.eStoreContext.Current.User.UserID);
            if (ls != null && ls.Any())
            {
                foreach (var u in userGls)
                    u.delete();
            }
            foreach (var c in ls)
            {
                c.UserId = Presentation.eStoreContext.Current.User.UserID;
                c.save();
            }
            HttpCookie cookieID = new HttpCookie("LogTemId");
            cookieID.Expires = DateTime.Now.AddHours(-24);
            HttpContext.Current.Response.Cookies.Add(cookieID);
            return true;
        }

        [WebMethod(enableSession: true, Description = "领取金蛋")]
        public object submintUserInfor(string userAddrss, string userTel, bool isSubMit = false)
        {
            if (Presentation.eStoreContext.Current.User == null)
                return false;
            if (!isSubMit)
                return true;
            if (string.IsNullOrEmpty(userAddrss) && string.IsNullOrEmpty(userTel))
                return false;
            var user = Presentation.eStoreContext.Current.User;
            var _addrss = esUtilities.StringUtility.replaceSpecialString(userAddrss);
            var _tel = esUtilities.StringUtility.replaceSpecialString(userTel);
            user.MemberX.MOBILE = _tel;
            user.MemberX.ADDRESS = _addrss;
            if (user.MemberX.save() == 0)
            {
                var userGls = Presentation.eStoreContext.Current.Store.getGiftLogByUserid(Presentation.eStoreContext.Current.User.UserID);
                foreach (var c in userGls)
                {
                    c.LogStatuse = true;
                    c.save();
                }
                return true;
            }
            else
                return false;

        }


        [WebMethod(enableSession: true, Description = "转盘活动")]
        public object getGiftActivityCount(int advid)
        {
            int activityCount = 1; //抽奖机会
            if (advid == null)
                return new { ErrorMsg = "活动已结束!" };
            if (Presentation.eStoreContext.Current.User == null)
                return new { ErrorMsg = "请先登录!", ActivityCount = 0, GiftTitle = "" };

            var userGiftlogls = Presentation.eStoreContext.Current.Store.getGiftLogByUserid(Presentation.eStoreContext.Current.User.UserID, advid);
            var nowDate = Presentation.eStoreContext.Current.Store.getLocalDateTime(DateTime.Now).Date;
            activityCount = activityCount - userGiftlogls.Where(c => Presentation.eStoreContext.Current.Store.getLocalDateTime(c.CreateDate.Value).Date == nowDate).Count();
            var userGift = userGiftlogls.Where(c => c.HasSend.GetValueOrDefault()).FirstOrDefault();
            return new { ActivityCount = activityCount, GiftTitle = (userGift == null ? "" : userGift.GiftActivityX.GiftName ) };
        }

        [WebMethod(enableSession: true, Description = "转盘活动")]
        public object getRateActivitByUser(int advid)
        {
            if(advid == null)
                return new { ErrorMsg = "活动已结束!" };
            if (Presentation.eStoreContext.Current.User == null)
                return new { ErrorMsg = "请先登录!" };

            var userGiftlogls = Presentation.eStoreContext.Current.Store.getGiftLogByUserid(Presentation.eStoreContext.Current.User.UserID, advid);
            var nowDate = Presentation.eStoreContext.Current.Store.getLocalDateTime(DateTime.Now).Date;
            if (userGiftlogls.Where(c => Presentation.eStoreContext.Current.Store.getLocalDateTime(c.CreateDate.Value).Date == nowDate).Any())
                return new { ErrorMsg = string.Format(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.RateGift_rategifttime), 0) };
            
            List<POCOS.GiftActivity> giftls = new List<POCOS.GiftActivity>();
            var adv = Presentation.eStoreContext.Current.Store.getAdByID(advid);
            int maxNumber = 999999; // 中奖之后将不能在中大奖
            if (adv.isInActivity)
            {
                giftls = adv.GiftActivities.ToList();

                if (userGiftlogls.Where(c => c.HasSend.GetValueOrDefault()).Any()) //if user have gift will not get gift again
                    giftls = giftls.Where(c => c.MaxNumber == maxNumber).ToList(); //不会在中奖
                else //
                {
                    List<POCOS.GiftActivity> giftlstemp = new List<POCOS.GiftActivity>();
                    var lostGift = giftls.Where(c => c.MaxNumber != maxNumber && c.GiftLogs.Count >= c.MaxNumber).ToList();
                    foreach (var item in giftls)
                    {
                        if (lostGift.FirstOrDefault(c => c.GiftId == item.GiftId) == null)
                            giftlstemp.Add(item);
                    }
                    giftls = giftlstemp;
                }

                if (giftls.Any()) // adv is in activity
                {

                    Dictionary<int, decimal> round = new Dictionary<int, decimal>();
                    foreach (var c in giftls)
                        round.Add(c.GiftId, c.Probability.Value * 100);
                    int giftid = esUtilities.StringUtility.lottery(round);
                    POCOS.GiftActivity gift = giftls.FirstOrDefault(c => c.GiftId == giftid);

                    POCOS.GiftLog giftlog;
                    giftlog = new POCOS.GiftLog()
                    {
                        CreateDate = DateTime.Now,
                        IP = HttpContext.Current.Request.ServerVariables["Remote_Addr"],
                        Storeid = Presentation.eStoreContext.Current.Store.storeID,
                        LogStatuse = true,
                        UserId = Presentation.eStoreContext.Current.User.UserID,
                        LogId = Guid.NewGuid(),
                        GiftId = gift.GiftId,
                        HasSend = !(gift.MaxNumber == maxNumber), // 是否已经中奖，中奖后将不能再次抽奖
                        AdvId = advid
                    };
                    if (giftlog.save() == 0)
                    {
                        if (giftlog.HasSend.GetValueOrDefault())
                        {
                            // send mail
                            eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
                            mailTemplate.getRateGiftContent(giftlog, Presentation.eStoreContext.Current.User);
                        }
                        return new { ErrorMsg = "", GiftId = gift.ImageUrl, Title = gift.GiftName, Desc = gift.GiftDescription, GiftGuid = giftlog.LogId };
                    }
                    else
                        return new { ErrorMsg = "抽取失败!" };
                }

            }

            return new { ErrorMsg = "礼品已发送完毕!" };
        }





        [WebMethod(enableSession: true, Description = "获取最近热销产品")]
        public object getMostBuyPro(int count, string storeid = "", bool eStorePro = true)
        {
            string storeurl = "";
            if (eStorePro)
                storeurl = "http://" + Presentation.eStoreContext.Current.Store.profile.StoreURL;
            if (string.IsNullOrEmpty(storeid))
                storeid = Presentation.eStoreContext.Current.Store.storeID;
            List<POCOS.Product> ls = Presentation.eStoreContext.Current.Store.getHotDeals(200, storeid, 3);
            var obj = from p in ls
                      select new 
                      {
                          SProductID = p.name,
                          Image = p.thumbnailImageX,
                          Desc = p.productDescX,
                          Link = storeurl + System.Web.VirtualPathUtility.ToAbsolute(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(p))
                      };
            return obj;
        }


        [WebMethod(enableSession: true, Description = "GetProduct")]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = true, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public void GetProduct(string data, bool isRoot, int page, int appendix)
        {
            List<POCOS.Product> Partls = new List<POCOS.Product>();
            eStore.POCOS.CachePool pool = eStore.POCOS.CachePool.getInstance();
            Dictionary<string, List<POCOS.Product>> appendixDic = null;

            if (appendix > 0)
                appendixDic = pool.getAppendixSpecCategory(appendix);

            if (isRoot)
            {
                int id = 0;
                int.TryParse(data, out id);

                var _category = pool.getSpecCategory(id);
                if (_category != null)
                {
                    _category.StoreID = eStore.Presentation.eStoreContext.Current.Store.storeID;
                    Partls = eStore.Presentation.eStoreContext.Current.Store.getProductList(string.Join(",", _category.allSpecParts.Where(p => string.IsNullOrEmpty(p.StoreID) || p.StoreID == eStore.Presentation.eStoreContext.Current.Store.storeID).Select(c => c.PART_NO)));
                }
                else
                {
                    _category = eStore.Presentation.eStoreContext.Current.Store.getSpecCategoryById(id);
                    _category.StoreID = eStore.Presentation.eStoreContext.Current.Store.storeID;
                    Partls = eStore.Presentation.eStoreContext.Current.Store.getProductList(string.Join(",", _category.allSpecParts.Where(p => string.IsNullOrEmpty(p.StoreID) || p.StoreID == eStore.Presentation.eStoreContext.Current.Store.storeID).Select(c => c.PART_NO)));
                    pool.cacheSpecCategory(_category, id);
                }

                if (appendix > 0 && appendixDic == null)
                {
                    foreach (var p in Partls)
                    {
                        if (p is POCOS.Product_Ctos)
                        {
                            var v3 = _category.allSpecParts.Where(s => s.PART_NO == p.SProductID && s.StoreID == p.StoreID && !string.IsNullOrEmpty(s.MainPartModelNo)).FirstOrDefault() ?? null;
                            if (v3 != null)
                                p.PageTitle = v3.MainPartModelNo;
                            else
                                p.PageTitle = string.Empty;
                        }
                    }
                }
            }
            else
            {
                List<int> ls = new List<int>();
                if (!string.IsNullOrEmpty(data))
                {
                    int c = 0;
                    foreach (var t in data.Split(','))
                    {
                        int.TryParse(t, out c);
                        if (c > 0 && !ls.Contains(c))
                            ls.Add(c);
                    }
                }
                Partls = eStore.Presentation.eStoreContext.Current.Store.getSpecByCategoryIds(ls, eStore.Presentation.eStoreContext.Current.Store.storeID).Select(c => (c.part as POCOS.Product)).ToList();
            }
            List<eStore.POCOS.Part_Spec_V3> allSpec = pool.getPartSpecV3List();
            if (Partls.Any())
            {
                if (allSpec == null)
                {
                    allSpec = eStore.Presentation.eStoreContext.Current.Store.getAllPart_Spec_V3();
                    pool.cachePartSpecV3(allSpec);
                }
            }

            if (appendixDic == null)
            {
                appendixDic = new Dictionary<string, List<POCOS.Product>>();
                if (appendix > 0)
                {
                    List<POCOS.PocoX.EasyUITreeNode> nodes = Presentation.eStoreContext.Current.Store.getEasyUITreeNodeByCategoryID(appendix);
                    if (nodes != null && nodes.Any())
                    {
                        var root = nodes.FirstOrDefault();
                        foreach (POCOS.Product p in Partls)
                        {
                            string modelNo = p.ModelNo;
                            if (p is POCOS.Product_Ctos)
                            {
                                if (!string.IsNullOrEmpty(p.PageTitle))
                                    modelNo = p.PageTitle;
                                else
                                    continue;
                            }
                            var matchModels = root.children.Where(c => c.text == modelNo).ToList();
                            Dictionary<string, string> appendixModelDic = new Dictionary<string, string>();
                            foreach (var mm in matchModels)
                            {
                                foreach (var c in mm.children)
                                {
                                    if (!appendixModelDic.ContainsKey(c.text))
                                        appendixModelDic.Add(c.text, string.Format("{0},{1}", c.nodeTree, c.displayType));
                                }
                            }
                            var appendModels = appendixModelDic.Keys.ToList();
                            if (appendModels.Any())
                            {
                                List<POCOS.Product> pd = Presentation.eStoreContext.Current.Store.getProductListByModelNo(appendModels);
                                if (pd.Any())
                                {
                                    pd = pd.Where(x => appendixModelDic.ContainsKey(x.ModelNo)).ToList();
                                    foreach (var d in pd)
                                    {
                                        if (appendixModelDic.ContainsKey(d.ModelNo))
                                            d.PageTitle = appendixModelDic[d.ModelNo];
                                    }
                                    if (appendixDic.ContainsKey(p.SProductID))
                                    {
                                        var newList = appendixDic[p.SProductID];
                                        newList.AddRange(pd);
                                        appendixDic[p.SProductID] = newList;
                                    }
                                    else
                                        appendixDic.Add(p.SProductID, pd);
                                }
                            }
                        }
                        pool.cacheAppendixSpecCategory(appendixDic, appendix);
                    }
                }
            }

            var ps = Partls.Select(p => new
            {
                Count = Partls.Count,
                SProductID = p.DisplayPartno,
                VendorFeatures = p.productFeatures,
                TumbnailimageID = p.thumbnailImageX,
                DataSheet = p.dataSheetX,
                URL = eStore.Presentation.Widget.WidgetHandler.ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(p)),
                Desc = p.productDescX,
                Categoryids = string.Join(",", allSpec.Where(c => c.PART_NO.Equals(p.SProductID, StringComparison.OrdinalIgnoreCase)).Select(c => c.CATEGORY_ID)),
                Appendix = appendixDic.ContainsKey(p.SProductID) ? appendixDic[p.SProductID].Select(c => new
                {
                    PartNo = c.SProductID,
                    ImageURL = c.ImageURL,
                    Category = c.PageTitle,
                    ItemURL = Presentation.Widget.WidgetHandler.ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(c))
                }).ToList() :  null
            }).Skip(page * 10 - 10).Take(10).ToList();
            Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(ps));
        }

        [WebMethod(enableSession: true, Description = "SetCompare")]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = true, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public void SetCompare(string data)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                List<string> ls = data.Split(',').ToList();
                if (!ls.Any())
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_select_product_first));
                    Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(false));
                }
                else
                {
                    Presentation.Product.ProductCompareManagement.setProductIDs(ls);
                    Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(true));
                }
            }
            else
                Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(false));
            Context.Response.End();
        }

        [WebMethod(enableSession: true, Description = "search Service")]
        public string getSearchURL(string key)
        {
            if (string.IsNullOrEmpty(key))
                return searchPage(key);
            List<POCOS.spProductSearch_Result> result = eStore.Presentation.eStoreContext.Current.Store.getMatchProducts(key, 5);
            if (result == null || (result.Count > 1 && !result.Any(c => c.SProductID.Equals(key,StringComparison.OrdinalIgnoreCase) || c.Name.Equals(key,StringComparison.OrdinalIgnoreCase))))
                return searchPage(key);
            else
            {
                var part = eStore.Presentation.eStoreContext.Current.Store.getPartbyDisplayName(key);
                if (part == null)
                    return searchPage(key);
                else
                    return esUtilities.CommonHelper.ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(part));
            }
        }
        private string searchPage(string key)
        {
            return esUtilities.CommonHelper.GetStoreLocation() + "Search.aspx?skey=" + key;
        }
        #endregion


        #region ushop


        [WebMethod(enableSession: true, Description = "get all state by country name")]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json, UseHttpGet = true)]
        public bool checkUser()
        {
            string useremail = esUtilities.StringUtility.striphtml(HttpContext.Current.Request["useremail"]) ?? "";
            return Presentation.eStoreContext.Current.Store.sso.isExist(useremail, "ESTORE_CN_USHOP");
        }

        [WebMethod(enableSession: true, Description = "Check user by Id")]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json, UseHttpGet = true)]
        public bool CheckUser(string userId)
        {
            string useremail = esUtilities.StringUtility.striphtml(userId ?? "");
            return Presentation.eStoreContext.Current.Store.CheckUser(useremail);
        }
        [WebMethod(enableSession: true, Description = "Check current user psd")]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json, UseHttpGet = true)]
        public object CheckCurrPSD(string psd)
        {
            bool d = false;
            if (eStore.Presentation.eStoreContext.Current.User == null)
                d = false;
            else
            {
                if (esUtilities.StringUtility.StringEncry(psd) == eStore.Presentation.eStoreContext.Current.User.LoginPassword)
                    d = true;
            }
            return new { d = d };        
        }

        [WebMethod(enableSession: true, Description = "get all state by country name")]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json, UseHttpGet = true)]
        public string getAllStateByCountry()
        {
            string countryname = HttpContext.Current.Request["countryname"] ?? "";
            string jsonStr = "";
            countryname = esUtilities.StringUtility.striphtml(countryname);
            if (!string.IsNullOrEmpty(countryname))
                jsonStr = Presentation.eStoreContext.Current.Store.sso.GetStateListByCountryInJson(countryname);
            return jsonStr;
        }


        #endregion

        public enum ObjectType
        {
            Product,
            Menu,
            Category,
            Widget
        }
    }
}
