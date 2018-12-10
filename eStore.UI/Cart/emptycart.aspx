<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="emptycart.aspx.cs" Inherits="eStore.UI.Cart.emptycart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/order")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/order")%>
</asp:Content>
<asp:Content ID="eStoreContent" ContentPlaceHolderID="eStoreMainContent" runat="server">

    <fieldset>
    
<asp:Label ID="lblCartItemMessage" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
    <h4>
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Your_shopping_cart_is_empty__How_would_you)%>
    </h4>
        <div class="storebutton ">
            <a href='<%= esUtilities.CommonHelper.GetStoreLocation(false) %>' class="eStore_btn"><span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Keep_Shopping)%></span></a>
        </div>
    </fieldset>
</asp:Content>
