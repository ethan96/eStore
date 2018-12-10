<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="applicationsintabs.aspx.cs" Inherits="eStore.UI.Product.applicationsintabs" %>

<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div id="rightprod" style="width: 780px;">
        <%=sbTabs.ToString()%>
        <asp:Image runat="server" ID="imgApp" />
        <div class="title">
            <asp:Literal ID="lTitle" runat="server"></asp:Literal>
        </div>
        <eStore:Repeater ID="rpCategories" runat="server" OnItemDataBound="rpCategories_ItemDataBound">
            <ItemTemplate>
                <div class="prod">
                    <div class="img">
                        <%# (Eval("ProductCategory") == null || Eval("ProductCategory.ImageURL") == null || string.IsNullOrEmpty(Eval("ProductCategory.ImageURL").ToString()))
?string.Empty
                                : string.Format("<img src=\"{0}\"  alt=\"{1}\" width=\"163px\" height=\"97px\" />"

                            , eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString() + Eval("ProductCategory.ImageURL")
                            , Eval("ProductCategory.LocalCategoryName"))%>
                    </div>
                    <h1 class="subtitle">
                        <%# Eval("ProductCategory.LocalCategoryName")%></h1>
                    <div class="text">
                        <eStore:Repeater ID="rpModels" runat="server">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li><a href="<%# Eval("URL")%>">
                                    <%# Eval("ModelNO")%></a></li>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </eStore:Repeater>
                    </div>
                </div>
            </ItemTemplate>
        </eStore:Repeater>
    </div>
  
  <script>
      $(document).ready(function () {
          initTabs($("#categoriestabs ul:first"));
      });
      function initTabs(c) {
          if ($(c).data("init") == undefined) {
              var tabstotalwidth = 0;
              var padding = 0;
              var tabscount = 0;
              $(c).find("li").each(function (j, n) {
                  tabscount++;
                  tabstotalwidth += $(n).outerWidth();
              });
              padding = (780 - tabstotalwidth - 4 * tabscount) / tabscount;
              $(c).find("li").each(function (j, n) {
                  $(n).css("width", ($(n).outerWidth() + padding) + "px");
              });
              $(c).data("init", true);
          }
      }
  </script>
                          
</asp:Content>
