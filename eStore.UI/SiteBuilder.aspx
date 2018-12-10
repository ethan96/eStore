<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="SiteBuilder.aspx.cs" Inherits="eStore.UI.Account.SiteBuilder" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <link href="/App_Themes/V4/bySiteBuilder.css" rel="stylesheet" />
    <script src="/Scripts/dropzone.js"></script>
    <script src="/Scripts/jquery.validate.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="body">
        <div id="banner">
            <h1>e-Commerce Site<br />
                Builder Form<br />
                &nbsp;<br />
            </h1>
        </div>
        <!--intro-->
        <div class="intro">
            <p>
                This information will assist the Advantech technical and
      marketing teams with configuring and hosting the ecommerce
      site for your company. If there are any questions, please
      contact your Advantech representative or send email to
      <a href="mailto:buy@advantech.com" class="email" title="Send email">buy@advantech.com</a>.
            </p>
        </div>
        <!--end of intro-->
        <div class="section">
 	<div class="color">
    	<p>&nbsp;</p>
     </div>
            <div class="title">
                <h2 class="h2">CONTACT INFORMATION</h2>
            </div>
        </div>
        <div class="company">
            <label class="FirstName">First Name</label>
            <asp:TextBox ID="txtFirstName" CssClass="required" runat="server"></asp:TextBox><br />
            <label class="lastName">Last Name</label>
            <asp:TextBox ID="txtLastName" CssClass="required" runat="server"></asp:TextBox><br />
            <label class="emailInfo">E-mail</label>
            <asp:TextBox ID="txteMail" CssClass="required" runat="server"></asp:TextBox>
        </div>
        <!--company info-->
        <div class="section">
            <div class="color">
                <p>&nbsp;</p>
            </div>
            <div class="title">
                <h2 class="h2">COMPANY INFORMATION
                </h2>
            </div>
        </div>
        <div class="company">
            <label class="companyname">Company Name</label>
            <asp:TextBox ID="txtCompanyName" CssClass="required" runat="server"></asp:TextBox><br />
            <label class="phone">
                Phone Number to
    Display</label>
            <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="required"></asp:TextBox><br />
            <label class="hours">Store Hours*</label><asp:TextBox ID="txtStoreHours" CssClass="required" runat="server"></asp:TextBox><br />
            <p class="small">
                *Please provide in format of “from X am till X pm (EST)
      Mon-Fri”
            </p>
        </div>
        <p>
            Please specify Product categories on <a href="http://buy.advantech.com/">Buy.Advantech.com</a> to include in
    your eCommerce site
        </p>
        <ul class="container">
            <eStore:Repeater ID="rpCategories" runat="server">
                <itemtemplate>
                    <li class="column" >
                       
                        <input type="checkbox" name="SelectedCategories" value="<%# Eval("CategoryID") %>"  <%# (bool)Eval("IsSelected")?"checked='checked'":"" %>    /> <%# Eval("localCategoryNameX") %>
                    </li>
                </itemtemplate>

            </eStore:Repeater>
            <li class="clear"></li>
        </ul>


        <div class="bg">
            <div class="section">
                <div class="color">
                    <p>&nbsp;</p>
                </div>
                <div class="title">
                    <h2 class="h2">TECHNICAL DETAILS
                    </h2>
                </div>
            </div>
            <div class="company">
                <p class="domain">
                    <strong>Company’s domain’s DNS settings</strong>
                </p>
                <br />
                <p class="small">
                    While your new store will be hosted on Advantech servers,
        it can feature your own (www.)domain. Provided you have a
        hosting account for your domain, and would like your new
        online store to feature it, you will need to provide
        Advantech with your domain’s name server (DNS) addresses.
                </p>
                <br />
                <p>
                    Please provide DNS settings in email to <a href="mailto:SiteBuild@Advantech.com"
                        class="email" title="Send email">SiteBuild@Advantech.com</a>.
                </p>
                <br />
                <div class="container2">
                    <div class="right">
                        <asp:RadioButtonList ID="rblHasSSLCertificate" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Text="YES" Value="true"
                                Selected="True"></asp:ListItem>
                            <asp:ListItem Text="NO" Value="false"></asp:ListItem>
                        </asp:RadioButtonList>

                    </div>
                    <div class="left">
                        <p>
                            Does your company have a SSL Certificate for your
            domain?
                        </p>
                    </div>
                </div>
                <div class="container2">
                    <div class="right">
                        <asp:RadioButtonList ID="rblHaseCommerceChat" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Text="YES" Value="true"
                                Selected="True"></asp:ListItem>
                            <asp:ListItem Text="NO" Value="false"></asp:ListItem>
                        </asp:RadioButtonList>

                    </div>
                    <div class="left">
                        <p>
                            Do you require an eCommerce Chat feature?*
                        </p>
                    </div>
                </div>
                <br />
                <p class="small">
                    *If you have replied ‘yes’ to above, it is required that
        you confirm you have staff at your company who can mind the
        Chat interface for incoming chats during all business hours
        for<br />
                    the foreseeable timeframe.
                </p>
                <br />
                <p>
                    I confirm that I have staff for incoming chats.
        <input type="checkbox" name="chats" value="" />
                </p>
                <br />
            </div>
        </div>
        <div class="section">
            <div class="color">
                <p>&nbsp;</p>
            </div>
            <div class="title">
                <h2 class="h2">STYLE CUSTOMIZATION
                </h2>
            </div>
            <div class="company">
                <p class="logo">
                    Company Logo Upload* &nbsp;
                    <asp:FileUpload ID="fupLogo" ClientIDMode="Static" runat="server" />
                    <asp:Image ID="imgLogo" runat="server" ClientIDMode="Static" />
                </p>
                <br />
                <br />
                <p class="small">
                    <strong>*Logo Requirements</strong>: jpeg/jpg/png/gif file
        in RGB color mode; size: 300px - maximum width &amp; 40px -
        maximum height
                </p>
                <br />
                <br />
                <p>
                    Please select a color theme you wish to use for your
        e-commerce site:
                </p>
                <br />
                <br />
                <ul class="container4 Palette">
                    <eStore:Repeater ID="rpThemes" runat="server">
                        <itemtemplate>
                    <li class="column <%# Eval("Name")%>" >
                         <input type="radio" <%# (bool)Eval("IsSelected")?"checked='checked'":"" %>  name="storeTheme" value="<%# Eval("Name")%>" />
                    </li>
                </itemtemplate>

                    </eStore:Repeater>
                    <div class="clear"></div>
                </ul>
            </div>
        </div>
        <div class="buttons">
            <div class="container3">
                <div class="left">
                    <a href="#" class="preview">Preview</a>
                </div>
                <div class="right">
                    <asp:LinkButton ID="lbtnSubmit" CssClass="submit_btn" runat="server" OnClick="lbtnSubmit_Click1">Submit</asp:LinkButton>
                </div>
            </div>
        </div>

    </div>

    <script>
        $(document).ready(function () {
            $(".Palette li").click(function () {
                $(this).siblings().removeClass("active");
                $(this).addClass("active");
                $(this).find('input').prop("checked", "checked");
            });
            $(".preview").click(function () {
                var logo = $("<img/>");
                var template = $(":radio[name='storeTheme']:checked").val();
                if (template == "" || template == undefined)
                {
                    popupMessagewithTitle("Message", "Please Select theme first.");
                
                    return false;
                }
                if (fupLogo.files && fupLogo.files[0]) {
                    var reader = new FileReader();

                    reader.onload = function (e) {
                        logo
                          .attr('src', e.target.result).css("height", "40px");
                    };

                    reader.readAsDataURL(fupLogo.files[0]);
                }
                else if ($("#imgLogo").attr("src") != "")
                {
                    logo.attr('src', $("#imgLogo").attr("src")).css("height", "40px");
                }
                else {
                    popupMessagewithTitle("Message", "Please upload logo first.");
                    return false;
                }

                var content = $("<div></div>").append(
                        $("<div></div>").append(
                               $("<img/>").attr("src", "/App_Themes/V4/images/SiteBuilder/home-page-" + template + ".jpg")
                        )
                    )
                .append(
                        $("<div></div>").append(
                       logo
                        ).css("position", "absolute").css("left", "0px").css("top", "0px").css("display", "block")
                    ).css("position", "relative")
                ;

                popupDialog(content);
                return false;

            });


            $("#eStoreMainContent_lbtnSubmit").on("click",function () {
                if (!checkvalidate())
                {
                    return false;
                }
            });
            function checkvalidate()
            {
                var res = true;
                if (!$("ul.Palette input[name='storeTheme']").is(":checked")) {
                    $("ul.Palette").addClass("error");
                    $("ul.Palette input[name='storeTheme']").first().focus();
                    res = false;
                }
                if (!$("ul.container input[name='SelectedCategories']").is(':checked')) {
                    $("ul.container").addClass("error");
                    $("ul.container input[name='SelectedCategories']").first().focus()
                    res = false;
                }
                if (!$("#eStoreMainForm").valid())
                {
                    $(".error[aria-required]").first().focus();
                    res = false;
                }                
                return res;
            }
            $("ul.Palette input[name='storeTheme']").click(function () {
                if (!$("ul.Palette input[name='storeTheme']").is(":checked")) {
                    $("ul.Palette").addClass("error");
                }
                else {
                    $("ul.Palette").removeClass("error");
                }
            });
            $("ul.container input[name='SelectedCategories']").click(function () {
                if (!$("ul.container input[name='SelectedCategories']").is(':checked')) {
                    $("ul.container").addClass("error");
                }
                else {
                    $("ul.container").removeClass("error");
                }
            });
            $.extend($.validator.messages, {
                required: '<%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_required")%>'
            });
        });

    </script>
</asp:Content>

