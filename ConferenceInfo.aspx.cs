using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

public partial class ConferenceInfo : System.Web.UI.Page
{
    void DisplayCurrentPage()
    {
        // Calculate the current page number.
        int currentPage = courseInConferenceView.PageIndex + 1;
    }
    
    protected List<ListItem> getConferences()
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        List<ListItem> availableConferences = new List<ListItem>();
        try
        {
            dbConnection.Open();

            string eventnameQuery = "SELECT EVENT_NAME FROM EVENT_INFO WHERE EVENTS_AVAILABLE = 'T';";
            SqlCommand eventName = new SqlCommand(eventnameQuery, dbConnection);
            SqlDataReader getEventNames = eventName.ExecuteReader();
            if (getEventNames.Read())
            {
                do
                {
                    availableConferences.Add(new ListItem(getEventNames["EVENT_NAME"].ToString()));
                }
                while (getEventNames.Read());
            }
            dbConnection.Close();
        }
        catch (SqlException exception)
        {
            Response.Write("<p> Error Code " + exception.Number + ": " + exception.Message + "</p>");
        }
        return availableConferences;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Cache.Remove("courseInConferenceView");
    }

    protected void courseInConferenceView_PageIndexChanging1(object sender, GridViewPageEventArgs e)
    {
        courseInConferenceView.PageIndex = e.NewPageIndex;
    }
}
