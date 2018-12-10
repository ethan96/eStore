<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChannelPartner.ascx.cs" Inherits="eStore.UI.Modules.ChannelPartner" %>
<style>
    /*ChannelPartner.ascx Start*/
    #divChannelPartner fieldset{padding:10px 10px;}
    #divChannelPartner .mainDiv{margin-top:10px; font-family:Arial, Helvetica, sans-serif;}
    #divChannelPartner .channeTd{ width:200px; font-weight:bold; text-transform :uppercase;}
    /*ChannelPartner.ascx End*/
</style>
<div id="divChannelPartner">
    <script type="text/javascript">
        function setChannelPartnerID() {
            var channelID = "";
            var channelName = "";
            var cpChnenelGroup = document.getElementsByName("cpSelector");
            for (var i = 0; i < cpChnenelGroup.length; i++) {
                if (cpChnenelGroup[i].checked) {
                    channelID = cpChnenelGroup[i].value;
                    channelName = cpChnenelGroup[i].title;
                }
            }
            //window.opener.window.channelID = channelID;
            var sourceID = Math.round(Math.random() * 100000);
            $.ajax({
                url: "<%=esUtilities.CommonHelper.GetStoreLocation() %>html.aspx",
                data: { type: "ChannelParter", channelIDStr: channelID, channelNameStr: channelName, sourceIDStr: sourceID },
                success: function (msg) {
                    window.returnValue = channelID + "||" + channelName;
                    window.opener = null;
                    window.open('', '_self');
                    window.close();
                }
            });
        }
    </script>
    <fieldset>
        <legend>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Please_Select_one_of_the)%>
        </legend>        
        <div class="mainDiv">
            <ul><b><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Node)%>:</b></ul>
            <ol>
                <li>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Should_a_Channel_Partner_be_selected)%>
                </li>
                <li><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_All_shipments_from_Advantech)%></li>
                <li>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_For_all_shipments_arranged_by_Advantech_Channel_Partners)%>
                </li>
            </ol>
            <table>
                    <tr>
                    <td class="channeTd" valign="top">
                        <input id="cpSelector" type="radio" name="cpSelector" value = "<%=eStore.Presentation.eStoreContext.Current.Store.profile.storeChannelAccount.Channelid%>" checked="checked" 
                            title="<%=eStore.Presentation.eStoreContext.Current.Store.profile.storeChannelAccount.Company%>"  />
                        <%=eStore.Presentation.eStoreContext.Current.Store.profile.storeChannelAccount.Company%>
                        <%--<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Directly_via_Advantech)%>--%>
                    </td>
                    <td>
                        <asp:Literal ID="ltRadio" runat="server" ></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <input type="button" value="<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select) %>" onclick="setChannelPartnerID()" />
                    </td>
                </tr>

            </table>
        </div>
    </fieldset>
    
</div>