/*************************************************************
 * Backend for User Menu Page
 * Options in Menu:
 *      View All Conferences
 *      View Single Conference
 *      View Past Registrations
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

public partial class RegisterForConference : System.Web.UI.Page
{
    public static void CapitalizeCase(GridView value)
    {
        foreach (GridViewRow row in value.Rows)
        {
            foreach (DataControlFieldCell Cell in row.Cells)
            {
                if (Cell.Text.Length > 1)
                {
                    Cell.Text = Cell.Text[0] + Cell.Text.Substring(1).ToLower();
                }
            }
        }
    }

   private void bindEventsGridView(string email)
   {
       string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
       SqlConnection dbConnection = new SqlConnection(connection);
       try
       {
           dbConnection.Open();


           SqlParameter EmailParam = new SqlParameter("@EMAIL", System.Data.SqlDbType.NVarChar, 50);
           EmailParam.Value = email;

           string getEvents = "SELECT EI.EVENT_NAME, EC.CLASS_NAME, " +
                              "LEFT(CONVERT(VARCHAR, EC.START_DTTM, 101), 10) AS EVENT_DATE, " +
                              "CAST(EC.START_DTTM AS TIME) AS START_TIME, " +
                              "CAST(EC.END_DTTM AS TIME) AS END_TIME, " +
                              "EC.COST, " +
                              "EC.MAX_ENROLLMENT - COALESCE(EC.CURRENT_ENROLLMENT, 0) AS SPOTS_REMAINING " +
                              "FROM EVENT_CLASSES EC " +
                              "JOIN EVENT_INFO EI ON EC.EVENT_ID = EI.EVENT_ID " +
                              "WHERE COALESCE(EC.CURRENT_ENROLLMENT, 0) < EC.MAX_ENROLLMENT AND " +
                              "      EC.CLASS_ID NOT IN (SELECT RL.CLASS_ID " +
	                          "                          FROM REGISTRATION_LINE RL " +
						      "                          JOIN REGISTRATION R ON R.REGISTRATION_NUMBER = RL.REGISTRATION_NUMBER " +
						      "                          WHERE R.EMAIL = @EMAIL) " +
                              "ORDER BY EI.EVENT_NAME;";

           SqlCommand retrieveEvents = new SqlCommand(null, dbConnection);
           retrieveEvents.Parameters.Add(EmailParam);
           retrieveEvents.CommandText = getEvents;
           retrieveEvents.Prepare();
           SqlDataAdapter adaptEvents = new SqlDataAdapter(retrieveEvents);
           DataSet dsEvents = new DataSet();
           adaptEvents.Fill(dsEvents);
           coursesForAllConferences.DataSource = dsEvents.Tables[0];
           coursesForAllConferences.DataBind();
           CapitalizeCase(coursesForAllConferences);
           dbConnection.Close();
       }
       catch (SqlException exception)
       {
           Response.Write("<p> Error Code " + exception.Number + ": " + exception.Message + "</p>");
       }
   }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ConferencesToRegisterDiv.Style["display"] = "inline";
            string email = Request.Cookies["loginUser"]["email"].ToString();
            bindEventsGridView(email);

            UserPreference.Value = "ALL_CONFERENCES";

            if (coursesForAllConferences.Rows.Count == 0)
            {
                noRows.Visible = true;
                RegisterForCourses.Visible = false;
            }
        }
    }

    protected void RegisterForCourses_Click(object sender, EventArgs e)
    {
        HttpCookie ConferencesCoursesRegistering = new HttpCookie("CoursesToRegisterFor");
        ConferencesCoursesRegistering.Expires = DateTime.Now.AddMinutes(20);
        ConferencesCoursesRegistering.Path = "/";
        Dictionary<string, Dictionary<string, float>> coursesToRegisterFor = new Dictionary<string, Dictionary<string, float>>();
        List<Conference> chosenConferences = new List<Conference>();

        int coursesChecked = 0;
        foreach (GridViewRow row in coursesForAllConferences.Rows)
        {
            Conference newConference = new Conference(row.Cells[0].Text);
            CheckBox registerForCourse = ((CheckBox)row.Cells[7].FindControl("RegisterForEventCB"));
            if (registerForCourse.Checked)
            {
                coursesChecked += 1;
                ConferencesCoursesRegistering.Values.Add(row.Cells[1].Text, row.Cells[0].Text);
            }
        }
            
        if (coursesChecked > 0)
        {
            Response.Cookies.Add(ConferencesCoursesRegistering);

            Response.Redirect("FinalizeRegistration.aspx");
        }
        else
        {
            OneEventChosen.IsValid = false;
        }
    }
    protected void coursesForAllConferences_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        string email = Request.Cookies["loginUser"]["email"].ToString();
        coursesForAllConferences.PageIndex = e.NewPageIndex;
        bindEventsGridView(email);
    }
}
