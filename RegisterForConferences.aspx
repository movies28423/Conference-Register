<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RegisterForConference.aspx.cs" Inherits="RegisterForConference" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Register For Conferences</title>
    <link href="gridview.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:Menu ID="Menu1" runat="server" DataSourceID="SiteMapDataSource1" Orientation="Horizontal">
        </asp:Menu>
        <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />
        <asp:HiddenField ID="UserPreference" runat="server" />
        <div id="ConferencesToRegisterDiv" style="display: none;" runat="server">
            <div id="groupBox">
            <fieldset id="FS1">
            <legend runat="server" id="MessageLegend"> All Available Events </legend>
            <asp:Label ID="noRows" Visible ="false" runat="server">No conferences to register for at this time.</asp:Label>
            <asp:GridView ID="coursesForAllConferences" CssClass="GridViewStyle" runat="server" AutoGenerateColumns="false"
                AllowSorting="true" AllowPaging="true" 
                OnPageIndexChanging="coursesForAllConferences_PageIndexChanging"> 
            <FooterStyle CssClass="GridViewFooterStyle" />
            <RowStyle CssClass="GridViewRowStyle" />    
            <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
            <PagerStyle CssClass="GridViewPagerStyle" />
            <AlternatingRowStyle CssClass="GridViewAlternatingRowStyle" />
            <HeaderStyle CssClass="GridViewHeaderStyle" />
            <Columns>
                <asp:BoundField DataField="EVENT_NAME" HeaderText="Event Name"/>
                <asp:BoundField DataField="CLASS_NAME" HeaderText="Course Name" />
                <asp:BoundField DataField="EVENT_DATE" HeaderText="Date" />
                <asp:BoundField DataField="START_TIME" HeaderText="Start Time" />
                <asp:BoundField DataField="END_TIME" HeaderText="End Time" />
                <asp:BoundField DataField="COST" HeaderText="Course Cost" />
                <asp:BoundField DataField="SPOTS_REMAINING" HeaderText="Spots Remaining" />
                <asp:TemplateField HeaderText="Register For Event">
                        <ItemTemplate>
                            <asp:CheckBox ID="RegisterForEventCB" runat="server" />
                        </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            </asp:GridView>
            <asp:Button ID="RegisterForCourses" Text="Click to Register" runat="server" OnClick="RegisterForCourses_Click" 
                CausesValidation="true" ValidationGroup="FindCourse"/>
            <asp:CustomValidator ID="OneEventChosen" ForeColor="Red"
                ErrorMessage="Please check at least one course to register for." 
                ValidationGroup="FindCourse"
                runat="server"></asp:CustomValidator>
          	</fieldset>
         </div>
        </div>
        <footer> ConferenceRegister 2015  </footer>
    </form>
</body>
</html>
