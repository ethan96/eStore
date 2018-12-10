<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimulateFreightAdd2Cart.ascx.cs" Inherits="eStore.UI.Modules.V4.SimulateFreightAdd2Cart" %>
<table width="100%">
    <tr>
        <td width="130px">
            <strong>
                <span>Add New Item:
                </span>
            </strong>
        </td>
        <td width="130px">
            <input id="txtpn" type="text" placeholder="Please enter part No." class="orderbypartnotextbox" />
        </td>
        <td width="180px">&nbsp;&nbsp;<span>To</span>
                <select id="parentitem"></select>
        </td>
        <td>
            <button id="btnAdd2Cart" type="button">Add</button>
        </td>
    </tr>
</table>
<div id="dvcartlist"></div>