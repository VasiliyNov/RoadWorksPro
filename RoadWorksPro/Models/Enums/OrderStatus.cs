namespace RoadWorksPro.Models.Enums
{
    public enum OrderStatus
    {
        New = 0,        // New order
        InProgress = 1, // Admin is processing
        Completed = 2,  // Order completed
        Cancelled = 3   // Order cancelled
    }
}
