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

        public void ProcessNewOrders()
        {
            var newOrders = _dbQueries.GetOrdersByStatus(OrderStatus.Created);
            foreach (var order in newOrders)
            {
                //Insert 3 steps with name ConvertToTag, Geofluent and ConvertFromTag
                _dbQueries.CreateStep(
                    new Step
                    { 
                        OrderId = order.OrderId,
                        StepName = "ConvertToTag",
                        InputFileURL = order.InputFileURL,
                        StartDate = DateTime.Now,
                        StatusId = (short)StepStatus.ReadyToBePicked,
                        UpdateDate = DateTime.Now,
                        ExecutionOrder = 1
                    });

                _dbQueries.CreateStep(
                    new Step
                    {
                        OrderId = order.OrderId,
                        StepName = "GFTranslate",
                        InputFileURL = order.InputFileURL,
                        StartDate = DateTime.Now,
                        StatusId = (short)StepStatus.Created,
                        UpdateDate = DateTime.Now,
                        ExecutionOrder = 2
                    });

                _dbQueries.CreateStep(
                    new Step
                    {
                        OrderId = order.OrderId,
                        StepName = "ConvertFromTag",
                        InputFileURL = order.InputFileURL,
                        StartDate = DateTime.Now,
                        StatusId = (short)StepStatus.Created,
                        UpdateDate = DateTime.Now,
                        ExecutionOrder = 3
                    });

                _dbQueries.SetOrderStatus(order.OrderId, OrderStatus.Processing);
            }
            //Dump the steps into the database
        }

        public void ProcessSteps()
        {
            foreach(var step in _dbQueries.GetStepsByStatus(StepStatus.ReadyToBePicked))
            {
                if(step.StepName == "ConvertToTag")
                {
                    _dbQueries.UpdateStep(new Step
                    {
                        StepId = step.StepId,
                        StatusId = (short)StepStatus.Completed,
                        OutputFileURL = step.InputFileURL,
                        UpdateDate = DateTime.Now
                    });
                    _dbQueries.MakeNextStepReadyToPickUp(step.ExecutionOrder, step.OrderId);
                }
                else if(step.StepName == "GFTranslate")
                {
                    _dbQueries.UpdateStep(new Step
                    {
                        StepId = step.StepId,
                        StatusId = (short)StepStatus.Completed,
                        OutputFileURL = step.InputFileURL,
                        UpdateDate = DateTime.Now
                    });
                    _dbQueries.MakeNextStepReadyToPickUp(step.ExecutionOrder, step.OrderId);
                }
                else if (step.StepName == "ConvertFromTag")
                {
                    _dbQueries.UpdateStep(new Step
                    {
                        StepId = step.StepId,
                        StatusId = (short)StepStatus.Completed,
                        OutputFileURL = step.InputFileURL,
                        UpdateDate = DateTime.Now
                    });
                    _dbQueries.UpdateOrder(new Order
                    {
                        OrderId = step.OrderId,
                        Status = OrderStatus.Completed,
                        OutputFileURL = step.InputFileURL,
                        CompletedDate = DateTime.Now
                    });
                }
            }
            //Update the database with the status of the step
        }
    }
}