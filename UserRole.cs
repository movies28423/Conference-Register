using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

/// <summary>
/// Summary description for UserRoles
/// </summary>
public class UserRoles
{
    User applicationUser;
    Role role;
    
    public UserRoles(User applicationUser)
    {
        this.applicationUser = applicationUser;
        GetRole();
    }

    public void GetRole()
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter EmailParam = new SqlParameter("@EMAIL", System.Data.SqlDbType.NVarChar, 50);
            EmailParam.Value = this.applicationUser.Email;
            string findRoleQuery = "SELECT ROLE_NAME FROM USER_ROLE WHERE EMAIL = @EMAIL;";
            SqlCommand findRole = new SqlCommand(findRoleQuery, dbConnection);
            findRole.Parameters.Add(EmailParam);
            SqlDataReader getRole = findRole.ExecuteReader();
            if (getRole.Read())
                this.role = new Role(getRole["ROLE_NAME"].ToString());
            else
                AddUserRole();
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
        }
    }

    public void AddUserRole(string roleID = "REGISTERER")
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter EmailParam = new SqlParameter("@EMAIL", System.Data.SqlDbType.NVarChar, 50);
            EmailParam.Value = this.applicationUser.Email;
            SqlParameter RoleParam = new SqlParameter("@ROLE_NAME", System.Data.SqlDbType.NVarChar, 70);
            RoleParam.Value = roleID;
            SqlParameter[] AddRoleParameters = new SqlParameter[] { EmailParam, RoleParam };
            string addRoleCommand = "INSERT INTO USER_ROLE (EMAIL, ROLE_NAME) VALUES (@EMAIL, @ROLE_NAME);";
            SqlCommand addRole = new SqlCommand(null, dbConnection);
            addRole.Parameters.AddRange(AddRoleParameters);
            addRole.CommandText = addRoleCommand;
            addRole.Prepare();
            addRole.ExecuteNonQuery();
            dbConnection.Close();
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
        }
    }

    public Role Role
	{
        get { return role; }
        set { role = value; }
	}
}
