<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuantityDiscountRequest.ascx.cs"
    Inherits="eStore.UI.Modules.QuantityDiscountRequest" %>
<%@ Register Src="ServerTime.ascx" TagName="ServerTime" TagPrefix="uc1" %>
<%@ Register Src="~/Modules/UVerification.ascx" TagName="UVerification" TagPrefix="uc2" %>
<style>
    .title_med
    {
        font-size: 0.88em;
        font-weight: bold;
        color: #000;
    }
    .title_para
    {
        font-family: Arial , Helvetica , sans-serif;
        font-size: 12px;
        font-weight: bold;
        color: #000;
    }
    .highlight
    {
        color: #8b0000;
        font-weight: bold;
        font-size: medium;
    }
    .labels
    {
        width: 100%;
        font-size: 12px;
        padding: 2px 5px 2px 2px;
        text-align: left;
        display: inline-block;
        vertical-align: top;
        font-weight: bold;
    }
    .separaterow
    {
        height: 18px;
    }

    #txt_CLAAccount {
        width: 300px;
        margin-top:10px;
    }
    #divCLAAccount {
        text-align:center;
    }
</style>
<div>
    <div>
        <asp:Label ID="lbl_formdesc" runat="server" CssClass="highlight" Text="Please fill out the form below and press submit. We will contact you soon."></asp:Label>
    </div>
    <div>
        <table width="575" border="0" cellspacing="0" cellpadding="2">
            <tr class="separaterow">
                <td colspan="2">
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_product" runat="server" CssClass="title_para" Text="Product"></asp:Label>
                </td>
                <td>
                    <asp:Literal ID="txt_product" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_desc" runat="server" CssClass="title_para" Text="Product Description"></asp:Label>
                </td>
                <td>
                    <asp:Literal ID="txt_desc" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr class="separaterow">
                <td colspan="2">
                </td>
            </tr>
            <tr id="trquantity" runat="server">
                <td>
                    <asp:Label ID="lbl_quantity" runat="server" CssClass="highlight" Text="Quantity"> </asp:Label>
                </td>
                <td>
                    <eStore:TextBox ID="txt_quantity" runat="server"></eStore:TextBox>
                    pcs
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please input Quantity."
                        ControlToValidate="txt_quantity" ForeColor="Red" Display="Dynamic" ValidationGroup="gQuantityDiscountRequest"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Type Must Int.(1~10000)"
                        ControlToValidate="txt_quantity" Text="Type Must Int(1~10000)" ForeColor="Red"
                        ValidationExpression="\d{1,5}" Display="Dynamic" ValidationGroup="gQuantityDiscountRequest"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr id="trleadtime" runat="server">
                <td>
                    <asp:Label ID="lbl_leadtime" runat="server" CssClass="title_para" Text="Expected Lead Time"></asp:Label>
                </td>
                <td>
                    <eStore:TextBox ID="txt_leadtime" runat="server" ClientIDMode="Static"></eStore:TextBox>
                    (MM/DD/YYYY)
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="Please Format date time."
                        ControlToValidate="txt_leadtime" Text="Format" ForeColor="Red" ValidationExpression="\d{1,2}/\d{1,2}/\d{4}"
                        Display="Dynamic" ValidationGroup="gQuantityDiscountRequest"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr id="trbudget" runat="server">
                <td>
                    <asp:Label ID="lbl_budget" runat="server" CssClass="title_para" Text="Budget"></asp:Label>
                </td>
                <td>
                    <eStore:TextBox ID="txt_budget" runat="server"></eStore:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_name" runat="server" CssClass="title_para" Text="First Name"></asp:Label>
                </td>
                <td>
                    <eStore:TextBox ID="txt_firstname" runat="server"></eStore:TextBox>
                    &nbsp;<asp:Label ID="ltLastName" runat="server" Text="Last Name" CssClass="title_para"></asp:Label>
                    <eStore:TextBox ID="txt_lastname" runat="server"></eStore:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_country" runat="server" CssClass="title_para" Text="Country"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddl_country" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr  id="trZipCode" runat="server">
                <td style="width: 27%;">
                    <asp:Label ID="lblzipcode" runat="server" CssClass="title_para" Text="Zip Code(USA - 5 Digit)"></asp:Label>
                </td>
                <td>
                    <eStore:TextBox ID="txtzipcode" runat="server"></eStore:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_companyname" runat="server" CssClass="title_para" Text="Company"></asp:Label>
                </td>
                <td>
                    <eStore:TextBox ID="txt_companyname" runat="server"></eStore:TextBox>
                </td>
            </tr>
            <tr  id="trAddress" runat="server">
                <td>
                    <asp:Label ID="lbl_address" runat="server" CssClass="title_para" Text="Address"></asp:Label>
                    <span class="colorRed">*</span>
                </td>
                <td>
                    <eStore:TextBox ID="txt_address" runat="server"></eStore:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredAddress" runat="server" ErrorMessage="Please input address."
                        ControlToValidate="txt_address" ForeColor="Red" Display="Dynamic" ValidationGroup="gQuantityDiscountRequest"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_email" runat="server" CssClass="title_para" Text="eMail"></asp:Label>
                </td>
                <td>
                    <eStore:TextBox ID="txt_email" runat="server"></eStore:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_telephone" runat="server" CssClass="title_para" Text="Telephone"></asp:Label>
                </td>
                <td>
                    <eStore:TextBox ID="txt_telephone" runat="server"></eStore:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_contactvia" runat="server" CssClass="title_para" Text="You prefer we contact<br>you via"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="cb_email" runat="server" />
                    <asp:Label ID="lbl_byemail" runat="server" CssClass="title_para">eMail</asp:Label>
                    <asp:CheckBox ID="cb_phone" runat="server" />
                    <asp:Label ID="lbl_bytel" runat="server" CssClass="title_para">Telephone</asp:Label>
                    <br />
                    <asp:Label ID="lbl_whattimeArea" runat="server">
                        (<asp:Label ID="lbl_whattime" CssClass="title_para" runat="server">What time is appropriate for us to call you?</asp:Label>
                        <uc1:ServerTime ID="ServerTime1" runat="server" />
                        )
                    </asp:Label>
                </td>
            </tr>
            <tr class="separaterow">
                <td colspan="2">
                </td>
            </tr>
            <tr id="trComments" runat="server">
                <td>
                    <asp:Label ID="lbl_comments" runat="server" CssClass="title_para" Text="Comments"></asp:Label>
                </td>
                <td>
                    <eStore:TextBox ID="txt_comments" TextMode="MultiLine" Columns="40" Rows="5" runat="server"></eStore:TextBox>
                </td>
            </tr>

        </table>
        <div id="divCLAAccount" runat="server" visible="false">    
            <div>
                <asp:Label ID="lbl_CLAAccount" runat="server" CssClass="highlight" Text="Please provide your CLA account # provided by Microsoft"></asp:Label>
            </div>               
            <eStore:TextBox ID="txt_CLAAccount" runat="server"></eStore:TextBox>
        </div>
        <asp:HiddenField ID="hSProductID" runat="server" />
        <uc2:UVerification ID="UVerification1" lableCss="editorpanelplabel" runat="server" />
        <div class="float-right  row10">
            <asp:Button ID="btn_Submit" runat="server" CssClass="QuantityDiscountRequest eStore_btn borderBlue" Text="Submit"
                OnClick="btn_Submit_Click" ValidationGroup="gQuantityDiscountRequest" />
            <asp:ValidationSummary ID="vts_QuantityDiscountRequest" runat="server" ShowMessageBox="True"
                ShowSummary="False" ValidationGroup="gQuantityDiscountRequest" />
        </div>
    </div>
</div>

<script type="text/javascript" language="javascript">
    $(function () {
        $("#txt_leadtime").datepicker({
            onSelect: function () { $(".ui-datepicker a").removeAttr("href"); }
        });
    });
</script>
