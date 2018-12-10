<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="MyRegister.aspx.cs" Inherits="eStore.UI.MyRegister" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <link href="/App_Themes/V4/byregister.css" rel="stylesheet" />
    <script src="/Scripts/jquery.validate.js"></script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="register-container">
        <!--personal info-->
        <div class="register-container">
            <h4><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Personal_Information")%></h4>
            <div class="left-column">
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("Cart_First_Name")%></label><br />
                <asp:TextBox ID="tbFirst_Name" ClientIDMode="Static" CssClass="required" runat="server"></asp:TextBox><br />
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("Cart_Last_Name")%></label><br />
                <asp:TextBox ID="tbLast_Name" ClientIDMode="Static" CssClass="required" runat="server"></asp:TextBox><br />
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_eMail")%></label><br />
                <asp:TextBox ID="tbEmail" ClientIDMode="Static" TextMode="Email" runat="server"></asp:TextBox>
            </div>
            <div class="right-column">
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Password")%></label><br />
                <asp:TextBox TextMode="Password" ID="tbPassword" ClientIDMode="Static" runat="server"></asp:TextBox><br />
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Confirm_Password")%></label><br />
                <asp:TextBox ID="tbConfirm_Password" TextMode="Password" ClientIDMode="Static" runat="server"></asp:TextBox>
                <% if (type != "regist")
                    {%>
                <div class="mask">
                    <div>
                        <label class="mousehand" data-ac="changepassword">Change password</label>
                    </div>
                    <div data-form="currentpassword" class="hidden">
                        <label><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Current_Password")%></label><br />
                        <input type="password" id="tbCurrentpsd" class="width250 marginbottom5" /><br />
                        <button data-ac="checkpsd" type="button" class="eStore_btn">
                            <%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Go")%>
                        </button>
                    </div>
                </div>
                <%}%>
            </div>

        </div>
        <!--end of personal info-->
        <!--Company info-->
        <div class="container2">
            <h4><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Company_Information")%></h4>
            <div class="left-column2">
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("Cart_Company_Name")%></label><br />
                <asp:TextBox ID="tbCompany_Name" CssClass="required" ClientIDMode="Static" runat="server"></asp:TextBox><br />
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("Cart_Country")%></label><br />
                <asp:DropDownList ID="ddlCountry" runat="server" ClientIDMode="Static" CssClass="spacing"></asp:DropDownList><br />
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Job_Function")%></label><br />
                <asp:DropDownList ID="ddlJob_Function" runat="server" ClientIDMode="Static" CssClass="spacing">
                    <asp:ListItem Value="Application Engineer">Application Engineer</asp:ListItem>
                </asp:DropDownList><br />
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Job_Title")%></label><br />
                <asp:TextBox ID="tbJob_Title" ClientIDMode="Static" runat="server"></asp:TextBox><br />
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Phone_number")%></label><br />
                <asp:TextBox ID="tbPhone_number" TextMode="Phone" ClientIDMode="Static" runat="server"></asp:TextBox><br />

                <asp:Button ID="btsubmit" ClientIDMode="Static" OnClick="btsubmit_Click" runat="server" />

            </div>

            <div class="right-column2">
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Mobile_number")%></label><br />
                <asp:TextBox ID="tbMobile_number" TextMode="Phone" ClientIDMode="Static" runat="server"></asp:TextBox><br />
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("Cart_Address")%></label><br />
                <asp:TextBox ID="tbAddress" CssClass="required" ClientIDMode="Static" runat="server"></asp:TextBox><br />
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("Cart_City")%></label><br />
                <asp:TextBox ID="tbCity" ClientIDMode="Static" CssClass="required" runat="server"></asp:TextBox><br />
                <label><%=eStore.Presentation.eStoreLocalization.Tanslation("Cart_State")%></label>&nbsp;
        <asp:DropDownList ID="ddlState" runat="server" ClientIDMode="Static" CssClass="spacing"></asp:DropDownList>
                &nbsp; &nbsp; &nbsp;
        <label><%=eStore.Presentation.eStoreLocalization.Tanslation("Cart_ZIP")%></label>&nbsp;<asp:TextBox ID="tbZip" ClientIDMode="Static" runat="server" CssClass="zip required"></asp:TextBox><br />
                <br />
            </div>
        </div>
    </div>
    <!--end of Company info-->

    <script type="text/javascript">
        var _pressing = false;
        $(function () {
            //register
            <% if(type == "regist") {%>
            validateRegist();


            <%} else {%>
            validateEdit();
            $('[data-ac="changepassword"]').click(function () {
                $(this).parent().hide();
                $('[data-form="currentpassword"]').show();
            });

            $('[data-ac="checkpsd"]').click(function () {
                eStore.UI.eStoreScripts.CheckCurrPSD($("#tbCurrentpsd").val(), function (res) {
                    if (res.d == false) {
                        if ($("#tbCurrentpsd").next("em").length == 0)
                            $("#tbCurrentpsd").addClass("error").after('<em id="tbCurrentpsd-error" class="error" for="tbCurrentpsd"><span></span>Password error.</em>');
                    }
                    else {
                        $(".mask").remove();
                        $("#tbPassword").rules("add", {
                            required: true,
                            minlength: 5
                        });
                        $("#tbConfirm_Password").rules("add", {
                            required: true,
                            minlength: 5,
                            equalTo: "#tbPassword"
                        });
                    }
                        
                });
            });


            <%}%>
            


            $("#ddlCountry").change(function () {
                changeCustomerCountry($("#ddlCountry").val(), 'ddlState');
            });

            $("#btsubmit").click(function () {
                if (_pressing)
                    return false;
                if ($("#eStoreMainForm").valid())
                {
                    _pressing = true;
                    $(this).addClass("ds");
                }
            });
        });



        function changeCustomerCountry(country, sate, hfState) {
            if (country != "") {
                var selectBlow = "[<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Select_Below) %>]";
                $.getJSON("<%=esUtilities.CommonHelper.GetStoreLocation() %>proc/do.aspx?func=<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.getStoreCountries %>", { id: country },
                function (data) {
                    var ddlState = document.getElementById(sate);
                    ddlState.options.length = 0;
                    if (data == null || data == "") {
                        ddlState.options.add(new Option("None", "None"));
                        ddlState.disabled = "disabled";
                        return;
                    }
                    else if (data.length > 0) {
                        ddlState.options.add(new Option(selectBlow, ""));
                        ddlState.disabled = false;
                        for (var i = 0; i < data.length; i++) {
                            ddlState.options.add(new Option(data[i].state, data[i].short));
                        }
                    }
                    else {
                        ddlState.options.add(new Option("None", "None"));
                        ddlState.disabled = "disabled";
                    }
                });
            }
            else {
                var ddlState = document.getElementById(sate);
                ddlState.options.length = 0;
                ddlState.options.add(new Option("None", "None"));
                ddlState.disabled = "disabled";
            }
        }
        
        function validateRegist()
        {
            $("#eStoreMainForm").validate({
                rules: {
                    ctl00$eStoreMainContent$tbEmail: {
                        required: true,
                        email: true,
                        remote: {
                            url: "/eStoreScripts.asmx/CheckUser",
                            type: "get",
                            dataType: "json",
                            data: {
                                userId: function () {
                                    return $("#tbEmail").val();
                                }
                            },
                            dataFilter: function (data) {
                                var temp = $.trim(data.replace(/<[^>]+>/g, ""));
                                return temp === "true";
                            }
                        }
                    },
                    ctl00$eStoreMainContent$tbPassword: {
                        required: true,
                        minlength: 5
                    },
                    ctl00$eStoreMainContent$tbConfirm_Password: {
                        required: true,
                        minlength: 5,
                        equalTo: "#tbPassword"
                    },
                    ctl00$eStoreMainContent$ddlCountry: {
                        required: true
                    },
                    ctl00$eStoreMainContent$ddlState: {
                        required: true
                    }
                },
                messages: {
                    ctl00$eStoreMainContent$tbEmail: {
                        remote: '<%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_email_is_exists_already")%>'
                    }
                }
            });
        }

        function validateEdit()
        {
            $("#eStoreMainForm").validate({
                rules: {
                    ctl00$eStoreMainContent$tbFirst_Name: "required",
                    ctl00$eStoreMainContent$tbLast_Name: "required",
                    ctl00$eStoreMainContent$tbCompany_Name: {
                        required: true
                    }
                }
            });
        }
        $.validator.setDefaults({
            errorElement: "em"
        });
        $.extend($.validator.messages, {
            required: '<%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_required")%>'
        });
    </script>

</asp:Content>

