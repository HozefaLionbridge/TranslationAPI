using Translation.ReportsGenerator;

namespace TranslationEngine
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                JobsProcessor jobsProcessor = new();
                jobsProcessor.Initialize();
                jobsProcessor.ProcessNewOrders();
                jobsProcessor.ProcessSteps();
                new GenerateReports().SendReportsToUsers("Server=tcp:copiloteval.database.windows.net,1433;Initial Catalog=Translation;Persist Security Info=False;User ID=cpadmin;Password=Lion@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                //GFTranslate.UploadFiles();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}