using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.Security;
using System.Configuration;

public partial class ViewPastConferences : System.Web.UI.Page
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

    private void bindRegistrationsGridView(string registeringEmail)
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();


            SqlParameter EmailParam = new SqlParameter("@EMAIL", System.Data.SqlDbType.NVarChar, 50);
            EmailParam.Value = registeringEmail;

            string getRegistration =
                               "SELECT REGISTRATION_NUMBER AS REGISTRATION_ID, DATE_ENTERED, COST " +
                               "FROM REGISTRATION " +
                               "WHERE EMAIL = @EMAIL;";
            SqlCommand retrieveRegistration = new SqlCommand(getRegistration, dbConnection);
            retrieveRegistration.Parameters.Add(EmailParam);
            SqlDataAdapter adaptEvents = new SqlDataAdapter(retrieveRegistration);
            DataSet dsEvents = new DataSet();
            adaptEvents.Fill(dsEvents);
            PastRegisteredConferences.DataSource = dsEvents.Tables[0];
            PastRegisteredConferences.DataBind();
            CapitalizeCase(PastRegisteredConferences);
            dbConnection.Close();
        }
        catch (SqlException exception)
        {
            Response.Write("<p> Error Code " + exception.Number + ": " + exception.Message + "</p>");
        }
    }

    protected void viewRegistration(Registration thisRegistration)
    {
        var getGridViews = from rl in thisRegistration.RegisteringLine
                           select new
                           {
                               rl.LineCourse.CourseConference
                           };

        foreach (var conferenceCourse in getGridViews.Distinct())
        {
            Conference currentConference = new Conference(conferenceCourse.CourseConference);
            Table conferenceTable = new Table();
            TableRow headerRow = new TableRow();
            TableCell headerConferenceInfo = new TableCell();

            headerConferenceInfo.Text = "Conference Information:";

            TableRow valueRow = new TableRow();
            TableCell valueConferenceInfo = new TableCell();

            valueConferenceInfo.Text = conferenceCourse.CourseConference + " Dates: " +
                currentConference.StartDate.ToShortDateString() + "-" + currentConference.EndDate.ToShortDateString();

            TableCell[] headerRowCells = new TableCell[] { headerConferenceInfo };
            headerRow.Cells.AddRange(headerRowCells);
            TableCell[] valueRowCells = new TableCell[] { valueConferenceInfo };
            valueRow.Cells.AddRange(valueRowCells);

            TableRow[] conferenceTableRows = new TableRow[] { headerRow, valueRow };
            conferenceTable.Rows.AddRange(conferenceTableRows);
            conferenceTable.CssClass = "TableStyle";
            headerRow.CssClass = "TableFirstRowStyle";
            valueRow.CssClass = "TableRowStyle";

            GridView courseGridView = new GridView();

            var getCourseViewDate = from rl in thisRegistration.RegisteringLine
                                    where rl.LineCourse.CourseConference == conferenceCourse.CourseConference
                                    select new
                                    {
                                        CLASS_NAME = rl.LineCourse.CourseName.ToString(),
                                        EVENT_DATE = rl.LineCourse.StartDttm.Date.ToString(),
                                        START_TIME = rl.LineCourse.StartDttm.TimeOfDay.ToString(),
                                        END_TIME = rl.LineCourse.EndDttm.TimeOfDay.ToString(),
                                        COST = (rl.LineCourse.Cost == 0 ? "--" : rl.LineCourse.Cost.ToString())
                                    };

            valueConferenceInfo.Text += " Cost: ";
            float conferenceCost = getCourseViewDate.Sum(x =>
            {
                return (x.COST != "--") ? float.Parse(x.COST) : 0;
            });
            if (conferenceCost == 0)
            {
                valueConferenceInfo.Text += currentConference.WholeConferenceCost;
            }
            else
            {
                valueConferenceInfo.Text += conferenceCost;
            }
            courseGridView.DataSource = getCourseViewDate;
            courseGridView.DataBind();
            courseGridView.CssClass = "GridViewStyle";
            courseGridView.RowStyle.CssClass = "GridViewRowStyle";
            courseGridView.FooterStyle.CssClass = "GridViewFooterStyle";
            courseGridView.SelectedRowStyle.CssClass = "GridViewSelectedRowStyle";
            courseGridView.PagerStyle.CssClass = "GridViewPagerStyle";
            courseGridView.AlternatingRowStyle.CssClass = "GridViewAlternatingRowStyle";
            courseGridView.HeaderStyle.CssClass = "GridViewHeaderStyle";

            viewRegFS.Controls.Add(conferenceTable);
            viewRegFS.Controls.Add(courseGridView);
            viewRegFS.Controls.Add(new LiteralControl("<br />"));
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string email = Request.Cookies["loginUser"]["email"].ToString();

        if (!IsPostBack)
        {
            bindRegistrationsGridView(email);

            foreach (GridViewRow row in PastRegisteredConferences.Rows)
            {
                TableCell rowCell = row.Cells[3];
                ControlCollection cont = row.Cells[3].Controls;
                Button lb = (Button)row.Cells[3].Controls[1];
                lb.Text = "View Registration";
            }
        }
    }
    protected void PastRegisteredConferences_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        string email = Request.Cookies["loginUser"]["email"].ToString();
        PastRegisteredConferences.PageIndex = e.NewPageIndex;

        bindRegistrationsGridView(email);
    }

    protected void PastRegisteredConferences_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "viewRegistration")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = PastRegisteredConferences.Rows[index];
            ViewRegistration.Visible = true;
            viewRegistration(new Registration(PastRegisteredConferences.Rows[index].Cells[0].Text));
        }
    }
}
