using Translation.Models;
using TranslationEngine.DAL;

namespace TranslationEngine.BLL
{
    public class JobsProcessor
    {
        private DBQueries _dbQueries;

        public JobsProcessor()
        {
            Initialize();
        }

        public void Initialize()
        {
            _dbQueries = new DBQueries();
            //ProcessNewOrders();
        }

        private void ProcessNewOrders()
        {
            var newOrders = _dbQueries.GetOrdersByStatus(OrderStatus.Created);
            foreach (var order in newOrders)
            {
                //Process the order
                //Update the database with the status of the order
            }
            //Dump the steps into the database
        }

        private void ProcessSteps()
        {
            //Query the database for steps that are not completed
            //Process the steps
            //Update the database with the status of the step
        }
    }
}