<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SAPContatDetailTip.ascx.cs" Inherits="eStore.UI.Modules.SAPContatDetailTip" %>
<h4><%=vSAPCompany.CompanyName%></h4>
<p><label>CompanyID: </label><%=vSAPCompany.CompanyID%></p>
<p><label>Address: </label><%=vSAPCompany.Address%></p>
<p><label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
 </label><%=vSAPCompany.City%>,<%=vSAPCompany.stateX%> <%=vSAPCompany.ZipCode%>, <%=vSAPCompany.Country%> </p>