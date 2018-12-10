<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true" CodeBehind="RedeemPoints.aspx.cs" Inherits="eStore.UI.Reward.RedeemPoints" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreRightContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="UserRewPoints">
        <asp:Literal ID="ltUserRewPoints" runat="server"></asp:Literal></div>
    <ul class="photolist">
        <asp:Repeater ID="rptGiftItem" runat="server" onitemdatabound="rptGiftItem_ItemDataBound">
            <ItemTemplate>
                 <li>
                    <div class="giftContainerImg">
                        <a href='<%# Eval("ProductUrl") %>' title='<%# Eval("Name") %>' target="_blank">
                            <asp:Image ID="imgUrl" runat="server" Width="180" Height="100" onerror="this.src='../images/photounavailable.gif'" />
                        </a>
                    </div>
                    <div class="giftTitle">
                        <p class="pointP">
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Reward_Item_Points)%>: 
                            <span class="pointSpan"><%#Eval("Cost") %></span>
                        </p>
                        <p class="descriptP"><a href='<%# Eval("ProductUrl") %>' target="_blank"><%#Eval("Description") %></a></p>
                    </div>
                    <div class="giftIteminfo">
                        <asp:Button ID="btnRedeem" runat="server" Text="Redeem" class="btnRedeem" CommandArgument='<%#Eval("ItemNo") %>' OnClick="btnRedeem_Click" />
                    </div>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</asp:Content>
