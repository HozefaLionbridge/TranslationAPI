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
