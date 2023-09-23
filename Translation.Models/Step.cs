namespace Translation.Models
{
    public class Step
    {
        public long StepId { get; set; }
        public long OrderId { get; set; }
        public string StepName { get; set; }
        public string InputFileURL { get; set; }
        public string OutputFileURL { get; set; }
        public short StatusId { get; set; }
        public short ExecutionOrder { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string TransactionData { get; set; }
    }
}
