<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConferenceInfo.aspx.cs" Inherits="ConferenceInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Conference Info </title>
</head>
<body>
    <form id="conferences" runat="server">
    <asp:Label ID="availableConferences" runat="server"> View available conferences: </asp:Label>
        <br />
    <asp:DropDownList ID="conferencesInDB" runat="server" AutoPostBack="true"> </asp:DropDownList>
        <br />
    <asp:Label ID="availableEvents" runat="server"> Events for conferences: </asp:Label>
        <br />
    <asp:GridView ID="courseInConferenceView" OnPageIndexChanging="courseInConferenceView_PageIndexChanging1" runat="server" AutoGenerateColumns="false">
    <Columns>
        <asp:BoundField DataField="CLASS_NAME" HeaderText="Course Name" />
        <asp:BoundField DataField="EVENT_DATE" HeaderText="Date" />
        <asp:BoundField DataField="START_TIME" HeaderText="Start Time" />
        <asp:BoundField DataField="END_TIME" HeaderText="End Time" />
        <asp:BoundField DataField="COST" HeaderText="Course Cost" />
        <asp:BoundField DataField="SPOTS_REMAINING" HeaderText="Spots Remaining" />
    </Columns>
    </asp:GridView>
     <div id="footer"> ConferenceRegister 2015 </div>    
    </form>
</body>
</html>
