<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true" CodeBehind="CreditReward.aspx.cs" Inherits="eStore.UI.Reward.CreditReward" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreRightContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <h1>
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.My_Credit_Reward)%>
        <%if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("EnableRewardSystem", false))
          { %>
        <a href="RedeemPoints.aspx" class="h1_Reward_A">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Redeem_My_Points)%>
        </a>
        <%} %>
    </h1>
    <asp:GridView ID="gvReward" runat="server" CssClass="estoretable" 
        AutoGenerateColumns="False" GridLines="None" OnRowDataBound="gvReward_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="Date" ItemStyle-CssClass="center" ItemStyle-Width="140">
                <ItemTemplate>
                    <asp:Label ID="lblUpdateDate" runat="server"><%# Eval("UpdateDate","{0:MM/dd/yyyy}") %></asp:Label>
                </ItemTemplate>
                <HeaderTemplate>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_date)%>
                </HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Item#">
                <ItemTemplate>
                    <asp:HyperLink ID="hyItemNo" runat="server" NavigateUrl="#" Target="_blank">None</asp:HyperLink>
                </ItemTemplate>
                <HeaderTemplate>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Reward_Item_No)%>
                </HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Reward points" ItemStyle-CssClass="center" ItemStyle-Width="120">
                <ItemTemplate>
                    <asp:Label ID="lblPoint" runat="server"><%# Eval("RewardPoint") %></asp:Label>
                </ItemTemplate>
                <HeaderTemplate>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Reward_Item_Points)%>
                </HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status" ItemStyle-CssClass="center" ItemStyle-Width="170">
                <ItemTemplate>
                    <asp:Label ID="lblStatus" runat="server"></asp:Label>
                </ItemTemplate>
                <HeaderTemplate>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Reward_Item_Status)%>
                </HeaderTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
