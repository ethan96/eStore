<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Products.ascx.cs" Inherits="eStore.UI.Modules.KitTheme.Products" %>

<div>
    <div id="theme-category" class="categoryv2-list eStore_block980">
        <ul class="col-height-equal AEU-container">
            <asp:Repeater ID="rpCategories" runat="server">
                <ItemTemplate>
                    <li class="float-left equal-block" style="position:relative">
                        <a class="categorytitle" href='<%#Eval("Href") %>'>
                            <div class="textcenter height130 width200">
                            <img src='<%# Eval("Image") %>' class="imgRadius" alt="<%# Eval("Description") %>"/>
                        </div>
                        </a>
                        <h3 class="eStore_productBlock_name"><%# Eval("Name") %></h3>
                        <div class="buttom">
                        <label class="regularprice"><%# Eval("Price") %></label><br />
                        <span class="price"><a class="eStore_btn select1" href='<%#Eval("Href") %>'>More Products</a></span></div>
                    <div class="clearfix"></div>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
        <div class="clearfix"></div>
    </div>
</div>
<script type="text/javascript">
    $(function () {
        equalHeightBlock();
    });
</script>