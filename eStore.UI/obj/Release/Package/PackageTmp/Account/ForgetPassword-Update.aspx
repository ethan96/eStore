<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="ForgetPassword-Update.aspx.cs" Inherits="eStore.UI.Account.ForgetPassword_Update" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <link href="/App_Themes/V4/byregister.css" rel="stylesheet" />
    <script src="/Scripts/jquery.validate.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="register-container"> 
  <!--personal info-->
  <div class="container">
    <h4><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Personal_Information")%></h4>
    <div class="fw-context ma-emailcontext">
      <div id="regFormNew">
        <div class="formRow">
          <label><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_New_Password")%></label> <em>*</em>
            <asp:TextBox ID="tbpassword" CssClass="width250" ClientIDMode="Static" TextMode="Password" runat="server"></asp:TextBox>
        </div>
        <div class="formRow">
          <label><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Confirm_Password")%></label> <em>*</em>
            <asp:TextBox ID="tbconpassword" CssClass="width250" ClientIDMode="Static" TextMode="Password" runat="server"></asp:TextBox>
        </div>
        <div class="formRow">
            <label> </label>
            <asp:Button ID="btnSubmit" ClientIDMode="Static" CssClass="eStore_btn noneborder widthauto" runat="server" OnClick="btnSubmit_Click" />
        </div>
      </div>
    </div>
  </div>
</div>

    <script type="text/javascript">
        $("#eStoreMainForm").validate({
            rules: {
                ctl00$eStoreMainContent$tbpassword: {
                    required: true,
                    minlength: 5
                },
                ctl00$eStoreMainContent$tbconpassword: {
                    required: true,
                    minlength: 5,
                    equalTo: "#tbpassword"
                }
            }
        });
    </script>
</asp:Content>

