<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryWithSubCategoryAndProductsV2.ascx.cs"
    Inherits="eStore.UI.Modules.CategoryWithSubCategoryAndProductsV2" %>
<div class="iPlanet-tabox">
    <div class="iPlanet-tabox2">
        <div class="iPlanet-tabox3">
            <div class="iPlanet-box">
                <eStore:Repeater ID="rpTabNav" runat="server">
                    <HeaderTemplate>
                        <div class="iPlanet-boxTop">
                            <ul class="iPlanet-tabNav">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li><a href='<%#(Eval("CustomURL")==null || string.IsNullOrEmpty(Eval("CustomURL").ToString()))? "#iPlanet-tab"+Container.ItemIndex:Eval("CustomURL").ToString()%>'  title="<%# Eval("LocalCategoryName")%>">
                            <%# Eval("LocalCategoryName")%></a></li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul> </div>
                    </FooterTemplate>
                </eStore:Repeater>
                <eStore:Repeater ID="rpCategories" runat="server">
                    <HeaderTemplate>
                        <div class="iPlanet-boxContent">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div id="iPlanet-tab<%# Container.ItemIndex %>" class="iPlanet-tabContent">


                            
                            <%# mappCategroyTable(Eval("childCategoriesX")) %>


                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </eStore:Repeater>
            </div>
            <!-- iPlanet-box : end -->
        </div>
    </div>
</div>





<script type="text/javascript" language="javascript">

    $(function () {
        var _showTab = 0;
        $('.iPlanet-tabox').each(function () {
            // 目前的頁籤區塊
            var $tab = $(this);
            $('ul.iPlanet-tabNav li', $tab).eq(_showTab).addClass('active');
            $('.iPlanet-tabContent', $tab).hide().eq(_showTab).fadeIn(500);
            $('ul.iPlanet-tabNav li', $tab).click(function () {
                var $this = $(this),
			_clickTab = $this.find('a').attr('href');  // 找出 li 中的超連結 href(#id)
                $this.addClass('active').siblings('.active').removeClass('active');
                $(_clickTab).stop(false, true).fadeIn().siblings().hide();
                if (_clickTab.indexOf("#") == 0)
                    return false;
                else
                    return true;
            }).find('a').focus(function () {
                this.blur();
            });
        });

        $.each($(".iPlanet-price span"), function () {
            var pricepanel = $(this);
            $.getJSON(GetStoreLocation() + "proc/do.aspx",
             { func: "<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.LowestPrice %>"
             , id: $(pricepanel).attr("id")
             },
             function (data) {
                 $(pricepanel).html(data.LowestPrice);
             });
        });
    });

</script>
