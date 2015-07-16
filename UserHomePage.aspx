<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserHomePage.aspx.cs" Inherits="UserHomePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title> User Home Page </title>
    <link href="gridview.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">    
        <asp:Menu ID="Menu1" runat="server" DataSourceID="SiteMapDataSource1" Orientation="Horizontal"> 
        </asp:Menu>
        <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server"/>
    <asp:Label ID="UserInfoLbl" runat="server"></asp:Label>
    <div id="groupBox">
    <fieldset id="FS1">
    <legend runat="server" id="MessageLegend"> Messages </legend>
    <asp:MultiView ID="messageView" runat="server">
        <asp:View ID="messages" runat="server">
		<asp:GridView ID="usersMessages" CssClass="GridViewStyle" runat="server"
                      AutoGenerateColumns="false" AllowSorting ="true" AllowPaging="true" SelectedIndex="0" OnRowCommand="UserMessage_RowCommand"> 
				<FooterStyle CssClass="GridViewFooterStyle" />
				<RowStyle CssClass="GridViewRowStyle" />    
				<SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
				<PagerStyle CssClass="GridViewPagerStyle" />
				<AlternatingRowStyle CssClass="GridViewAlternatingRowStyle" />
				<HeaderStyle CssClass="GridViewHeaderStyle" />
				<Columns>
					<asp:BoundField DataField="MESSAGE_SENDER" HeaderText="Email Sender"/>
					<asp:BoundField DataField="MESSAGE_TITLE" HeaderText="Course Name" />
					<asp:BoundField DataField="MESSAGE_TIME" HeaderText="When Sent" />
					<asp:BoundField DataField="MESSAGE_ID"/>
                    <asp:BoundField DataField="MESSAGE_OPENED"/>
                    <asp:TemplateField HeaderText="Open Message">
                        <ItemTemplate>
                            <asp:Button ID="OpenMessageBtn" runat="server" Text="Open Message" 
                                CommandName="SetOpenMessage" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"/>
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
		 </asp:GridView>
         </asp:View>
         <asp:View ID="indvMessage" runat="server">
             <asp:Table ID="MessageTbl" runat="server" BorderWidth="1" Width="800">
                 <asp:TableHeaderRow>
                    <asp:TableHeaderCell ColumnSpan="2">
                        <asp:Label ID="EmailInfo" runat="server"> </asp:Label>  
                        <asp:Button ID="BackToMessages" runat="server" OnClick="BackToMessages_Click" Text="Back to Messages"/>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableHeaderCell ColumnSpan="1" Height="400">
                        <asp:Label ID="EmailText" runat="server"> </asp:Label>
                    </asp:TableHeaderCell>
                </asp:TableRow>
                <asp:TableFooterRow>
                    <asp:TableCell>
                        <<asp:Button ID="DeleteMessage" runat="server" OnClick="DeleteMessage_Click" Text="Delete Message"/>
                    </asp:TableCell>
                </asp:TableFooterRow>
             </asp:Table>
         </asp:View>
    </asp:MultiView>&nbsp;<br />
  	</fieldset>
    </div>
    <footer> ConferenceRegister 2015  </footer>
    </form>
</body>
</html>
