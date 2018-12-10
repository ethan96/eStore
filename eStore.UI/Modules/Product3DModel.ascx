<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Product3DModel.ascx.cs" Inherits="eStore.UI.Modules.Product3DModel" %>
<div class="Product3DModelDiv">
    <div class="Product3DModelHeadr clear"></div>
    <div>
        <table width="100%">
        <tr>
            <td>
                <h3 class="productname">
                    <asp:Literal ID="lProductName" Text="" runat="server" />
                </h3>
                <span class="colorRed"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Please_right_click_the_link)%></span>            
            </td>
        </tr>
        <tr>
            <td>
                <ul class="Product3DModelFeature">
                    <asp:Literal ID="lProductFeature" runat="server"></asp:Literal>
                </ul>
            </td>
        </tr>
        <tr>
            <td><br />
                <span class="colorBlue"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Compatible_software_includes)%> <br />
                &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_SolidWorks__Pro_E_Solid_Edge)%>
                </span><br />
                <ul class="Product3DModelFeatureA">
                    <li><a target="_blank" href="http://www.edrawingsviewer.com/pages/programs/download/index.html"><b>> 
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Download_eDrawings_Viewer)%></b></a></li>
                </ul>
            </td>
        </tr>
        </table>
    </div>
    <div class="Product3DModelHeadr clear"></div>
</div>