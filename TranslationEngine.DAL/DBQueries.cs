using Microsoft.Data.SqlClient;
using Translation.Models;

namespace TranslationEngine.DAL
{
    public class DBQueries
    {
        private static readonly string _connectionString = "Server=tcp:copiloteval.database.windows.net,1433;Initial Catalog=Translation;Persist Security Info=False;User ID=cpadmin;Password=Lion@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private readonly SqlConnection _connection;

        public DBQueries()
        {
            _connection = new SqlConnection(_connectionString);
        }

        //write a function to get orders by status
        public List<Order> GetOrdersByStatus(OrderStatus status)
        {
            var orders = new List<Order>();
            try
            {
                _connection.Open();
                var query = $"SELECT * FROM Orders WHERE StatusId = {status}";
                var command = new SqlCommand(query, _connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderId = (long)reader["OrderId"],
                        OrderName = reader["OrderName"].ToString(),
                        InputFileURL = reader["InputFileURL"].ToString(),
                        OutputFileURL = reader["OutputFileURL"].ToString(),
                        Status = (OrderStatus)(int)reader["Status"],
                        SubmissionDate = (DateTime)reader["SubmissionDate"],
                        CompletedDate = (DateTime)reader["CompletedDate"]
                    });
                }
                _connection.Close();
            }
            catch (Exception ex)
            {
                //log the exception
            }
            finally
            {
                _connection.Close();
            }
            return orders;
        }


    }   
}