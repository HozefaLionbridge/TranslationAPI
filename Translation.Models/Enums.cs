namespace Translation.Models
{
    //Create an enum of OrderStatus
    public enum OrderStatus
    {
        Created,
        Processing,
        Completed,
        Failed
    }

    public enum StepStatus
    {
        Created,
        Processing,
        Completed,
        Failed
    }

    public enum AlertStatus
    {
        Active,
        Inactive
    }
}