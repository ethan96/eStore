using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using eStore.Presentation.Widget;
using System.IO;
using System.Web.UI.HtmlControls;
using eStore.Presentation.Product;

namespace eStore.UI.Modules
{
    public partial class Widget : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public string outhtml { get; set; }
        public string WidgetName { get; set; }
        public bool isFullSize { get; set; } = false;
        public bool isMobileFriendly { get; set; } = false;

        public string CANONICAL { get; set; }

        HTMLPage htmlPage;
        protected bool widgetControlLoaded = false;
        protected override void OnInit(EventArgs e)
        {
            this.EnsureChildControls();
            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            if (!base.ChildControlsCreated)
            {
                base.CreateChildControls();
                this.LoadWidgetControls();
                base.ChildControlsCreated = true;
            }
        }

        private void LoadWidgetControls()
        {
            if (widgetControlLoaded)
                this.phServerControls.Controls.Clear();

            String widgetPageName = (Request["widgetPage"] != null) ? Request["widgetPage"] : string.Empty;
            //html名字.
            String widgetHtmlPage = (Request["htmlPage"] != null) ? Request["htmlPage"] + ".html" : string.Empty;
            string fileName = string.Empty;
            //String.IsNullOrEmpty(widgetHtmlPage)是刚加上去的. 为了不影响以前的逻辑.
            if (!String.IsNullOrEmpty(widgetPageName) && String.IsNullOrEmpty(widgetHtmlPage))
            {
                fileName = string.Format("{0}\\{1}\\{2}.htm", ConfigurationManager.AppSettings.Get("Widget_Path"), Presentation.eStoreContext.Current.Store.storeID, widgetPageName);
            }
            else
            {
                POCOS.WidgetPage widgetpage = null;
                if (!string.IsNullOrEmpty(WidgetName))
                {
                    widgetpage = Presentation.eStoreContext.Current.Store.getWidgetPage(WidgetName);
                }
                else if (Request["widgetid"] != null)
                {
                    int widgetid;
                    if (int.TryParse(Request["widgetid"], out widgetid))
                        widgetpage = Presentation.eStoreContext.Current.Store.getWidgetPage(widgetid);
                }
                else if (Request["WidgetName"] != null)
                    widgetpage = Presentation.eStoreContext.Current.Store.getWidgetPage(Request["WidgetName"]);
                else if (!String.IsNullOrEmpty(widgetPageName) && !String.IsNullOrEmpty(widgetHtmlPage))
                    widgetpage = Presentation.eStoreContext.Current.Store.getWidgetPage(widgetPageName);

                if (widgetpage != null)
                {
                    List<string> partids = widgetpage.Widgets.Where(x => !string.IsNullOrEmpty(x.SProductIDs)).Select(x => x.SProductIDs).Distinct().ToList();
                    if (partids.Any())
                        (new POCOS.DAL.PartHelper()).prefetchPartList(widgetpage.StoreID, partids);
                    widgetPageName = widgetpage.PageName;
                    fileName = string.Format("{0}\\Unzip\\{1}\\{2}\\{3}"
                        , ConfigurationManager.AppSettings.Get("Widget_Path")
                        , Presentation.eStoreContext.Current.Store.storeID
                        , widgetPageName
                        , string.IsNullOrEmpty(widgetHtmlPage) ? widgetpage.Path : widgetHtmlPage);

                    if (!string.IsNullOrEmpty(widgetpage.BusinessGroup))
                    {
                        var storeaddress = Presentation.eStoreContext.Current.Store.profile.StoreAddresses.FirstOrDefault(p => p.Division == widgetpage.BusinessGroup);
                        if (storeaddress != null && storeaddress.Address != null && !string.IsNullOrEmpty(storeaddress.Address.Tel))
                            ((MasterPages.MPV4)this.Page.Master).Tel = string.Format("<li><b>{0}</b></li>", storeaddress.Address.Tel);
                    }
                }
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                //no relative link replacement shall be done at the request through widgetPage parameter
                String leadingURL = (Request["widgetPage"] != null && String.IsNullOrEmpty(widgetHtmlPage)) ? String.Empty : String.Format(@"/resource/Widget/Unzip/{0}/{1}", Presentation.eStoreContext.Current.Store.storeID, widgetPageName);
                htmlPage = WidgetHandler.processHTMLFile(fileName, Presentation.eStoreContext.Current.Store, leadingURL,Request.RawUrl);
                if (htmlPage != null && htmlPage.ServerControl != null && htmlPage.ServerControl.Count > 0)
                {
                    foreach (ServerControl con in htmlPage.ServerControl)
                    {
                        LoadServerControl(con);
                    }
                    widgetControlLoaded = true;
                }
            }


        }
 
