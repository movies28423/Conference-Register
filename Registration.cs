using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Each registration is made up of a user, registration lines, the date entered and total cost
/// TODO: Add functionality for modifying registration on a certain date
/// </summary>
public class Registration
{
    User registeringUser;
    List<Conference> registeringConferences;
    List<Course> registeringCourses;
    List<RegistrationLine> registeringLine;
    string registrationID;
    float cost;

    public Registration(User registeringUser, List<RegistrationLine> registeringLine)
	{
        this.registeringUser = registeringUser;
        this.registeringLine = registeringLine;
	}

    public Registration(string registrationID)
    {
        this.registrationID = registrationID;
        GetRegistrationInfo();
    }

    public User RegisteringUser
    {
        get { return registeringUser; }
        set { registeringUser = value; }
    }

    public List<RegistrationLine> RegisteringLine
    {
        get { return registeringLine; }
        set { registeringLine = value; }
    }

    public string RegistrationID
    {
        get { return registrationID; }
        set { registrationID = value; }
    }

    public float Cost
    {
        get { return cost; }
    }

    public void CalculateCost()
    {
        this.cost = 0;
        Dictionary<string, List<Course>> conferenceCourseCount = new Dictionary<string, List<Course>>();
        foreach (RegistrationLine regLine in registeringLine)
        {
            if (conferenceCourseCount.ContainsKey(regLine.LineCourse.CourseConference))
            {
                conferenceCourseCount[regLine.LineCourse.CourseConference].Add(regLine.LineCourse);
            }
            else
            {
                conferenceCourseCount.Add(regLine.LineCourse.CourseConference, new List<Course>{regLine.LineCourse});
            }
        }

        foreach(string regCon in conferenceCourseCount.Keys)
        {
            Conference newConference = new Conference(regCon);
            if (conferenceCourseCount[regCon].Count == newConference.CoursesInConference.Count)
            {
                this.cost += newConference.WholeConferenceCost;
                conferenceCourseCount[regCon].Select(course => course.Cost = 0).ToList();
            }
            else
            {
                if (newConference.GetPreviousRegCoursesInConference(registeringUser.Email)[0] + conferenceCourseCount[regCon].Count
                    == newConference.CoursesInConference.Count)
                {
                    this.cost += newConference.WholeConferenceCost -
                                 newConference.GetPreviousRegCoursesInConference(registeringUser.Email)[1];
                    conferenceCourseCount[regCon].Select(course => course.Cost = 0).ToList();
                }
                else
                {
                    this.cost += conferenceCourseCount[regCon].Sum(x => x.Cost);
                }
            }
        }
    }

    public void AddRegistrationLine()
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            foreach (RegistrationLine regLine in registeringLine)
            {
                SqlParameter RegistrationIDParam = new SqlParameter("@REGISTRATION_NUMBER", System.Data.SqlDbType.Int);
                RegistrationIDParam.Value = Int32.Parse(registrationID);
                SqlParameter ClassIDParam = new SqlParameter("@CLASS_ID", System.Data.SqlDbType.Int);
                ClassIDParam.Value = Int32.Parse(regLine.LineCourse.CourseID);

                List<SqlParameter> RegistrationLineParameters = new List<SqlParameter> { RegistrationIDParam, ClassIDParam };
                string registrationLineCommand =
                    "INSERT INTO REGISTRATION_LINE (REGISTRATION_NUMBER, CLASS_ID) VALUES " +
                    "(@REGISTRATION_NUMBER, @CLASS_ID);";

                SqlCommand addRegLine = new SqlCommand(null, dbConnection);
                addRegLine.Parameters.AddRange(RegistrationLineParameters.ToArray());
                addRegLine.CommandText = registrationLineCommand;
                addRegLine.Prepare();
                addRegLine.ExecuteNonQuery();
            }
            dbConnection.Close();
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
        }
    }

    protected void GetRegistrationInfo()
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter RegistrationIDParam = new SqlParameter("@REGISTRATION_NUMBER", System.Data.SqlDbType.Int);
            RegistrationIDParam.Value = Int32.Parse(registrationID);

            List<SqlParameter> RegistrationInfoParameters = new List<SqlParameter> { RegistrationIDParam };
            string registrationLineCommand =
                "SELECT R.EMAIL, EC.CLASS_NAME " +
                "FROM REGISTRATION R " +
                "JOIN REGISTRATION_LINE RL " + 
                "ON RL.REGISTRATION_NUMBER = R.REGISTRATION_NUMBER " +
                "JOIN EVENT_CLASSES EC " +
                "ON EC.CLASS_ID = RL.CLASS_ID " +
                "WHERE R.REGISTRATION_NUMBER = @REGISTRATION_NUMBER;";

            SqlCommand registrationInfo = new SqlCommand(null, dbConnection);
            registrationInfo.Parameters.Add(RegistrationIDParam);
            registrationInfo.CommandText = registrationLineCommand;
            registrationInfo.Prepare();           
            SqlDataReader getRegInfo = registrationInfo.ExecuteReader();
            this.RegisteringLine = new List<RegistrationLine>();
            if (getRegInfo.Read())
            {
                do
                {
                    if (this.registeringUser == null)
                        this.registeringUser = new User(getRegInfo["EMAIL"].ToString());
                    string te = getRegInfo["CLASS_NAME"].ToString();
                    RegistrationLine newRegLine = new RegistrationLine(new Course(getRegInfo["CLASS_NAME"].ToString()));
                    this.registeringLine.Add(newRegLine);
                } while (getRegInfo.Read());
            }
            dbConnection.Close();
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
            string e = "";
        }
    }
}
