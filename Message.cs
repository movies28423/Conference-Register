using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for Message
/// </summary>
public class Message
{
    List<User> messageRecipients;
    User messageSender;
    string messageTitle;
    string messageText;
    string messageID;
    string threadID;
    DateTime messageCreationTime;

    //call function to change BLOB data to text for message body
    public static byte[] StrToByteArray(string strValue)
    {
        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        return encoding.GetBytes(strValue);
    }

    public Message(string messageID)
    {
        this.messageID = messageID;
    }

    public Message(List<User> messageRecipients, User messageSender, string messageTitle, string messageText)
	{
        this.messageSender = messageSender;
        this.messageRecipients = messageRecipients;
        this.messageTitle = messageTitle;
        this.messageText = messageText;
        this.messageCreationTime = DateTime.Now;

        CreateMessage();
        SendMessage();   
    }

    public string MessageID
    {
        get { return messageID; }
        set { messageID = value; }
    }

    public Message(List<User> messageRecipients, User messageSender, string messageTitle, string messageText, string threadID)
    {
        this.messageSender = messageSender;
        this.messageRecipients = messageRecipients;
        this.messageTitle = messageTitle;
        this.messageText = messageText;
        this.threadID = threadID;
        this.messageCreationTime = System.DateTime.Now;

        CreateMessage();
        SendMessage();
    }

    protected void CreateMessage()
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter MessageSenderParam = new SqlParameter("@MESSAGE_SENDER", System.Data.SqlDbType.NVarChar, 50);
            MessageSenderParam.Value = this.messageSender.Email;
            SqlParameter MessageTitleParam = new SqlParameter("@MESSAGE_TITLE", System.Data.SqlDbType.NVarChar, 1200);
            MessageTitleParam.Value = this.messageTitle;
            SqlParameter MessageTextParam = new SqlParameter("@MESSAGE_TEXT", System.Data.SqlDbType.VarBinary, 8000);
            MessageTextParam.Value = StrToByteArray(this.messageText);

            SqlParameter messageID = new SqlParameter("MESSAGE_ID", System.Data.SqlDbType.Int);
            messageID.Direction = System.Data.ParameterDirection.Output;

            List<SqlParameter> CreateMessageParameters = 
                new List<SqlParameter> { MessageSenderParam, MessageTitleParam, MessageTextParam, messageID };

            SqlCommand createMessage = new SqlCommand("CREATE_MESSAGE_NO_THREAD", dbConnection);
            createMessage.CommandType = System.Data.CommandType.StoredProcedure;
            createMessage.Parameters.AddRange(CreateMessageParameters.ToArray());

            createMessage.ExecuteNonQuery();
            this.messageID = createMessage.Parameters["MESSAGE_ID"].Value.ToString();
            dbConnection.Close();
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
        }
    }

    protected void SendMessage()
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            foreach(User recipient in messageRecipients)
            {
                SqlParameter MessageRecipientParam = new SqlParameter("@MESSAGE_RECIPIENT", System.Data.SqlDbType.NVarChar, 50);
                MessageRecipientParam.Value = recipient.Email;
                SqlParameter MessageIDParam = new SqlParameter("@MESSAGE_ID", System.Data.SqlDbType.Int);
                MessageIDParam.Value = Int32.Parse(this.messageID);

                List<SqlParameter> SendMessageParameters = new List<SqlParameter> { MessageIDParam, MessageRecipientParam};
                string sendMessageCommand = 
                    "INSERT INTO USER_MESSAGE_RECIPIENTS (MESSAGE_ID, MESSAGE_RECIPIENT, MESSAGE_OPENED, IS_DELETED) VALUES " +
                    "(@MESSAGE_ID, @MESSAGE_RECIPIENT, 'F', 'F');";

                SqlCommand sendMessage = new SqlCommand(null, dbConnection);
                sendMessage.Parameters.AddRange(SendMessageParameters.ToArray());
                sendMessage.CommandText = sendMessageCommand;
                sendMessage.Prepare();
                sendMessage.ExecuteNonQuery();
                dbConnection.Close();
            }
            
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
        }
    }

    public void SetMessageOpened(User messageOpener)
    {
        string connection = ConfigurationManager.ConnectionStrings["EVENTSConnectionString"].ToString();
        SqlConnection dbConnection = new SqlConnection(connection);
        try
        {
            dbConnection.Open();

            SqlParameter MessageOpenerParam = new SqlParameter("@MESSAGE_RECIPIENT", System.Data.SqlDbType.NVarChar, 50);
            MessageOpenerParam.Value = messageOpener.Email;
            SqlParameter MessageIDParam = new SqlParameter("@MESSAGE_ID", System.Data.SqlDbType.Int);
            MessageIDParam.Value = Int32.Parse(this.messageID);

            List<SqlParameter> OpenMessageParameters = new List<SqlParameter> { MessageIDParam, MessageOpenerParam };
            string openMessageCommand =
                "UPDATE USER_MESSAGE_RECIPIENTS " +
                "SET MESSAGE_OPENED = 'T' " +
                "WHERE MESSAGE_RECIPIENT = @MESSAGE_RECIPIENT AND MESSAGE_ID = @MESSAGE_ID;";

            SqlCommand openMessage = new SqlCommand(null, dbConnection);
            openMessage.Parameters.AddRange(OpenMessageParameters.ToArray());
            openMessage.CommandText = openMessageCommand;
            openMessage.Prepare();
            openMessage.ExecuteNonQuery();
            dbConnection.Close();
        }
        catch (SqlException exception)
        {
            // "<p> Error Code " + exception.Number + ": " + exception.Message + "</p>";
        }
    }
}
