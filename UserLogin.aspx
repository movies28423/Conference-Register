             <%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserLogin.aspx.cs" Inherits="UserLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Login</title>
    <link href="userlogin.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="loginForm" runat="server">
    <div id="wrapperHeader">
     <div id="header">
      <img src="images/topbar.jpg" alt="logo" />
     </div> 
    </div>
    <div id="groupBox">
    <fieldset id="FS1">
    <legend> User Login </legend>
    <asp:Label runat="server">Register as new user:</asp:Label>
    <asp:CheckBox ID="newUserCB" OnCheckedChanged="newUserCB_CheckedChanged1"
            runat="server" 
            autopostback="true" />  
     <br />
     <asp:MultiView ID="userView" runat="server">
         <asp:View ID="registeringView" runat="server">
             <asp:Label ID="EmailReg" runat ="server"> Email Address: </asp:Label>
				<asp:TextBox ID="emailRegTB" TextMode="Email" runat="server"> </asp:TextBox>
					<asp:CustomValidator ID="NewEmailValidatior" runat="server" ControlToValidate="EmailLogTB" ErrorMessage="Email Address exists, please enter a differnent email address or click on retrieve user information below."></asp:CustomValidator>
					<br />
				<asp:Label ID="FirstNameReg" runat="server"> First Name: </asp:Label>
				<asp:TextBox ID="firstnameRegTB" runat="server"> </asp:TextBox>
					<br />
				<asp:Label ID="LastNameReg" runat="server"> Last Name: </asp:Label>
				<asp:TextBox ID="lastnameRegTB" runat="server"> </asp:TextBox>
					<br />
				<asp:Label ID="PasswordReg" runat="server"> Password: </asp:Label>
				<asp:TextBox ID="PasswordRegTB" TextMode="Password" runat="server"> </asp:TextBox>
					<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                        ErrorMessage="Please enter a password that is between 8 &amp; 15 characters long and contains a number, upper case letter and symbol(eg. @, #, !)" 
                        ValidationExpression="^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{8,15}$" 
                        ControlToValidate="PasswordRegTB" ValidationGroup="validateForm" Display="Dynamic" ForeColor="#990000"></asp:RegularExpressionValidator>
					<br />
				<asp:Label ID="RenterPasswordReg" runat="server"> Re-enter Password: </asp:Label>
				<asp:TextBox ID="RenterPasswordRegTB" TextMode="Password" runat="server"> </asp:TextBox>
					<br />
				<asp:Button ID="SubmitNameReg" Text="Register" runat="server" OnClick="SubmitNameReg_Click1" 
                    ValidationGroup="validateForm"/>
         </asp:View>
          <asp:View ID="loginView" runat="server">
                 <asp:Label ID="EmailLog" runat ="server"> Email Address: </asp:Label>
                 <asp:TextBox ID="EmailLogTB" TextMode="Email" runat="server"> </asp:TextBox>
                 <br />
                 <asp:Label ID="PasswordLog" runat="server"> Password: </asp:Label>
                 <asp:TextBox ID="PasswordLogTB" TextMode="Password" runat="server"> </asp:TextBox>
             <asp:Button ID="SubmitNameLog" Text="Login" runat="server" OnClick="SubmitNameLog_Click"/>
         </asp:View>
    </asp:MultiView>&nbsp;<br />
    </fieldset>
    </div>
    <div id="groupBox2">
    <fieldset id="FS1">
    <legend> Forgot User Information </legend>
        <asp:Label runat="server">Enter email to receive temporary password:</asp:Label>
        <asp:TextBox ID="passwordLookup" TextMode="Email" OnTextChanged="passwordLookup_TextChanged" AutoPostBack="true"  runat="server"> </asp:TextBox>
        <asp:CustomValidator ID="ExistingUser" runat="server" ControlToValidate="passwordLookup" Display="Dynamic" 
            ErrorMessage="User not found, please enter different email address."
            ForeColor="#0066CC" ValidationGroup="UserFound"></asp:CustomValidator>
        <asp:Button ID="RetrievePassword" runat="server" OnClick="RetrievePassword_Click" ValidationGroup="UserFound" Text="Get New Password"/>
        <asp:Label ID="SuccessfulEmailFound" runat="server"></asp:Label>
    </fieldset>
    </div>
    </form>
</body>
</html>
