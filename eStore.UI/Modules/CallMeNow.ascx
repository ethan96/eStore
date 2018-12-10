<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CallMeNow.ascx.cs" Inherits="eStore.UI.Modules.CallMeNow"  EnableViewState="true"%>
<%@ Register src="UVerification.ascx" tagname="UVerification" tagprefix="uc1" %>
<%@ Register Src="~/Modules/CountrySelector.ascx" TagName="CountrySelector" TagPrefix="eStore" %>
<div class="callmenow hiddenitem editorpanel">
    <img src="/images/phone_icon.gif" alt="" width="60px" height="60px" />
    <div class="headertitle">
        <asp:Literal ID="ltTalk_now" runat="server"></asp:Literal>
    </div>
    <p>
        <asp:Literal ID="ltInfor_below" runat="server"></asp:Literal></p>
    <table><eStore:CountrySelector ID="CountrySelector1" runat="server" ValidationGroup="LiveCallMeNow" /></table>
    <p>
        <label>
            <asp:Literal ID="ltFirestName" runat="server"></asp:Literal>
        </label>
        <eStore:TextBox ID="txtFirstName" runat="server"></eStore:TextBox>        
    </p>
    <p>
        <label>
            <asp:Literal ID="ltLastName" runat="server"></asp:Literal>
        </label>
        <eStore:TextBox ID="txtLastName" runat="server"></eStore:TextBox>
    </p>
    <p>
        <label>
            <asp:Literal ID="ltPhone" runat="server"></asp:Literal>
        </label>
        <eStore:TextBox ID="txtPhone" runat="server"></eStore:TextBox>
    </p>
    <p>
        <label>
            <asp:Literal ID="ltExt" runat="server"></asp:Literal>
        </label>
        <eStore:TextBox ID="txtExt" runat="server"></eStore:TextBox>
    </p>
    <p>
        <label>
            <asp:Literal ID="ltEmail" runat="server"></asp:Literal>
        </label>
        <eStore:TextBox ID="txtEmail" runat="server"></eStore:TextBox>
    </p>
    <uc1:UVerification ID="UVerification1" lableCss="editorpanelplabel" runat="server" />
    <p>
        <asp:Literal ID="ltWillKeep" runat="server"></asp:Literal>
    </p>
    <p>
        <asp:Literal ID="ltCopyRight" runat="server"></asp:Literal>
    </p>
     <asp:Button ID="btnCallmeNow" runat="server" Text="Submit" ValidationGroup="LiveCallMeNow"
            ClientIDMode="Static" OnClick="btnSubmit_Click" OnClientClick = "return validate();"/>
</div>

<script type="text/javascript" language="javascript">
    $(function () {
        $(".popcallmenow").click(function () { return popCallMeDialog(); });
    });

    function validate() {
        if (!$(".callmenow :text").validateTextBoxWithToolTip())
            return false;
        else
            return true;
    }

    function popCallMeDialog() {
        popupDialog(".callmenow");
        return false;
    }
</script>
