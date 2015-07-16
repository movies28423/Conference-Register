<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewPastConferences.aspx.cs" Inherits="ViewPastConferences" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title> View Past Conferences </title>
    <link href="gridview.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:Menu ID="Menu1" runat="server" DataSourceID="SiteMapDataSource1" Orientation="Horizontal">
    </asp:Menu>
    <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />
    <asp:HiddenField ID="UserPreference" runat="server" />
        <div id="groupBox">
            <fieldset id="FS1">
            <legend runat="server" id="Registrations"> Past Registrations </legend>
            <asp:GridView ID="PastRegisteredConferences" CssClass="GridViewStyle" runat="server" 
                AutoGenerateColumns="false" AllowPaging="true" 
                OnPageIndexChanging="PastRegisteredConferences_PageIndexChanging" OnRowCommand="PastRegisteredConferences_RowCommand">
            <FooterStyle CssClass="GridViewFooterStyle" />
            <RowStyle CssClass="GridViewRowStyle" />    
            <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
            <PagerStyle CssClass="GridViewPagerStyle" />
            <AlternatingRowStyle CssClass="GridViewAlternatingRowStyle" />
            <HeaderStyle CssClass="GridViewHeaderStyle" />
            <Columns>
                <asp:BoundField DataField="REGISTRATION_ID" HeaderText="Registration Number" />
                <asp:BoundField DataField="DATE_ENTERED" HeaderText="Registration Date" />
                <asp:BoundField DataField="COST" HeaderText="Cost" />
                <asp:TemplateField HeaderText="View Registration">
                        <ItemTemplate>
                            <asp:Button ID="viewRegistration" runat="server" Text="View Registration" 
                                CommandName="viewRegistration" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"/>
                        </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            </asp:GridView>
            </fieldset>
        </div>
        <div id="groupBox2" runat="server">
        <fieldset id="viewRegFS" runat="server">
        <legend runat="server" id="ViewRegistration" visible="false"> Registration </legend>
        
        </fieldset>
        </div>
            <footer> ConferenceRegister 2015  </footer>
    </form>
</body>
</html>
