<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="ProductListWithModel.aspx.cs" Inherits="eStore.UI.Product.ProductListWithModel" %>

<%@ Register Src="../Modules/eStoreLiquidSlider.ascx" TagName="eStoreLiquidSlider" TagPrefix="eStore" %>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:YouAreHere ID="YouAreHere1" runat="server" />
   <eStore:eStoreLiquidSlider ID="eStoreLiquidSlider1" runat="server" navigationType="Numbers"
    showDescription="false" MinHeight="120"  />
    <div id="rightprod" style="width: 780px;">
        <div class="title">
              <asp:Literal ID="lTitle" runat="server"></asp:Literal></div>
        <eStore:Repeater ID="rpCategories" runat="server" OnItemDataBound="rpCategories_ItemDataBound">
            <ItemTemplate>
                <div class="prod">
                    <div class="img">
                        <%# (Eval("ImageURL") == null|| string.IsNullOrEmpty(Eval("ImageURL").ToString()))
?string.Empty
                                : string.Format("<a href=\"{0}\"><img src=\"{1}\"  alt=\"{2}\" width=\"163px\" height=\"97px\" /></a> "
, ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))
, eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString() + Eval("ImageURL")
, Eval("LocalCategoryName") )%>
                    </div>
                    <h1 class="subtitle">
                        <%# Eval("LocalCategoryName")%></h1>
                    <div class="text">
                        <eStore:Repeater ID="rpModels" runat="server">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                  <li><a href="<%# Eval("URL")%>">
                                    <%# Eval("ModelNO")%></a></li>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </eStore:Repeater>
                    </div>
                </div>
            </ItemTemplate>
        </eStore:Repeater>
    </div>
</asp:Content>
