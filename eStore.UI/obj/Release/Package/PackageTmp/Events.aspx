<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true" CodeBehind="Events.aspx.cs" Inherits="eStore.UI.Events" %>

<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
<h1><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Events)%></h1>
    <asp:GridView ID="gvEvents" runat="server" AutoGenerateColumns="False"
            Width="96%" BorderColor="gray" Height="100px" GridLines="Horizontal" 
        BorderWidth="0" CellPadding="8" CellSpacing="3" EnableViewState="False" 
        ShowHeader="False" onrowdatabound="gvEvents_RowDataBound">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink ID="hltitle" CssClass="eventsTitle" runat="server" Target="_blank" Text='<%#Eval("TITLE")%>'
                            NavigateUrl='<%#Eval("HYPER_LINK")%>'></asp:HyperLink><br />
                        <asp:Image ID="imgevent" Height="50px" runat="server" ImageUrl='<%#Eval("RECORD_IMG")%>' />
                    </ItemTemplate>
                    <ItemStyle Width="40%"/>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        Location:<%#Eval("COUNTRY")%>,<%#Eval("CITY")%><br />Booth:<%#Eval("BOOTH")%><a href='http://www.advantech.com/eventnews/events.aspx?doc_id=<%#Eval("RECORD_ID")%>'><img src="http://www.advantech.com/images/btn_see.gif" alt="" border="0" height="18"
                                width="87"></a>
                    </ItemTemplate>
                    <ItemStyle Width="20%"/>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <span class="eventsRedLine"><%# Eval("EVENT_START")%></span>-<span class="eventsRedLine"><%# Eval("EVENT_END")%></span><br />Contact Name:<%#Eval("CONTACT_NAME")%><br />Contact Email:<%#Eval("CONTACT_EMAIL")%>
                    </ItemTemplate>
                    <ItemStyle Width="40%" />
                </asp:TemplateField>
            </Columns>
       </asp:GridView>
</asp:Content>
