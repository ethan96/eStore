<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductSpec.ascx.cs"
    EnableViewState="false" Inherits="eStore.UI.Modules.ProductSpec" %>
<div>
    <div id="selectedfilter" class="ui-widget-content ui-helper-reset">
        <ul>
            <%=string.IsNullOrEmpty(Request["keyword"]) ? "" : "<li id='f_keyword'><span class=\"ui-icon ui-icon-close\"></span><label>Keyword-</Label>" + Request["keyword"] + "</>"%>
            <asp:Repeater ID="rpseletedfilter" runat="server">
                <ItemTemplate>
                    <li id="<%#"f_" + Eval("key") %>"><span class="ui-icon ui-icon-close"></span>
                        <%# Eval("display") %>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
        <asp:Button ID="btnFilter" runat="server" OnClick="btnFilter_Click" />
    </div>
    <div id="specfilter">
        <h3>
            <a href="#">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Keyword)%></a></h3>
        <div>
            <input type="text" id="speckeyword" name="speckeyword" value='<%=Request["speckeyword"]%>' />
        </div>
        <eStore:Repeater ID="rpSpecFilter" runat="server" OnItemDataBound="rpSpecFilter_ItemDataBound"
            EnableViewState="false">
            <ItemTemplate>
                <h3>
                    <a href="#">
                        <%# Eval("display") %></a></h3>
                <eStore:Repeater ID="rpAttributes" runat="server" OnItemDataBound="rpAttributes_ItemDataBound">
                    <HeaderTemplate>
                        <div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <span id="cat_<%# Eval("key") %>">
                            <%# Eval("display") %></span>
                        <eStore:Repeater ID="rpAttributeValues" runat="server">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="attributevalue">
                                    <input type="checkbox" value="<%# Eval("key") %>" name="specfilter" <%# Eval("ischecked") %> /><%# Eval("display") %>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </eStore:Repeater>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </eStore:Repeater>
            </ItemTemplate>
        </eStore:Repeater>
    </div>
</div>
<script type="text/ecmascript" language="javascript">
    $(function () {
        $("#specfilter").accordion({
            autoHeight: false,
            navigation: true,
            collapsible: true,
            active: 0
        });
        $("img[src='']").attr("src", "/images/photounavailable.gif");
    });
    $("#specfilter :checkbox").click(function () {
        if (this.checked) {

            category = $("#cat_" + this.value.substring(0, this.value.lastIndexOf("_") - 2)).html();

            $("<li></li>").text(this.nextSibling.data.replace(/(.*?)\(\d+\)/g, "$1")).attr("id", "f_" + this.value)
            .prepend($("<span></span>").attr("class", "ui-icon ui-icon-close"))
            .prepend($("<label></label>").text(category + " :"))
            .appendTo($("#selectedfilter ul"));
        }
        else {
            $("#selectedfilter ul").find("#f_" + this.value).remove();
        }

    });
    $("#speckeyword").keyup(function () {

        if (this.value == "") {
            $("#selectedfilter ul").find("#f_keyword").remove();
        }
        else {
            if ($("#selectedfilter ul").find("#f_keyword").length == 0)
                $("<li></li>").text(this.value).attr("id", "f_keyword")
                    .prepend($("<span></span>").attr("class", "ui-icon ui-icon-close"))
                    .prepend($("<label></label>").text("Keyword-"))
                    .appendTo($("#selectedfilter ul"));
            else {
                $("#selectedfilter ul").find("#f_keyword").text(this.value)
                    .prepend($("<span></span>").attr("class", "ui-icon ui-icon-close"))
                    .prepend($("<label></label>").text("Keyword : "));
            }
        }

    });
    $("#selectedfilter li").click(function () {

        if (this.id == "f_keyword") {
            $("#speckeyword").val("");
        }
        else {
            $("#specfilter").find(":checkbox[value='" + this.id.substring(2) + "']").attr("checked", "");
        }
        $("#selectedfilter ul").find("#" + this.id).remove();
    });
    $("#selectedfilter li").live("click", function () {

        if (this.id == "f_keyword") {
            $("#speckeyword").val("");
        }
        else {
            $("#specfilter").find(":checkbox[value='" + this.id.substring(2) + "']").attr("checked", "");
        }
        $("#selectedfilter ul").find("#" + this.id).remove();
    });
</script>
