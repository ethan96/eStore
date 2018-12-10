<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangeRegion.ascx.cs" EnableViewState="false"
  Inherits="eStore.UI.Modules.ChangeRegion" %>
 
<asp:Image ID="imgCountry" runat="server"  Height="18" CssClass="showCountryRegion mOut" />
<asp:Label ID="lblChangeCountryRegion" runat="server" CssClass="showCountryRegion mOut" ForeColor="Red" Font-Bold="true">&nbsp;</asp:Label>
<input type="hidden" id="hdSearchCountry" />
<asp:Button ID="btnChangeRegion" runat="server" CommandArgument="AUS"   Visible="false"
    Text="CHANGE" onclick="btnChangeRegion_Click" />
<script type="text/javascript">
    $(function () {
        $(".showCountryRegion").click(function () {
            clearTimeout(closeCountryTimeout);
            $("body").bind("click", countryMoustOut);
            $("body").bind("keydown", searchCountry);
            if ($("#changeCountryRegion").css("display") == "none") {
                loadCountryRegion();
                $("#changeCountryRegion").show(0);
            }
            else {
                $("#changeCountryRegion").hide();
                $("body").unbind("click", countryMoustOut);
                $("body").unbind("keydown", searchCountry);
            }
        });
        $("#changeCountryRegion").hover(function () {
            clearTimeout(closeCountryTimeout);
        }, function () {
            closeCountryRegion();
        });
    });

    var closeCountryTimeout;
    var searchCountryTimeout; //clear searchCountry value
    var countryCssTimeout;
    jQuery.expr[':'].OverStartWith = function (a, i, m) {
        if ((a.textContent || a.innerText || "").length < m[3].length)
            return false;
        return (a.textContent || a.innerText || "").toUpperCase().substr(0, m[3].length).toUpperCase() == m[3].toUpperCase();
    };
    function countryMoustOut(e) {
        e = e || window.event;
        var currentObjActive = e.srcElement ? e.srcElement : e.target;
        if (!$(currentObjActive).hasClass("mOut")) {
            $("body").unbind("click", countryMoustOut);
            $("body").unbind("keydown", searchCountry);
            $("#changeCountryRegion").hide(0);
            clearTimeout(closeCountryTimeout);
        }
    }
    function searchCountry(e) {
        e = e || window.event;
        var keyCountry = e.keyCode || e.which || e.charCode;
        if (keyCountry >= 65 && keyCountry <= 90) {
            keyCountry = String.fromCharCode(keyCountry);
            $("#hdSearchCountry").val($("#hdSearchCountry").val() + keyCountry);
            clearInterval(searchCountryTimeout);
            countryFocus();
            searchCountryTimeout = setInterval(function () { 
                clearInterval(searchCountryTimeout);
                $("#hdSearchCountry").val(""); //clear 
            }, 1200);
        }
    }
    function countryFocus() {
        var filterCountry = $("#hdSearchCountry").val();
        var filterList = $("#countryRegionUl .dir").find(':OverStartWith(' + filterCountry + ')'); //.parent();
        clearTimeout(countryCssTimeout);
        if (filterList.length > 0) {
            var scrollTop = $("#changeCountryRegionRight").scrollTop();
            var pos = scrollTop + $(filterList[0]).position().top - 120;
            $("#changeCountryRegionRight").animate({ scrollTop: pos }, 800);
            
            countryCssTimeout = setTimeout(function () {
                clearTimeout(countryCssTimeout);
                $(filterList[0]).css("background-color", "#ffffff");
                $(filterList[0]).css("color", "black");
                $(filterList[0]).parent().css("background-color", "#ffffff");

                //clear country color
                setTimeout(function () {
                    $(filterList[0]).css("background-color", "");
                    $(filterList[0]).css("color", "");
                    $(filterList[0]).parent().css("background-color", "");
                }, 2000);
            }, 500);
        }
    }

    function loadCountryRegion() {
        var countryRegionLiCount = $("#countryRegionUl li").length;
        if (countryRegionLiCount == 0) {
            eStore.UI.eStoreScripts.getAllCountries(
                function (result) {
                    if (result && result.length>0) {
                        for (var i = 0; i < result.length; i++) {
                            $("#countryRegionUl").append("<li class='dir mOut'><span class='mOut' onclick='changeCountryRegion(this)'>" + result[i].countryName + "</span></li>");
                        }
                    }
                }
            );
        }
    }
    function changeCountryRegion(th) {

        __doPostBack('<%= btnChangeRegion.UniqueID %>',$(th).text());
    }
    function closeCountryRegion() {
        closeCountryTimeout = setTimeout(function () {
            $("#changeCountryRegion").hide(0);
            $("body").unbind("click", countryMoustOut);
            $("body").unbind("keydown", searchCountry);
        },500);
    }
</script>