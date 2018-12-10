<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaginationProductList.ascx.cs"
    Inherits="eStore.UI.Modules.CertifiedPeripherals.PaginationProductList" %>
<%@ Register Src="ProductList.ascx" TagName="ProductList" TagPrefix="uc1" %>
<div class="epaps-productRow2 epaps-comparecontainer" id="<%=ClientID %>">
    <uc1:ProductList ID="ProductList1" runat="server" ShowBorder="false" ShowCompareCheckbox="true" />
    <div class="clearfix">
    </div>
    <div class="epaps-rowBottom epaps-rowBottomline">
        <div class="simple-pagination">
        </div>
        <%if (ShowCompareCheckbox)
          { %>
        <div class="epaps-btn pull-right">
            <a class="epaps-comparedisabled disabled">Uncheck All</a><a href="#" class="epaps-comparelink" target="_blank">Compare</a></div>
            <%} %>
    </div>
</div>
