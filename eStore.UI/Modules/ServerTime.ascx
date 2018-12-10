<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServerTime.ascx.cs" Inherits="eStore.UI.Modules.ServerTime" %>
<script type="text/javascript" language="javascript">
    function changeContactDate() {
        var startDate = document.getElementById("<%=TimeId %>ContactHourStart").value;
        var endDate = document.getElementById("<%=TimeId %>ContactHourEnd").value;
        if (startDate >= endDate) {
            alert($.eStoreLocalizaion("Best_contact_start_date_must_earlier_end_date"));
        }
    }
</script>

<select name="<%=TimeId %>ContactHourStart" class="text" id="<%=TimeId %>ContactHourStart" onchange="changeContactDate()">
    <option selected="selected">09:00</option>
    <option>09:30</option>
    <option>10:00</option>
    <option>10:30</option>
    <option>11:00</option>
    <option>11:30</option>
    <option>12:00</option>
    <option>12:30</option>
    <option>13:00</option>
    <option>13:30</option>
    <option>14:00</option>
    <option>14:30</option>
    <option>15:00</option>
    <option>15:30</option>
    <option>16:00</option>
    <option>16:30</option>
    <option>17:00</option>
    <option>17:30</option>
    <option>18:00</option>
    <option>18:30</option>
</select>
~
<select name="<%=TimeId %>ContactHourEnd" class="text" id="<%=TimeId %>ContactHourEnd" onchange="changeContactDate()">
    <option>09:00</option>
    <option>09:30</option>
    <option>10:00</option>
    <option>10:30</option>
    <option>11:00</option>
    <option>11:30</option>
    <option>12:00</option>
    <option>12:30</option>
    <option>13:00</option>
    <option>13:30</option>
    <option>14:00</option>
    <option>14:30</option>
    <option>15:00</option>
    <option>15:30</option>
    <option>16:00</option>
    <option>16:30</option>
    <option>17:00</option>
    <option>17:30</option>
    <option>18:00</option>
    <option selected="selected">18:30</option>
</select>