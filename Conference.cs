using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Each conference is made up of several courses
/// </summary>
public class Conference
{
    private string conferenceID;
    private string conferenceName;
    private DateTime startDate;
    private DateTime endDate;
    private float wholeConferenceCost;
    private bool coursesAvailable;
    private string location;
    private List<Course> coursesInConference = new List<Course>();

    public Conference(string conferenceName, DateTime startDate, DateTime endDate, float wholeConferenceCost)
	{
        this.conferenceName = conferenceName;
        this.startDate = startDate;
        this.endDate = endDate;
        this.wholeConferenceCost = wholeConferenceCost;
	}

    public Conference(string conferenceName, DateTime startDate, DateTime endDate)
    {
        this.conferenceName = conferenceName;
        this.startDate = startDate;
        this.endDate = endDate;
    }

    public Conference(string conferenceName)
    {
        this.conferenceName = conferenceName;

        getConferenceInfo();
        getCourseList();
    }

    public string ConferenceID
    {
        get { return conferenceID; }
        set { conferenceID = value; }
    }

    public string ConferenceName
    {
        get { return conferenceName; }
        set { conferenceName = value; }
    }

    public DateTime StartDate
    {
        get { return startDate; }
        set { startDate = value; }
    }

    public DateTime EndDate
    {
        get { return endDate; }
        set { endDate = value; }
    }

    public float WholeConferenceCost
    {
        get { return wholeConferenceCost; }
        set { wholeConferenceCost = value; }
    }

    public bool CoursesAvailable 
    {
        get { return coursesAvailable; }
        set { coursesAvailable = value; }
    }

    public List<Course> CoursesInConference
    {
        get { return coursesInConference; }
        set { coursesInConference = value; }
    }

    public void CreateConference(List<Course> NewCourses)
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter ConferenceNameParam = new SqlParameter("@EVENT_NAME", System.Data.SqlDbType.NVarChar, 70);
            ConferenceNameParam.Value = this.conferenceName;
            SqlParameter LocationParam = new SqlParameter("@LOCATION", System.Data.SqlDbType.NVarChar, 70);
            LocationParam.Value = this.location;
            SqlParameter StartDateParam = new SqlParameter("@START_DT", System.Data.SqlDbType.DateTime);
            StartDateParam.Value = this.startDate;
            SqlParameter EndDateParam = new SqlParameter("@END_DT", System.Data.SqlDbType.DateTime);
            StartDateParam.Value = this.endDate;
            SqlParameter CostParam = new SqlParameter("@COST", System.Data.SqlDbType.Float);
            CostParam.Value = this.wholeConferenceCost;
            SqlParameter[] CreateConferenceParameters = new SqlParameter[] { ConferenceNameParam, LocationParam, StartDateParam, EndDateParam, CostParam };
            SqlCommand createConference = new SqlCommand("CREATE_CONFERENCE", dbConnection);
            createConference.CommandType = System.Data.CommandType.StoredProcedure;
            createConference.Parameters.AddRange(CreateConferenceParameters.ToArray());

            createConference.ExecuteNonQuery();
            this.conferenceID = createConference.Parameters["EVENT_ID"].Value.ToString();
            dbConnection.Close();

