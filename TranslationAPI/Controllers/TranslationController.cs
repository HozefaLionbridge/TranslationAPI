using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TranslationAPI.Controllers
{
    [ApiController]
    
    public class TranslationController : ControllerBase
    {
        private readonly TranslationContext _context;

        public TranslationController(TranslationContext context)
        {
            _context = context;
        }

        // create a api method , use raw sql to get the data from database from order table
        [HttpGet]
        [EnableCors]
        [Route("api/[controller]/GetOrders")]
        public ActionResult<IEnumerable<Order>> GetOrders()
        {
            // create a variable to store the sql query
            string query = "select * from [dbo].[Order]";
            // create a variable to store the data from database
            var orders = _context.Orders.FromSqlRaw(query).ToList();
            // return the data from database
            return Ok(orders);
        }


        //create method to add order in sql database
        [HttpPost]
        [EnableCors]
        [Route("api/[controller]/AddOrder")]
        public ActionResult<Order> AddOrder(Order order)
        {
            string query = "INSERT INTO [dbo].[Order] (OrderName, StatusId, InputFileURL, OutputFileURL, SubmissionDate) " +
             "VALUES ('" + order.OrderName + "'," + order.StatusId + ",'" + order.InputFileURL + "','" + order.OutputFileURL + "','"
               + order.SubmissionDate.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            // execute the query
            _context.Database.ExecuteSqlRaw(query);
            // return the data from database
            return Ok(order);
        }
    }
}
