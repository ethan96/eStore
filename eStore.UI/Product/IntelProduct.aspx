<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true" CodeBehind="IntelProduct.aspx.cs" Inherits="eStore.UI.Product.IntelProduct" %>
<%@ Register Src="~/Modules/eStoreLiquidSlider.ascx" TagName="eStoreLiquidSlider" TagPrefix="eStore" %>
<%@ Register src="~/Modules/ProductMatrix.ascx" tagname="ProductMatrix" tagprefix="eStore" %>
<asp:Content ID="Content1" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <asp:PlaceHolder ID="phRightSide" runat="server"></asp:PlaceHolder>
    <div id="storeSideAds"></div>
    <br />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:YouAreHere ID="YouAreHere1" runat="server" />
    <eStore:eStoreLiquidSlider ID="eStoreLiquidSlider1" runat="server" navigationType="Numbers"
    showDescription="false" MinHeight="120" />
    <asp:Panel ID="pCategoryHeader" runat="server">
        <h1>
            <asp:Literal ID="lCategoryName" runat="server" EnableViewState="false">
                Intel Based Product
            </asp:Literal>
        </h1>
<%--        <div class="CategoryDescription">
            <asp:Literal ID="lCategoryDescription" runat="server" EnableViewState="false">
                Thank you for interest in our products. You can find the product quickly by using product selection below. 
            </asp:Literal>
        </div>
--%>    </asp:Panel>
    <div class="clear">
    </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="intelTable" cellspacing="0" cellpadding="0">
                <tr>
                    <td width="150px">Select Product Type</td>
                    <td>
                        <asp:RadioButtonList ID="rblIntel" runat="server" RepeatDirection="Horizontal" CssClass="rblIntel"
                                        AutoPostBack="true"  OnSelectedIndexChanged="rblIntel_SelectedIndexChanged" >
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td>Select Category</td>
                    <td>
                        <asp:DropDownList ID="ddlIntel1" runat="server" AutoPostBack="true" onselectedindexchanged="ddlIntel1_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlIntel2" runat="server" AutoPostBack="true" onselectedindexchanged="ddlIntel2_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlIntel3" runat="server" AutoPostBack="true" onselectedindexchanged="ddlIntel3_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlIntel4" runat="server" AutoPostBack="true" onselectedindexchanged="ddlIntel4_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlIntel5" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr class="lastTr">
                    <td class="firstTd">&nbsp;</td>
                    <td><input type="button" value="Go" id="btSearchSubmit" onclick="searchCategoryMatrix()" style="" /></td>
                </tr>
            </table>

            <div class="clear"></div>

            <asp:UpdateProgress ID="searchProgress" runat="server">
                <ProgressTemplate>
                    <div style=" text-align:center; height:25px; line-height:25px; margin: 10px 0;" >
                        <asp:Image ID="imgLoad" runat="server" ImageUrl="~/App_Themes/Default/loader.gif" />
                        Working on your request...
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="clear"></div>
    <asp:Button ID="btSearch" runat="server" Text="Go" onclick="btSearchClick" class="hiddenitem"/>
    <eStore:ProductMatrix ID="ProductMatrix1" runat="server" Visible="false" />
    <eStore:ProductList ID="ProductList1" runat="server" Visible="false" />
    <eStore:ProductCompare ID="ProductCompare1" runat="server" showPrintButton="false" 
        showRemoveButton="false" showHeaderButtons="true" IsLoad="true" PagingModeType="PostBack" />

    <script>
        function searchCategoryMatrix() {
            $("#<%=searchProgress.ClientID %>").show();
            $("#<%=btSearch.ClientID %>").focus();
            $("#<%=btSearch.ClientID %>").trigger("click");
        }


        $(document).ready(function () {
            $("#<%=searchProgress.ClientID %>").hide();
        });
    </script>
    
</asp:Content>

