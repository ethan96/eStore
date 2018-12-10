<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="ProductCategoryV4S.aspx.cs" Inherits="eStore.UI.Product.ProductCategoryV4S" %>

<%@ Register Src="~/Modules/V4/CategoryV4.ascx" TagPrefix="eStore" TagName="CategoryV4" %>
<%@ Register Src="~/Modules/V4/CategoryV2.ascx" TagPrefix="eStore" TagName="CategoryV2" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreHeaderFullSizeContent" runat="server">
    
    <eStore:YouAreHere ID="YouAreHere1" runat="server" />
    <div class="eStore_category_banner row20<%=hasfullsizebanner?" hidden":"" %>">
        <div class="eStore_block980" style="background-image: <%=BannerImage%>" data-bind="style: { backgroundImage: BannerImage() }">
            <div class="eStore_category_banner_txt float-left">
                <h1 data-bind="html: Name()">
                    <asp:Literal ID="lCategoryName" runat="server" EnableViewState="false"></asp:Literal></h1>
                <span data-bind="html: Description()">
                    <asp:Literal ID="lCategoryDescription" runat="server" EnableViewState="false"></asp:Literal>
                </span>
            </div>
        </div>
    </div>
    <div id="HeaderBanner" class="eStore_container<%=hasfullsizebanner?"":" hidden" %>">
        <div class="cycle-slideshow" data-cycle-fx="scrollHorz" data-cycle-timeout="5000" data-cycle-random="true" data-cycle-log="false"
            data-cycle-slides=">div.cycle-slideitem"
            data-cycle-pause-on-hover="true" data-cycle-overlay-template="<div class='eStore_block980 {{overlaystyle}}'><h1>{{title}}</h1><h4>{{desc}}</h4><br><a   href='{{href}}' class='eStore_btn {{btnstyle}}'>{{btntext}}</a></div>">
            <div class="cycle-overlay">
            </div>
            <div class="cycle-pager">
            </div>
            <eStore:Repeater ID="rpBanners" runat="server">
                <itemtemplate>
                        <div class="cycle-slideitem"
                             style ="background-image:url(<%#ResolveUrl("~/resource"+ Eval("Imagefile")) %>)"
                             data-cycle-href="<%# Eval("Hyperlink") %>" alt="<%# Eval("Title") %>" <%# string.IsNullOrEmpty((string) Eval("HtmlContent"))?"data-cycle-btnstyle='blue' data-cycle-btnstyle='' data-cycle-overlaystyle='' data-cycle-btntext='More'":Eval("HtmlContent")%> 
                        data-cycle-title="<%#  System.Net.WebUtility.HtmlEncode ((string)Eval("Title")) %>" data-cycle-desc="<%#  System.Net.WebUtility.HtmlEncode ((string)Eval("AlternateText")) %>" ></div>
                </itemtemplate>
            </eStore:Repeater>
        </div>
    </div>
    <div id="categoryHtml" data-bind="html: HtmlContext()">
        <asp:Literal ID="ltHtml" runat="server"></asp:Literal>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <asp:PlaceHolder ID="phContext" runat="server"></asp:PlaceHolder>
    <script type="text/javascript">
        $(function () {
            if ('<%= eStore.Presentation.eStoreContext.Current.getStringSetting("eStore_ReadMore_for_Category")%>' == "true"){
                addReadMore();;
            }
        });
    </script>
</asp:Content>

