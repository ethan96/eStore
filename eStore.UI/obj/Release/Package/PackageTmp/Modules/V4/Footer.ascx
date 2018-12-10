<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="eStore.UI.Modules.V4.Footer"
    EnableViewState="false" %>

<div class="eStore_footer">
    <eStore:Repeater ID="rpStores" runat="server" OnItemCommand="rpStores_ItemCommand" OnItemDataBound="rpStores_ItemDataBound">
        <HeaderTemplate>
            <div class="eStore_footerArea">
            <span class="eStore_footerAreaTop"><span class="logo">
                <asp:Image ID="logoImage" runat="server" AlternateText="Advantech eStore Logo"/>
<%--                <img src='<%= ResolveUrl("~/images/eStore_logoS.png") %>'  alt="Advantech eStore Logo"/>--%>
            </span> <span class="eStore_footerAreaNow">
                </span><span class="eStore_footerAreaBtn">Select</span> </span><span class="eStore_footerAreaBottom">
        </HeaderTemplate>
        <ItemTemplate>
            <asp:LinkButton ID="lbtnStore" runat="server" CommandArgument='<%# Eval("StoreID") %>' CommandName ="ChangeStore" ToolTip='<%#Eval("RegionDescription") %>'
                Text='<%#Eval("RegionName") %>' CssClass='<%#eStore.Presentation.eStoreContext.Current.Store.storeID==Eval("StoreID").ToString()? "on":  ""%>'></asp:LinkButton>
        </ItemTemplate>
        <FooterTemplate>
            </span> </div></FooterTemplate>
    </eStore:Repeater>
    <asp:Literal ID="lFooter" runat="server"></asp:Literal>
    <asp:Literal ID="lPolicyInformation" runat="server"></asp:Literal>
<%--<%if (TypeFormID() != null) { %>
<div class="eStore_type_form">
    <a class="typeform-share link" href="https://estore.typeform.com/to/<%=TypeFormID()%>" data-mode="2" target="_blank"><%=eStore.Presentation.eStoreLocalization.Tanslation("eStoreOnlineSurvey")%></a>
    <script>(function(){var qs,js,q,s,d=document,gi=d.getElementById,ce=d.createElement,gt=d.getElementsByTagName,id='typef_orm',b='https://s3-eu-west-1.amazonaws.com/share.typeform.com/';if(!gi.call(d,id)){js=ce.call(d,'script');js.id=id;js.src=b+'share.js';q=gt.call(d,'script')[0];q.parentNode.insertBefore(js,q)}})()</script>
</div>
<%} %> --%>
</div>