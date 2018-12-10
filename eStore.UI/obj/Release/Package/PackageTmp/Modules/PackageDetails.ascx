<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PackageDetails.ascx.cs"
    Inherits="eStore.UI.Modules.PackageDetails" %>
<div id="packingdetails" class="hiddenitem">
    <eStore:Repeater ID="rpPackingBox" runat="server">
        <HeaderTemplate>
            <table class="estoretable carttable" width="100%">
                <thead>
                    <tr class="carthead">
                        <th>
                            Index
                        </th>
                        <th>
                            Weight(lbs)
                        </th>
                        <th>
                             Width(inch)
                        </th>
                        <th>
                           Length(inch)
                        </th>
                        <th>
                            Height(inch)
                        </th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="cartitem">
                <td>
                    <%# Container.ItemIndex+1 %>
                </td>
                <td>
                    <%# Eval("Weight")%>
                </td>
                <td>
                    <%# Eval("Width")%>
                </td>
                <td>
                    <%# Eval("Length")%>
                </td>
                <td>
                    <%# Eval("Height")%>
                </td>
            </tr>
            <eStore:Repeater ID="rpPackingDetails" runat="server" DataSource='<%# Eval("PackingBoxDetails")%>'>
                <ItemTemplate>
                    <tr>
                        <td class="center">
                            <%# Container.ItemIndex+1 %>
                        </td>
                        <td>
                            <%# Eval("SProductID")%>
                        </td>
                        <td colspan="3">
                            <%# Eval("Qty")%>
                        </td>
                    </tr>
                </ItemTemplate>
            </eStore:Repeater>
        </ItemTemplate>
        <FooterTemplate>
            </tbody> </table>
        </FooterTemplate>
    </eStore:Repeater>
</div>
<script type="text/javascript">
    function showpackingdetailsDialog() {
        popupDialog("#packingdetails");
        return false;
    }
</script>
