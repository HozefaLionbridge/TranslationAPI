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
                var query = $"SELECT * FROM [Order] WHERE StatusId = {(short)status}";
                var command = new SqlCommand(query, _connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderId = Convert.ToInt64(reader["OrderId"].ToString()),
                        OrderName = reader["OrderName"].ToString(),
                        InputFileURL = reader["InputFileURL"].ToString(),
                        OutputFileURL = reader["OutputFileURL"].ToString(),
                        Status = (OrderStatus)(short)reader["StatusId"],
                        SubmissionDate = (DateTime)reader["SubmissionDate"],
                        //CompletedDate = (DateTime)reader["CompletedDate"]
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

        public void UpdateOrder(Order order)
        {
            try
            {
                _connection.Open();
                var query = $"UPDATE [Order] SET StatusId = {(int)order.Status}, OutputFileURL = '{order.OutputFileURL}', CompletedDate = '{order.CompletedDate.ToString("yyyy-MM-dd HH:mm:ss.fff")}' WHERE OrderId = {order.OrderId}"; // Building the SQL command string with the provided Order object
                var command = new SqlCommand(query, _connection);
                command.ExecuteNonQuery();
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
        }

        public void SetOrderStatus(long orderId, OrderStatus orderStatus)
        {
            try
            {
                _connection.Open();
                var query = $"UPDATE [Order] SET StatusId = {(short)orderStatus} WHERE OrderId = {orderId}"; // Building the SQL command string with the provided Order object
                var command = new SqlCommand(query, _connection);
                command.ExecuteNonQuery();
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
        }


        public void CreateStep(Step step)
        {
            try
            {
                _connection.Open();
                var query = $"INSERT INTO Step (OrderId, StepName, ExecutionOrder, InputFileURL, StatusId, StartDate, UpdateDate, TransactionData) VALUES ( {step.OrderId}, '{step.StepName}', '{step.ExecutionOrder}', '{step.InputFileURL}', {step.StatusId}, '{step.StartDate.ToString("yyyy-MM-dd HH:mm:ss.fff")}', '{step.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss.fff")}', '{step.TransactionData}')";
                var command = new SqlCommand(query, _connection);
                command.ExecuteNonQuery();
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
        }

        public void UpdateStep(Step step)
        {
            try
            {
                _connection.Open();
                var query = $"UPDATE Step SET StatusId = {(int)step.StatusId}, OutputFileURL = '{step.OutputFileURL}', UpdateDate = '{step.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss.fff")}', TransactionData = '{step.TransactionData}' WHERE StepId = {step.StepId}";
                var command = new SqlCommand(query, _connection);
                command.ExecuteNonQuery();
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
        }

        public List<Step> GetStepsByStatus(StepStatus status)
        {
            var steps = new List<Step>();
            try
            {
                _connection.Open();
                var query = $"SELECT * FROM Step WHERE StatusId = {(short)status}";
                var command = new SqlCommand(query, _connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    steps.Add(new Step
                    {
                        StepId = Convert.ToInt64(reader["StepId"].ToString()),
                        OrderId = Convert.ToInt64(reader["OrderId"].ToString()),
                        StepName = reader["StepName"].ToString(),
                        ExecutionOrder = (short)reader["ExecutionOrder"],
                        InputFileURL = reader["InputFileURL"].ToString(),
                        OutputFileURL = reader["OutputFileURL"].ToString(),
                        StatusId = (short)reader["StatusId"],
                        StartDate = (DateTime)reader["StartDate"],
                        UpdateDate = (DateTime)reader["UpdateDate"],
                        TransactionData = reader["TransactionData"].ToString()
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
            return steps;
        }

        public void MakeNextStepReadyToPickUp(short executionOrder, long orderId)
        {
            //Write an update query to make the next step ready to be picked up
            try
            {
                _connection.Open();
                var query = $"UPDATE Step SET StatusId = {(int)StepStatus.ReadyToBePicked} WHERE StepId = (SELECT StepId FROM Step WHERE orderId = {orderId} and ExecutionOrder={executionOrder + 1})";
                var command = new SqlCommand(query, _connection);
                command.ExecuteNonQuery();
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
        }
    }
}