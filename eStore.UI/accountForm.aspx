<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumnIoT.Master"
    AutoEventWireup="true" CodeBehind="accountForm.aspx.cs" Inherits="eStore.UI.accountForm" %>

<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <link href="Styles/jquery.fancybox-1.3.4.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/registerutilities.js" type="text/javascript"></script>
    <script src="Scripts/jquery.fancybox-1.3.4.js" type="text/javascript"></script>
    <asp:Panel ID="pnaccount" runat="server">
        <div class="iot-uShop">
            <div class="iot-block">
                <div class="iot-breadcrumb">
                    <a href="/">首页</a> > 会员专区</div>
            </div>
            <!--iot-block-->
            <p style="text-align: center; font-size: 24px; color: Red;">
                <asp:Literal ID="ltMessage" runat="server"></asp:Literal></p>
            <div class="iot-block acctount">
                <h1 class="iot-uShop-title">
                    您的用户资料 <span class="iot-proBtn pull-right"><span class="btnStyle" id="editUserInfor">
                        修改资料</span></span>
                </h1>
                <div class="iot-uShop-formBlock">
                    <div class="iot-uShop-formTitle">
                        登录帐号</div>
                    <div class="iot-uShop-form">
                        <table width="100%" border="0" cellspacing="1" cellpadding="0" align="center">
                            <tr>
                                <th>
                                    会员帐号
                                </th>
                                <td>
                                    <asp:Literal ID="ltEmail" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    密码
                                </th>
                                <td>
                                    <label>
                                        *******</label><a href="#iot_uShop_accountPassword" id="editpsd" class="fancybox accountPassword">修改密码</a>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <!--iot-uShop-form-->
                    <div class="iot-uShop-formTitle">
                        联系人信息</div>
                    <div class="iot-uShop-form">
                        <table width="100%" border="0" cellspacing="1" cellpadding="0" align="center">
                            <tr>
                                <th>
                                    姓
                                </th>
                                <td>
                                    <asp:Label ID="lbNa" CssClass="infor" runat="server" Text=""></asp:Label><eStore:TextBox
                                        ID="tbUserNa" runat="server" ClientIDMode="Static" CssClass="cannotnull2 uedit"></eStore:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    名
                                </th>
                                <td>
                                    <asp:Label ID="lbMe" CssClass="infor" runat="server" Text=""></asp:Label><eStore:TextBox
                                        ID="tbUserMe" runat="server" ClientIDMode="Static" CssClass="cannotnull2 uedit"></eStore:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    所在部门
                                </th>
                                <td>
                                    <asp:Label ID="lbBumen" CssClass="infor" runat="server" Text=""></asp:Label>
                                    <asp:DropDownList ID="ddlBumen" runat="server" CssClass="uedit" ClientIDMode="Static">
                                        <asp:ListItem Value="营运部">营运部</asp:ListItem>
                                        <asp:ListItem Value="营销企划部">营销企划部</asp:ListItem>
                                        <asp:ListItem Value="采购部">采购部</asp:ListItem>
                                        <asp:ListItem Value="信息部">信息部</asp:ListItem>
                                        <asp:ListItem Value="总经理室">总经理室</asp:ListItem>
                                        <asp:ListItem Value="店长">店长</asp:ListItem>
                                        <asp:ListItem Selected="True" Value="其他">其他</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    固定电话
                                </th>
                                <td>
                                    +<asp:Label ID="lbTelArea" CssClass="infor" runat="server" Text=""></asp:Label>
                                    <asp:Label ID="lbTel" runat="server" CssClass="infor" Text=""></asp:Label>
                                    <span class="infor">分机</span>
                                    <asp:Label ID="lbExt" runat="server" Text="" CssClass="infor"></asp:Label>
                                    <asp:Label ID="spTelArea" ClientIDMode="Static" CssClass="uedit" runat="server" Text=""></asp:Label>
                                    <eStore:TextBox ID="tbTel" runat="server" ClientIDMode="Static" Width="150" CssClass="cannotnull2 uedit"></eStore:TextBox>
                                    <span class="uedit">分机</span>
                                    <eStore:TextBox ID="tbExt" runat="server" ClientIDMode="Static" CssClass="uedit"
                                        Width="80"></eStore:TextBox>
                                    <asp:HiddenField ID="hfareacode" ClientIDMode="Static" runat="server" Value="86" />
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="red">*</span>手机号码
                                </th>
                                <td>
                                    <asp:Label ID="lbmobile" runat="server" CssClass="infor" Text=""></asp:Label><eStore:TextBox
                                        ID="tbmobile" runat="server" ClientIDMode="Static" CssClass="uedit"></eStore:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <!--iot-uShop-form-->
                    <div class="iot-uShop-formTitle">
                        公司信息</div>
                    <div class="iot-uShop-form">
                        <table width="100%" border="0" cellspacing="1" cellpadding="0" align="center">
                            <tr>
                                <th>
                                    公司名称
                                </th>
                                <td>
                                    <asp:Label ID="lbcompany" CssClass="infor" runat="server" Text=""></asp:Label><eStore:TextBox
                                        ID="tbcompany" runat="server" ClientIDMode="Static" CssClass="cannotnull2 uedit"></eStore:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    所在国家/地区
                                </th>
                                <td>
                                    <asp:Label ID="lbcountry" runat="server" CssClass="infor" Text=""></asp:Label>
                                    <asp:DropDownList ID="ddlCountries" runat="server" ClientIDMode="Static" CssClass="uedit">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    公司所在地
                                </th>
                                <td>
                                    <asp:Label ID="lbstate" runat="server" ClientIDMode="Static" Text="" CssClass="infor"></asp:Label>
                                    <asp:DropDownList ID="ddlStates" runat="server" ClientIDMode="Static" CssClass="uedit">
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="hfstate" ClientIDMode="Static" runat="server" />
                                    <span class="uedit">城市:</span>
                                    <asp:Label ID="lbCity" runat="server" ClientIDMode="Static" Text="" CssClass="infor"></asp:Label>
                                    <eStore:TextBox ID="tbcity" runat="server" ClientIDMode="Static" CssClass="uedit"
                                        Width="80px"></eStore:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    公司地址
                                </th>
                                <td>
                                    <asp:Label ID="lbaddress" runat="server" Text="" CssClass="infor"></asp:Label>
                                    <eStore:TextBox ID="tbaddress" runat="server" ClientIDMode="Static" CssClass="cannotnull2 uedit"></eStore:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    公司性质
                                </th>
                                <td>
                                    <asp:Label ID="lbcompanyproperty" runat="server" Text="" CssClass="infor"></asp:Label>
                                    <asp:DropDownList ID="ddlCompanyProperty" runat="server" ClientIDMode="Static" CssClass="uedit">
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
                        </table>
                    </div>
                    <!--iot-uShop-form-->
                    <div class="iot-uShop-formTitle">
                        智能零售需求</div>
                    <div class="iot-uShop-form">
                        <!--checkBlock-->
                        <div class="iot-uShop-checkBlock">
                            <div>
                                您感兴趣的解决方案：</div>
                            <div class="checkBox infor">
                                <asp:Label ID="lbinterest" runat="server" Text="" ClientIDMode="Static"></asp:Label>
                            </div>
                            <div class="checkBox uedit">
                                <ul class="cannotnullbox3">
                                    <li>
                                        <input type="checkbox" name="cblInterest" value="智能POS结帐终端" />
                                        智能POS结帐终端</li>
                                    <li>
                                        <input type="checkbox" name="cblInterest" value="互动多媒体" />
                                        互动多媒体</li>
                                    <li>
                                        <input type="checkbox" name="cblInterest" value="智能影像分析解决方案" />
                                        智能影像分析解决方案</li>
                                    <li>
                                        <input type="checkbox" name="cblInterest" value="顾客自助终端" />
                                        顾客自助终端</li>
                                    <li>
                                        <input type="checkbox" name="cblInterest" value="多功能触控一体机" />
                                        多功能触控一体机</li>
                                    <li>
                                        <input type="checkbox" name="cblInterest" value="智能云端管理平台" />
                                        智能云端管理平台（如: 优店联网UShop+）</li>
                                    <li>
                                        <input type="checkbox" name="cblInterest" value="物流终端解决方案" />
                                        物流终端解决方案</li>
                                    <li>
                                        <input type="checkbox" name="cblInterest" value="其他" />
                                        其他
                                        <eStore:TextBox ID="tbInterest" runat="server" ClientIDMode="Static"></eStore:TextBox>
                                    </li>
                                    <div class="clearfix">
                                    </div>
                                </ul>
                            </div>
                        </div>
                        <!--checkBlock-->
                        <div class="iot-uShop-checkBlock">
                            <div>
                                近期有采购智能零售相关解决产品的需求吗？</div>
                            <div class="checkBox">
                                <asp:Label ID="lbcaigou" runat="server" Text="" CssClass="infor"></asp:Label>
                                <asp:DropDownList ID="ddlCaigou" runat="server" CssClass="uedit">
                                    <asp:ListItem>无</asp:ListItem>
                                    <asp:ListItem Selected="True">有 (近一个月有需求，请尽快联系我)</asp:ListItem>
                                    <asp:ListItem>有 (三个月左右有需求)</asp:ListItem>
                                    <asp:ListItem>有 (半年左右有需求)</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <!--checkBlock-->
                        <div class="iot-uShop-checkBlock">
                            <div>
                                您希望获得的资讯与服务？</div>
                            <div class="checkBox infor">
                                <asp:Label ID="lbzixunfuwq" runat="server" Text="" ClientIDMode="Static"></asp:Label>
                            </div>
                            <div class="checkBox uedit">
                                <ul>
                                    <li>
                                        <input type="checkbox" name="cklzixunfuwu" value="行业应用与案例分享" />行业应用与案例分享</li>
                                    <li>
                                        <input type="checkbox" name="cklzixunfuwu" value="产品资讯" />产品资讯</li>
                                    <li>
                                        <input type="checkbox" name="cklzixunfuwu" value="产业趋势研讨会" />产业趋势研讨会</li>
                                    <li>
                                        <input type="checkbox" name="cklzixunfuwu" value="其他" />其他谘询与服务
                                        <eStore:TextBox ID="tbzixunfuwu" runat="server" ClientIDMode="Static"></eStore:TextBox></li>
                                    <div class="clearfix">
                                    </div>
                                </ul>
                            </div>
                        </div>
                        <!--checkBlock-->
                        <!--checkBlock-->
                        <div class="iot-uShop-checkBlock">
                            <div>
                                您所属公司业态：</div>
                            <div class="checkBox infor">
                                <asp:Label ID="lbcompanyyt" ClientIDMode="Static" runat="server" Text=""></asp:Label>
                            </div>
                            <div class="checkBox uedit">
                                <ul>
                                    <li>
                                        <input type="checkbox" name="CompanyYt" value="连锁零售" />连锁零售(3C,服饰,家居建材,药妆等)</li>
                                    <li>
                                        <input type="checkbox" name="CompanyYt" value="便利店" />便利店</li>
                                    <li>
                                        <input type="checkbox" name="CompanyYt" value="餐饮店" />餐饮店</li>
                                    <li>
                                        <input type="checkbox" name="CompanyYt" value="超市" />超市</li>
                                    <li>
                                        <input type="checkbox" name="CompanyYt" value="百货/购物中心" />百货/购物中心</li>
                                    <li>
                                        <input type="checkbox" name="CompanyYt" value="其他" />其他
                                        <eStore:TextBox ID="tbCompanyYt" ClientIDMode="Static" runat="server"></eStore:TextBox></li>
                                    <div class="clearfix">
                                    </div>
                                </ul>
                            </div>
                        </div>
                        <!--checkBlock--> 
                        <div class="iot-uShop-checkBlock">
                            <div>
                                连锁店数：</div>
                            <div class="checkBox">
                                <asp:Label ID="lbliansuo" runat="server" Text="" CssClass="infor"></asp:Label>
                                <asp:DropDownList ID="ddlLianSuo" runat="server" CssClass="uedit">
                                    <asp:ListItem Value="">-- 请选择 --</asp:ListItem>
                                    <asp:ListItem>独立经营/自主加盟</asp:ListItem>
                                    <asp:ListItem>0-50间</asp:ListItem>
                                    <asp:ListItem>51-300间</asp:ListItem>
                                    <asp:ListItem>301-500间</asp:ListItem>
                                    <asp:ListItem>501间以上</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <!--iot-uShop-form-->
                    <div class="iot-uShop-checkbtn">
                        <asp:Button ID="btUpdateUserInfor" runat="server" Text="保存更新" CssClass="btnStyle uedit"
                            OnClick="btUpdateUserInfor_Click" />
                    </div>
                    <!--checkBlock-->
                </div>
                <!--iot-uShop-formBlock-->
            </div>
            <!--iot-block-->
        </div>
    </asp:Panel>
    <div id="iot_uShop_accountPassword" class="iot_uShopPOP uShopPOP_account" style="display: none;">
        <h1>
            修改密码</h1>
        <table border="0" cellspacing="1" cellpadding="0" class="formBlcok">
            <tr id="troldpws" runat="server">
                <th>
                    舊密碼
                </th>
                <td>
                    <eStore:TextBox ID="tbuserOldPSD" runat="server" ClientIDMode="Static" TextMode="Password"></eStore:TextBox>
                </td>
            </tr>
            <tr>
                <th>
                    新密码
                </th>
                <td>
                    <eStore:TextBox ID="tbUserPassWord" runat="server" ClientIDMode="Static" TextMode="Password"></eStore:TextBox><span
                        class="iot-uShop-formAtt"><span id="splengthpsd" class="hiddenitem red">密码太短了 </span>
                        <br />
                        請勿使用特殊符號 ^ ' ( ) | ; \.</span>
                </td>
            </tr>
            <tr>
                <th>
                    確認新密碼
                </th>
                <td>
                    <eStore:TextBox ID="tbUserPassWordDou" runat="server" ClientIDMode="Static" TextMode="Password"></eStore:TextBox><span
                        class="iot-uShop-formAtt red hiddenitem" id="spanconfpassword">密码不相同</span>
                </td>
            </tr>
            <tr>
                <th>
                </th>
                <td>
                    <asp:Button ID="btupdatepsd" runat="server" ClientIDMode="Static" Text="更新密码" CssClass="btnStyle"
                        OnClick="btupdatepsd_Click" />
                </td>
            </tr>
        </table>
    </div>
    <div id="ito_ushop_chagepswSuccess" href="#ito_ushop_chagepswSuccess" class="iot_uShopPOP uShopPOP_account"
        style="display: none;">
        <h1>
            重置密码</h1>
        <p class="bigTxt">
            您的密码已重置，请重新登录。</p>
        <table border="0" cellspacing="1" cellpadding="0" class="formBlcok">
            <tr>
                <th style="font-size: 15px;">
                </th>
                <td>
                    <a class="btnStyle" href="/" onclick="gotohomepage()">返回首页</a>
                    <p>
                        &nbsp;</p>
                    <p class="att">
                        3秒后将自动跳至商城首页</p>
                </td>
            </tr>
        </table>
    </div>
    <p style="display: none;" href="#iot_ushop_Dialog" id="pshopdialog">
    </p>
    <div id="iot_ushop_Dialog" class="iot_uShopPOP uShopPOP_account" style="display: none;">
        <h1>
            重置密码</h1>
        <p class="bigTxt">
            <asp:Literal ID="ltdialogmessage" runat="server"></asp:Literal></p>
        <p>
            &nbsp;</p>
        <p>
            &nbsp;</p>
        <p>
            &nbsp;</p>
    </div>
    <script type="text/javascript">
        String.prototype.startWith = function (str) {
            if (str == null || str == "" || this.length == 0 || str.length > this.length)
                return false;
            if (this.substr(0, str.length) == str)
                return true;
            else
                return false;
            return true;
        }
        String.prototype.endWith = function (str) {
            if (str == null || str == "" || this.length == 0 || str.length > this.length)
                return false;
            if (this.substring(this.length - str.length) == str)
                return true;
            else
                return false;
            return true;
        }

        $(document).ready(function () {

            $("#ddlCountries").change(function () {
                var code = $(this).find("option:selected").attr("phonecode");
                $("#spTelArea").html("+" + code);
                $("#hfareacode").val(code);
                bindStates($(this).find("option:selected").text());
            });


            $("#editUserInfor").click(function () {
                $(".uedit").show();
                $(".infor,#editUserInfor").hide();
                $("#ddlStates").find("option[text='" + $("#lbstate").text() + "']").attr("selected", true);
            });

            $(function () {
                $("#editpsd").fancybox({
                    padding: 0,
                    margin: 0,
                    onStart: function () { $("#iot_uShop_accountPassword").show() },
                    onClosed: function () { $("#iot_uShop_accountPassword").hide() }
                });
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

            $("#btupdatepsd").click(function () {
                var result = false;
                $(this).attr("disabled", "disabled");
                if (doubleCheckPSD()) {
                    result = true;
                }

                $(this).removeAttr("disabled");
                return result;
            });

            var yts = $("#lbcompanyyt").text().split(",");
            $(yts).each(function (n, m) {
                if (m.startWith("其他:")) {
                    var tt = m.replace("其他:", "");
                    m = "其他";
                    $("#tbCompanyYt").val(tt);
                }
                $.each($("input[name='CompanyYt']"), function (i, item) {
                    if (m == $(item).val())
                        $(item).attr("checked", 'true');
                });
            });

            var interests = $("#lbinterest").text().split(",");
            $(interests).each(function (n, m) {
                if (m.startWith("其他:")) {
                    var tt = m.replace("其他:", "");
                    m = "其他";
                    $("#tbInterest").val(tt);
                }
                $.each($("input[name='cblInterest']"), function (i, item) {
                    if (m == $(item).val())
                        $(item).attr("checked", 'true');
                });
            });

            var zixuns = $("#lbzixunfuwq").text().split(",");
            $(zixuns).each(function (n, m) {
                if (m.startWith("其他:")) {
                    var tt = m.replace("其他:", "");
                    m = "其他";
                    $("#tbzixunfuwu").val(tt);
                }
                $.each($("input[name='cklzixunfuwu']"), function (i, item) {
                    if (m == $(item).val())
                        $(item).attr("checked", 'true');
                });
            });

            $("#ddlStates").change(function () {
                $("#hfstate").val($(this).val());
            });

            $("#ito_ushop_chagepswSuccess").fancybox({
                padding: 0,
                margin: 0,
                onStart: function () { $("#ito_ushop_chagepswSuccess").show(), window.setTimeout(gotohomepage, 3000); },
                onClosed: function () { $("#ito_ushop_chagepswSuccess").hide() }
            });
            $("#pshopdialog").fancybox({
                padding: 0,
                margin: 0,
                onStart: function () { $("#iot_ushop_Dialog").show() },
                onClosed: function () { $("#iot_ushop_Dialog").hide() }
            });

            if ($("#ddlCountries").find("option:selected") != null && $(this).find("option:selected") != undefined) {
                bindStates($("#ddlCountries").find("option:selected").val())

            }

        });

        function gotohomepage() {
            window.location.href = "<%=esUtilities.CommonHelper.GetStoreLocation() %>";
        }
    </script>
</asp:Content>