        private void LoadServerControl(ServerControl con)
        {

            switch (con.ServerControlType)
            {
                case ServerControlType.ProductList:
                    POCOS.ProductSpecRules psr = new POCOS.ProductSpecRules();
                    psr = Presentation.eStoreContext.Current.Store.getMatchProducts(con.para, string.Empty);
                    Modules.ProductList ucProductList = (Modules.ProductList)this.LoadControl("~/Modules/ProductList.ascx");
                    ucProductList.productList = psr._products;
                    ucProductList.ID = con.ID.ToString();
                    this.phServerControls.Controls.Add(ucProductList);
                    break;
                case ServerControlType.ProductMatrix:
                    POCOS.ProductSpecRules ProductMatrixpsr = new POCOS.ProductSpecRules();
                    ProductMatrixpsr = Presentation.eStoreContext.Current.Store.getMatchProducts(con.para, string.Empty);
                    Modules.ProductMatrix ucProductMatrix = (Modules.ProductMatrix)this.LoadControl("~/Modules/ProductMatrix.ascx");
                    ucProductMatrix.Products = ProductMatrixpsr._products;
                    ucProductMatrix.VProductMatrixList = ProductMatrixpsr._specrules;
                    ucProductMatrix.ID = con.ID.ToString();
                    this.phServerControls.Controls.Add(ucProductMatrix);
                    break;
                case ServerControlType.SimpleMatrix:


                    Modules.WidgetServerControls.SimpleMatrix ucSimpleMatrix = (Modules.WidgetServerControls.SimpleMatrix)this.LoadControl("~/Modules/WidgetServerControls/SimpleMatrix.ascx");
                    ucSimpleMatrix.CategoryID = con.para;
                    ucSimpleMatrix.ID = con.ID.ToString();
                    this.phServerControls.Controls.Add(ucSimpleMatrix);
                    break;
                case ServerControlType.ProductOperationButtons:
                    Panel pa = new Panel();
                    pa.CssClass = "rightside";
                    Presentation.eStoreControls.Button bt_compare = new Presentation.eStoreControls.Button();
                    bt_compare.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare);
                    bt_compare.Click += new EventHandler(bt_compare_Click);

                    Presentation.eStoreControls.Button bt_AddToQuote = new Presentation.eStoreControls.Button();
                    bt_AddToQuote.Click += new EventHandler(bt_AddToQuote_Click);

                    Presentation.eStoreControls.Button bt_AddToCart = new Presentation.eStoreControls.Button();
                    bt_AddToCart.Click += new EventHandler(bt_AddToCart_Click);

                    bt_AddToQuote.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation);
                    bt_AddToCart.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart);

                    pa.Controls.Add(bt_compare);
                    pa.Controls.Add(bt_AddToQuote);
                    pa.Controls.Add(bt_AddToCart);
                    pa.ID = con.ID.ToString();
                    pa.Visible = true;
                    this.phServerControls.Controls.Add(pa);
                    break;
                default:
                    break;

            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (htmlPage != null)
            {
                if (htmlPage.header != null && !string.IsNullOrEmpty(htmlPage.header.links))
                {
                    HtmlGenericControl node = new HtmlGenericControl("link");
                    node.InnerHtml = htmlPage.header.links; ;
                    Page.Header.Controls.Add(node);
                }

                if (htmlPage.header != null && !string.IsNullOrEmpty(htmlPage.header.scripts))
                {
                    HtmlGenericControl node = new HtmlGenericControl("scripts");
                    node.InnerHtml = htmlPage.header.scripts; ;
                    Page.Header.Controls.Add(node);
                }
                // htmlPage.header.
                if (!string.IsNullOrEmpty(htmlPage.header.title) || htmlPage.header.meta.ContainsKey("KEYWORDS") || htmlPage.header.meta.ContainsKey("DESCRIPTION"))
                    this.setPageMeta(htmlPage.header.title, (string)htmlPage.header.meta["DESCRIPTION"], (string)htmlPage.header.meta["KEYWORDS"]);
                if (htmlPage.header.meta.ContainsKey("KEYWORDS"))
                    Presentation.eStoreContext.Current.keywords.Add("Keywords", (string)htmlPage.header.meta["KEYWORDS"]);

                if (htmlPage.header.meta.ContainsKey("FULLSIZE") && htmlPage.header.meta["FULLSIZE"].ToString().ToUpper() == "TRUE")
                {
                    this.isFullSize = true;
                }
                if (htmlPage.header.meta.ContainsKey("MOBILEFRIENDLY") && htmlPage.header.meta["MOBILEFRIENDLY"].ToString().ToUpper() == "TRUE")
                {
                    this.isMobileFriendly = true;
                }

                if (htmlPage.header.meta.ContainsKey("CANONICAL") && !string.IsNullOrEmpty(htmlPage.header.meta["CANONICAL"].ToString()))
                {
                    this.CANONICAL = htmlPage.header.meta["CANONICAL"].ToString();
                }
            }

            postEventFunc();
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if (htmlPage != null)
            {
                if (phServerControls != null) { 
                foreach (Control con in phServerControls.Controls)
                {
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htmlw = new HtmlTextWriter(sw);
                    con.RenderControl(htmlw);
                    htmlw.Flush();
                    htmlw.Close();
                    string conContent = sw.ToString();
                    htmlPage = WidgetHandler.retrieveServerControlContent(new Guid(con.ID), conContent, htmlPage);
                }
                }
                outhtml = htmlPage.body.value;
                writer.Write(outhtml);
            }
            //base.Render(writer);
        }
      

        protected void bt_compare_Click(object sender, EventArgs e)
        {
            List<string> selectProductList = getStrSlectProductNo();
            ProductCompareManagement.setProductIDs(selectProductList);
            Response.Redirect("~/Compare.aspx");
        }

        protected void bt_AddToQuote_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            List<POCOS.Product> selectProductList = getSlectProductNo();
            QuoteCartCompareManager qcc = new QuoteCartCompareManager(selectProductList.ToList<POCOS.Part>());
            qcc.AddToQuote();
            this.Response.Redirect("~/Quotation/Quote.aspx");
        }

        protected void bt_AddToCart_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            List<POCOS.Product> selectProductList = getSlectProductNo();
            QuoteCartCompareManager qcc = new QuoteCartCompareManager(selectProductList.ToList<POCOS.Part>());
            qcc.AddToCart();
            //if (Presentation.eStoreContext.Current.Store.storeID == "AEU")
            //    this.Response.Redirect("~/Cart/ChannelPartner.aspx");
            //else
            this.Response.Redirect("~/Cart/Cart.aspx");
        }

        protected List<string> getStrSlectProductNo()
        {
            List<string> ls = new List<string>();
            string requestproducts = Request["cbproduct"];
            if (!string.IsNullOrEmpty(requestproducts))
            {
                ls = requestproducts.Split(',').ToList<string>();
            }
            return ls;
        }

        protected List<POCOS.Product> getSlectProductNo()
        {
            List<POCOS.Product> proLS = new List<POCOS.Product>();
            var proNOls = getStrSlectProductNo();
            foreach (string proNo in proNOls)
            {
                POCOS.Product product = Presentation.eStoreContext.Current.Store.getProduct(proNo);
                if (product != null)
                {
                    proLS.Add(product);
                }
            }
            return proLS;
        }

        protected void postEventFunc()
        {
            if (IsPostBack)
            {
                if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("WidgetCanPost"))
                {
                    esUtilities.EMailReponse response = null;

                    var keys = Request.Form.Keys;
                    List<BusinessModules.VWidgetModel> viewmodels = new List<BusinessModules.VWidgetModel>();
                    foreach (string key in keys)
                    {
                        if (key.StartsWith("RQ-P_", StringComparison.OrdinalIgnoreCase))
                            viewmodels.Add(new BusinessModules.VWidgetModel {
                                Key = key.Replace("RQ-P_", ""),
                                Value = Request.Form[key] ?? "",
                                Title = Request.Form[key.Replace("RQ-P_", "RQ-PN_")] ?? ""
                            });
                    }
                    if (viewmodels.Any() && viewmodels.FirstOrDefault(c=>c.Key.Equals("TempId",StringComparison.OrdinalIgnoreCase)) != null)
                    {
                        viewmodels.Add(new BusinessModules.VWidgetModel { Key = "PageUrl", Value = Request.Url.AbsoluteUri, Title = "Widget Link" });
                        string subject = Request.Form["RQ-Subject"] ?? eStore.Presentation.eStoreLocalization.Tanslation("eStore_widget_RequestSubjec");
                        string mailto = Request.Form["RQ-Mails"] ?? "";
                        eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
                        var vemail = viewmodels.FirstOrDefault(c => c.Key.Equals("email", StringComparison.OrdinalIgnoreCase));
                        response = mailTemplate.sendWidgetRequestEmail(viewmodels
                            , vemail != null ? vemail.Value.Split('@')[0] : subject
                            , subject
                            , eStore.Presentation.eStoreContext.Current.Store
                            , eStore.Presentation.eStoreContext.Current.CurrentLanguage
                            , eStore.Presentation.eStoreContext.Current.MiniSite
                            , mailto);
                    }
                    if (response != null && response.ErrCode == esUtilities.EMailReponse.ErrorCode.NoError)
                    {
                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Your_request_is_sent));
                        if (Request.Form["RQ-Save"] == "true")
                        {                            
                            if (viewmodels.Any())
                            {
                                try
                                {
                                    POCOS.WidgetRequest widgetRequest = new POCOS.WidgetRequest
                                    {
                                        StoreId = eStore.Presentation.eStoreContext.Current.Store.storeID,
                                        UserId = eStore.Presentation.eStoreContext.Current.User == null ? "" : eStore.Presentation.eStoreContext.Current.User.UserID,
                                        CreateDate = DateTime.Now,
                                        Url = Request.Url.AbsoluteUri
                                    };
                                    widgetRequest.WidgetRequestDetails = new List<POCOS.WidgetRequestDetail>();
                                    foreach (var item in viewmodels)
                                        widgetRequest.WidgetRequestDetails.Add(new POCOS.WidgetRequestDetail
                                        {
                                            Key = item.Key,
                                            Name = item.Title ?? item.Key,
                                            Value = HttpUtility.HtmlEncode(item.Value)
                                        });
                                    widgetRequest.save();
                                }
                                catch (Exception ex)
                                {
                                    throw;
                                }
                                
                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
        }
    }
}