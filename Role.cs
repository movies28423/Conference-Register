using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Role
/// </summary>
public class Role
{
    private string roleID = "";
    private List<SysFeature> roleFeatures = new List<SysFeature>();

    public Role(string roleID)
	{
        this.roleID = roleID;
        GetRoles();
	}

    public void GetRoles()
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter RoleParam = new SqlParameter("@ROLE_NAME", System.Data.SqlDbType.NVarChar, 70);
            RoleParam.Value = this.roleID;
            string findRoleSetQuery = "SELECT FEATURE_NAME FROM ROLE_SET WHERE ROLE_NAME = @ROLE_NAME;";
            SqlCommand findRoleSet = new SqlCommand(findRoleSetQuery, dbConnection);
            findRoleSet.Parameters.Add(RoleParam);
            SqlDataReader getRoleSet = findRoleSet.ExecuteReader();
            if (getRoleSet.Read())
            {
                while (getRoleSet.Read())
                    this.roleFeatures.Add(new SysFeature(getRoleSet["FEATURE_NAME"].ToString()));
            }
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
        }
    }

    public string AddFeatureToRole(SysFeature feature)
    {
        if (!HasFeature(feature))
        {
            string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
            SqlConnection dbConnection = new SqlConnection(connection);
            try
            {
                dbConnection.Open();

                SqlParameter FeatureParam = new SqlParameter("@FEATURE_NAME", System.Data.SqlDbType.NVarChar, 70);
                FeatureParam.Value = feature.FeatureID;
                SqlParameter RoleParam = new SqlParameter("@ROLE_NAME", System.Data.SqlDbType.NVarChar, 70);
                RoleParam.Value = this.roleID;
                SqlParameter TimeStampParam = new SqlParameter("@SYS_TIMESTAMP", System.Data.SqlDbType.NVarChar, 70);
                TimeStampParam.Value = System.DateTime.Now;
                SqlParameter[] AddFeatureParam = new SqlParameter[] { FeatureParam, RoleParam, TimeStampParam };
                string addFeatureCommand = "INSERT INTO ROLE_SET (ROLE_NAME,FEATURE_NAME,SYS_TIMESTAMP) VALUES (@ROLE_NAME, @FEATURE_NAME, @SYS_TIMESTAMP);";
                SqlCommand addFeature = new SqlCommand(null, dbConnection);
                addFeature.Parameters.AddRange(AddFeatureParam);
                addFeature.CommandText = addFeatureCommand;
                addFeature.Prepare();
                addFeature.ExecuteNonQuery();
                dbConnection.Close();
                return feature.FeatureID + " has been added to " + this.roleID;
            }
            catch (SqlException exception)
            {
                return "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
            }
        }
        {
            return "Role already has feature";
        }
    }

    public bool HasFeature(SysFeature feature)
    {
        return roleFeatures.Find(x => x.FeatureID == feature.FeatureID) != null;
    }

    public string RoleID
    {
        get { return roleID; }
        set { roleID = value; }
    }
}
