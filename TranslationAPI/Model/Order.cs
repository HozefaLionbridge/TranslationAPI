public class Order
{
    public decimal OrderId { get; set; }
    public string? OrderName { get; set; }
    public string? InputFileURL { get; set; }
    public string? OutputFileURL { get; set; }
    public short StatusId { get; set; }
    public DateTime SubmissionDate { get; set; }
    public DateTime? CompletedDate { get; set; }
}

public enum Status
{
    InProgress = 1,
    Completed = 2,
}

public class Report
{
    public int RequestId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string EmailId { get; set; }
    public int StatusId { get; set; }
    public int OrderStatusId { get; set; }
}