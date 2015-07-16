using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Security;
using System.Configuration;
using System.Web.UI.HtmlControls;

public partial class FinalizeRegistration : System.Web.UI.Page
{
    
    public void sendEmailConfirmation(Registration finalizedRegistration)
    {
        string confirmationMessageText = "Registration confirmed for: " + finalizedRegistration.RegistrationID;
        string confirmationMessageTitle = "Registration confirmed for: " + finalizedRegistration.RegistrationID;
        User confirmationMessageSender = new User("conference_register_sys@conreg.com");

        Message confirmationMessage = new Message(new List<User> { finalizedRegistration.RegisteringUser }, confirmationMessageSender, 
                                                  confirmationMessageTitle, confirmationMessageText);
    }

    public Registration CreateRegistration(User curRegisteringUser)
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            List<Conference> registeringConferences = new List<Conference>();
            List<RegistrationLine> lineItemCourses = new List<RegistrationLine>();
            HttpCookie testc = Request.Cookies["CoursesToRegisterFor"];
            foreach (string course in Request.Cookies["CoursesToRegisterFor"].Values)
            {
                Course newCourse = new Course(course);
                if (registeringConferences.ToString().Contains(((Request.Cookies["CoursesToRegisterFor"][course]))))
                {
                    lineItemCourses.Add(new RegistrationLine(newCourse));
                }
                else
                {
                    lineItemCourses.Add(new RegistrationLine(newCourse));
                    registeringConferences.Add(new Conference(Request.Cookies["CoursesToRegisterFor"][course]));
                    newCourse.getCourseInfo();
                }
            }
            if (lineItemCourses.Count > 0)
            {
                Registration thisRegistration = new Registration(curRegisteringUser, lineItemCourses);
                thisRegistration.CalculateCost();

                SqlParameter RegisteringUserParam = new SqlParameter("@REGISTERING_USER", System.Data.SqlDbType.NVarChar, 50);
                RegisteringUserParam.Value = thisRegistration.RegisteringUser.Email;

                SqlParameter CostParam = new SqlParameter("@REGISTRATION_COST", System.Data.SqlDbType.Int);
                CostParam.Value = thisRegistration.Cost;

                SqlParameter registrationID = new SqlParameter("REGISTRATION_ID", System.Data.SqlDbType.Int);
                //parameter value can be set server side and will be returned to client
                registrationID.Direction = System.Data.ParameterDirection.Output;

                SqlCommand createRegistration = new SqlCommand("CREATE_REGISTRATION", dbConnection);

                List<SqlParameter> CreateRegistrationParameters =
                        new List<SqlParameter> { RegisteringUserParam, CostParam, registrationID };

                createRegistration.CommandType = System.Data.CommandType.StoredProcedure;
                createRegistration.Parameters.AddRange(CreateRegistrationParameters.ToArray());

                createRegistration.ExecuteNonQuery();
                thisRegistration.RegistrationID = createRegistration.Parameters["REGISTRATION_ID"].Value.ToString();
                thisRegistration.AddRegistrationLine();
                dbConnection.Close();
                return thisRegistration;
            }
            else
                return new Registration("");
        }
        catch (SqlException exception)
        {
            Response.Write("<p> Error Code " + exception.Number + ": " + exception.Message + "</p>");
            return new Registration("");
        }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        string email = Request.Cookies["loginUser"]["email"].ToString();
        string firstName = Request.Cookies["loginUser"]["firstName"].ToString();
        string lastName = Request.Cookies["loginUser"]["lastName"].ToString();

        if (!IsPostBack)
        {
            Registration thisRegistration = CreateRegistration(new User(email));
            if (thisRegistration.RegistrationID != "")
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

                    form1.Controls.Add(conferenceTable);
                    form1.Controls.Add(courseGridView);
                    form1.Controls.Add(new LiteralControl("<br />"));
                }

                sendEmailConfirmation(thisRegistration);
            }
        }
        else
        {
            HtmlGenericControl infoDiv = new HtmlGenericControl("div");
            Label responseDiv = new Label();
            form1.Controls.Add(infoDiv);
            form1.Controls.Add(responseDiv);

            responseDiv.Text = "Unable to create registration at this time.";

        }
    }
}
