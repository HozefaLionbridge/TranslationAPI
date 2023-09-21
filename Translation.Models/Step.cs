namespace Translation.Models
{
    public class Step
    {
        public int StepId { get; set; }
        public int OrderId { get; set; }
        public string StepName { get; set; }
        public string InputFileURL { get; set; }
        public string OutputFileURL { get; set; }
        public short StatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string TransactionData { get; set; }
    }
}
