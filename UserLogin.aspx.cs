/*************************************************************
 * Backend for User Login Page
 * Contains methods for determining if a user exists 
 * && user's password matches
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Net.Mail;

public partial class UserLogin : System.Web.UI.Page
{
    public static string SendTemporaryPassword(User passwordReceiver)
    {
        MailMessage m = new MailMessage();
        SmtpClient sc = new SmtpClient();
        try
        {
            m.From = new MailAddress("conferenceregistersys@gmail.com", "Display name");
            m.To.Add(new MailAddress(passwordReceiver.Email, "Display name To"));

            m.Subject = "ConferenceRegister Password Reset";
            m.IsBodyHtml = true;
            string newPassword = Security.GenerateRandomPassord();
            m.Body = "Dear " + passwordReceiver.FirstName + " " + passwordReceiver.LastName + ", <br> Your new password is: " + newPassword;
            m.Body += "<br> On your next loging you will be prompted to reset your password.";

            sc.Host = "smtp.gmail.com";
            sc.Port = 587;
            sc.Credentials = new
            System.Net.NetworkCredential("conferenceregistersys@gmail.com", "ConReg01");
            sc.EnableSsl = true;
            sc.Send(m);

            passwordReceiver.Password = newPassword;
            passwordReceiver.ResetOnNextLogin("T");
            
            return "Message sent to email address: " + passwordReceiver.Email;
        }
        catch(SmtpException e)
        {
           return "<p> Error Code " + e.Data + ": " + e.Message + "</p>";
        }
    }

    //query the db to see if a record is found with the email the user entered
    protected bool userExists(string email)
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter EmailParam = new SqlParameter("@EMAIL", System.Data.SqlDbType.NVarChar, 50);
            EmailParam.Value = email;

            string findEmailQuery = "SELECT EMAIL FROM APPLICATION_USERS WHERE UPPER(EMAIL) = UPPER(@EMAIL);";
            SqlCommand findEmail = new SqlCommand(findEmailQuery, dbConnection);
            findEmail.Parameters.Add(EmailParam);
            SqlDataReader getEmail = findEmail.ExecuteReader();
            bool emailExists = getEmail.Read();
            return emailExists;
        }
        catch (SqlException exception)
        {
            Response.Write("<p> Error Code " + exception.Number + ": " + exception.Message + "</p>");
            return true;
        }
        finally
        {
            dbConnection.Close();
        }
    }

    //query the db to see if a record is found with the password the user entered
    public bool passwordMatches(string password)
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter PasswordParam = new SqlParameter("@PASSWORD", System.Data.SqlDbType.NVarChar, 70);
            PasswordParam.Value = Security.HashSHA1(password);

            string findPasswordQuery = "SELECT EMAIL FROM APPLICATION_USERS WHERE PASSWORD = @PASSWORD;";
            SqlCommand findPassword = new SqlCommand(findPasswordQuery, dbConnection);
            findPassword.Parameters.Add(PasswordParam);
            SqlDataReader getPassword = findPassword.ExecuteReader();
            bool emailPassword = getPassword.Read();
            return emailPassword;
        }
        catch (SqlException exception)
        {
            Response.Write("<p> Password not found. Error Code " + exception.Number + ": " + exception.Message + "</p>");
            return true;
        }
        finally
        {
            dbConnection.Close();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Cookies["loginUser"] != null && Request.Cookies["loginUser"]["email"] != null && IsPostBack)
            FormsAuthentication.SignOut();
        if (newUserCB.Checked)
        {
            userView.SetActiveView(registeringView);
        }
        else
        {
            userView.SetActiveView(loginView);
        }
    }

    protected void notExistingUserRB_CheckedChanged(object sender, EventArgs e)
    {
    }
    protected void newUserCB_CheckedChanged(object sender, EventArgs e)
    {

    }
    protected void SubmitNameLog_Click(object sender, EventArgs e)
    {
        const string LOGIN_ROLE = "LOGIN";
        if (userExists(EmailLogTB.Text))
        {
            User existingUser = new User(EmailLogTB.Text);
            existingUser.GetUserInfo();
            if (passwordMatches(PasswordLogTB.Text))
            {
                UserRoles userRole = new UserRoles(existingUser);
                if (userRole.Role.HasFeature(new SysFeature(LOGIN_ROLE)))
                {
                    if (existingUser.ResetPassword)
                    {
                        PasswordLogTB.Text = "";
                        Label ResetPasswordConfirm = new Label();
                        ResetPasswordConfirm.Text = "<br> Confirm Password:";
                        loginView.Controls.Add(ResetPasswordConfirm);
                        TextBox ResetPasswordConfirmTB = new TextBox();
                        ResetPasswordConfirmTB.TextMode = TextBoxMode.Password;
                        loginView.Controls.Add(ResetPasswordConfirmTB);

                        userView.SetActiveView(loginView);
                        EmailLogTB.Text = existingUser.Email;

                        existingUser.ResetOnNextLogin("F");
                        Label SignInLabel = new Label();
                        SignInLabel.Text = "<br> Password Reset Successful, please enter password above to sign in.";
                        loginView.Controls.Add(SignInLabel);
                    }
                    else
                    {
                        HttpCookie loginUser = new HttpCookie("loginUser");
                        loginUser.Values["email"] = existingUser.Email;
                        loginUser.Values["firstName"] = existingUser.FirstName;
                        loginUser.Values["lastName"] = existingUser.LastName;
                        loginUser.Expires = DateTime.Now.AddDays(7);
                        loginUser.Path = "/";
                        Response.Cookies.Add(loginUser);
                        Response.Redirect("/UserHomePage.aspx");
                    }
                }
                else
                {
                    Label InsufficientLoginLabel = new Label();
                    InsufficientLoginLabel.Text = "<br> Unable to login, insufficient rights. please contact your system administrator.";
                    loginView.Controls.Add(InsufficientLoginLabel);
                }
            }
        }
    }

    //if the username entered returns a record in the db and the password matches
    //log the user into the system and redirect to the user home page
    protected void SubmitNameReg_Click1(object sender, EventArgs e)
    {
        if (!userExists(emailRegTB.Text))
        {
            User newUser = new User(emailRegTB.Text, firstnameRegTB.Text, lastnameRegTB.Text, PasswordRegTB.Text);
            newUser.CreateUser();
            userView.SetActiveView(loginView);
            newUserCB.Checked = false;
            EmailLogTB.Text = newUser.Email;
            
            Label SignInLabel = new Label();
            SignInLabel.Text = "<br> Registration Successful, please enter password above to sign in.";
            loginView.Controls.Add(SignInLabel);
        }
        else
        {
            NewEmailValidatior.IsValid = false;
        } 
    }
    protected void newUserCB_CheckedChanged1(object sender, EventArgs e)
    {

    }
    protected void RetrievePassword_Click(object sender, EventArgs e)
    {
        RetrievePassword.Visible = false;
        User existingUser = new User(passwordLookup.Text);
        string sentToAdress = SendTemporaryPassword(existingUser);
        SuccessfulEmailFound.Text = sentToAdress;
    }
    protected void passwordLookup_TextChanged(object sender, EventArgs e)
    {
        if(!userExists(passwordLookup.Text))
        {
            ExistingUser.IsValid = false;
        }
        else
        {
            RetrievePassword.Visible = true;
        }
    }
}
