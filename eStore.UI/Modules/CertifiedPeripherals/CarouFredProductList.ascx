<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CarouFredProductList.ascx.cs"
    Inherits="eStore.UI.Modules.CertifiedPeripherals.CarouFredProductList" %>
<%@ Register Src="ProductList.ascx" TagName="ProductList" TagPrefix="uc1" %>
<div class="epaps-row780<%=ShowCompareCheckbox?" epaps-comparecontainer":""%>">
    <% if (!string.IsNullOrEmpty(Subject))
       {%>
    <div class="epaps-title-bgGray">
        <h2>
            <%=Subject%>
        </h2>
        <%if (ShowCompareCheckbox)
          { %>
        <a class="epaps-comparelink'" href="#">Compare</a>
        <%}  %>
        <div class="clearfix">
        </div>
    </div>
    <%}  %>
    <div class="epaps-productRow epaps-carousel" id="<%=ClientID %>">
        <div class="caroufredsel_wrapper">
            <uc1:ProductList ID="ProductList1" runat="server" ShowBorder="true" ShowCompareCheckbox="false" />
            <div class="carousel-control">
                <a class="epaps-arrow1" href="#"></a><span id="pager" class="pager"></span><a class="epaps-arrow2"
                    href="#"></a>
            </div>
        </div>
    </div>
    <div class="clearfix">
    </div>
</div>