            foreach (Course newCourse in coursesInConference)
                newCourse.CreateCourse(this);
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
        }
    }

    protected void getConferenceInfo()
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter ConferenceNameParam = new SqlParameter("@CONFERENCE_NAME", System.Data.SqlDbType.NVarChar, 70);
            ConferenceNameParam.Value = this.conferenceName;
            SqlCommand eventInfo = new SqlCommand(null, dbConnection);
            eventInfo.Parameters.Add(ConferenceNameParam);
            string eventInfoQuery = "SELECT START_DT, END_DT, COST FROM EVENT_INFO WHERE EVENT_NAME = @CONFERENCE_NAME;";
            eventInfo.CommandText = eventInfoQuery;
            eventInfo.Prepare();
            SqlDataReader getEventInfo = eventInfo.ExecuteReader();
            if (getEventInfo.Read())
            {
                do
                {
                    this.startDate = DateTime.Parse(getEventInfo["START_DT"].ToString());
                    this.endDate = DateTime.Parse(getEventInfo["END_DT"].ToString());
                    this.wholeConferenceCost = float.Parse(getEventInfo["COST"].ToString());
                }
                while (getEventInfo.Read());
            }
        }
        catch (SqlException exception)
        {
            //Response.Write("<p> Error Code " + exception.Number + ": " + exception.Message + "</p>");
        }
        finally
        {
            dbConnection.Close();
        }
    }

    protected void getCourseList()
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter ConferenceNameParam = new SqlParameter("@CONFERENCE_NAME", System.Data.SqlDbType.NVarChar, 70);
            ConferenceNameParam.Value = this.conferenceName;
            SqlCommand eventCourses = new SqlCommand(null, dbConnection);
            eventCourses.Parameters.Add(ConferenceNameParam);
            string eventCoursesQuery = "SELECT CLASS_NAME FROM EVENT_CLASSES EC JOIN EVENT_INFO EI ON EI.EVENT_ID = EC.EVENT_ID " +
                                       "WHERE EI.EVENT_NAME = @CONFERENCE_NAME;";
            eventCourses.CommandText = eventCoursesQuery;
            eventCourses.Prepare();
            SqlDataReader getEventCourses = eventCourses.ExecuteReader();
            if (getEventCourses.Read())
            {
                do
                {
                    this.coursesInConference.Add(new Course(getEventCourses["CLASS_NAME"].ToString()));
                }
                while (getEventCourses.Read());
            }
        }
        catch (SqlException exception)
        {
            //Response.Write("<p> Error Code " + exception.Number + ": " + exception.Message + "</p>");
        }
        finally
        {
            dbConnection.Close();
        }
    }

    public float[] GetPreviousRegCoursesInConference(string email)
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter ConferenceNameParam = new SqlParameter("@CONFERENCE_NAME", System.Data.SqlDbType.NVarChar, 70);
            ConferenceNameParam.Value = this.conferenceName;
            SqlParameter EmailParam = new SqlParameter("@EMAIL", System.Data.SqlDbType.NVarChar, 50);
            EmailParam.Value = email;
            SqlCommand eventCourses = new SqlCommand(null, dbConnection);
            eventCourses.Parameters.AddRange(new SqlParameter[]{ConferenceNameParam, EmailParam});
            string previousRegisteredEventsQuery = 
                              "SELECT COUNT(*) AS REGISTERED_COUNT, SUM(EC.COST) " +
                              "FROM EVENT_CLASSES EC " +
                              "JOIN EVENT_INFO EI ON EC.EVENT_ID = EI.EVENT_ID " +
                              "WHERE COALESCE(EC.CURRENT_ENROLLMENT, 0) < EC.MAX_ENROLLMENT AND " +
                              "      EC.CLASS_ID IN (SELECT RL.CLASS_ID " +
                              "                      FROM REGISTRATION_LINE RL " +
                              "                      JOIN REGISTRATION R ON R.REGISTRATION_NUMBER = RL.REGISTRATION_NUMBER " +
                              "                      WHERE R.EMAIL = @EMAIL " +
                              "ORDER BY EI.EVENT_NAME;";
            eventCourses.CommandText = previousRegisteredEventsQuery;
            eventCourses.Prepare();
            SqlDataReader getEventCourses = eventCourses.ExecuteReader();
            if (getEventCourses.Read())
            {
               float regCount = float.Parse(getEventCourses["REGISTERED_COUNT"].ToString());
               float regCost = float.Parse(getEventCourses["COST"].ToString());
               dbConnection.Close();
               return new float[] { regCount, regCost };
            }
            else
            {
                return new float[]{0,0};
            }
        }
        catch (SqlException exception)
        {
            //Response.Write("<p> Error Code " + exception.Number + ": " + exception.Message + "</p>");
            return new float[] { 0, 0 };
        }
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
