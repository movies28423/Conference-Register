using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Class contains methods with info for each course
/// </summary>
public class Course
{
    private string courseName;
    private DateTime startDttm;
    private DateTime endDttm;
    private float cost;
    private int maxEnrollment;
    private string courseConference;
    string courseID;

    public Course(string courseName, DateTime startDttm, DateTime endDttm, float cost, int maxEnrollment)
	{
        this.courseName = courseName;
        this.startDttm = startDttm;
        this.endDttm = endDttm;
        this.cost = cost;
        this.maxEnrollment = maxEnrollment;
	}

    public Course (string courseName)
    {
        this.courseName = courseName;
        this.getCourseInfo();
    }

    public string CourseName
    {
        get { return courseName; }
        set { courseName = value; }
    }

    public DateTime StartDttm
    {
        get { return startDttm; }
        set { startDttm = value; }
    }

    public DateTime EndDttm
    {
        get { return endDttm; }
        set { endDttm = value; }
    }

    public float Cost
    {
        get { return cost; }
        set { cost = value; }
    }

    public int MaxEnrollment
    {
        get { return maxEnrollment; }
        set { maxEnrollment = value; }
    }

    public string CourseConference
    {
        get { return courseConference; }
        set { courseConference = value; }
    }

    public string CourseID
    {
        get { return courseID; }
    }

    public void CreateCourse(Conference newConference)
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter ConferenceIDParam = new SqlParameter("@EVENT_ID", System.Data.SqlDbType.Int);
            ConferenceIDParam.Value = Int32.Parse(newConference.ConferenceID);
            SqlParameter CourseNameParam = new SqlParameter("@CLASS_NAME", System.Data.SqlDbType.NVarChar, 50);
            CourseNameParam.Value = this.courseName;
            SqlParameter CourseStartTime = new SqlParameter("@START_DTTM", System.Data.SqlDbType.DateTime);
            CourseStartTime.Value = this.startDttm;
            SqlParameter CourseEndTime = new SqlParameter("@END_DTTM", System.Data.SqlDbType.DateTime);
            CourseEndTime.Value = this.endDttm;
            SqlParameter CostParam = new SqlParameter("@COST", System.Data.SqlDbType.Float);
            CostParam.Value = this.Cost;
            SqlParameter MaxEnrollment = new SqlParameter("@MAX_ENROLLMENT", System.Data.SqlDbType.Float);
            MaxEnrollment.Value = this.MaxEnrollment;

            SqlParameter courseID = new SqlParameter("@COURSE_ID", System.Data.SqlDbType.Int);
            courseID.Direction = System.Data.ParameterDirection.Output;

            SqlParameter[] CreateCourseParameters = new SqlParameter[] { ConferenceIDParam, CourseNameParam, CourseStartTime, CourseEndTime, CostParam, MaxEnrollment };
            SqlCommand createCourse = new SqlCommand("CREATE_COURSE", dbConnection);
            createCourse.CommandType = System.Data.CommandType.StoredProcedure;
            createCourse.Parameters.AddRange(CreateCourseParameters.ToArray());

            createCourse.ExecuteNonQuery();
            this.courseID = createCourse.Parameters["EVENT_ID"].Value.ToString();
            dbConnection.Close();
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
        }
    }

    public void getCourseInfo()
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlCommand courseInfo = new SqlCommand(null, dbConnection);

            SqlParameter CourseNameParam = new SqlParameter("@COURSE_NAME", System.Data.SqlDbType.NVarChar, 50);
            CourseNameParam.Value = this.courseName;

            courseInfo.Parameters.AddRange(new SqlParameter[]{CourseNameParam});

           string courseInfoQuery = "SELECT EI.EVENT_NAME, EC.CLASS_ID, EC.CLASS_NAME, EC.START_DTTM, EC.END_DTTM, EC.COST, EC.MAX_ENROLLMENT FROM EVENT_CLASSES EC JOIN EVENT_INFO EI " + 
                "ON EI.EVENT_ID = EC.EVENT_ID WHERE EC.CLASS_NAME = @COURSE_NAME;";

            courseInfo.CommandText = courseInfoQuery;
            SqlDataReader getCourseInfo = courseInfo.ExecuteReader();

            if (getCourseInfo.Read())
            {
                do
                {
                    this.courseConference = getCourseInfo["EVENT_NAME"].ToString();
                    this.startDttm = DateTime.Parse(getCourseInfo["START_DTTM"].ToString());
                    this.endDttm = DateTime.Parse(getCourseInfo["END_DTTM"].ToString());
                    this.cost = float.Parse(getCourseInfo["COST"].ToString());
                    this.maxEnrollment = Int32.Parse(getCourseInfo["MAX_ENROLLMENT"].ToString());
                    this.courseID = getCourseInfo["CLASS_ID"].ToString();
                }
                while (getCourseInfo.Read());
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
}
