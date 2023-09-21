namespace Translation.Models
{
    public class Order
    {
        public long OrderId { get; set; }

        public string OrderName { get; set; }

        public string InputFileURL { get; set; }
        
        public string OutputFileURL { get; set;}

        public OrderStatus Status { get; set; }

        public DateTime SubmissionDate { get; set; }

        public DateTime CompletedDate { get; set;}
    }
}