<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ECOPartnerList.ascx.cs" Inherits="eStore.UI.Modules.ECOPartnerList" %>
<%@ Register Src="../CountrySelector.ascx" TagName="SelectCountry" TagPrefix="uc1" %>
<%@ Register src="../ServerTime.ascx" tagname="ServerTime" tagprefix="uc2" %>


    <div id="ecoParterList">
        <asp:Panel ID="ecoSearchHere" runat="server">
        <div id="ecoParterSearchResult">
            Search Results for:
            <asp:Label ID="liSpecialty" runat="server" Text="" CssClass="fontbold"></asp:Label>
            in
            <asp:Label ID="ltCountry" runat="server" Text="" CssClass="fontbold"></asp:Label>
            <asp:Label ID="ltIndustry" runat="server" Text="" CssClass="fontbold"></asp:Label>
            (
            <asp:Label ID="ltCount" runat="server" Text="0" CssClass="fontbold"></asp:Label>
            results)
        </div>
        </asp:Panel>
        <div class="ProductListTopPager">
            <eStore:CollectionPager ID="CollectionPager1" runat="server" PageSize="10" PagingMode="PostBack"
                EnableViewState="false" ResultsLocation="None">
            </eStore:CollectionPager>
        </div>
        <div>
        </div>
        <asp:Repeater ID="rpECOSearch" runat="server">
            <ItemTemplate>
                <div class="ecoParter-List">
                    <div id="ecoParter-logo">
                        <img src='<%#setLogoUrl(Eval("ImageUrl")) %>' height="100" alt='<%#Eval("CompanyName")%>' /></div>
                    <ul>
                        <li><span class="ecoMtitle">Company:</span>
                            <p class="ecoMDes">
                                <%#Eval("CompanyName")%></p>
                        </li>
                        <li><span class="ecoMtitle">Industries:</span>
                            <p class="ecoMDes">
                                <%#Eval("IndustryX") %></p>
                        </li>
                        <li><span class="ecoMtitle">Website:</span>
                            <p class="ecoMDes">
                                <a href='<%#Eval("WebSiteUrl") %>' target="_blank"><%#Eval("WebSiteUrl") %></a></p>
                        </li>
                        <li><span class="ecoMtitle">Office:</span>
                            <p class="ecoMDes">
                               <%#Eval("CountryName")%> <%#Eval("State")%> <%#Eval("Address") %></p>
                        </li>
                        <li><span class="ecoMtitle">About:</span>
                            <p class="ecoMDes">
                                <%#Eval("Description") %></p>
                        </li>
                    </ul>
                    <div class="clearfix">
                    </div>
                    <p id="ecoButton-Below" class="ecoButton-Below">
                        <input name="" type="button" class="sendRequestECOEmail" value="Request Assistance"
                            objfr="{PartnerId:'<%#Eval("PartnerId") %>'}" />
                        <input name="" type="button" class="sendECOFrindEmail" value="Recommend to a Friend"
                            objfr="{Company:'<%#Eval("CompanyName") %>',PartnerId:'<%#Eval("PartnerId") %>'}" />
                    </p>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="ProductListBottomPager">
            <asp:Literal ID="bottomPager" runat="server"></asp:Literal>
        </div>
        <div>
        </div>
    </div>

    <div id="ecoRequestAssistance" class="hiddenitem">
        <ul>
            <li><span class="ecoInPartner-title fontbold">Project Name:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtRa_Project" runat="server" CssClass="ecoPaPaMustPut"></asp:TextBox><span
                        class="colorRed">*</span></p>
            </li>
            <li><span class="ecoInPartner-title fontbold">First Name:</span>
                <p class="ecoInPartner-twocolumn">
                    <asp:TextBox ID="txtRa_FirstName" runat="server" CssClass="ecoPaPaMustPut"></asp:TextBox><span
                        class="colorRed">*</span></p>
                <span class="ecoInPartner-title100 fontbold">Last Name:</span>
                <p class="ecoInPartner-twocolumn">
                    <asp:TextBox ID="txtRa_LastName" runat="server" CssClass="ecoPaPaMustPut" Width="100px"></asp:TextBox><span
                        class="colorRed">*</span></p>
            </li>
            <li><span class="ecoInPartner-title fontbold">Product Interest:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtRa_InterestPro" runat="server" Width="350px" CssClass="ecoPaPaMustPut"></asp:TextBox><span
                        class="colorRed">*</span></p>
            </li>
            <li><span class="ecoInPartner-title fontbold">Budget:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtRa_Budget" runat="server"></asp:TextBox></p>
            </li>
            <li>
                <uc1:SelectCountry ID="SelectCountry1" runat="server" CountryCss="ecoPaPaMustPut" ddlWidth="200" />
            </li>
            <li><span class="ecoInPartner-title fontbold">Address:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtRa_Address" runat="server" Width="350px"></asp:TextBox></p>
            </li>
            <li><span class="ecoInPartner-title fontbold">Zip:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtRa_Zip" runat="server"></asp:TextBox></p>
            </li>
            <li><span class="ecoInPartner-title fontbold mustEmail">E-mail:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtRa_Email" runat="server" Width="350px" CssClass="ecoPaPaMustPut mustEmail"></asp:TextBox><span
                        class="colorRed">*</span></p>
            </li>
            <li><span class="ecoInPartner-title fontbold">Tel:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtRa_Tel" runat="server" Width="350px" CssClass="ecoPaPaMustPut"></asp:TextBox></p>
            </li>
            <li><span class="ecoInPartner-titleWith200 fontbold">Best time to Contact:</span>
                <p class="ecoInPartner-onecolumn">
                    <uc2:ServerTime ID="ServerTime2" TimeId="ceoRA" runat="server" />
                </p>
            </li>
            <li><span class="ecoInPartner-title fontbold">Project Details:</span>
                <p class="ecoInPartner-onecolumn">
                    <asp:TextBox ID="txtRa_ProjectDetail" runat="server" Height="105px" TextMode="MultiLine"
                        CssClass="ecoPaPaMustPut" Width="350px"></asp:TextBox>
                </p>
            </li>
        </ul>
        <div>
        </div>
        <p class="ecoButton-Below">
            <asp:Button ID="btRa_Send" runat="server" Text="Send" CssClass="needlogin" OnClick="btRa_Send_Click"
                OnClientClick="return checkECOInput('ecoPaPaMustPut')" /></p>
    </div>
    
    <div id="ecoEmailFrinds" class="hiddenitem">
        <p>
            <span class="ecoEmailFrinds-title">Company Name:</span><asp:TextBox ID="txtFr_Company"
                runat="server" ClientIDMode="Static" CssClass="ecoFrPaMustPut"></asp:TextBox><span class="colorRed">*</span></p>
        <p>
            <span class="ecoEmailFrinds-title">Your Name:</span><asp:TextBox ID="txtFr_YourName"
                runat="server" CssClass="ecoFrPaMustPut"></asp:TextBox><span class="colorRed">*</span></p>
        <p>
            <span class="ecoEmailFrinds-title">Your E-mail:</span><asp:TextBox ID="txtFr_YourEmail"
                runat="server" Width="225px" CssClass="ecoFrPaMustPut mustEmail"></asp:TextBox><span class="colorRed">*</span></p>
        <p>
            <span class="ecoEmailFrinds-title" style="width:100%">Your Friend’s E-mail:</span><asp:TextBox ID="txtFr_FrindsEmail"
                runat="server" Width="225px" CssClass="ecoFrPaMustPut mustEmail"></asp:TextBox><span class="colorRed">*</span></p>
        <p class="ecoButton-Below">
            <asp:Button ID="btFr_Send" runat="server" Text="Send" CssClass="needlogin" OnClick="btFr_Send_Click" OnClientClick="return checkECOInput('ecoFrPaMustPut')" />
        </p>
    </div>
    <asp:HiddenField ID="hfFr_PartnerId" runat="server" ClientIDMode="Static" />