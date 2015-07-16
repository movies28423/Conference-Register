using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for User
/// </summary>
public class User
{
    string email;
    string userID;
    string lastName;
    string firstName;
    string password;
    bool resetPassword;
    List<UserRoles> userRoles = new List<UserRoles>();

    public User(string email, string lastName, string firstName, string password)
    {
        this.email = email;
        this.lastName = lastName;
        this.firstName = firstName;
        this.password = password;
    }

    public User(string email)
    {
        this.email = email;
    }

    public string Email
    {
        get { return email; }
        set { email = value; }
    }

    public string LastName
    {
        get { return lastName; }
        set { lastName = value; }
    }

    public string FirstName
    {
        get { return firstName; }
        set { firstName = value; }
    }

    public string Password
    {
        get { return password; }
        set { password = value; 
              ResetPasswordForLogin(value); }
    }

    public string UserID
    {
        set { userID = value; }
    }

    public bool ResetPassword
    {
        get { return resetPassword; }
        set { resetPassword = value;}
    }
    public void CreateUser()
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter EmailParam = new SqlParameter("@EMAIL", System.Data.SqlDbType.NVarChar, 50);
            EmailParam.Value = this.email;
            SqlParameter FirstNameParam = new SqlParameter("@FIRST_NAME", System.Data.SqlDbType.NVarChar, 70);
            FirstNameParam.Value = this.firstName;
            SqlParameter LastNameParam = new SqlParameter("@LAST_NAME", System.Data.SqlDbType.NVarChar, 70);
            LastNameParam.Value = this.lastName;
            SqlParameter PasswordParam = new SqlParameter("@PASSWORD", System.Data.SqlDbType.NVarChar, 70);
            PasswordParam.Value = Security.HashSHA1(this.password);
            SqlParameter[] CreateUserParameters = new SqlParameter[] { EmailParam, LastNameParam, FirstNameParam, PasswordParam };
            string createUserCommand = "INSERT INTO APPLICATION_USERS (EMAIL, FIRST_NAME, LAST_NAME, PASSWORD) VALUES (@EMAIL, @FIRST_NAME, @LAST_NAME, @PASSWORD);";
            SqlCommand createUser = new SqlCommand(null, dbConnection);
            createUser.Parameters.AddRange(CreateUserParameters);
            createUser.CommandText = createUserCommand;
            createUser.Prepare();
            createUser.ExecuteNonQuery();
            dbConnection.Close();

            UserRoles createRole = new UserRoles(this);
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
        }
    }
    public void GetUserInfo()
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();
            SqlParameter EmailParam = new SqlParameter("@EMAIL", System.Data.SqlDbType.NVarChar, 50);
            EmailParam.Value = this.email;
            SqlParameter[] GetUserParameters = new SqlParameter[] { EmailParam };
            string getUserCommand = "SELECT FIRST_NAME, LAST_NAME, RESET_PASSWORD FROM APPLICATION_USERS WHERE EMAIL = @EMAIL;";
            SqlCommand getUser = new SqlCommand(null, dbConnection);
            getUser.Parameters.AddRange(GetUserParameters);
            getUser.CommandText = getUserCommand;
            getUser.Prepare();
            SqlDataReader getUserInfo = getUser.ExecuteReader();

            if(getUserInfo.Read())
            {
                this.firstName = getUserInfo["FIRST_NAME"].ToString();
                this.lastName = getUserInfo["LAST_NAME"].ToString();
                this.resetPassword = getUserInfo["RESET_PASSWORD"].ToString() == "T";
            }

            dbConnection.Close();
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
        }
    }

    public void ResetPasswordForLogin(string newPassword)
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter EmailParam = new SqlParameter("@EMAIL", System.Data.SqlDbType.NVarChar, 50);
            EmailParam.Value = this.email;
            SqlParameter PasswordParam = new SqlParameter("@PASSWORD", System.Data.SqlDbType.NVarChar, 70);
            PasswordParam.Value = Security.HashSHA1(newPassword);
            SqlParameter[] ResetPasswordParameters = new SqlParameter[] { EmailParam, PasswordParam };
            string resetPasswordCommand = "UPDATE APPLICATION_USERS SET PASSWORD = @PASSWORD WHERE EMAIL = @EMAIL;";
            SqlCommand resetPassword = new SqlCommand(null, dbConnection);
            resetPassword.Parameters.AddRange(ResetPasswordParameters);
            resetPassword.CommandText = resetPasswordCommand;
            resetPassword.Prepare();
            resetPassword.ExecuteNonQuery();
            dbConnection.Close();
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
        }
    }

    public void ResetOnNextLogin(string reset)
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter EmailParam = new SqlParameter("@EMAIL", System.Data.SqlDbType.NVarChar, 50);
            EmailParam.Value = this.email;
            SqlParameter ResetParam = new SqlParameter("@RESET", System.Data.SqlDbType.Char, 1);
            ResetParam.Value = reset;

            SqlParameter[] ResetNextLoginParameters = new SqlParameter[] { EmailParam, ResetParam };
            string resetNextLoginCommand = "UPDATE APPLICATION_USERS SET RESET_PASSWORD = @RESET WHERE EMAIL = @EMAIL;";
            SqlCommand resetNextLogin = new SqlCommand(null, dbConnection);
            resetNextLogin.Parameters.AddRange(ResetNextLoginParameters);
            resetNextLogin.CommandText = resetNextLoginCommand;
            resetNextLogin.Prepare();
            resetNextLogin.ExecuteNonQuery();
            dbConnection.Close();
            this.resetPassword = true;
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
        }
    }
}
