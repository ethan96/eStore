<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="applications.aspx.cs" Inherits="eStore.UI.Product.applications" %>

<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:YouAreHere ID="YouAreHere1" runat="server" />
    <div class="clear"></div>
    <div id="applicationcontainer">
        <div id='demoarea'>
        </div>
        <div id='categoriestabs'>
        </div>
        <div id='products'>
        </div>
        <div class="clearfix"></div>
    </div>
    <script language="Javascript" type="text/javascript">
        $(document).ready(function () {
            $.renderApplication($("#applicationcontainer"), '<%= Request["category"] %>');
        });
    </script>
</asp:Content>
