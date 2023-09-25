using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Buffers.Text;
using System.Reflection.PortableExecutable;
using Translation.Common;

namespace TranslationAPI.Controllers
{
    [ApiController]
    
    public class TranslationController : ControllerBase
    {
        private static readonly string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=eastransferdev;AccountKey=Fb2t75TrcIeuS394IguuEXdc2NiuJTu7PTv/p6Fbq4cGbdi+VmdfxD3kUn3Bs99Hei+1WcbZfQKI4nGpu/S2Ow==;EndpointSuffix=core.windows.net";
        private readonly TranslationContext _context;
        private readonly BlobUtility _blobUtility;

        public TranslationController(TranslationContext context, BlobUtility blobUtility)
        {
            _context = context;
            _blobUtility = blobUtility;
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


            order.InputFileURL = order.InputFileURL.Split("base64,").Last();

            if (!System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\Temp"))
            {
                //create this path
                System.IO.Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "\\Temp");
            }

            System.IO.File.WriteAllBytes(System.AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + order.OrderName + ".docx", Convert.FromBase64String(order.InputFileURL as string));
            var fileUrl = _blobUtility.UploadToBlobAsync(blobConnectionString, order.OrderName.ToLower().Trim(), System.AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + order.OrderName + ".docx").GetAwaiter().GetResult();

            string query = "INSERT INTO [dbo].[Order] (OrderName, StatusId, InputFileURL, OutputFileURL, SubmissionDate) " +
             "VALUES ('" + order.OrderName + "'," + order.StatusId + ",'" + fileUrl + "','" + order.OutputFileURL + "','"
               + order.SubmissionDate.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            // execute the query
            _context.Database.ExecuteSqlRaw(query);


            // return the data from database
            return Ok(order);
        }

        [HttpPost]
        [EnableCors]
        [Route("api/[controller]/DownloadFile")]
        public ActionResult<Order> DownloadFile(Order order)
        {
            string bloburl = order.InputFileURL;
            string filename = order.OrderName + ".docx";
            _blobUtility.DownloadFromBlobAsync(bloburl, filename).GetAwaiter().GetResult();
            return Ok(order);
        }


        [HttpPost]
        [EnableCors]
        [Route("api/[controller]/AddReport")]
        public ActionResult<Order> EmailReportGeneration(Report report)
        {

            string query = "INSERT INTO [dbo].[RequestedReport] (FromDate, ToDate, EmailId, StatusId, OrderStatusId) " +
             "VALUES ('" + report.FromDate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + report.ToDate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + report.EmailId + "'," + report.StatusId + "," + report.OrderStatusId + ")";

            // execute the query
            _context.Database.ExecuteSqlRaw(query);


            // return the data from database
            return Ok(report);
        }
    }
}
