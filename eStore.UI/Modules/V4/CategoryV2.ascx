<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryV2.ascx.cs" Inherits="eStore.UI.Modules.V4.CategoryV2" %>

<%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/category")%>

<div class="eStore_wrapper">
    <div id="advxx" class="categoryv2-list eStore_block980">
        <ul>
            <asp:Repeater ID="rpCategories" runat="server">
                <ItemTemplate>
                    <li class="float-left">
                        <h3 class="eStore_productBlock_name"><%# Eval("Name") %></h3>
                        <p class="eStore_productBlock_att"><%# Eval("Description") %></p>
                        <a href='<%#Eval("Href") %>'>
                            <img src='<%# Eval("Image") %>' style="max-width: 200px" alt="<%# Eval("Description") %>"/></a><br />
                        <label class="regularprice float-right"><%# Eval("Price") %></label><br />
                        <span class="price float-right"><a class="eStore_btn select1" href='<%#Eval("Href") %>'>View Complete Selection now</a></span>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
        <div class="clearfix"></div>
    </div>
</div>
<script type="text/javascript">
    $(function () {
        fixTableLayout("#advxx", ".float-left");
        $("#advxx .eStore_productBlock_att").equalHeight();
        $("#advxx .eStore_productBlock_att").next('a').equalHeight()
    });
</script>