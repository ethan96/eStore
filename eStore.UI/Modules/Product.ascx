<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Product.ascx.cs" Inherits="eStore.UI.Modules.Product"
    EnableViewState="false" %>
<%@ Register Src="ProductLiterature.ascx" TagName="ProductLiterature" TagPrefix="eStore" %>
<%@ Register Src="ProductSharetoFriends.ascx" TagName="ProductSharetoFriends" TagPrefix="eStore" %>
<%@ Register Src="QuantityDiscountRequest.ascx" TagName="QuantityDiscountRequest"
    TagPrefix="eStore" %>
<%@ Register Src="YouAreHereMutli.ascx" TagName="YouAreHereMutli" TagPrefix="eStore" %>
<%@ Register Src="ChangeCurrency.ascx" TagName="ChangeCurrency" TagPrefix="eStore" %>
<%@ Register Src="ProductDependencies.ascx" TagName="ProductDependencies" TagPrefix="eStore" %>


<div class="eStore_product_productID">
    <asp:HiddenField ID="hfProductID" runat="server" />
</div>
<eStore:YouAreHereMutli ID="YouAreHereMutli1" runat="server" />
<div class="eStore_container eStore_block980">
    <div class="eStore_product_content">
        <div class="eStore_product_product row20">
            <div class="eStore_product_productName row20">
                <h2>
                    <asp:Literal ID="lProductName" runat="server" /><span class="icon">
                        <asp:Literal ID="ltproductStatus" runat="server"></asp:Literal></span> <span class="remind font18">
                            <asp:Literal ID="ltPhaseOut" runat="server" Visible="false"></asp:Literal></span>
                </h2>
                <h1>
                    <asp:Literal ID="lShortDescription" runat="server" />
                </h1>
            </div>
            <div class="eStore_product_productPic row20">
                <div class="eStore_product_picBig">
                    <asp:Image ID="imgLargePicture" runat="server" CssClass="eStore_product_picBigImg" />
                    <span class="eStore_product_focusBlock"></span>
                    <div class="eStore_product_picZoom">
                        <asp:Image ID="imgLargePicturelarg" runat="server" />
                    </div>
                </div>
                <div class="eStore_product_picSmall carouselBannerSingle" id="productPicture">
                    <ul id="productframes" runat="server" clientidmode="Static">
                    </ul>
                    <div class="clearfix">
                    </div>
                    <div class="carousel-control">
                        <a id="prev" class="prev" href="#"></a><a id="next" class="next" href="#"></a>
                    </div>
                </div>
            </div>
            <div class="eStore_product_productDetail row20">
                <ol class="eStore_listPoint">
                    <asp:Literal ID="lProductFeature" runat="server"></asp:Literal>
                    <div class="eStore_product_productMsg">
                        <eStore:Repeater ID="rpBTOSConfigDetails" runat="server" Visible="false">
                            <HeaderTemplate>
                                <div class="eStore_product_recommended">
                                    <div class="eStore_title">
                                        <%= eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Recommending_replacement)%>:</div>
                                    <div class="eStore_resourcesList">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <span><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>"
                                    id='<%# Eval("VProductID") %>' name='<%# Eval("name")  %>' class="jTipProductDetail">
                                    <%# Eval("name")%></a></span>
                            </ItemTemplate>
                            <FooterTemplate>
                                </div> </div>
                            </FooterTemplate>
                        </eStore:Repeater>
                    </div>
                </ol>
                <div class="eStore_product_productMsg">
                    <asp:Panel ID="plCertification" runat="server" Visible="false">
                        <div class="eStore_product_productMsgBlock">
                            <div class="eStore_title">
                                <asp:Label ID="lblCertification" runat="server" Text="Certifications"></asp:Label>
                            </div>
                            <span>
                                <asp:Repeater ID="rptCertification" runat="server" OnItemDataBound="rptCertification_ItemDataBound">
                                    <ItemTemplate>
                                        <asp:Image ID="imgCertification" runat="server" />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </span>
                        </div>
                    </asp:Panel>
                    <div class="eStore_product_productMsgBlock resourcesBlock">
                        <div class="eStore_title">
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Resouces)%>
                        </div>
                        <div class="eStore_resourcesTop">
                            <span>Resources Download</span>
                        </div>
                        <div class="eStore_resourcesList">
                            <asp:Literal ID="productresources" runat="server"></asp:Literal>
                        </div>
                    </div>
                    
                </div>
                <div class="eStore_product_mobile_message eStore_redStar">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_mobile_message)%>
                </div>
            </div>
            <div class="eStore_product_productAction row20">
                <div class="eStore_product_price row10">
                    <asp:Literal ID="lProductprice" runat="server"></asp:Literal>
                </div>
                <div class="icon">
                    <a runat="server" class="productwarranty" id="imgproductwarranty" title="2 years extended warranty">
                        <asp:Image ID="imgWarranty" runat="server" />
                    </a>
                    <asp:Literal ID="ltFastDelivery" runat="server" Visible="false"></asp:Literal>
                    <b>
                        <asp:Label ID="lPriceExtendedDescripton" runat="server" CssClass="pricered"></asp:Label></b>
                </div>
                <div class="eStore_product_date">
                    <asp:Label ID="LtPromotionMessage" Visible="false" runat="server"></asp:Label>
                </div>
                <div class="eStore_product_btnBlock">
                      <asp:HyperLink ID="hDatasheet" runat="server" CssClass="eStore_btn borderBlue"
                        Target="_blank" Visible="false"><span>Datasheet</span></asp:HyperLink>
                    <asp:HyperLink ID="hRequestQuantityDiscountTop" runat="server" CssClass="eStore_btn borderBlue"
                        Target="_blank"><span>Request Quantity Discount</span></asp:HyperLink>
                    <asp:HyperLink runat="server" ID="lBuilditNow" CssClass="eStore_btn borderBlue">Build it Now</asp:HyperLink>
                    <asp:LinkButton CssClass="needlogin eStore_btn borderBlue SystemIntegrityCheck" data-id="Add2Quote" ID="btnAdd2QuoteTop" runat="server"
                        Text="Add to Quote" OnClick="btnAdd2Quote_Click" />
                    <asp:LinkButton CssClass="needlogin eStore_btn SystemIntegrityCheck" data-id="Add2Cart" ID="btnAdd2CartTop" runat="server"
                         OnClick="btnAdd2Cart_Click" />
                    <asp:LinkButton runat="server" ID="btnMSLicenseSignUp" CssClass="eStore_btn" OnClick="btnMSLicenseSignUp_Click" Text="I am NOT a CLA Member" Visible="false"></asp:LinkButton>
                </div>
                <div class="eStore_product_link">
              
                </div>
            </div>
        </div>
        <!--eStore_article_product-->
        <div class="positionFixed">
            <div class="eStore_product_leftTable">
                <div id="ProductWidget"></div>

                <div id="ProductExpansionInfor" class="eStore_product_orderInfo row20">
                    <h4>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Order_Information)%></h4>
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
                        <tr>
                            <th width="170">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)%>
                            </th>
                            <th width="380">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Description)%>
                            </th>
                            <th width="50" id="atpabcheader" runat="server" class="adminonly">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableABC)%>
                            </th>
                            <th id="atpdateheader" runat="server" class="adminonly">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableDate)%>
                            </th>
                            <th width="50" id="atpqtyheader" runat="server" class="adminonly">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableQty)%>
                            </th>
                            <th width="70">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty)%>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblOrderPartNo" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblOrderDesc" runat="server"></asp:Label>
                            </td>
                            <%if (ShowATP) { %>
                            <td class="remind adminonly" id="atpabcItem" runat="server">
                                <asp:Label ID="lblOrderSapABC" runat="server"></asp:Label>
                            </td>
                            <td class="remind adminonly" id="atpdateItem" runat="server">
                                <asp:Label ID="lblOrderAvaliability" runat="server"></asp:Label>
                            </td>
                            <td class="remind adminonly" id="atpqtyitem" runat="server">
                                <asp:Label ID="lblOrderSapQty" runat="server"></asp:Label>
                            </td>
                            <%}%>
                            <td>
                                <eStore:TextBox ID="txtOrderQty" runat="server" CssClass="qtytextbox" ClientIDMode="Static">1</eStore:TextBox>
                            </td>
                        </tr>
                    </table>
                    <div class="eStore_system_mobile">
                        <div class="title">
                            <span>
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)%></span>
                            <span class="inputQty">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty)%></span>
                        </div>
                        <div class="content">
                            <div class="title">
                                <span>
                                    <asp:Literal ID="lbMobileOrderPartNo" runat="server"></asp:Literal></span>
                                <eStore:TextBox ID="txtMobileOrderQty" runat="server" CssClass="qtytextbox inputQty"
                                    ClientIDMode="Static">1</eStore:TextBox>
                            </div>
                            <div class="description">
                                <asp:Literal ID="lbMobileOrderDesc" runat="server"></asp:Literal></div>
                            <%if (ShowATP) { %>
                            <div class="ind remind adminonly">
                                <span>
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableABC)%>:
                                </span>
                                <asp:Literal ID="lbMobileOrderSapABC" runat="server"></asp:Literal></div>
                            <div class="date remind adminonly">
                                <span>
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableDate)%>:
                                </span>
                                <asp:Literal ID="lbMibleOrderAvaliability" runat="server"></asp:Literal></div>
                            <div class="inv remind adminonly">
                                <span>
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableQty)%>:
                                </span>
                                <asp:Literal ID="lbMobileOrderSapQty" runat="server"></asp:Literal></div>
                            <%}%>
                        </div>
                    </div>
                </div>
                <!--eStore_article_orderInfo-->
                <eStore:Repeater ID="rpPeripheralCompatibles" runat="server" OnItemDataBound="rpPeripheralCompatibles_ItemDataBound">
                    <HeaderTemplate>
                        <div class="eStore_product_recommended row20">
                            <h4>
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Recommended_Industrial_Grade_Peripherals)%>
                                <span class="small remind">(<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Optional_Not_Required)%>)</span></h4>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="eStore_table_MsgBG" id="Peripheral-<%# Eval("ID")%>" <%# Eval("IntegrityCheckType")==""?"":("integritychecktype='"+Eval("IntegrityCheckType")+"'")%>>
                            <%# Eval("CategoryName")%></div>
                        <eStore:Repeater ID="rpPeripheralProducts" runat="server" OnItemDataBound="rpPeripheralProducts_ItemDataBound">
                            <HeaderTemplate>
                                <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
                                    <tr>
                                        <th width="170">
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)%>
                                        </th>
                                        <th width="380">
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Description)%>
                                        </th>
                                        <%if (ShowATP)
                                          { %>
                                        <th class="adminonly" width="50">
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableABC)%>
                                        </th>
                                        <th class="adminonly">
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableDate)%>
                                        </th>
                                        <%} %>
                                        <th class="adminonly">
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_UnitPrice)%>
                                        </th>
                                        <%if (ShowATP)
                                          { %>
                                        <th class="adminonly" width="50">
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableQty)%>
                                        </th>
                                        <%} %>
                                        <th width="70">
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty)%>
                                        </th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <span id='<%# Eval("SProductID") %>' name='<%# Eval("SProductID") %>' class="jTipProductDetail">
                                            <%# Eval("SProductID")%></span>
                                        <asp:HiddenField ID="hSProductID" runat="server" Value='<%# Eval("SProductID") %> ' />
                                    </td>
                                    <td>
                                        <asp:Literal ID="ltDescription" runat="server"></asp:Literal>
                                    </td>
                                    <%if (ShowATP)
                                      { %>
                                    <td class="remind adminonly">
                                        <%#Eval("ABCInd")%>
                                    </td>
                                    <td class="remind adminonly">
                                        <%#eStore.Presentation.eStoreLocalization.Date(Eval("atpX.availableDate"))%>
                                    </td>
                                    <%}%>
                                    <td class="remind adminonly">
                                        <%# getPeripheralProductPrice(((eStore.POCOS.PeripheralProduct)Container.DataItem).partsX)%>
                                    </td>
                                    <%if (ShowATP)
                                      { %>
                                    <td class="remind adminonly">
                                        <%#Eval("atpX.availableQty")%>
                                    </td>
                                    <%}%>
                                    <td>
                                        <eStore:TextBox ID="txtQty" runat="server" CssClass="qtytextbox PeripheralItem"></eStore:TextBox>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </eStore:Repeater>
                        <eStore:Repeater ID="rpPeripheralProductsMobile" runat="server" OnItemDataBound="rpPeripheralProducts_ItemDataBound">
                            <HeaderTemplate>
                                <div class="eStore_system_mobile">
                                    <div class="title">
                                        <span>
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)%></span>
                                        <span class="inputQty">
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty)%></span>
                                    </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="content">
                                    <div class="title">
                                        <span>
                                            <%# Eval("SProductID") %></span>
                                        <eStore:TextBox ID="txtMobileQty" runat="server" CssClass="inputQty qtytextbox PeripheralItem"></eStore:TextBox>
                                        <asp:HiddenField ID="hSProductID" runat="server" Value='<%# Eval("SProductID") %> ' />
                                    </div>
                                    <div class="description">
                                        <asp:Literal ID="ltDescription" runat="server"></asp:Literal></div>
                                    <%if (ShowATP) { %>
                                    <div class="ind remind">
                                        <span>
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableABC)%>:
                                        </span>
                                        <%#Eval("ABCInd")%></div>
                                    <div class="date remind">
                                        <span>
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableDate)%>:
                                        </span>
                                        <%#eStore.Presentation.eStoreLocalization.Date(Eval("atpX.availableDate"))%></div>
                                    <%}%>
                                    <div class="unitPrice remind">
                                        <span>
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_UnitPrice)%>:
                                        </span>
                                        <%# getPeripheralProductPrice(((eStore.POCOS.PeripheralProduct)Container.DataItem).partsX)%></div>
                                    <%if (ShowATP) { %>
                                    <div class="inv remind">
                                        <span>
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableQty)%>:
                                        </span>
                                        <%#Eval("atpX.availableQty")%></div>
                                    <%}%>
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                </div>
                            </FooterTemplate>
                        </eStore:Repeater>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </eStore:Repeater>
                <!-- product addons -->
                <div id="divProductAddons" class="hiddenitem eStore_product_recommended row20">
                    <div id="productaddonstitle" name="productaddonstitle">
                        <h4>
                            Compatible Display Kits for the
                            <asp:Literal ID="ltKits" runat="server"></asp:Literal>
                            <asp:Image ID="Image1" ImageUrl="~/images/icon_new.jpg" runat="server" CssClass="statueIoc"
                                AlternateText="Icon - New" /></h4>
                    </div>
                    <div class="eStore_product_recommendedSelect pDDLcontent">
                        <div class="eStore_product_selectBlock">
                            <span class="eStore_product_selectTitle">Size</span>
                            <select name="ddlSize" id="ddlSize" class="styled" attr="Size">
                                <option value="none">-- select --</option>
                            </select>
                        </div>
                        <div class="eStore_product_selectBlock">
                            <span class="eStore_product_selectTitle">Brightness </span>
                            <select name="ddlOriginal_Brightness" id="ddlOriginal_Brightness" class="styled"
                                attr="Original Brightness">
                                <option value="none">-- select --</option>
                            </select>
                        </div>
                        <div class="eStore_product_selectBlock">
                            <span class="eStore_product_selectTitle">Touch Screen </span>
                            <select class="styled" name="ddlTouch_Screen" id="ddlTouch_Screen" attr="Touch Screen">
                                <option value="none">-- select --</option>
                            </select>
                        </div>
                    </div>
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
                        <thead>
                            <tr>
                                <th width="145">
                                    Part NO.
                                </th>
                                <th width="320">
                                    Description
                                </th>
                                <th>
                                    Unit Price
                                </th>
                                <th width="70">
                                    Qty
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>

                    <div class="eStore_system_mobile">
                        <div class="title">
                            <span>Part No.</span>
                            <span class="inputQty">Qty</span>
                        </div>
                        <div id="mobile_idk"></div>
                    </div>

                </div>
                <div class="clear">
                </div>
                <!--IDK -->
                <div id="divreversionaddons" class="hiddenitem eStore_product_recommended row20">
                    <div>
                        <h4>
                            Compatible Embedded Boards for the
                            <asp:Literal ID="lreversionaddons" runat="server"></asp:Literal></h4>
                        <div>
                            For more details, see the <a href="http://buy.advantech.com/Widget.aspx?WidgetID=520"
                                class="blueLink" target="_blank">Industrial Display Kit Selection Guide</a>
                        </div>
                    </div>
                    <div class="pDDLcontent eStore_product_recommendedSelect">
                        <div class="eStore_product_selectBlock">
                            <span class="eStore_product_selectTitle">Form Factor</span> <span class="select">
                            </span>
                            <select name="ddlFactor" id="ddlFactor" attr="Category" selectvalue='1' class="styled">
                                <option value="none">-- select --</option>
                            </select>
                        </div>
                        <div class="eStore_product_selectBlock">
                            <span class="eStore_product_selectTitle">Processor</span> <span class="select"></span>
                            <select name="ddlProcessor" id="ddlProcessor" attr="Processor" class="styled">
                                <option value="none">-- select --</option>
                            </select>
                        </div>
                    </div>
                    <table class='estoretable fontbold' width='100%' id='Table1' style='display: table;'>
                        <thead>
                            <tr>
                                <th class='tablecolwidth145'>
                                    Part No.
                                </th>
                                <th>
                                    Description
                                </th>
                                <th class='tablecolwidth75'>
                                    Unit Price
                                </th>
                                <th class='tablecolwidth45'>
                                    Qty
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <div class="clear">
                </div>
                <!---- Accessory Products  ------>
                <eStore:Repeater ID="rpRelatedProducts" runat="server" OnItemDataBound="rpRelatedProducts_ItemDataBound">
                    <HeaderTemplate>
                        <h4>
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Accessory_Products)%></h4>
                        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
                            <tr>
                                <th width="170">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)%>
                                </th>
                                <th width="380">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Description)%>
                                </th>
                                <%if (ShowATP)
                                  { %>
                                <th width="50" class="adminonly">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableABC)%>
                                </th>
                                <th class="adminonly">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableDate)%>
                                </th>
                                <th class="adminonly">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableQty)%>
                                </th>
                                <%} %>
                                <th width="50">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_UnitPrice)%>
                                </th>
                                <th width="70">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty)%>
                                </th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <span id='<%# Eval("SProductID") %>' name='<%# Eval("SProductID") %>' class="jTipProductDetailWithoutImage">
                                    <%# Eval("SProductID")%></span>
                                <asp:HiddenField ID="hSProductID" runat="server" Value='<%# Eval("SProductID") %> ' />
                            </td>
                            <td>
                                <%# Eval("productDescX")%>
                            </td>
                            <%if (ShowATP)
                              { %>
                            <td class="remind adminonly">
                                <%# Eval("ABCInd")%>
                            </td>
                            <td class="remind adminonly">
                                <%#eStore.Presentation.eStoreLocalization.Date(Eval("atpX.availableDate"))%>
                            </td>
                            <td class="remind adminonly">
                                <%#Eval("atpX.availableQty")%>
                            </td>
                            <%}%>
                            <td class="remind">
                                <%# eStore.Presentation.Product.ProductPrice.FormartPrice((decimal)((eStore.POCOS.Part)Container.DataItem).getListingPrice().value)%>
                            </td>
                            <td>
                                <eStore:TextBox ID="txtQty" runat="server" CssClass="qtytextbox"></eStore:TextBox>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </eStore:Repeater>
                <eStore:Repeater ID="rpRelatedProductsMobile" runat="server">
                    <HeaderTemplate>
                        <div class="eStore_system_mobile">
                            <div class="title">
                                <span>
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)%></span>
                                <span class="inputQty">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty)%></span>
                            </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="content">
                            <div class="title">
                                <span>
                                    <%# Eval("SProductID") %></span>
                                <eStore:TextBox ID="txtMobileQty" runat="server" CssClass="inputQty qtytextbox PeripheralItem"></eStore:TextBox>
                                <asp:HiddenField ID="hSProductID" runat="server" Value='<%# Eval("SProductID") %> ' />
                            </div>
                            <div class="description">
                                <%# Eval("productDescX")%></div>
                            <%if (ShowATP) { %>
                            <div class="ind remind">
                                <span>
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableABC)%>:
                                </span>
                                <%#Eval("ABCInd")%></div>
                            <div class="date remind">
                                <span>
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableDate)%>:
                                </span>
                                <%#eStore.Presentation.eStoreLocalization.Date(Eval("atpX.availableDate"))%></div>
                            <%}%>
                            <div class="unitPrice remind">
                                <span>
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_UnitPrice)%>:
                                </span>
                                <%# eStore.Presentation.Product.ProductPrice.FormartPrice((decimal)((eStore.POCOS.Part)Container.DataItem).getListingPrice().value)%></div>
                            <%if (ShowATP) { %>
                            <div class="inv remind">
                                <span>
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableQty)%>:
                                </span>
                                <%#Eval("atpX.availableQty")%></div>
                            <%}%>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </eStore:Repeater>
                <asp:Panel runat="server" ID="panelWarranty" Visible="False">
                    <div class="eStore_table_MsgBG">
                        <a class="eStoreHelper" id="Product_Extended_warranty">
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Extended_warranty)%>
                        </a>
                    </div>
                    <asp:RadioButtonList ID="rblWarranty" runat="server" CssClass="fontbold" ClientIDMode="Static">
                    </asp:RadioButtonList>
                    <script type="text/javascript" language="javascript">
                        $(document).ready(function () {
                            culculateWarranty();
                            $(":text[warrantyprice]").keyup(function () {
                                culculateWarranty();
                            });

                        });
                        function culculateWarranty() {

                            //get total price
                            var btototal = 0;
                            var addontotal = 0;
                            var accessoriestotal = 0;
                            var itemqty = parseInt($("#txtOrderQty").val());
                            if (isNaN(itemqty))
                                itemqty = 0;
                            $.each($(":text[warrantyprice]"), function (i, iem) {
                                if (parseInt($(this).val()) > 0) {
                                    if (this.id == "txtOrderQty") {
                                        btototal += parseInt($(this).attr("warrantyprice"));
                                    }
                                    else if ($(this).hasClass("PeripheralItem")) {
                                        btototal += parseInt($(this).attr("warrantyprice")) * parseInt($(this).val());
                                    }
                                    else if (this.id.indexOf("rpRelatedProducts") > 0) {
                                        accessoriestotal += parseInt($(this).attr("warrantyprice")) * parseInt($(this).val());
                                    }
                                    else {
                                        addontotal += parseInt($(this).attr("warrantyprice")) * parseInt($(this).val());
                                    }
                                }
                            });

                            var selecteditemprice = 0;
                            $.each($("#rblWarranty span[rate!='0']").has(".addtionprice"), function (i, n) {
                                var warrantyitemprice = calculateItemWarrantyPrice(btototal, addontotal, accessoriestotal, parseFloat($(this).attr("rate")), itemqty)
    - selecteditemprice;

                                var sumSign = "";
                                if (warrantyitemprice >= 0) {
                                    sumSign = "+";
                                }
                                else if (warrantyitemprice < 0) {
                                    sumSign = "-";
                                }
                                $(this).find(".priceSing").html(sumSign);
                                $(this).find(".addtionprice").html(formatdecimal(Math.abs(warrantyitemprice)));
                            });

                        }
                        function calculateItemWarrantyPrice(btototal, addontotal, accessoriestotal, rate, qty) {
                            return Math.ceil(btototal * rate) * qty / 100 + Math.ceil(addontotal * rate) * qty / 100 + Math.ceil(accessoriestotal * rate) / 100;
                        }
                    </script>
                </asp:Panel>
                <div id="ProductDependency" style="display: none;">
                    <h5><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Product_Dependency)%></h5>
                    <eStore:ProductDependencies ID="ProductDependencies1" runat="server" />
                </div>
                <div class="eStore_product_btnBlock row20 floatRight">
                    <asp:LinkButton CssClass="needlogin eStore_btn borderBlue SystemIntegrityCheck" ID="btnAdd2Quote" runat="server" data-id="Add2Quote"
                        Text="Add to Quote" OnClick="btnAdd2Quote_Click" />
                    <asp:LinkButton CssClass="needlogin eStore_btn SystemIntegrityCheck" ID="btnAdd2Cart" runat="server" Text="Add to Cart" data-id="Add2Cart"
                        OnClick="btnAdd2Cart_Click" />
                </div>
                <div class="clearfix" />
            </div>
        </div>
        <!--eStore_product_leftTable-->
        <div class="eStore_system_listFloat">
            <div id="storeSideAds">
            </div>
        </div>
    </div>
    <!--positionFixed-->
    <div id="mostcategory" class="row20">
    </div>
    <!--eStore_article_moreInfo-->
