<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactUS.ascx.cs" Inherits="eStore.UI.Modules.ContactUS" %>
<%@ Register Src="~/Modules/T_CustomerProfile.ascx" TagName="T_CustomerProfile" TagPrefix="eStore" %>
<%@ Register Src="~/Modules/UVerification.ascx" TagName="UVerification" TagPrefix="uc1" %>
<div class="eStore_rightBlock float-left">
    <h2>
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Contact_Us)%></h2>
    <ol class="eStore_contactUs_address  eStore_contactUs">
        <asp:PlaceHolder ID="phContactInformationWidget" runat="server"></asp:PlaceHolder>
    </ol>
    <ol class="eStore_contactUs_form  eStore_contactUs">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Submit_Inquiry_Form)%>
        <div class="eStore_contactUs_input inquiryType">
            <span class="title"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Inquiry_type)%><span class="eStore_redStar">*</span>：</span>
            <select class="styled" name="inquiryType">
                <option value="" selected="selected"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select)%></option>
                <option value="generalInquiries"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_General_Inquiries)%></option>
                <option value="technicalSupport"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Technical_Support)%></option>
                <option value="sales"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Sales)%></option>
            </select>
        </div>
        <div class="eStore_inputBlock hide">
            <eStore:T_CustomerProfile ID="T_CustomerProfile" runat="server" />
            <div class="eStore_contactUs_input softwareVersion hide">
                <label class="title">
                    <asp:Label ID="lbl_SoftwareVersion" runat="server"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Software_Product_Name_Version)%></asp:Label>:
                </label>
                <input type="text" id="inSoftwareVersion" name="inSoftwareVersion" />
            </div>
            <div class="eStore_contactUs_input productCategory hide">
                <label class="title">
                    <asp:Label ID="lbl_ProductCategory" runat="server"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Product_Category)%></asp:Label>:
                </label>
                <select id="inProductCategory" name="inProductCategory">
                    <option selected="selected">--
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_select_a_category)%>
                        -- </option>
                    <option>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Single_Board_Computers)%></option>
                    <option>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Embedded_Computers)%></option>
                    <option>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Industrial_Computers)%></option>
                    <option>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Panel_Computers)%></option>
                    <option>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Panel_Solutions)%></option>
                    <option>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Embedded_Software)%></option>
                </select>
            </div>
            <div class="eStore_contactUs_input productModelNO hide">
                <label class="title">
                    <asp:Label ID="lbl_ProductModelNO" runat="server"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Product_Model_Number)%></asp:Label>:
                </label>
                <input type="text" id="inModelNo" name="inModelNo" />
            </div>
            <div class="eStore_contactUs_input purchaseDate hide">
                <label class="title">
                    <asp:Label ID="lbl_PurchaseDate" runat="server"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Date_of_Purchase)%></asp:Label>:
                </label>
                <input type="text" id="inPurchaseDate" name="inPurchaseDate" />
            </div>
            <div class="eStore_contactUs_input productInterest hide">
                <label class="title">
                    <asp:Label ID="lbl_ProductInterest" runat="server"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Product_Interest)%></asp:Label>:
                </label>
                <select id="inProductInterest" name="inProductInterest">
                    <option selected="selected">--
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_select_a_category)%>
                        -- </option>
                    <option>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Single_Board_Computers)%></option>
                    <option>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Embedded_Computers)%></option>
                    <option>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Industrial_Computers)%></option>
                    <option>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Panel_Computers)%></option>
                    <option>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Panel_Solutions)%></option>
                    <option>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Embedded_Software)%></option>
                </select>
            </div>
            <div class="eStore_contactUs_input pick hide">
                <label class="title">
                    <asp:Label ID="lbl_Pick" runat="server"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Pick_the_MOST_appropriate)%></asp:Label>:
                </label>
                <input type="text" id="inPick" name="inPick" />
            </div>
            <div class="eStore_contactUs_input comment">
                <label class="title">
                    <asp:Label ID="lbl_Comment" runat="server"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Comments)%></asp:Label>
                    <span class="eStore_redStar">*</span>: &nbsp;
                </label>
                <asp:TextBox ID="inComment" ClientIDMode="Static" runat="server" Height="107px" TextMode="MultiLine"
                    Width="325px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Display="None" runat="server"
                    ErrorMessage="Comment cannot be empty." ValidationGroup="CustomerProfile" ControlToValidate="inComment"></asp:RequiredFieldValidator>
            </div>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Government_Message)%>
            <uc1:UVerification ID="UVerification1" lableCss="editorpanelplabel" runat="server" />
            <div class="eStore_contactUs_btnBlock">
                <asp:Button ID="btn_Submit" runat="server" Text="Submit" ValidationGroup="CustomerProfile"
                    OnClick="btn_Submit_Click" CssClass="eStore_btn eStore_btn_Submit" BorderColor="#4398ef" />
                <input type="button" runat="server" id="btn_Reset" value="Reset" class="eStore_btn borderBlue eStore_btn_Reset" />
            </div>
        </div>
    </ol>
</div>
