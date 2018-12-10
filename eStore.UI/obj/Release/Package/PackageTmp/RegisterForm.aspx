<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumnIoT.Master"
    AutoEventWireup="true" CodeBehind="RegisterForm.aspx.cs" Inherits="eStore.UI.RegisterForm" %>

<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <script src="Scripts/registerutilities.js" type="text/javascript"></script>
    <div class="iot-uShop">
        <div class="iot-block">
            <div class="iot-breadcrumb">
                <a href="/">首页</a> &gt; 用户注册</div>
            <div class="clearfix">
            </div>
        </div>
        <!--iot-block-->
        <p style="text-align: center; font-size: 24px; color: Red;">
            <asp:Literal ID="ltMessage" runat="server"></asp:Literal></p>
        <asp:Panel ID="pnStep1" runat="server">
            <!-- Step 1  -->
            <div class="iot-block">
                <h1 class="iot-uShop-title">
                    欢迎注册成为优店联网用户！<div class="clearfix">
                    </div>
                </h1>
                <p>
                    您将收到零售行业精采案例、最新产品资讯、线上商店特殊优惠！<br />
                    立即注册，启用您的会员服务！</p>
                <ul class="iot_uShop_stepBG stepBG-1">
                    <li class="on"><span>1</span>登录帐号</li>
                    <li><span>2</span>基本信息</li>
                    <li><span>3</span>智能零售需求</li>
                    <div class="clearfix">
                    </div>
                </ul>
                <div class="iot-uShop-formBlock">
                    <div class="iot-uShop-form">
                        <table width="100%" border="0" cellspacing="1" cellpadding="0" align="center">
                            <tbody>
                                <tr>
                                    <th>
                                        会员帐号
                                    </th>
                                    <td>
                                        <eStore:TextBox ID="tbUserEmail" runat="server" ClientIDMode="Static"></eStore:TextBox><span
                                            class="iot-uShop-formAtt"><span id="spanemailstr" class="red hiddenitem"></span>请填写您的邮箱，做为商城会员登入帐号</span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        密码
                                    </th>
                                    <td>
                                        <eStore:TextBox ID="tbUserPassWord" runat="server" ClientIDMode="Static" TextMode="Password"></eStore:TextBox><span
                                            class="iot-uShop-formAtt"><span id="splengthpsd" class="hiddenitem red">密码太短了</span>
                                            請勿使用特殊符號 ^ ' ( ) | ; \.</span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        确认密码
                                    </th>
                                    <td>
                                        <eStore:TextBox ID="tbUserPassWordDou" runat="server" ClientIDMode="Static" TextMode="Password"></eStore:TextBox><span
                                            class="iot-uShop-formAtt red hiddenitem" id="spanconfpassword">密码不相同</span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                    </th>
                                    <td class="iot-proBtn">
                                        <asp:Button ID="btnStep1Next" runat="server" CssClass="btnStyle" ClientIDMode="Static"
                                            Text="下一步" OnClick="btnStep1Next_Click" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <!--iot-uShop-form-->
                </div>
                <!--iot-uShop-formBlock-->
                <div class="clearfix">
                </div>
            </div>
            <!--iot-block-->
            <!-- end Step 1  -->
        </asp:Panel>
        <asp:Panel ID="pnStep2" runat="server">
            <!-- Step 2  -->
            <div class="iot-block">
                <h1 class="iot-uShop-title">
                    欢迎注册成为优店联网用户！<div class="clearfix">
                    </div>
                    <h1>
                    </h1>
                    <p>
                        您将收到零售行业精采案例、最新产品资讯、线上商店特殊优惠！<br />
                        立即注册，启用您的会员服务！
                    </p>
                    <ul class="iot_uShop_stepBG stepBG-2">
                        <li class="on"><span>1</span>登录帐号</li>
                        <li class="on"><span>2</span>基本信息</li>
                        <li><span>3</span>智能零售需求</li>
                        <div class="clearfix">
                        </div>
                    </ul>
                    <div class="iot-uShop-formBlock">
                        <div class="text-right">
                            <span class="red">* 为非必填</span></div>
                        <div class="iot-uShop-formTitle">
                            联系人信息</div>
                        <div class="iot-uShop-form">
                            <table align="center" border="0" cellpadding="0" cellspacing="1" width="100%">
                                <tbody>
                                    <tr>
                                        <th>
                                            姓
                                        </th>
                                        <td>
                                            <eStore:TextBox ID="tbUserNa" runat="server" ClientIDMode="Static" CssClass="cannotnull2"></eStore:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            名
                                        </th>
                                        <td>
                                            <eStore:TextBox ID="tbUserMe" runat="server" ClientIDMode="Static" CssClass="cannotnull2"></eStore:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            所在部门
                                        </th>
                                        <td>
                                            <asp:DropDownList ID="ddlBumen" runat="server" ClientIDMode="Static" CssClass="cannotnull2">
                                                <asp:ListItem Selected="True" Value="">-- 请选择 --</asp:ListItem>
                                                <asp:ListItem Value="营运部">营运部</asp:ListItem>
                                                <asp:ListItem Value="营销企划部">营销企划部</asp:ListItem>
                                                <asp:ListItem Value="采购部">采购部</asp:ListItem>
                                                <asp:ListItem Value="信息部">信息部</asp:ListItem>
                                                <asp:ListItem Value="总经理室">总经理室</asp:ListItem>
                                                <asp:ListItem Value="店长">店长</asp:ListItem>
                                                <asp:ListItem Value="其他">其他</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            固定电话
                                        </th>
                                        <td>
                                            <span id="spTelArea">+86</span>
                                            <eStore:TextBox ID="tbTel" runat="server" ClientIDMode="Static" CssClass="cannotnull2"
                                                Width="150"></eStore:TextBox>
                                            分机
                                            <eStore:TextBox ID="tbExt" runat="server" ClientIDMode="Static" Width="80"></eStore:TextBox>
                                            <asp:HiddenField ID="hfareacode" ClientIDMode="Static" runat="server" Value="86" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <span class="red">*</span>手机号码
                                        </th>
                                        <td>
                                            <eStore:TextBox ID="tbmobile" runat="server" ClientIDMode="Static"></eStore:TextBox>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <!--iot-uShop-form-->
                        <div class="iot-uShop-formTitle">
                            公司信息</div>
                        <div class="iot-uShop-form">
                            <table align="center" border="0" cellpadding="0" cellspacing="1" width="100%">
                                <tbody>
                                    <tr>
                                        <th>
                                            公司名称
                                        </th>
                                        <td>
                                            <eStore:TextBox ID="tbcompany" runat="server" ClientIDMode="Static" CssClass="cannotnull2"></eStore:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            所在国家/地区
                                        </th>
                                        <td>
                                            <asp:DropDownList ID="ddlCountries" runat="server" ClientIDMode="Static" CssClass="cannotnull2">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            公司所在地
                                        </th>
                                        <td>
                                            <asp:DropDownList ID="ddlStates" runat="server" ClientIDMode="Static">
                                            </asp:DropDownList>
                                            <asp:HiddenField ID="hfstate" ClientIDMode="Static" runat="server" />
                                            城市:<eStore:TextBox ID="tbcity" runat="server" ClientIDMode="Static" Width="80px"></eStore:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            公司地址
                                        </th>
                                        <td>
                                            <eStore:TextBox ID="tbaddress" runat="server" ClientIDMode="Static" CssClass="cannotnull2"></eStore:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            公司性质
                                        </th>
                                        <td>
                                            <asp:DropDownList ID="ddlCompanyProperty" runat="server" ClientIDMode="Static" CssClass="cannotnull2">
                                                <asp:ListItem Selected="True" Value="">-- 请选择 --</asp:ListItem>
                                                <asp:ListItem>零售门店经营商</asp:ListItem>
                                                <asp:ListItem>零售软硬件开发商</asp:ListItem>
                                                <asp:ListItem>零售设备代理商</asp:ListItem>
                                                <asp:ListItem>系统整合商</asp:ListItem>
                                                <asp:ListItem>学术研究单位</asp:ListItem>
                                                <asp:ListItem>政府/财团法人</asp:ListItem>
                                                <asp:ListItem>零售产业顾问服务商</asp:ListItem>
                                                <asp:ListItem>其他</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                        </th>
                                        <td class="iot-proBtn">
                                            <asp:Button ID="btnSetp2" runat="server" ClientIDMode="Static" CssClass="btnStyle"
                                                OnClick="btnSetp2_Click" Text="下一步" />
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <!--iot-uShop-form-->
                    </div>
                    <!--iot-uShop-formBlock-->
                    <div class="clearfix">
                    </div>
                </h1>
            </div>
            <!--iot-block-->
            <!-- end Step 2  -->
        </asp:Panel>
        <asp:Panel ID="pnStep3" runat="server">
            <!-- Step 3  -->
            <div class="iot-block">
                <h1 class="iot-uShop-title">
                    欢迎注册成为优店联网用户！<div class="clearfix">
                    </div>
                    <h1>
                    </h1>
                    <p>
                        您将收到零售行业精采案例、最新产品资讯、线上商店特殊优惠！<br>立即注册，启用您的会员服务！ </br>
                    </p>
                    <ul class="iot_uShop_stepBG stepBG-3">
                        <li class="on"><span>1</span>登录帐号</li>
                        <li class="on"><span>2</span>基本信息</li>
                        <li class="on"><span>3</span>智能零售需求</li>
                        <div class="clearfix">
                        </div>
                    </ul>
                    <div class="iot-uShop-formBlock">
                        <div class="text-right">
                            <span class="red">* 为非必填</span></div>
                        <div class="iot-uShop-form">
                            <div class="iot-uShop-checkBlock">
                                <div>
                                    您感兴趣的智能零售产品与解决方案？（可复选）</div>
                                <ul class="cannotnullbox3">
                                    <% foreach (var item in lsInterest)
                                       { %>
                                    <% if (item.Text == "其他") continue; %>
                                    <li>
                                        <input type="checkbox" name="cblInterest" value="<%=item.Text %>" />
                                        <%=item.DisplayName%></li>
                                    <%} %>
                                    <li>
                                        <input type="checkbox" name="cblInterest" value="其他" />
                                        其他
                                        <eStore:TextBox ID="tbInterest" runat="server"></eStore:TextBox>
                                    </li>
                                    <div class="clearfix">
                                    </div>
                                </ul>
                                <div class="clearfix">
                                </div>
                            </div>
                            <!--checkBlock-->
                            <div class="iot-uShop-checkBlock">
                                <div>
                                    <span class="red">*</span>近期有采购智能零售相关解决产品的需求吗？</div>
                                <ul>
                                    <li>
                                        <asp:DropDownList ID="ddlCaigou" runat="server" Width="359px">
                                            <asp:ListItem Selected="True" Value="">-- 请选择 --</asp:ListItem>
                                            <asp:ListItem>无</asp:ListItem>
                                            <asp:ListItem>有 (近一个月有需求，请尽快联系我)</asp:ListItem>
                                            <asp:ListItem>有 (三个月左右有需求)</asp:ListItem>
                                            <asp:ListItem>有 (半年左右有需求)</asp:ListItem>
                                        </asp:DropDownList>
                                    </li>
                                </ul>
                                <div class="clearfix">
                                </div>
                            </div>
                            <!--checkBlock-->
                            <div class="iot-uShop-checkBlock">
                                <div>
                                    <span class="red">*</span>您希望获得的资讯与服务？ （可复选）</div>
                                <ul>
                                    <li>
                                        <input type="checkbox" name="cklzixunfuwu" value="行业应用与案例分享" />
                                        行业应用与案例分享</li>
                                    <li>
                                        <input type="checkbox" name="cklzixunfuwu" value="产品资讯" />
                                        产品资讯</li>
                                    <li>
                                        <input type="checkbox" name="cklzixunfuwu" value="产业趋势研讨会" />
                                        产业趋势研讨会</li>
                                    <li>
                                        <input type="checkbox" name="cklzixunfuwu" value="其他" />
                                        其他谘询与服务
                                        <eStore:TextBox ID="tbzixunfuwu" runat="server"></eStore:TextBox>
                                    </li>
                                </ul>
                                <div class="clearfix">
                                </div>
                            </div>
                            <!--checkBlock-->
                            <div class="iot-uShop-checkBlock">
                                <div>
                                    <span class="red">*</span>若您为零售业者，您所属公司业态：</div>
                                <ul>
                                    <li>
                                        <input type="checkbox" name="CompanyYt" value="连锁零售" />
                                        连锁零售(3C,服饰,家居建材,药妆等)</li>
                                    <li>
                                        <input type="checkbox" name="CompanyYt" value="便利店" />
                                        便利店</li>
                                    <li>
                                        <input type="checkbox" name="CompanyYt" value="餐饮店" />
                                        餐饮店</li>
                                    <li>
                                        <input type="checkbox" name="CompanyYt" value="超市" />
                                        超市</li>
                                    <li>
                                        <input type="checkbox" name="CompanyYt" value="百货/购物中心" />
                                        百货/购物中心</li>
                                    <li>
                                        <input type="checkbox" name="CompanyYt" value="其他" />
                                        其他
                                        <eStore:TextBox ID="tbCompanyYt" runat="server"></eStore:TextBox>
                                    </li>
                                </ul>
                                <div class="clearfix">
                                </div>
                            </div>
                            <!--checkBlock-->
                            <div class="iot-uShop-checkBlock">
                                <div>
                                    <span class="red">*</span>呈上题，您的连锁店数：</div>
                                <ul>
                                    <li>
                                        <asp:DropDownList ID="ddlLianSuo" runat="server">
                                            <asp:ListItem Value="">-- 请选择 --</asp:ListItem>
                                            <asp:ListItem>独立经营/自主加盟</asp:ListItem>
                                            <asp:ListItem>0-50间</asp:ListItem>
                                            <asp:ListItem>51-300间</asp:ListItem>
                                            <asp:ListItem>301-500间</asp:ListItem>
                                            <asp:ListItem>501间以上</asp:ListItem>
                                        </asp:DropDownList>
                                    </li>
                                </ul>
                                <div class="clearfix">
                                </div>
                            </div>
                            <!--checkBlock-->
                        </div>
                        <!--iot-uShop-form-->
                        <div class="iot-uShop-checkbtn">
                            <div>
                                感谢您耐心填答，欢迎成为优店联网用户！</div>
                            <asp:Button ID="tbStep3" runat="server" ClientIDMode="Static" CssClass="btnStyle"
                                OnClick="tbStep3_Click" Text="完成注册" />
                            <asp:Panel ID="pnSuccess" runat="server" Visible="false">
                                <p id="linkRegisterSuccess" class="btnStyle" href="#iot_uShop_registerSuccess">
                                    完成注册</p>
                            </asp:Panel>
                        </div>
                        <!--checkBlock-->
                    </div>
                    <!--iot-uShop-formBlock-->
                    <div class="clearfix">
                    </div>
                </h1>
            </div>
            <!--iot-block-->
            <!-- end Step 3  -->
        </asp:Panel>
    </div>
    <div id="iot_uShop_registerSuccess" class="iot_uShopPOP" style="display: none;">
        <div class="iot_uShop_registerSuccess-top">
            <div class="iot_uShop_registerSuccess-icon">
                <h1>
                    恭喜您注册成功！</h1>
                <p>
                    此帐号可同时登录使用研华科技相关服务</p>
            </div>
        </div>
        <div class="iot_uShop_registerSuccess-bottom">
            <p class="wellcome">
                欢迎前往商城选购 <a href="http://www.ushop-iotmart.com.cn" class="btnStyle" onclick="gotohomepage()">
                    前往选购</a>
            </p>
            <p class="att">
                3秒后将自动跳至商城首页</p>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {

            $("#ddlCountries").change(function () {
                var code = $(this).find("option:selected").attr("phonecode");
                $("#spTelArea").html("+" + code);
                $("#hfareacode").val(code);
                bindStates($(this).find("option:selected").text());
            });

            $("#tbUserEmail").blur(function () {
                var re = /^(\w-*\.*)+@(\w-?)+(\.\w{2,})+$/;
                if (re.test($.trim($("#tbUserEmail").val()))) {
                    $.ajax({
                        type: "GET",
                        contentType: "application/json; charset=utf-8",
                        url: "eStoreScripts.asmx/checkUser",
                        data: { useremail: $.trim($("#tbUserEmail").val()) },
                        dataType: "json",
                        success: function (result) {
                            if (!result.d) {
                                $("#spanemailstr").html("").show().append($("<img />").attr("src", GetStoreLocation() + "images/ok-icon.png"));
                            }
                            else {
                                $("#spanemailstr").show().html("对不起，这个帐号（电子邮件）不可用。");
                            }
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) { }
                    });
                } else {
                    $("#spanemailstr").show().html("邮箱格式不正确");
                }
            });
            $("#tbUserPassWord").focus(function () {
                $("#splengthpsd").hide();
            }).blur(function () {
                doubleCheckPSD();
            });
            $("#tbUserPassWordDou").focus(function () {
                $("#spanconfpassword").hide();
            }).blur(function () {
                doubleCheckPSD();
            });

            $("#btnStep1Next").click(function () {
                var result = false;
                $(this).attr("disabled", "disabled");
                if (doubleCheckPSD() && checkEmail()) {
                    result = true;
                }
                $(this).removeAttr("disabled");
                return result;
            });

            $("#btnSetp2").click(function () {
                if (!checkMustInputInfor(2)) {
                    return false;
                }
                return true;
            });

            $("#tbStep3").click(function () {
                if ($(this).hasClass("loging"))
                    return false;
                if (!checkMustInputInfor(3)) {
                    $(this).removeAttr("disabled").removeAttr("style");
                    return false;
                }
                $(this).addClass("loging").attr({ "style": "background-image: url(" + GetStoreLocation() + "images/Loading.gif); background-repeat: no-repeat;background-position: 5px 8px;" });
                return true;
            });

            $("#linkRegisterSuccess").fancybox({
                padding: 0,
                margin: 0,
                onStart: function () { $("#iot_uShop_registerSuccess").show(), window.setTimeout(gotohomepage, 3000); },
                onClosed: function () { $("#iot_uShop_registerSuccess").hide() }
            });

            $("#ddlStates").change(function () {
                $("#hfstate").val($(this).val());
            });

            function gotohomepage() {
                window.location.href = "<%=esUtilities.CommonHelper.GetStoreLocation() %>";
            }

            if ($("#ddlCountries").find("option:selected") != null && $(this).find("option:selected") != undefined) {
                bindStates($("#ddlCountries").find("option:selected").val())
            }
        });

    
    </script>
</asp:Content>
