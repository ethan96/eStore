<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="System.aspx.cs" Inherits="eStore.UI.Product.SystemPage" EnableEventValidation="false" %>

<%@ Register Src="~/Modules/ProductLiterature.ascx" TagName="ProductLiterature" TagPrefix="eStore" %>
<%@ Register Src="~/Modules/ProductSharetoFriends.ascx" TagName="ProductSharetoFriends"
    TagPrefix="eStore" %>
<%@ Register Src="~/Modules/QuantityDiscountRequest.ascx" TagName="QuantityDiscountRequest"
    TagPrefix="eStore" %>
<%@ Register Src="~/Modules/YouAreHereMutli.ascx" TagName="YouAreHereMutli" TagPrefix="eStore" %>
<%@ Register Src="~/Modules/ChangeCurrency.ascx" TagName="ChangeCurrency" TagPrefix="eStore" %>
<%@ Register Src="~/Modules/SocialNetworkContent.ascx" TagName="SocialNetworkContent"
    TagPrefix="eStore" %>
<asp:Content ID="eStoreContent" runat="server" ContentPlaceHolderID="eStoreMainContent">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/system")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/system")%>
    <div class="eStore_product_productID">
        <asp:HiddenField ID="hfProductID" runat="server" />
    </div>
    <eStore:YouAreHereMutli ID="YouAreHereMutli1" runat="server" />
    <div class="eStore_container eStore_block980">
        <div class="eStore_product_content">
            <div class="eStore_product_product row20">
                <div class="eStore_product_productName row20">
                    <h2>
                        <asp:Literal ID="lProductName" runat="server"  EnableViewState ="false" /><span class="icon">
                            <asp:Literal ID="ltproductStatus" runat="server"  EnableViewState ="false"></asp:Literal><span class="remind font18"><asp:Literal ID="ltPhaseOut" runat="server"
                                Visible="false"  EnableViewState ="false"></asp:Literal></span>
                        </span>
                    </h2>
                    <h1>
                        <asp:Literal ID="lShortDescription" runat="server"  EnableViewState ="false"/>
                    </h1>
                </div>
                <div class="eStore_product_productPic row20">
                    <div class="eStore_product_picBig">
                        <asp:Literal ID="imgbigimage" runat="server"   EnableViewState ="false"></asp:Literal>
                        <span class="eStore_product_focusBlock"></span>
                        <div class="eStore_product_picZoom">
                            <asp:Literal ID="imgbigimage2" runat="server"  EnableViewState ="false"></asp:Literal>
                        </div>
                    </div>
                    <div class="eStore_product_picSmall carouselBannerSingle" id="productPicture">
                        <ul>
                            <asp:Literal ID="productLightBox" runat="server"  EnableViewState ="false"></asp:Literal>
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
                        <asp:Literal ID="lProductFeature" runat="server"  EnableViewState ="false"></asp:Literal>
                    </ol>
                    <eStore:Repeater ID="rpBTOSConfigDetails" runat="server" Visible="false"  EnableViewState ="false">
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
                            </ul> </div>
                        </FooterTemplate>
                    </eStore:Repeater>
                    <div class="eStore_product_productMsg">
                        <asp:Panel ID="plCertification" runat="server" Visible="false"  EnableViewState ="false">
                            <div class="eStore_product_productMsgBlock">
                                <div class="eStore_title">
                                    <asp:Label ID="lblCertification" runat="server" Text="Certifications"></asp:Label>
                                </div>
                                <asp:Repeater ID="rptCertification" runat="server" OnItemDataBound="rptCertification_ItemDataBound">
                                    <ItemTemplate>
                                        <span>
                                            <asp:Image ID="imgCertification" runat="server" /></span>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </asp:Panel>
                        <div class="eStore_product_productMsgBlock resourcesBlock">
                            <asp:Literal ID="ltResources" runat="server"  EnableViewState ="false"></asp:Literal>
                        </div>
                         
                    </div>
                    <div class="eStore_product_mobile_message eStore_redStar">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_mobile_message)%>
                    </div>
                </div>
                <div class="eStore_product_productAction row20">
                    <div class="eStore_product_price row10">
                         <asp:Literal ID="lSoftwareSubscriptionMessage" runat="server"  EnableViewState ="false"></asp:Literal>
                        <asp:Literal ID="lProductprice" runat="server"  EnableViewState ="false"></asp:Literal>
                        <eStore:ChangeCurrency ID="ChangeCurrency1" runat="server" />
                    </div>
                    <div class="icon">
                        <asp:Image ID="imgproductwarranty" ImageUrl="/images/2years.jpg" CssClass="productwarranty"
                            AlternateText="2 years extended warranty" runat="server"  EnableViewState ="false"/>
                        <asp:Literal ID="ltFastDelivery" runat="server"  EnableViewState ="false"></asp:Literal>
                        <br />
                        <b>
                            <asp:Label ID="lPriceExtendedDescripton" runat="server" CssClass="colorRed"  EnableViewState ="false"></asp:Label></b>
                    </div>
                    <div class="eStore_product_date">
                        <asp:Label ID="LtPromotionMessage" Visible="false" runat="server"  EnableViewState ="false"></asp:Label>
                    </div>
                    <div class="eStore_product_btnBlock">
                        <asp:HyperLink ID="hDatasheet" runat="server" CssClass="eStore_btn borderBlue"
                        Target="_blank" Visible="false"><span>Datasheet</span></asp:HyperLink>
                       <asp:HyperLink ID="hRequestQuantityDiscountTop" runat="server" CssClass="eStore_btn borderBlue"  EnableViewState ="false"
                            Target="_blank"></asp:HyperLink>
                        <asp:HyperLink ID="hlPreview" CssClass="eStore_btn borderBlue systemBomPreview ctosneedlogin mousehand" runat="server"  EnableViewState ="false">Preview</asp:HyperLink>
                        <a <%if (!hasWidgetConfig)
                         {%>style="display: none;" <%}%>>
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.ctos_reconfig)%></a>
                        <a style="display: none;" class="ctosneedlogin">
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_widget)%></a>
                        <asp:LinkButton ID="btnUpdateTop" runat="server" CssClass="ctosneedlogin eStore_btn SystemIntegrityCheck"  EnableViewState ="false"
                            OnClick="btnUpdate_Click" Text="Update" />
                        <asp:LinkButton ID="btnAdd2QuoteTop" runat="server" CssClass="ctosneedlogin eStore_btn borderBlue SystemIntegrityCheck"  EnableViewState ="false"
                            OnClick="btnAdd2Quote_Click" Text="Add to Quote" />
                        <asp:LinkButton ID="btnAdd2CartTop" runat="server" CssClass="ctosneedlogin eStore_btn SystemIntegrityCheck"  EnableViewState ="false"
                            OnClick="btnAdd2Cart_Click" Text="Add to Cart" />
                    </div>
                    <div class="eStore_product_link">
                     
                    </div>
                </div>
            </div>
        </div>
        <!--eStore_product_product-->
        <%=eStore.Presentation.eStoreContext.Current.getStringSetting("CtosMemo")%>
        <div class="positionFixed">
            <div class="eStore_4Steps">
                <div id="ProductWidget"></div>
                <%=navigator %>
                <!--eStore_4Step_title-->
                <div class="eStore_4Step_content" runat="server" id="system_content" enableviewstate="false">
                    <div id="configItemsTitle" class="<%if (hasWidgetConfig)
                                                    {%>hiddenitem<%}%>">
                        <asp:Label ID="lbl_configItemsTitle" Text="Build Your System" runat="server"   EnableViewState ="false"></asp:Label>
                        <asp:Label ID="lbl_configItemsmessage" Text="(Login first to build your system)"
                            CssClass="colorRed" Visible="false" runat="server"   EnableViewState ="false"></asp:Label>
                    </div>
                    <div id="configurationsystem" class="row20">
                        <asp:Literal ID="CTOSModules" runat="server" EnableViewState ="false" />
                    </div>
                    <div id="commentArea" class="eStore_remindBlockPOP">
                        <p></p>
                        <h4>
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Comment)%>
                        </h4>
                        <eStore:TextBox TextMode="MultiLine" ClientIDMode="Static" Width="755px" Height="150px" runat="server"  EnableViewState ="false"
                            ToolTip="Write your special instruction here" ID="txtComment"></eStore:TextBox>
                    </div>
                    <!--row-->
                </div>
                <!--eStore_4Step_content-->
            </div>
            <!--eStore_4Step-->
            <div class="eStore_system_listFloat">
                <!--eStore_system_listFloatResources-->
                <div class="eStore_system_listFloatPrice" runat="server" enableviewstate="false" id="panel_system_listFloat">
                    <div class="eStore_system_title">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Your_System)%>
                    </div>
                    <div class="eStore_system_action">
                        <div class="price">
                            <span class="priceOrange float-right text-right">
                                <asp:Literal ID="lbfloatPrice" runat="server"   EnableViewState ="false"></asp:Literal></span>
                        </div>
                        <div class="eStore_product_btnBlock">
                            <a class="eStore_btn borderBlue fancybox systemBomPreview mousehand">Preview</a>
                            <asp:LinkButton CssClass="ctosneedlogin eStore_btn SystemIntegrityCheck" ID="btnUpdateFloat" runat="server"  EnableViewState ="false"
                                Text="Update" OnClick="btnUpdate_Click" />
                            <asp:LinkButton CssClass="ctosneedlogin eStore_btn borderBlue SystemIntegrityCheck" ID="btnAdd2QuoteFloat"  EnableViewState ="false"
                                runat="server" Text="Add to Quote" OnClick="btnAdd2Quote_Click" />
                            <asp:LinkButton CssClass="ctosneedlogin eStore_btn SystemIntegrityCheck" ID="btnAdd2CartFloat" runat="server"  EnableViewState ="false"
                                Text="Add to Cart" OnClick="btnAdd2Cart_Click" />
                        </div>
                    </div>
                </div>
                <!--eStore_system_listFloatPrice-->
                <div id="storeSideAds">
                </div>
            </div>
            <!--eStore_system_listFloat-->
        </div>
        <div class="clearfix"></div>
        <div id="mostcategory" class="row20">
        </div>
        <!--eStore_article_moreInfo-->
        <!--eStore_article_content-->
        <div id="ProductSharetoFriendsDialog" style="display: none;">
            <eStore:ProductSharetoFriends ID="ProductSharetoFriends1" runat="server"  EnableViewState ="false" />
        </div>
        <div id="QuantityDiscountRequestDialog" style="display: none;">
            <eStore:QuantityDiscountRequest ID="QuantityDiscount" runat="server"  EnableViewState ="false" NeedLogin="false" HideQuantity="false" />
        </div>
        <div id="inline1" style="width: 600px; display: none;">
        </div>
        <script type="text/javascript" language="javascript">
        var _unit = '<%=eStore.Presentation.eStoreContext.Current.Store.profile.Settings["CartPriceRoundingUnit"] %>';
        //var ctosscrollFollow;
        var preventswithToNextPanel=false;
        $(document).ready(function () {
            $(function () {
                $('a.fancybox').fancybox();
            });
            <%if (eStore.Presentation.eStoreContext.Current.User == null && eStore.Presentation.eStoreContext.Current.Store.storeID != "AUS" && eStore.Presentation.eStoreContext.Current.Store.storeID != "AKR")
              { %>
            $("#eStore_LogIn_input .divFeature").removeClass("hiddenitem");
            popLoginDialog(360,480,"View System Page");
            $("#configurationsystem input").attr("disabled","disabled");
            $("#configurationsystem *").click(function () {
                $("#eStore_LogIn_input .divFeature").removeClass("hiddenitem");
                return popLoginDialog(360,480,"Configuration System");
            }
           );
                <%}%>
        });
        function selectItem(obj,isOnlyOne) {
            //set btos changing
            var currentprice = $("#hdefaultprice").val();
            var ctoModuleID = obj.parent().parent().parent().attr("id").replace("module", "btos");
            insertCheckBoxBtosInfor(obj,ctoModuleID,isOnlyOne);

            //set price
            $(".specialprice span").html($("<img>").attr("src","<%=esUtilities.CommonHelper.GetStoreLocation()%>images/priceprocessing.gif"));
            $(".regularprice span").html($("<img>").attr("src","<%=esUtilities.CommonHelper.GetStoreLocation()%>images/priceprocessing.gif"));
            $(".exchangedprice").html($("<img>").attr("src","<%=esUtilities.CommonHelper.GetStoreLocation()%>images/priceprocessing.gif"));
            $("#module-2139 .addtionprice").html($("<img>").attr("src","<%=esUtilities.CommonHelper.GetStoreLocation()%>images/priceprocessing.gif"));


            $.getJSON("<%=esUtilities.CommonHelper.GetStoreLocation()%>proc/do.aspx",
                { func:"<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.getCBOMPrice %>"
                 ,packageid: "<%=esUtilities.CommonHelper.QueryString("packageid") %>"
                 ,productID: "<%= productId %>"
                 ,para: getBTOSId()
                 ,time: new Date().getTime()
                 ,addons: getAddons() },
        function (data) {
            $(".specialprice span").html(data.markupprice);
            $(".regularprice span").html(data.listprice);
            $("#hlistprice").val(data.hlistprice);
            if(data.exchangedprice&&data.exchangedprice!="0")
                $(".exchangedprice").html(data.exchangedprice);
            else
                $(".exchangedprice").html("");

            if(data.warrantyprice)
            {
                $.each(data.warrantyprice,function(i,n){
                    var sum = n.price;
                    var sumSign = "";
                    if(sum >= 0){
                        sumSign = "+";
                    }
                    else if(sum < 0){
                        sumSign = "-";
                    }
                    $("#module-2139 .options").has(":radio[value='"+ n.id+ "']").find(".priceSing").html(sumSign);
                    $("#module-2139 .options").has(":radio[value='"+ n.id+ "']").find(".addtionprice").html(formatRoundPrice(Math.abs(sum),_unit));
                });

            }
        });
        }

      
        function showProductSharetoFriendsDialog() {
            popupDialog("#ProductSharetoFriendsDialog");
            return false;
        }
        function showQuantityDiscountRequestDialog() {
            popupDialog("#QuantityDiscountRequestDialog");
            return false;
        }
        function showProductSpecsDialog(productid) {
    
            // popupDialogDelay(
		    //   $("<div></div>")
            //.load(GetStoreLocation() + "proc/html.aspx", { type: "ProductSpecList", ProductID: productid }) ,800);
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
            //    $("<div></div>")
            //        .load(GetStoreLocation() + "proc/html.aspx", { type: "Product3DModel", ProductID: productid }), 800);
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

    <%if (hasWidgetConfig)
      {%>
        var productWidgetId = <%=productWidgetId %>;
        $(document).ready(function () {
            if(productWidgetId != 0){
                $.get('ProWidget.ashx', { WidgetID: productWidgetId }, function(result){
                    $("#ProductWidget").html(result);
                    adjustContentHeight() 
                    if(isExitsFunction("reLoadWidgetPageFunction")) {
                        reLoadWidgetPageFunction();
                        adjustContentHeight() 
                    }
                    $("#btShowConfig").show();
                }, 'html');
            }
            $("#btShowConfig").click(function(){
                $("#configItemsTitle,#configurationsystem").show();
                $("#ProductWidget,#btShowConfig").hide();
                adjustContentHeight() 
            });
        });
        <%} %>

        function checkOSHardDisk(){
            if(checkResoruce() == false)
                return false;
                <%if (eStore.Presentation.eStoreContext.Current.User != null && EnableSystemIntegrityCheck)
                    {%>
            var rlt = SystemIntegrityCheck();
            if(rlt==false)
                return false;
            <%} %>
        }

        var confirmednoOS=false;
        var confirmedinstallationinstruction=false;
        function SystemIntegrityCheck(){
            preventswithToNextPanel=false;
            var noneedOS=true;
            $(".module[integritychecktype='OS']").each(function(i,os){
                if(getOrderedQty($(os))>0)
                    noneedOS=false;
            });
            var StorageCnt=0;
            $(".module[integritychecktype='Storage']").each(function(i,storage){
                StorageCnt+=getOrderedQty($(storage));
            });

            if(!noneedOS && StorageCnt>1)
            {
                if($("#txtComment").val()==""||$("#txtComment").val()==$("#txtComment").attr("title"))
                {
                    popupDialog($("<div style='width: 400px;' />").addClass("eStore_remindBlockPOP")
                        .append($("<p />").addClass("eStore_remindBlockTitle").html("Warning"))
                        .append($("<p />").addClass("eStore_remindBlockTop")
                        .html($.eStoreLocalizaion("please_indicate_which_device_you_would_like_the_OS_installed"))));
                    
                    $("#txtComment").val($.eStoreLocalizaion("Please_install_the_OS_in_Hard_Drive")).css("color", "#000");
                    $("#txtComment").focus();
                    preventswithToNextPanel=false;
                    return false;
                }
                else
                {
                    if(confirmedinstallationinstruction)
                    {
                        return true;
                    }
                    else
                    {
                        if (confirm($.eStoreLocalizaion("Did_you_tell_us_where_to_install_the_OS_in_the_installation_instruction"))) {
                            confirmedinstallationinstruction=true;
                            return true;
                        }
                        else {

                            $("#txtComment").focus();
                            preventswithToNextPanel=false;
                            return false;
                        }
                    }
                }
            }
            else if(noneedOS && StorageCnt>1)
            {
                if(!confirmednoOS){
                    if( confirm($.eStoreLocalizaion("Are_you_sure_you_dont_want_to_select_any_OS")))
                    {
                        confirmednoOS=true;
                        return true;
                    }
                    else
                    {
                        if($("div[integritychecktype='OS']").length>0)
                        {
                            window.location.hash=$("div[integritychecktype='OS']")[0].id;
                            preventswithToNextPanel=true;
                        }
                        return false;
                    }

                }
                else
                {
                    return true;
                }
            }
            else if(!noneedOS && StorageCnt==0)
            {

                popupDialog($("<div style='width: 400px;' />").addClass("eStore_remindBlockPOP")
                        .append($("<p />").addClass("eStore_remindBlockTitle").html("Warning"))
                        .append($("<p />").addClass("eStore_remindBlockTop")
                        .html($.eStoreLocalizaion("Are_you_sure_you_dont_want_any_storage_device_with_your_OS"))));

                if($("div[integritychecktype='Storage']").length>0)
                {
                    window.location.hash=$("div[integritychecktype='Storage']")[0].id;
                    preventswithToNextPanel=true;
                }
                return false;
            }
            else if(!noneedOS && StorageCnt==1)
            {
                if($("#txtComment").val()==$.eStoreLocalizaion("Please_install_the_OS_in_Hard_Drive"))
                {
                    $("#txtComment").val($("#txtComment").attr("title"));
                }
            }
        }

        function formatRoundPrice(num, unit) {
            if (unit == null || unit == "" || unit == undefined)
                unit = 1;
            num = num.toString().replace(/\$|\,/g, '');
            var cc = num;
            cc = Math.round(num / unit) * (unit * 10000) / 10000;
            var ccNun = cc.toString();
            var re = /(-?\d+)(\d{3})/;
            while (re.test(ccNun))
                ccNun = ccNun.replace(re, "$1,$2")
            return ccNun;
        }
        </script>
            <eStore:Advertisement ID="Advertisement1" runat="server" />
        <script id="_tmpcategories" type="text/x-jquery-tmpl">
    <div class="eStore_product_moreInfo eStore_other_BGBlock row20" data-bind="visible: $data.length  > 0">
        <h4>Check More Information for Your Project</h4>
        <!-- ko foreach:  $data -->
        <div class="eStore_productBlock">
            <div class="eStore_productBlock_pic row10">
                <img data-bind="attr: { src: Image, title: Name, alt: Description }" />
            </div>
            <div class="eStore_productBlock_txt row10" data-bind="text: Name">
            </div>
            <div class="eStore_productBlock_price row10">
                <div class="priceOrange" data-bind="html: Price">
                </div>
            </div>
            <a data-bind="attr: { href: Url }" class="eStore_btn">More</a>
        </div>
        <!-- /ko -->
        <div class="clearfix"></div>
    </div>
        </script>
</asp:Content>
