<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Bundle.ascx.cs" Inherits="eStore.UI.Modules.Bundle" %>
<%@ Register Src="ProductSharetoFriends.ascx" TagName="ProductSharetoFriends" TagPrefix="eStore" %>
<%@ Register Src="QuantityDiscountRequest.ascx" TagName="QuantityDiscountRequest"
    TagPrefix="eStore" %>
<%@ Register Src="YouAreHereMutli.ascx" TagName="YouAreHereMutli" TagPrefix="eStore" %>
<%@ Register Src="ChangeCurrency.ascx" TagName="ChangeCurrency" TagPrefix="eStore" %>
<%@ Register Src="SocialNetworkContent.ascx" TagName="SocialNetworkContent" TagPrefix="eStore" %>
<eStore:YouAreHereMutli ID="YouAreHereMutli1" runat="server" />
<div class="eStore_container eStore_block980">
    <div class="eStore_product_content">
        <div class="eStore_product_product row20">
            <div class="eStore_product_productName row20">
                <h2>
                    <asp:Literal ID="lProductName" runat="server" /><span class="icon"><asp:Literal ID="ltproductStatus"
                        runat="server"></asp:Literal></span>
                    <asp:Literal ID="ltPhaseOut" runat="server" Visible="false"></asp:Literal></h2>
                <h1>
                    <asp:Literal ID="lShortDescription" runat="server" /></h1>
            </div>
            <div class="eStore_product_productPic row20">
                <div class="eStore_product_picBig">
                    <asp:Image ID="imgLargePicture" runat="server" CssClass="eStore_product_picBigImg" />
                    <span class="eStore_product_focusBlock"></span>
                    <div class="eStore_product_picZoom">
                        <asp:Image ID="imgLargePicturelarg" runat="server" /></div>
                </div>
                <div class="eStore_product_picSmall carouselBannerSingle" id="productPicture">
                    <ul>
                        <asp:Literal ID="productLightBox" runat="server"></asp:Literal>
                    </ul>
                    <div class="clearfix">
                    </div>
                    <div class="carousel-control">
                        <a id="prev-productPicture" class="prev" href="#"></a><a id="next-productPicture" class="next" href="#"></a>
                    </div>
                </div>
            </div>
            <div class="eStore_product_productDetail row20">
                <ol class="eStore_listPoint">
                    <asp:Literal ID="lProductFeature" runat="server"></asp:Literal>
                </ol>
                <eStore:Repeater ID="rpBTOSConfigDetails" runat="server" Visible="false">
                    <HeaderTemplate>
                        <div class="replaceProduct">
                            <ul>
                                <h4>
                                    <%= eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Recommending_replacement)%>:</h4>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>"
                            id='<%# Eval("VProductID") %>' name='<%# Eval("name")  %>' class="jTipProductDetail">
                            <%# Eval("name")%></a> </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul> </div></FooterTemplate>
                </eStore:Repeater>
                <div class="eStore_product_productMsg">
                    <div class="eStore_product_productMsgBlock resourcesBlock">
                        <div class="eStore_resourcesList">
                        <asp:Literal ID="productresources" runat="server"></asp:Literal></div>
                        <div class="eStore_resourcesTop">
                                <span>Resources Download</span>
                            </div>
                    </div>
                </div>
            </div>
            <div class="eStore_product_productAction row20">
                <div class="eStore_product_price row10">
                    <asp:Literal ID="lProductprice" runat="server"></asp:Literal>
                    <eStore:ChangeCurrency ID="ChangeCurrency1" runat="server" />
                </div>
                 <div class="icon">
                     <a runat="server" class="productwarranty" id="imgproductwarranty" title="2 years extended warranty">
                        <asp:Image ID="imgWarranty" runat="server" />
                    </a>
                        <asp:Literal ID="ltFastDelivery" runat="server" Visible="false"></asp:Literal>
                        <br />
                        <b><asp:Label ID="lPriceExtendedDescripton" runat="server" CssClass="pricered"></asp:Label></b>
                    </div>
                    <div class="eStore_product_date">
                    <asp:Label ID="LtPromotionMessage" Visible="false" runat="server"></asp:Label></div>
                <div class="eStore_product_btnBlock">
                        <asp:HyperLink ID="hRequestQuantityDiscountTop" runat="server" CssClass="needlogin eStore_btn borderBlue"
                        Target="_blank"></asp:HyperLink>
                    <asp:LinkButton CssClass="needlogin eStore_btn borderBlue" ID="btnAdd2QuoteTop" runat="server"
                        Text="Add to Quote" OnClick="btnAdd2Quote_Click" />
                    <asp:LinkButton CssClass="needlogin eStore_btn" ID="btnAdd2CartTop" runat="server"
                        Text="Add to Cart" OnClick="btnAdd2Cart_Click" />
                </div>
                <div class="eStore_product_link">
            
                 
                </div>
            </div>
        </div>
        <!--eStore_article_product-->
        <div class="positionFixed">
            <div class="eStore_product_leftTable">
                <div class="eStore_product_orderInfo row20">
                    <h4>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Order_Information)%></h4>
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
                        <tr>
                            <th width="170">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)%>
                            </th>
                            <th>
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Description)%>
                            </th>
                            <th id="atpdateheader" runat="server" class="adminonly" width="75">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableDate)%>
                            </th>
                            <th id="atpqtyheader" runat="server" class="adminonly" width="50">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableQty)%>
                            </th>
                            <th width="50">
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
                            <td class="adminonly remind" id="atpdateItem" runat="server">
                                <asp:Label ID="lblOrderAvaliability" runat="server"></asp:Label>
                            </td>
                            <td class="adminonly remind" id="atpqtyitem" runat="server">
                                <asp:Label ID="lblOrderSapQty" runat="server"></asp:Label>
                            </td>
                            <td>
                                <eStore:TextBox ID="txtOrderQty" runat="server" CssClass="qtytextbox" ClientIDMode="Static">1</eStore:TextBox>
                            </td>
                        </tr>
                    </table>
                    <%--<div class="eStore_system_mobile">
                            <div class="title">
                                <span>Part No.</span>
                                <span class="inputQty">Qty</span>
                            </div>
                            <div class="content">
                                <div class="title">
                                    <span>PCE-5127G2-00A1E</span>
                                    <input type="text" class="inputQty">
                                </div>
                                <div class="description">4U 20-Slot Bare Rackmount Chassis with Multi-System Support, Front-Accessible Power, w/o SPS</div>
                                <div class="ind remind"><span>ABC Ind: </span>B</div>
                                <div class="date remind"><span>Available Date: </span>10/17/2014</div>
                                <div class="inv remind"><span>Inv: </span>25</div>
                            </div>
                         </div>--%>
                </div>
                <!--eStore_article_orderInfo-->
                <eStore:Repeater ID="rpBundletems" runat="server" OnItemDataBound="rpBundletems_ItemDataBound">
                    <HeaderTemplate>
                        <div class="eStore_product_recommended row20">
                            <h4>
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Bundle_items_detail)%></h4>
                            <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
                                <tr>
                                    <th width="170">
                                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)%>
                                    </th>
                                    <th>
                                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Description)%>
                                    </th>
                                    <%if (ShowATP)
                                      { %>
                                    <th class="adminonly" width="75">
                                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableDate)%>
                                    </th>
                                    <th class="adminonly" width="50">
                                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableQty)%>
                                    </th>
                                    <th width="50">
                                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_UnitPrice)%>
                                    </th>
                                    <%} %>
                                    <th width="50">
                                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty)%>
                                    </th>
                                </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <span id='<%# Eval("ItemSProductID") %>' name='<%# Eval("ItemSProductID") %>' class="jTipProductDetailWithoutImage">
                                    <%# Eval("ItemSProductID")%></span>
                                <asp:HiddenField ID="hSProductID" runat="server" Value='<%# Eval("ItemSProductID") %> ' />
                            </td>
                            <td>
                                <%# Eval("part.productDescX")%>
                            </td>
                            <%if (ShowATP)
                              { %>
                            <td class="adminonly remind" width="75">
                                <%#eStore.Presentation.eStoreLocalization.Date(Eval("part.atpX.availableDate"))%>
                            </td>
                            <td class="adminonly remind" width="50">
                                <%#Eval("part.atpX.availableQty")%>
                            </td>
                            <td class="remind" width="50">
                                <%# eStore.Presentation.Product.ProductPrice.FormartPrice((decimal) Eval("adjustedPrice"))%>
                            </td>
                            <%}%>
                            <td width="50">
                                <span>
                                    <%# Eval("Qty")%></span>
                                <asp:TextBox ID="hdBundletemQTY" CssClass="hiddenitem bundleItemQty" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                        <div class="eStore_system_mobile">
                            <div class="title">
                                <span>Part No.</span> <span class="inputQty">Qty</span>
                            </div>
                            <div class="content">
                                <div class="title">
                                    <span>PCE-5127G2-00A1E</span>
                                    <input type="text" class="inputQty">
                                </div>
                                <div class="description">
                                    4U 20-Slot Bare Rackmount Chassis with Multi-System Support, Front-Accessible Power,
                                    w/o SPS</div>
                                <div class="ind remind">
                                    <span>ABC Ind: </span>B</div>
                                <div class="date remind">
                                    <span>Available Date: </span>10/17/2014</div>
                                <div class="unitPrice remind">
                                    <span>Unit Price: </span>$178</div>
                                <div class="inv remind">
                                    <span>Inv: </span>25</div>
                            </div>
                            <div class="content">
                                <div class="title">
                                    <span>PCE-5127G2-00A1E</span>
                                    <input type="text" class="inputQty">
                                </div>
                                <div class="description">
                                    4U 20-Slot Bare Rackmount Chassis with Multi-System Support, Front-Accessible Power,
                                    w/o SPS</div>
                                <div class="ind remind">
                                    <span>ABC Ind: </span>B</div>
                                <div class="date remind">
                                    <span>Available Date: </span>10/17/2014</div>
                                <div class="unitPrice remind">
                                    <span>Unit Price: </span>$178</div>
                                <div class="inv remind">
                                    <span>Inv: </span>25</div>
                            </div>
                        </div>
                        </div><!--eStore_article_recommended-->
                    </FooterTemplate>
                </eStore:Repeater>
                <eStore:Repeater ID="rpPeripheralCompatibles" runat="server" OnItemDataBound="rpPeripheralCompatibles_ItemDataBound">
                    <HeaderTemplate>
                        <div class="eStore_product_recommended row20">
                            <h4>
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Recommended_Industrial_Grade_Peripherals)%>
                                <span class="small remind">(<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Optional_Not_Required)%>)</span></h4>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="eStore_table_MsgBG" id="Peripheral-<%# Eval("ID")%>">
                            <%# Eval("CategoryName")%>
                        </div>
                        <eStore:Repeater ID="rpPeripheralProducts" runat="server" OnItemDataBound="rpPeripheralProducts_ItemDataBound">
                            <HeaderTemplate>
                                <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
                                    <tr>
                                        <th width="170">
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)%>
                                        </th>
                                        <th>
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Description)%>
                                        </th>
                                        <%if (ShowATP)
                                          { %>
                                        <th class="adminonly" width="75">
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableDate)%>
                                        </th>
                                        <th class="adminonly" width="50">
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableQty)%>
                                        </th>
                                        <%} %>
                                        <th width="50">
                                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_UnitPrice)%>
                                        </th>
                                        <th width="50">
                                            Add
                                        </th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td width="170">
                                        <span id='<%# Eval("SProductID") %>' name='<%# Eval("SProductID") %>' class="jTipProductDetail">
                                            <%# Eval("SProductID")%></span>
                                        <asp:HiddenField ID="hSProductID" runat="server" Value='<%# Eval("SProductID") %> ' />
                                    </td>
                                    <td>
                                        <%# Eval("Description") != null && !string.IsNullOrEmpty(Eval("Description").ToString()) ? Eval("Description") : (string.Join(";",((eStore.POCOS.PeripheralProduct)Container.DataItem).partsX.Select(c => c.productDescX)))%>
                                    </td>
                                    <%if (ShowATP)
                                      { %>
                                    <td class="adminonly remind" width="75">
                                        <%#eStore.Presentation.eStoreLocalization.Date(Eval("atpX.availableDate"))%>
                                    </td>
                                    <td class="adminonly remind" width="50">
                                        <%#Eval("atpX.availableQty")%>
                                    </td>
                                    <%}%>
                                    <td width="50">
                                        <%# eStore.Presentation.Product.ProductPrice.FormartPrice((decimal)((eStore.POCOS.PeripheralProduct)Container.DataItem).getPeripheralProductPrice())%>
                                    </td>
                                    <td width="50">
                                        <asp:CheckBox ID="ckbSelected" runat="server" onclick="culculateWarranty()" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </eStore:Repeater>
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
                            $.each($(":text[warrantyprice],:checked[warrantyprice]"), function (i, iem) {
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
                            return Math.ceil((btototal + addontotal + accessoriestotal) * rate / 100) * qty;
                        }
                    </script>
                </asp:Panel>
                
            <div class="eStore_product_btnBlock row20">
                <asp:LinkButton CssClass="needlogin eStore_btn borderBlue" ID="btnAdd2Quote" runat="server"
                    Text="Add to Quote" OnClick="btnAdd2Quote_Click" />
                <asp:LinkButton CssClass="needlogin eStore_btn" ID="btnAdd2Cart" runat="server" Text="Add to Cart"
                    OnClick="btnAdd2Cart_Click" />
            </div>
            </div>
            
        </div>
        <!--eStore_product_leftTable-->
        <div id="storeSideAds">
        </div>
        <!--eStore_system_listFloat-->
    </div>
    <!--positionFixed-->
    
    <!--eStore_article_moreInfo-->
</div>
<!--eStore_article_content-->
<div id="ProductSharetoFriendsDialog" style="display: none;">
    <eStore:ProductSharetoFriends ID="ProductSharetoFriends1" runat="server" />
</div>
<div id="QuantityDiscountRequestDialog" style="display: none;">
    <eStore:QuantityDiscountRequest ID="QuantityDiscount" runat="server" />
</div>
<script type="text/javascript" language="javascript">

    $(document).ready(function(){
        $(function () {
            $('a.fancybox').fancybox();
        });

        if($("#productframes .frameUl").length>0)
        {
            $("#productframes").tabs({
                select: function (event, ui) {
                    $("#productimages img").addClass("ui-tabs-hide");
                    $("#productimages img:eq(" + ui.index + ")").removeClass("ui-tabs-hide");
                    $("#productresources div").addClass("ui-tabs-hide");
                    $("#productresources div:eq(" + ui.index + ")").removeClass("ui-tabs-hide");
                 
                }
            });
        }
    });
   <%if( eStore.Presentation.eStoreContext.Current.User==null && eStore.Presentation.eStoreContext.Current.Store.storeID != "AKR"){ %>

         popLoginDialog(270,450,"View bundle Page");
         $("#bundleproductdetails input").attr("disabled","disabled");
         $("#bundleproductdetails *").click(function () {
            return popLoginDialog(270,450,"View bundle Page");
        });
        <%}%>

    function showProductSharetoFriendsDialog() {
       popupDialog("#ProductSharetoFriendsDialog");
        return false;
    }
    function showQuantityDiscountRequestDialog() {
        popupDialog("#QuantityDiscountRequestDialog");
        return false;
    }
    function showProductSpecsDialog(productid) {
        //popupDialogDelay(
        //$("<div></div>")
        //.load(GetStoreLocation() + "proc/html.aspx", { type: "ProductSpecList", ProductID: productid }) , 800);
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
        //return false;

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
    function ValidateRequestSpecial(th) {
        if (th.value != "") {
            th.value = th.value.replace(/</g, "").replace(/>/g, "");
        }
    }

    $(document).ready(function(){

        
        $("#txtOrderQty").keyup(function () {
                var bundleQty = parseInt($(this).val());
                $(".bundleItemQty").each(function () {
                    var configQty = parseInt($(this).attr("configQty"));
                    if(bundleQty != 0 && !isNaN(bundleQty)){
                        $(this).val(bundleQty * configQty);
                        $(this).prev("span").text($(this).val());
                    }
                    else {
                        $(this).val(configQty);
                        $(this).prev("span").text($(this).val());
                    }
                });
            });

        $(".SystemIntegrityCheck").click(function () {
            var result=true;

            //result=checkResoruce();

            if(!checkMOQ())
                return false;
            <%if(eStore.Presentation.eStoreContext.Current.User!= null && EnableSystemIntegrityCheck) {%>
                if(result && isCheckOSandStorage != true){
                    result=  SystemIntegrityCheck();
                }
            <%} %>


            return result;
        });
    });


    //--- start check MininumnOrderQty ---//

    function checkMOQ()
	{
        var cc = true;
        var productids = "";
        var i = 0;
        var errorMessage = "<%= eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Minimum_Quantity_Error)%>";
		$("input:text[MOQ][value!=''],,:checked[MOQ][value!=''").each(function(){
			if(parseInt($(this).val()) < parseInt($(this).attr("MOQ")))
			{
               if(i == 0){
                productids = $(this).attr("sproductid") + "[" + $(this).attr("MOQ") +"]";     
                $(this).focus();
               }
               else 
                productids = productids  + "," + $(this).attr("sproductid") + "[" + $(this).attr("MOQ") +"]";
               cc = false;
               i++;
			}
		});
        if(!cc)
            alert(errorMessage.replace("{0}",productids));
        return cc;
	}

    //--- end check MininumnOrderQty ---//



    //--- start check OS and Storage ---//

    var isCheckOSandStorage = false; //是否需要检查OS 和 Storage
    function SystemIntegrityCheck(){

        var noneedOS=isNoNeed($('#Peripheral-<%= System.Configuration.ConfigurationManager.AppSettings["StandardOSID"]%>'));
        var StorageCnt=isNoNeed($('#Peripheral-<%= System.Configuration.ConfigurationManager.AppSettings["StandardStorageID"]%>'));
   

        if(noneedOS>0 && StorageCnt>1)
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
        else if(noneedOS==0 && StorageCnt>0)
        {
            return isCheckOSandStorage = confirm($.eStoreLocalizaion("Are_you_sure_you_dont_want_to_select_any_OS"));
        }
        else if(noneedOS>0 && StorageCnt==0)
        {
            alert($.eStoreLocalizaion("Are_you_sure_you_dont_want_any_storage_device_with_your_OS"));
            return false;
        }
        else if(noneedOS>0 && StorageCnt==1)
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
            $.each($(module).next().find(":text,:checked"), function(i, qty){
              if($(qty).val()!=""&&!isNaN(parseInt($(qty).val())))
                {
                    cnt+=parseInt($(qty).val());
                }
            });
        }
       return cnt;
    }

    //--- end check OS and Storage ---//


    //--- start check Limited Resource ---//

    function checkResoruce(){ 

        var resourceSettings=new Array();　
        var overcapacity=new Array();

        $("#ProductExpansionInfor").find(":text[resource][value!=''],:checked[resource][value!='']").each(function(i){
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
    //--- end check Limited Resource ---//

$(function() {
		
	$(".carouselBannerSingle").each(function(){
		var id = $(this).attr('id');
		//console.log(id);
		$("#"+id).find("ul").carouFredSel({
			auto: false,
			scroll: 1,
			prev: '#prev-' + id,
			next: '#next-' + id,
			pagination: '#pager-' + id
		});
	});	

});

</script>
