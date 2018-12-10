<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="INIsecureStart.aspx.cs"
    EnableViewState="false" EnableViewStateMac="false" Inherits="eStore.UI.INIsecureStart" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body
        {
            width: 780px;
            margin: auto 0;
        }
    </style>
</head>
<body onload="javascript:enable_click()">
    <form id="form1" runat="server" action="/completeIndirectPayment.aspx" onsubmit="return pay(this)"
    method="post">
    <div>
        <div class="paymentcontent">
            <div class="paymentheader">
                지불 요청
            </div>
            <div class="paymentbody koreaInipaynet">
                <p>
                    다소 시간이 걸릴수도 있으니 잠시 기다려 주세요.
                </p>
                <p>
                    <input type="hidden" id="gopaymethod" value="onlycard" runat="server" clientidmode="Static" />
                    <input type="hidden" id="quotainterest" runat="server" />
                    <input type="hidden" id="paymethod" runat="server" />
                    <input type="hidden" id="cardcode" runat="server" />
                    <input type="hidden" id="cardquota" runat="server" />
                    <input type="hidden" id="rbankcode" runat="server" />
                    <input type="hidden" id="reqsign" value="DONE" runat="server" />
                    <input type="hidden" id="encrypted" runat="server" />
                    <input type="hidden" id="sessionkey" runat="server" />
                    <input type="hidden" id="uid" runat="server" />
                    <input type="hidden" id="sid" runat="server" />
                    <input type="hidden" id="version" value="5000" runat="server" />
                    <input type="hidden" id="clickcontrol" runat="server" />
                </p>
            </div>
        </div>
    </div>
    <script language="javascript" src="http://plugin.inicis.com/pay40_sec.js"></script>
    <script type="text/javascript" language="javascript">
        enable_click();

        StartSmartUpdate();

        var openwin;

        function pay(theform) {
            // MakePayMessage()를 호출함으로써 플러그인이 화면에 나타나며, Hidden Field
            // 에 값들이 채워지게 됩니다. 일반적인 경우, 플러그인은 결제처리를 직접하는 것이
            // 아니라, 중요한 정보를 암호화 하여 Hidden Field의 값들을 채우고 종료하며,
            // 다음 페이지인 INIsecurepay.asp로 데이터가 포스트 되어 결제 처리됨을 유의하시기 바랍니다.
            if ($("#clickcontrol").val() == "enable") {
                if ($("#goodname").val() == "")  // 필수항목 체크 (상품명, 상품가격)
                {
                    alert("상품명이 빠졌습니다. 필수항목입니다.");
                    return false;
                }
                else if (document.INIpay == null || document.INIpay.object == null)  // 플러그인 설치유무 체크
                {
                    alert("\n이니페이 플러그인 128이 설치되지 않았습니다. \n\n안전한 결제를 위하여 이니페이 플러그인 128의 설치가 필요합니다. \n\n다시 설치하시려면 Ctrl + F5키를 누르시거나 메뉴의 [보기/새로고침]을 선택하여 주십시오.");
                    return false;
                }
                else {
                    if (MakePayMessage(theform)) {
                        disable_click();
                        //openwin = window.open("childwin.html", "childwin", "width=299,height=149");
                        return true;
                    } else {
                        alert("결제를 취소하셨습니다.");
                        window.location = "/Cart/confirm.aspx";
                        return false;
                    }
                }

            }
            else {
                return false;
            }
        }
        $(document).ready(function () {
            $("form:first").submit();
        });
        $("body").focus(function () { focus_control(); });
        function enable_click() {
            $("#clickcontrol").val("enable");
        }

        function disable_click() {
            $("#clickcontrol").val("disable");
        }

        function focus_control() {
           
        }
    </script>
    </form>
</body>
</html>
