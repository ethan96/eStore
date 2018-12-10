<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
 CodeBehind="AllSolution.aspx.cs" Inherits="eStore.UI.Product.AllSolution" %>
<asp:Content ID="Content6" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/allProductcss")%>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="eStore_breadcrumb eStore_block980">
    	<a href="/"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Home)%></a>
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Show_All_Solutions)%>
    </div>
    <div class="eStore_container eStore_block980">
        <div class="eStore_allList_content Solutions">
            <h2><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Show_All_Solutions)%></h2>
            <asp:Repeater ID="rp_AllSolution" runat="server">
                <ItemTemplate>
                    <ol>
                        <li>
                            <div class="img">
                                <a href="<%# Eval("Link")%>">
                                    <img src="<%# ResolveUrl("~/resource"+Eval("BannerFileX"))%>" alt="<%# Eval("Name")%>" /></a>
                                <a class="solutiontitle" href="<%# Eval("Link")%>">
                                    <%# Eval("Name")%></a>
                            </div>
                        </li>
                    </ol>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    <script>
        $(function () {
            fixTableLayout(".eStore_allList_content", "ol");
           $(window).resize(function () {
               fixTableLayout(".eStore_allList_content", "ol");
            });
        });
    </script>
</asp:Content>