</div>
<!--eStore_article_content-->
<div id="ProductSharetoFriendsDialog" style="display: none;">
    <eStore:ProductSharetoFriends ID="ProductSharetoFriends1" runat="server" />
</div>
<div id="QuantityDiscountRequestDialog" style="display: none;">
    <eStore:QuantityDiscountRequest ID="QuantityDiscount" runat="server" ClientIDMode="Static"  NeedLogin="false"  HideQuantity="false" />
</div>
<!--container-->
<script type="text/javascript" language="javascript">
    function showProductSharetoFriendsDialog() {
        popupDialog("#ProductSharetoFriendsDialog");
        return false;
    }
    function showQuantityDiscountRequestDialog(title) {
        popupDialog("#QuantityDiscountRequestDialog");
        return false;
    }
    function showProductSpecsDialog(productid) {
        //popupDialogDelay(
		//   $("<div></div>")
        //.load(GetStoreLocation() + "proc/html.aspx", { type: "ProductSpecList", ProductID: productid }) ,800);
        //$("#ProductSpecsDialog").empty();
        //return false;

        if ($("#ProductSpecsDialog").length == 0) {
            $("body").append($("<div id='ProductSpecsDialog' style='display: none;' />"));
        }

        if (!$.trim($('#ProductSpecsDialog').html()).length) { //When ProductSpecsDialog this div is empty. Use ajax to load data
            $("#ProductSpecsDialog").load(GetStoreLocation() + "proc/html.aspx", { type: "ProductSpecList", ProductID: productid }, function () {
                popupDialog("#ProductSpecsDialog");
            });
        }
        else
            popupDialog("#ProductSpecsDialog");
        return false;
    }
    function showProduct3DModelDialog(productid) {
        //popupDialogDelay(
        //$("<div></div>")
        //.load(GetStoreLocation() + "proc/html.aspx", { type: "Product3DModel", ProductID: productid }),800);
        //return false

        if ($("#Product3DModelDialog").length == 0) {
            $("body").append($("<div id='Product3DModelDialog' style='display: none;' />"));
        }

        if (!$.trim($('#Product3DModelDialog').html()).length) { //When ProductSpecsDialog this div is empty. Use ajax to load data
            $("#Product3DModelDialog").load(GetStoreLocation() + "proc/html.aspx", { type: "Product3DModel", ProductID: productid }, function () {
                popupDialog("#Product3DModelDialog");
            });
        }
        else
            popupDialog("#Product3DModelDialog");
        return false;
    }
    function createUnicaActivity(activity, productid, url) {
        $.ajax({
            url: '<%=esUtilities.CommonHelper.GetStoreLocation()%>proc/do.aspx?func=<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.CreateUnicaActivity%>&activitytype=' + activity + '&productID=' + productid + '&url=' + url,
            type: "POST",
            success: function (retData) {
            },
            error: function () {
            }
        });
        <%--$.getJSON('<%=esUtilities.CommonHelper.GetStoreLocation()%>proc/do.aspx?func=<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.CreateUnicaActivity%>&activitytype=' + activity + '&productID=' + productid + '&url=' + url, function (retData){ });--%>
        return true;
    }

    var isCheckOSandStorage = false; //是否需要检查OS 和 Storage
    function SystemIntegrityCheck(){
        var OSCnt=0;
        $("div[integritychecktype='StandardOS']").each(function(i,os){
            OSCnt+=isNoNeed($(os));
        });
        var StorageCnt=0;
        $("div[integritychecktype='StandardStorage']").each(function(i,storage){
            StorageCnt+=isNoNeed($(storage));
        });

        if(OSCnt>0 && StorageCnt>1)
        {
            if($("#txtComment").val()==""||$("#txtComment").val()==$("#txtComment").attr("title"))
            {
                alert($.eStoreLocalizaion("please_indicate_which_device_you_would_like_the_OS_installed"));
                $("#txtComment").val($.eStoreLocalizaion("Please_install_the_OS_in_Hard_Drive")).css("color", "#000");
                $("#txtComment").focus();
                return false;
            }
            else
            {
                if (confirm($.eStoreLocalizaion("Did_you_tell_us_where_to_install_the_OS_in_the_installation_instruction"))) {
                    return true;
                }
                else {

                    $("#txtComment").focus();
                    return false;
                }
            }
        } 
        else if(OSCnt==0 && StorageCnt>0)
        {
            return isCheckOSandStorage = confirm($.eStoreLocalizaion("Are_you_sure_you_dont_want_to_select_any_OS"));
        }
        else if(OSCnt>0 && StorageCnt==0)
        {
            var hasBoardFlash = false;
            $("#ProductExpansionInfor input[hasBuiltInStorage]").each(function(i){
                if($(this).val() != "" && $(this).val() != "0")
                    hasBoardFlash = true;
            });
            if(hasBoardFlash == false)
                alert($.eStoreLocalizaion("Are_you_sure_you_dont_want_any_storage_device_with_your_OS"));
            return hasBoardFlash;   
        }
        else if(OSCnt>0 && StorageCnt==1)
        {
            if($("#txtComment").val()==$.eStoreLocalizaion("Please_install_the_OS_in_Hard_Drive"))
            {
                $("#txtComment").val($("#txtComment").attr("title"));
            }
        }
    }

    function isNoNeed(module)
    { 
        var cnt=0;
        if(module!=null&&module.length>0)
        {
            $.each($(module).next().find(":text"), function(i, qty){
                if($(qty).val()!=""&& !isNaN(parseInt($(qty).val())))
                {
                    cnt+=parseInt($(qty).val());
                }
            });
        }
        return cnt;
    }
    
    var productid="<%=CurrentProduct.SProductID %>";
    var storeCurrencySign = "<%=storeCurrencySign %>";
    $(document).ready(function(){
        
        $(function () {
            $('a.fancybox').fancybox();
        });

        $("#divProductAddons").productaddons(productid,storeCurrencySign);
        $("#divreversionaddons").IDKCompatibilityEmbeddedBoard(productid,storeCurrencySign);
        $(".SystemIntegrityCheck").click(function(){
            var result=true;
            //result=checkResoruce();

            if(!checkMOQ())
                return false;
            <%if (eStore.Presentation.eStoreContext.Current.User != null && EnableSystemIntegrityCheck)
              {%>
		    if(result && isCheckOSandStorage != true){
		        result=  SystemIntegrityCheck();
		    }
            <%} %>
		    if(result || result == undefined)
		        result=checkNeedDependency($(this));
		    return result;
		});

      
	});

    function checkMOQ()
    {
        var cc = true;
        var productids = "";
        var moq = "";
        var i = 0;
        var errorMessage = "<%= eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Minimum_Quantity_Error)%>";
        $("input:text[MOQ][value!='']").each(function(){
            if(parseInt($(this).val()) < parseInt($(this).attr("MOQ")))
            {
                if(i == 0){
                    productids = $(this).attr("sproductid") ;     
                    $(this).focus();
                }
                else 
                    productids = productids  + "," + $(this).attr("sproductid");
                moq = $(this).attr("MOQ");
                cc = false;
                i++;
            }
        });
        if(!cc)
            alert(errorMessage.replace("{0}",productids).replace("{1}",moq));
        return cc;
    }

    function checkResoruce(){
        //debugger;
        var resourceSettings=new Array();　
        var overcapacity=new Array();

        $("#ProductExpansionInfor").find(":text[resource][value!='']").each(function(i){
            if(parseInt($(this).val())!=NaN && parseInt($(this).val())>0)
            {
                var qty=parseInt($(this).val());
                $(eval($(this).attr("resource"))).each(function(newindex,newitem){
                    var exists=false;
                    $(resourceSettings).each(function(index,item){
                        if(newitem.ResourceName==item.ResourceName)
                        {
                            item.AvaiableQty+=newitem.AvaiableQty*qty;
                            item.ConsumingQty+=newitem.ConsumingQty*qty;
                            exists=true;
                            return;
                        }
                    });
                    if(!exists)
                    {
                        newitem.AvaiableQty=newitem.AvaiableQty*qty;
                        newitem.ConsumingQty=newitem.ConsumingQty*qty;
                        resourceSettings.push(newitem);
                    }
                });
            }
        });

        resourceSettings=$.map(resourceSettings, function(n){
            if(n.AvaiableQty>=n.ConsumingQty)
            {
                n.AvaiableQty=n.AvaiableQty-n.ConsumingQty ;
                n.ConsumingQty=0;
            }
            else
            {
                n.AvaiableQty=0;
                n.ConsumingQty=n.ConsumingQty-n.AvaiableQty;
            }
            return n;
        });

        var MultiConsumingResource=$.grep(resourceSettings, function(n,i){
            return n.ConsumingQty>0;
        });
        $(MultiConsumingResource).each(function(ci,cn){
            var result=false;
            $(resourceSettings).each(function(ai,an){
                $(cn.ResourceName.split(",")).each(function(i,r)
                {
                    if(jQuery.inArray(r,an.ResourceName.split(","))>=0 && an.AvaiableQty>=cn.ConsumingQty)
                    {
                        an.AvaiableQty-=cn.ConsumingQty;
                        cn.ConsumingQty=0;
                        return;
                    }
                    else if(jQuery.inArray(r,an.ResourceName.split(","))>=0  && an.AvaiableQty>0 && an.AvaiableQty<cn.ConsumingQty)
                    {
                       
                        cn.ConsumingQty-=an.AvaiableQty;
                        an.AvaiableQty=0;
                    }
                });
            });
            if( cn.ConsumingQty>0)
            {
                $.merge(overcapacity ,cn.ResourceName.split(","));
            }
        });

        $($.grep(resourceSettings, function(n,i){
            return n.AvaiableQty <0 && n.ResourceName.indexOf(",")<0
        })).each(function(i,n){
            overcapacity.push(n.ResourceName);
        });

        if(overcapacity.length>0)
        {
            alert("The selection you made exceeds system's ["+ $.unique(overcapacity).join(",") +"] maximum capacity.");
            return false;
        }
        else
            return true;
        
    } 

    function checkDependency(obj)
    {
        var objDE = obj;
        var isOK = true;
        var allDepenLs = $("#ProductDependency input[type=text].qtytextbox");
        if(objDE.val() != "" && objDE.attr("dependency") != undefined)
        {
            var dependencyLS = objDE.attr("dependency").split("|");
            for(var n = 0; n < dependencyLS.length; n++)
            for(var n = 0; n < dependencyLS.length; n++)
            {
                    
                allDepenLs.each(function(){
                    if(dependencyLS[n] == $(this).attr("sproductid"))
                    {
                        if($(this).val() == "")
                        {
                            alert(dependencyLS[n] + " dependency part -> " + $(this).attr("sproductid"));
                            $(this).bind("focus",function(){
                                $(this).attr("style","background-color:#F60");
                            }).bind("blur",function(){
                                $(this).removeAttr("style");
                            });
                            $(this).focus();
                            isOK = false;
                            return false;
                        }
                    }
                });
                if(isOK == false) {
                    return false;
                }
            }
        }
        return isOK;
    }

    var isCheckDependency = true;
    function checkAllDependency()
    {
        var result = false;
        var allDepenLs = $("#ProductDependency input[type=text].qtytextbox");
        $("#ProductExpansionInfor input[type=text].qtytextbox").each(function(i){
            if($(this).val() != "" && $(this).attr("dependency") != undefined)
            {
                var dependencyStr = $(this).attr("dependency");
                var dependencyLS = dependencyStr.split("|");
                for(var n = 0; n < dependencyLS.length; n++)
                {
                    allDepenLs.each(function(){
                        if(dependencyLS[n] == $(this).attr("sproductid"))
                        {
                            var val = parseInt($(this).val()) || 0;
                            if(val < 1){
                                $(".lbProductDepend")
                                    .text($.eStoreLocalizaion("not_a_valid_char"))
                                    .css("color", "red");
                                $(this).attr("style","background-color:#F60")
                                    .bind("blur", function() { $(this).removeAttr("style"); });
                                result = false;
                                return false;
                            }
                            else
                                result = true;
                        }
                    });
                    if (result == false)
                    return false;
                }
            }
        });
        if(result == true) {
            $.fancybox.close();
            isCheckDependency = false;
        }
        return result;
    }

    function checkNeedDependency(obj)
    {
        if(showSelectDependency() == true)
        {
            var width=$("#ProductDependency").width();
            var height=$("#ProductDependency").height();
            if(width==0)width=600;
            if(height==0)height=400;
            showDependencyDiv(obj,width,height);
            return false;
        }
        else
            return true;
    }

    function showSelectDependency()
    {
        var isHasDependency = false;
        if(isCheckDependency == true)
        {
            var popDependencyItems=new Array();
            $("#ProductExpansionInfor input[dependency]").each(function(i){
                if($(this).val() != "" && $(this).val() != "0")
                {
                    jQuery.merge(popDependencyItems, $(this).attr("dependency").split("|")) ;
                      
                }
            });

            if(popDependencyItems.length>0)
            {
                $("#ProductDependency").find("tr").each(function(){
                    var pn=$(this).find("input[type=text].qtytextbox").attr("sproductid");
                    var matched=false;
                    if(jQuery.inArray(pn, popDependencyItems)==-1)
                    {
                        $(this).find("input[type=text].qtytextbox").val("");
                        //$(this).hide();
                    }
                    else
                    {
                        $(this).show();
                        isHasDependency=true;
                    }  
                });
            }
        }
        return isHasDependency;
    }

    function showDependencyDiv(obj,width,height)
    {
        if (obj.attr("data-id") == "Add2Quote") {
            $(".AddDpToQuote").show();
            $(".AddDpToCart").hide();
        }
        else if (obj.attr("data-id") == "Add2Cart") {
            $(".AddDpToCart").show();
            $(".AddDpToQuote").hide();
        }
        $.fancybox("#ProductDependency");  
        return false;
    }

    var productWidgetId = <%=productWidgetId %>;
    $(document).ready(function () {
        if(productWidgetId != 0){
            $.get('ProWidget.ashx', { WidgetID: productWidgetId }, function(result){
                $("#ProductWidget").html(result);
                compareHeight();
                if(isExitsFunction("reLoadWidgetPageFunction")) {
                    reLoadWidgetPageFunction();
                }
            }, 'html');
        }
    });


    $(function() {
		
        $(".carouselBannerSingle").each(function(){
            var id = $(this).attr('id');
            id = "#"+id ;
            //console.log(id);
            $(id).find("ul").carouFredSel({
                auto: false,
                scroll: 1,
                prev: '#prev',
                next: '#next',
                pagination: '#pager'
            });
        });	
            $(".resourcelist li a").click(function(){
         advWTTrackingEventFrame('click',productid,'ModelPage-TechnicalDoc',$(this).text());
    });
    });
