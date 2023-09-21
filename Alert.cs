namespace Translation.Models
{
    public class Alert
    {
        public int AlertId { get; set; }
        public int StepId { get; set; }
        public string Message { get; set; }
        public short StatusId { get; set; }
    }
}
