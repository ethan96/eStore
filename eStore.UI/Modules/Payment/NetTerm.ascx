<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NetTerm.ascx.cs" Inherits="eStore.UI.Modules.Payment.NetTerm" %>
<%@ Register src="PONo.ascx" tagname="PONo" tagprefix="uc1" %>
<div id="termcontent" class="paymentcontent hiddenitem">
    <div class="paymentheader">
    <span>
        Net Term Application File Attachment</span>
        <div class="paymentnote">
            <a id="NetTermEstablished" class="handCss">Net Term arrangement already established </a>
        </div>
    </div>
    <div class="paymentbody">
        <p id="termcontentTxtPONoSpan">
            
        </p>
        <p>
            Click <a href="/resource/AUS/Credit_Application_Forms.pdf" target="_blank" class="colorBlue">
                here</a> to download a Net Term application form.<br />
            You may upload your completed form as a PDF below.
        </p>
        <p>
            Note: If this is a rush order, it is advised that you use a Credit Card. It can
            take 7 to 10 working days before Net Term application approval.
        </p>
        <p>
            Upload your Net Term Application:</p>
        <p>
            <asp:FileUpload ID="fuploadterm" runat="server" />
            <asp:Button ID="tbnuploadterm" runat="server" Text="Upload" OnClick="tbnuploadterm_Click" />
        </p>
        <p id="pdownloadterm" runat="server" visible="false">
            click
            <asp:HyperLink ID="ldownloadterm" runat="server" CssClass="colorBlue"  target="_blank">here</asp:HyperLink>
            to review your Net Term Application.
        </p>
        <p>
            If faxing your application form,<br />
            please fax to 408-519-3899.
        </p>
    </div>
</div>
<div id="pocontent" class="paymentcontent hiddenitem">
    <div class="paymentheader">
        P.O. File Attachment
        <div class="paymentnote">
            <a id="UploadTermAttachment" class="handCss">Upload Net Term Application File Attachment
            </a>
        </div>
    </div>
    <div class="paymentbody">
        <p id="pocontentTxtPONoSpan">
            <span id="txtPONoSpan"><uc1:PONo ID="txtPONo" runat="server" mustInput="true" /></span>
        </p>
        <p>
            Upload your Purchase Order file:
        </p>
        <p>
            <asp:FileUpload ID="fuploadpo" runat="server" />
            <asp:Button ID="btnuploadpo" runat="server" Text="Upload" OnClick="btnuploadpo_Click" />
        </p>
       <p id="pdownloadpo" runat="server" visible="false">
            click
            <asp:HyperLink ID="ldownloadpo" runat="server" CssClass="colorBlue"  target="_blank">here</asp:HyperLink>
            to review your Purchase Order file.
        </p>
        <p>
            If faxing your Purchase Order, please include your Purchase Order number:
        </p>
        <p>
            Fax your Purchase Order to: 1-408-519-3888
        </p>
    </div>
</div>
<script type="text/javascript" language="javascript">
    var netTermObj = {
        canSubmit: false,
        hasTermFile: "<%=hasTermFile %>",
        hasPoFile: "<%= hasPoFile %>", // has upload po file
        customerTermHas: false,
        customerPoHas: false, // customer think has already send po file
        hasVerifyTermFile: false,
        hasVerifyPoFile: false, // page has show po tab to customer
        selectPOcontent: function (isShowMessage) {
            if (isShowMessage) {
                var mess = "";
                if (!this.hasVerifyPoFile && this.hasPoFile == "False" && !this.customerPoHas) {
                    mess = "Is your PO File already established with us?";
                }
                else if (this.hasVerifyPoFile && this.hasPoFile == "False" && !this.customerPoHas) {
                    mess = "Please upload PO File!";
                }
                if (mess != "") {
                    showConfirmMessage(mess
                    , function () {
                        netTermObj.customerPoHas = true;
                        if (netTermObj.canSubmit) {
                            $("#eStoreMainContent_btnCheckout").click();
                        }
                        else {
                            parent.$.fancybox.close();
                        }
                    }
                    , function () { netTermObj.customerPoHas = false; parent.$.fancybox.close(); });
                }
            }
            $("#termcontent").addClass("hiddenitem");
            $("#pocontent").removeClass("hiddenitem");
            if ($("#pocontentTxtPONoSpan").find("span[id='txtPONoSpan']").html() == null) {
                var _PONo = $("#eStoreMainContent_NetTerm_txtPONo_txtPoNo").val();
                $("#pocontentTxtPONoSpan").html($("#termcontentTxtPONoSpan").html());
                $("#termcontentTxtPONoSpan").html("");
                $("#eStoreMainContent_NetTerm_txtPONo_txtPoNo").val(_PONo)
            }
            $("#pocontent :text").PreFillToolTips();
            this.hasVerifyPoFile = true;
        },
        selectTermcontent: function (isShowMessage) {
            if (isShowMessage) {
                var mess = "";
                if (!this.hasVerifyTermFile && this.hasTermFile == "False" && !this.customerTermHas) {
                    mess = "Is your Net Term arrangement already established with us?";
                }
                else if (this.hasVerifyTermFile && this.hasTermFile == "False" && !this.customerTermHas) {
                    mess = "Please upload Term File!";
                }
                if (mess != "") {
                    showConfirmMessage(mess, function () { netTermObj.customerTermHas = true; parent.$.fancybox.close(); }, function () { netTermObj.customerTermHas = false; parent.$.fancybox.close(); });
                }
            }
            $("#pocontent").addClass("hiddenitem");
            $("#termcontent").removeClass("hiddenitem");
            if ($("#termcontentTxtPONoSpan").find("span[id='txtPONoSpan']").html() == null) {
                var _PONo = $("#eStoreMainContent_NetTerm_txtPONo_txtPoNo").val();
                $("#termcontentTxtPONoSpan").html($("#pocontentTxtPONoSpan").html());
                $("#pocontentTxtPONoSpan").html("");
                $("#eStoreMainContent_NetTerm_txtPONo_txtPoNo").val(_PONo)
            }
            $("#termcontent :text").PreFillToolTips();
            this.hasVerifyTermFile = true;
        }
    };

    $(function () {
        if ($("input[name='NetTermArrangement']").val() == "false") {
            if ($("input[name='IsPostBack']").val() == "false") {
                var mess = "Is your Net Term arrangement already established with us? ";
                showConfirmMessage(mess, function () { netTermObj.customerTermHas = true; parent.$.fancybox.close(); }, function () { netTermObj.customerTermHas = false; parent.$.fancybox.close(); });
            }
            else {
                netTermObj.selectPOcontent(true);
            }
        }
        else {
            if ($("input[name='IsPostBack']").val() == "false") {
                netTermObj.selectPOcontent(true);
            }
            else {
                netTermObj.selectPOcontent(false);
            }
        }
        $("#pocontent :text").PreFillToolTips();
        $("#termcontent :text").PreFillToolTips();
    });
    $("#UploadTermAttachment").click(function () {
        netTermObj.selectTermcontent(true);
    });
    $("#NetTermEstablished").click(function () {
        netTermObj.selectPOcontent(true)
    });



    function validatePO() {
        if (!netTermObj.hasVerifyPoFile && netTermObj.hasPoFile == "False" && !netTermObj.customerPoHas) {
            netTermObj.canSubmit = true;
            netTermObj.selectPOcontent(true);
            return false;
        }
        return $("#eStoreMainContent_NetTerm_txtPONo_txtPoNo").validateTextBoxWithToolTip();
    }
</script>
