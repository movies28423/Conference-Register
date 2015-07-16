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
using System.Net.Mail;

public partial class UserHomePage : System.Web.UI.Page
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

    public static List<string> DisplayMessage(Message message)
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();
            string EmailInfo = "";
            string EmailText = "";
            SqlParameter SelectedMessageParam = new SqlParameter("@MESSAGE_ID", System.Data.SqlDbType.Int);
            SelectedMessageParam.Value = Int32.Parse(message.MessageID);
            string getEvents = "SELECT UM.MESSAGE_TITLE, UM.MESSAGE_TIME, UM.MESSAGE_TEXT " +
                               "FROM USER_MESSAGE UM " +
                               "WHERE UM.MESSAGE_ID = @MESSAGE_ID;";
            SqlCommand retrieveEvents = new SqlCommand(null, dbConnection);
            retrieveEvents.Parameters.Add(SelectedMessageParam);
            retrieveEvents.CommandText = getEvents;
            retrieveEvents.Prepare();

            SqlDataReader getMessage = retrieveEvents.ExecuteReader();

            while (getMessage.Read())
            {
                EmailInfo = getMessage["MESSAGE_TITLE"].ToString() + " " + getMessage["MESSAGE_TIME"].ToString();
                foreach (byte messageByte in (byte[])getMessage["MESSAGE_TEXT"])
                {
                    EmailText += ((char)messageByte).ToString();
                }
            }
            dbConnection.Close();
            List<string> emailList = new List<string>(){EmailInfo, EmailText};
            return emailList;            
        }
        catch (SqlException exception)
        {
            //Response.Write("<p> Error Code " + exception.Number + ": " + exception.Message + "</p>");
            return new List<string>();
        }
    }

    public static void formatNewMessages(GridViewRow currentRow)
    {
        if(currentRow.Cells[currentRow.Cells.Count - 2].Text == "F")
        {
            foreach(DataControlFieldCell Cell in currentRow.Cells)
            {
                Cell.Font.Bold = true;
            }
        }
        else
        {
            foreach (DataControlFieldCell Cell in currentRow.Cells)
            {
                Cell.Font.Bold = false;
            }
        }
        DateTime cellsDateTime = new DateTime();
        if(DateTime.TryParse(currentRow.Cells[2].Text, out cellsDateTime))
        {
            if(cellsDateTime.Date == System.DateTime.Today)
            {
                currentRow.Cells[2].Text = cellsDateTime.TimeOfDay.ToString();
            }
            else
            {
                currentRow.Cells[2].Text = cellsDateTime.ToString("M/dd/yyyy");
            }
        }
    }

    public static DataSet DisplayMessages(string email)
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();
            SqlParameter EmailParam = new SqlParameter("@EMAIL", System.Data.SqlDbType.NVarChar, 50);
            EmailParam.Value = email;

            string getMessages = "SELECT UM.MESSAGE_SENDER, UM.MESSAGE_TITLE, UM.MESSAGE_TIME, UM.MESSAGE_ID, UMR.MESSAGE_OPENED " +
                                 "FROM USER_MESSAGE UM " +
                                 "JOIN USER_MESSAGE_RECIPIENTS UMR ON " +
                                 "UMR.MESSAGE_ID = UM.MESSAGE_ID " +
                                 "WHERE UMR.MESSAGE_RECIPIENT = @EMAIL AND " +
                                 "UMR.IS_DELETED = 'F' " +
                                 "ORDER BY UM.MESSAGE_TIME DESC;";
            SqlCommand retrieveMessages = new SqlCommand(null, dbConnection);
            retrieveMessages.Parameters.Add(EmailParam);
            retrieveMessages.CommandText = getMessages;
            retrieveMessages.Prepare();

            SqlDataAdapter adaptMessages = new SqlDataAdapter(retrieveMessages);
            DataSet dsEvents = new DataSet();
            adaptMessages.Fill(dsEvents);
            dbConnection.Close();
            return dsEvents;          
        }
        catch (SqlException exception)
        {
            return new DataSet();
            //Response.Write("<p> Error Code " + exception.Number + ": " + exception.Message + "</p>");
        }
    }

    public static void DeleteOpenMessage(Message message, string email)
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter SelectedMessageParam = new SqlParameter("@MESSAGE_ID", System.Data.SqlDbType.Int);
            SelectedMessageParam.Value = Int32.Parse(message.MessageID);

            SqlParameter EmailParam = new SqlParameter("@EMAIL", System.Data.SqlDbType.NVarChar, 50);
            EmailParam.Value = email;

            string deleteMessageCommand = "UPDATE USER_MESSAGE_RECIPIENTS " + 
                                          "SET IS_DELETED = 'T' " +
                                          "WHERE MESSAGE_ID = @MESSAGE_ID " +
                                          "AND MESSAGE_RECIPIENT = @EMAIL;";

            SqlCommand deleteMessage = new SqlCommand(null, dbConnection);
            deleteMessage.Parameters.Add(SelectedMessageParam);
            deleteMessage.Parameters.Add(EmailParam);
            deleteMessage.CommandText = deleteMessageCommand;
            deleteMessage.Prepare();
            deleteMessage.ExecuteNonQuery();

            dbConnection.Close();
        }
        catch (SqlException exception)
        {
            //Response.Write("<p> Error Code " + exception.Number + ": " + exception.Message + "</p>");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string email = Request.Cookies["loginUser"]["email"].ToString();
        string firstName = Request.Cookies["loginUser"]["firstName"].ToString();
        string lastName = Request.Cookies["loginUser"]["lastName"].ToString();

        UserInfoLbl.Text = email + ": " + lastName + ", " + firstName;

        const string MESSAGE_ROLE = "VIEW_MESSAGE";
        UserRoles userRole = new UserRoles(new User(email));
        if (userRole.Role.HasFeature(new SysFeature(MESSAGE_ROLE)))
        {
            if (IsPostBack && messageView.GetActiveView() == messages)
            {
                usersMessages.Columns[3].Visible = true;
                usersMessages.Columns[4].Visible = true;
            }
            else if (!IsPostBack)
            {
                messageView.SetActiveView(messages);

                usersMessages.Columns[3].Visible = true;
                usersMessages.Columns[4].Visible = true;

                usersMessages.DataSource = DisplayMessages(email).Tables[0];
                usersMessages.DataBind();
                foreach (GridViewRow row in usersMessages.Rows)
                {
                    formatNewMessages(row);
                }
                CapitalizeCase(usersMessages);
                usersMessages.Columns[3].Visible = false;
                usersMessages.Columns[4].Visible = false;
            }
        }
        else
            messageView.Visible = false;
    }

    protected void BackToMessages_Click(object sender, EventArgs e)
    {
        string email = Request.Cookies["loginUser"]["email"].ToString();
        messageView.SetActiveView(messages);

        usersMessages.Columns[3].Visible = true;
        usersMessages.Columns[4].Visible = true;

        usersMessages.DataSource = DisplayMessages(email).Tables[0];
        usersMessages.DataBind();
        CapitalizeCase(usersMessages);
        foreach (GridViewRow row in usersMessages.Rows)
        {
            formatNewMessages(row);
        }

        usersMessages.Columns[3].Visible = false;
        usersMessages.Columns[4].Visible = false;
    }
    protected void DeleteMessage_Click(object sender, EventArgs e)
    {
        string email = Request.Cookies["loginUser"]["email"].ToString();
        string messageID = usersMessages.SelectedRow.Cells[3].Text;
        DeleteOpenMessage(new Message(messageID), email);

        messageView.SetActiveView(messages);

        usersMessages.Columns[3].Visible = true;
        usersMessages.Columns[4].Visible = true;

        usersMessages.DataSource = DisplayMessages(email).Tables[0];
        usersMessages.DataBind();
        CapitalizeCase(usersMessages);

        usersMessages.Columns[3].Visible = false;
        usersMessages.Columns[4].Visible = false;
    }

    protected void UserMessage_RowCommand(object sender,GridViewCommandEventArgs e)
    {
        if (e.CommandName == "SetOpenMessage")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = usersMessages.Rows[index];

            messageView.SetActiveView(indvMessage);
            formatNewMessages(usersMessages.SelectedRow);
            if (row.Cells[4].Text == "F")
            {
                string email = Request.Cookies["loginUser"]["email"].ToString();
                Message openMessage = new Message(row.Cells[3].Text);
                formatNewMessages(row);
                openMessage.SetMessageOpened(new User(email));
            }

            List<string> emailStuff = DisplayMessage(new Message(row.Cells[3].Text));
            EmailInfo.Text = emailStuff[0];
            EmailText.Text = emailStuff[1];
            usersMessages.Columns[3].Visible = false;
            usersMessages.Columns[4].Visible = false;
        }
    }
}
