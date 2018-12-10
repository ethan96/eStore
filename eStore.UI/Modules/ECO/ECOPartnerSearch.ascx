<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ECOPartnerSearch.ascx.cs"
    Inherits="eStore.UI.Modules.ECO.ECOPartnerSearch" %>
<%@ Register src="../CountrySelector.ascx" tagname="CountrySelector" tagprefix="uc1" %>
<%@ Register Src="../CountrySelector.ascx" TagName="SelectCountry" TagPrefix="uc1" %>
<%@ Register Src="../ServerTime.ascx" TagName="ServerTime" TagPrefix="uc2" %>
<asp:Panel ID="pecoSearchInfor" runat="server">
<div id="ecoSearchInfor">
    <h3 id="ecotitle">
        Ecosystem Partners
    </h3>
    <div id="ecoContext">
        <p class="eco-desContext">
            Search through our Network to complete your solution</p>
        <div class="eco-ddlList">
            <uc1:CountrySelector ID="CountrySelector1" CountryCss="ecoSearchInput" runat="server"
                isShowName="false" ddlWidth="150" FilterCountry="USA,Canada" />
            <p>
                <asp:DropDownList ID="ddlEOCSpecialty" runat="server" Width="150px">
                </asp:DropDownList>
            </p>
            <p>
                <asp:DropDownList ID="ddlECOIndustry" runat="server" Width="150px" CssClass="ecoSearchInput">
                </asp:DropDownList>
                <span class="colorRed">*</span></p>
        </div>
               <div class="clearfix">
                    </div>
        <div id="ecoBTSearch">
            <asp:Button ID="btEcoSearch" runat="server" Text="Search" />
        </div>
        <p id="ecoInterested" class="mousehand ecoInterested" runat="server">
            Interested in being a partner?</p>
    </div>
</div>
       <div class="clearfix">
                    </div>
</asp:Panel>
<div id="ecoInterestedPartner" <%= IsShowECOInterestedPartner?"":"class='hiddenitem'" %>>
        <ul>
            <li class="clear"><span class="ecoInPartner-title fontbold">Company:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtPa_Company" runat="server" Width="363px" CssClass="ecoInPaMustPut"></asp:TextBox><span
                        class="colorRed">*</span></p>
            </li>
            <li class="clear"><span class="ecoInPartner-title fontbold">Website:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtPa_WebSite" Width="363px" runat="server" CssClass="ecoInPaMustPut"></asp:TextBox><span
                        class="colorRed">*</span></p>
            </li>
            <li class="clear"><span class="ecoInPartner-title fontbold">First Name:</span>
                <p class="ecoInPartner-twocolumn">
                    <asp:TextBox ID="txtPa_FirstName" runat="server" CssClass="ecoInPaMustPut"></asp:TextBox><span
                        class="colorRed">*</span></p>
                <span class="ecoInPartner-title fontbold">Last Name:</span>
                <p class="ecoInPartner-twocolumn">
                    <asp:TextBox ID="txtPa_LastName" runat="server" CssClass="ecoInPaMustPut" Width="90"></asp:TextBox><span
                        class="colorRed">*</span></p>
            </li>
            <li class="clear ecoInPartnerfloatLable">
                <uc1:SelectCountry ID="SelectCountry2" runat="server" ddlWidth="200" 
                    CountryCss="ecoInPaMustPut" />
            </li>
            <li class="clear"><span class="ecoInPartner-title fontbold">Address:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtPa_Address" runat="server" Width="363px" CssClass="ecoInPaMustPut"></asp:TextBox><span
                        class="colorRed">*</span></p>
            </li>
            <li class="clear"><span class="ecoInPartner-title fontbold">Zip:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtPa_Zip" runat="server" CssClass="ecoInPaMustPut"></asp:TextBox><span
                        class="colorRed">*</span></p>
            </li>
            <li class="clear"><span class="ecoInPartner-title fontbold mustEmail">E-mail:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtPa_Email" runat="server" Width="363px" CssClass="ecoInPaMustPut mustEmail"></asp:TextBox><span
                        class="colorRed">*</span></p>
            </li>
            <li class="clear"><span class="ecoInPartner-title fontbold">Tel:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtPa_Tel" runat="server" Width="363px" CssClass="ecoInPaMustPut"></asp:TextBox><span
                        class="colorRed">*</span></p>
            </li>
            <li class="clear"><span class="ecoInPartner-title2 fontbold">Industry:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:DropDownList ID="ddlInECOIndustry" runat="server" CssClass="ecoInPaMustPut ddlInECOIndustry">
                    </asp:DropDownList>
                    <span class="colorRed">*</span>
                    <span id="OtherIndustrySpan" class="hiddenitem">Other: 
                    <eStore:TextBox ID="txtOtherIndustry" runat="server" Width="188px"></eStore:TextBox></span>
                </p>
            </li>
            <li class="clear"><span class="ecoInPartner-title fontbold paddingTop10">Specialty:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:CheckBoxList ID="ckbInEOCSpecialty" runat="server" RepeatDirection="Horizontal"
                        RepeatColumns="2">
                    </asp:CheckBoxList>
                </p>
            </li>
            <li class="clear"><span class="ecoInPartner-titleWith200 fontbold">Best time to Contact:</span>
                <p class="ecoInPartner-onecolumn">
                    <uc2:ServerTime ID="ServerTime1" TimeId="ceoIn" runat="server" />
                </p>
            </li>
            <li class="clear"><span class="ecoInPartner-title fontbold normalLineHeigh paddingTop10">Tell us about your company:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtPa_Des" runat="server" Height="105px" TextMode="MultiLine" Width="363px"
                        CssClass="ecoInPaMustPut"></asp:TextBox><span class="colorRed">*</span>
                </p>
            </li>
        </ul>
        <div class="clear">
        </div>
        <p class="ecoButton-Below">
            <asp:Button ID="btIn_Send" runat="server" Text="Submit" CssClass="needlogin" 
                OnClick="btIn_Send_Click" OnClientClick="return checkECOInput('ecoInPaMustPut')" /></p>
    </div>
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=btEcoSearch.ClientID %>").click(function () {
            if (checkECOInput("ecoSearchInput"))
                $("form:first").attr("action", "<%=esUtilities.CommonHelper.GetStoreLocation(false) %>ECO/ECOSearch.aspx").submit();
        });
    });
</script>