</script>
<script id="_tmpcategories" type="text/x-jquery-tmpl">
    <div class="eStore_product_moreInfo eStore_other_BGBlock row20">
        <h4></h4>
        <!-- ko foreach:  $data -->
        <div class="eStore_productBlock">
            <div class="eStore_productBlock_pic row10">
                <img data-bind="attr: { src: Image, title: Name, alt: Description }" />
            </div>
            <div class="eStore_productBlock_txt row10" data-bind="text: Name"></div>
            <div class="eStore_productBlock_price row10">
                <div class="priceOrange" data-bind="html: Price"></div>
            </div>
            <a data-bind="attr: { href: Url }" class="eStore_btn">More</a>
        </div>
        <!-- /ko -->
        <div class="clearfix" />
    </div>
    </div>
</script>
<script id="_tempProAddon" type="text/x-jquery-tmpl">
<table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
  <tr>
    <th width="145">Part NO.</th>
    <th width="320">Description</th>
    <th>Unit Price</th>
    <th width="70">Qty</th>
  </tr>
  <!-- ko foreach:  $data -->
  <tr class="eStore_openBoxTable" data-bind="attr: {id:'mainSTR'+Id}" class="mainSTR eStore_openBoxTable">
    <td data-bind="attr: {id:'STR'+i}" style="text-align: left;"><span class="jTipProductDetail" data-bind="text: Name,attr:{id:Name,name:Name}"></span></td>
    <td style="text-align: left;" data-bind="text: Desc"></td>
    <td class="remind" data-bind="text: Price.valueWithoutCurrency"></td>
    <td><input data-bind="attr:{name:'inputAddonQTY_'+Id,id:'inputAddonQTY_'+Id,warrantyprice:Warrantyprice}" type="text" class="qtytextbox" /></td>
  </tr>
  <!-- /ko -->
</table>
</script>
