using ClosedXML.Excel;
using System.Data.SqlClient;
using System.Net.Mail;

namespace Translation.ReportsGenerator
{
    public class GenerateReports
    {
        public void SendReportsToUsers(string connectionString)
        {
            //string connectionString = @"server=tcp:copilotauthorization.database.windows.net,1433;Initial Catalog=authorization;user id=cpadmin;password=Lion@1234";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM [dbo].RequestedReport WHERE statusid = @status", connection);
                command.Parameters.AddWithValue("@status", 1);
                SqlDataReader reader = command.ExecuteReader();
                // Call Read before accessing data.
                while (reader.Read())
                {
                    var fromDate = reader["fromdate"];
                    var toDate = reader["todate"];
                    var sqlConnection2 = new SqlConnection(connectionString);
                    sqlConnection2.Open();
                    SqlCommand command2 = new SqlCommand("SELECT * FROM [Order] WHERE SubmissionDate BETWEEN @fromdate AND @todate and StatusId=@statusid", sqlConnection2);
                    command2.Parameters.AddWithValue("@fromdate", fromDate);
                    command2.Parameters.AddWithValue("@todate", toDate);
                    command2.Parameters.AddWithValue("@statusid", reader["OrderStatusId"]);
                    string path = @"C:\temp\OrderReport" + DateTime.Now.Ticks + ".xlsx";
                    using (SqlDataReader reader2 = command2.ExecuteReader())
                        DumpDataReaderToExcel(reader2, path);

                    SendMail(reader["EmailId"].ToString(), path);
                    command2 = new SqlCommand("Update [dbo].RequestedReport SET statusid = 2 WHERE RequestID=" + reader["RequestID"].ToString(), sqlConnection2);
                    command2.ExecuteNonQuery();
                    sqlConnection2.Close();
                }
                connection.Close();
            }
        }

        public void DumpDataReaderToExcel(SqlDataReader reader, string filePath)
        {
            // Create new Excel workbook and worksheet
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Orders Report");

            // Write column headers to worksheet
            for (int i = 0; i < reader.FieldCount; i++)
            {
                worksheet.Cell(1, i + 1).Value = reader.GetName(i);
            }

            // Write data rows to worksheet
            int rowIndex = 2;
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    worksheet.Cell(rowIndex, i + 1).Value = reader[i].ToString();
                }
                rowIndex++;
            }

            // Save workbook to file
            workbook.SaveAs(filePath);
        }

        public void SendMail(string recipientEmail, string filename)
        {
            // Create new email message
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("vijayaraghavan.ranganathan@lionbridge.com");
            mail.To.Add(recipientEmail);
            mail.Subject = "Order Report";
            mail.Body = "Please find Order Report attached";

            // Attach Excel file to email message
            Attachment attachment = new Attachment(filename);
            mail.Attachments.Add(attachment);

            // Send email
            SmtpClient smtp = new SmtpClient("smtp-connect.lionbridge.com");
            smtp.Port = 25;
            //smtp.Credentials = new NetworkCredential("noreply.liox@gmail.com", "!Tms2015*");
            smtp.EnableSsl = false;
            smtp.Send(mail);
        }
    }
}