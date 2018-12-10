<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyReward.ascx.cs" Inherits="eStore.UI.Modules.V4.MyReward" %>


<div class="eStore_account_msg">
    <div class="eStore_accountReward">
        <h2><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Reward_Item_Points)%></h2>
        <div class="eStore_accountReward_AvailablePoint eStore_accountReward_total">
            <span class="smallTitle"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Available_points)%></span>
            <div class="txt"><asp:Literal ID="ltAblePoints" runat="server"></asp:Literal></div>
            <a href="#account_loyalty_usedPoint" class="fancybox"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Details)%></a>
        </div>
        <div class="eStore_accountReward_TotalAmount eStore_accountReward_total">
            <span class="smallTitle"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Total_Amount)%></span>
            <div class="txt">
            <span><%= eStore.Presentation.eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign%></span><asp:Literal ID="ltTotalAmountTop" runat="server"></asp:Literal></div>
            <a style="cursor:pointer" onclick="location.hash='#TotalAmount';"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Details)%></a>
        </div>
        <div class="clearfix"></div>
        <div class="eStore_accountReward_Redeem" id="dRedeem_points">
            <h3><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Redeem_points)%></h3>
            <ul>
                <asp:Repeater ID="rptGiftItem" runat="server" OnItemDataBound="rptGiftItem_ItemDataBound">
                    <ItemTemplate>
                        <li>
                            <div class="RedeemTop">
                                <div class="RedeemTop_money">
                                    <%# Eval("Description")%>
                                </div>
                                <div class="RedeemTop_point"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Reward_Item_Points)%>:
                                    <%#Eval("Cost") %>
                                </div>
                            </div>
                            <div class="RedeemBottom">
                                <img src="/images/account_rewardPoints_RedeemLess.jpg" class="btnValue Less" alt="" />
                                <div class="value">0</div>
                                <asp:TextBox ID="hdRedeemCount" runat="server" CssClass="hiddenitem">0</asp:TextBox>
                                <img src="/images/account_rewardPoints_RedeemPlus.jpg" class="btnValue Plus" alt="" />
                                <div class="clearfix"></div>
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
                <div class="clearfix"></div>
            </ul>
            <div class="eStore_order_btnBlock">
                <asp:Button ID="btsendRemeed" CssClass="eStore_btn" runat="server" Text="Convert Now" OnClick="btsendRemeed_Click" />
            </div>
        </div> 

        <a id="TotalAmount" name="TotalAmount"></a>
        <h2><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Details)%><span>( <asp:Literal ID="ltOrderDate" runat="server"></asp:Literal> )</span></h2>
        <asp:Repeater ID="rpOrderList" runat="server">
            <HeaderTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
                <tr>
                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_No)%></th>
                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_date)%></th>
                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Amount)%></th>
                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_User_ID)%></th>
                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Status)%></th>
                </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                <td><a href='<%#Eval("OrderNoUrl")%>'><%#Eval("OrderNo")%></a></td>
                <td><%#Eval("OrderDate")%> </td>
                <td><%#Eval("SubTotal")%></td>
                <td><%#Eval("UserId")%></td>
                <td><%#Eval("Status")%></td>
                </tr>
            </ItemTemplate>   
            <FooterTemplate>
                </table>
            </FooterTemplate> 
        </asp:Repeater>
        <div class="eStore_accountReward_TotalBottom">
            <div class="amount"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Total_Amount)%>(<%= eStore.Presentation.eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign%>)：<span>
                <asp:Literal ID="ltTotalOrderAmount" runat="server"></asp:Literal></span></div>
            <div class="point"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Reward_point)%>：<span><asp:Literal ID="ltTotalRewardAmount" runat="server"></asp:Literal></span></div>
        </div>      
    </div><!--eStore_accountReward-->
</div>



<a href="#account_loyalty_thankU" class="fancybox" id="showloyalty_thankU"></a>
<div id="account_loyalty_thankU" style="display:none;">
    <h1><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Thank_You)%></h1>
    <p><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_We_will_send_virtual_vouchers)%></p>
</div>
<div id="account_loyalty_usedPoint" style="display:none;">
    <h1><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Record_of_redeemed_points)%></h1>
    <asp:Repeater ID="rpAblePoints" runat="server"  OnItemDataBound="rpAblePoints_ItemDataBound">
        <HeaderTemplate>
            <table width="600" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
              <tr>
                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_date)%></th>
                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_User_ID)%></th>
                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Status)%></th>
                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Redeemed_points)%></th>
                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Amazon_voucher)%></th>
              </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("UpdateDate","{0:MM/dd/yyyy}") %></td>
                <td><%# Eval("UserId")%></td>
                <td>
                    <asp:Literal ID="ltType" runat="server"></asp:Literal></td>
                <td>
                    <asp:Literal ID="ltPoint" runat="server"></asp:Literal></td>
                <td><asp:Literal ID="ltRewardName" runat="server"></asp:Literal></td>
              </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>

</div> 



<script type="text/javascript">
    $(function () {

        $(".fancybox").fancybox();

        var RedeemModel = eval("({availablePoints:" + $(".eStore_accountReward_AvailablePoint .txt").text() + "})");



        //Reward Points**************
        $('.eStore_accountReward_TotalBottom').each(function () {
            var span1 = $(this).find('span').eq(0).outerWidth();
            var span2 = $(this).find('span').eq(1).outerWidth();

            if (span1 > span2) {
                $('.eStore_accountReward_TotalBottom span').css({
                    'width': span1
                });
            }

        });

        $('.RedeemBottom .btnValue').click(function () {

            var non = $(this).parents('li').hasClass('non');
            var att = $(this).hasClass('Less');
            var number = Number($(this).siblings('.value').text());
            var cost = Number($(this).siblings('input[type="text"]').attr("cost"));
            if (non || (cost > RedeemModel.availablePoints && $(this).hasClass("Plus"))) {
                return false;
            } else {
                if (att && number != 0) {
                    number = number - 1;
                    RedeemModel.availablePoints += cost;
                    $(this).siblings('.value').text(number);
                    $(this).siblings('input[type="text"]').val(number);
                } else if (att == false) {
                    number = number + 1;
                    RedeemModel.availablePoints -= cost;
                    $(this).siblings('.value').text(number);
                    $(this).siblings('input[type="text"]').val(number);
                }

            }

        });

        checkGift();

        function checkGift() {
            $("#dRedeem_points").find("li input[item]").each(function (i, n) {
                if ($(this).attr("cost") > RedeemModel.availablePoints) {
                    $(this).closest("li").addClass("non");
                }
            });
        }
    });
</script>