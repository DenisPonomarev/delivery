namespace DeliveryApp.Queries.GetUncompletedOrders;

public class GetUncompletedOrdersResponse
{
    public List<Order> Orders { get; set; } = null!;
    
    public class Order
    {
        public Guid Id { get; set; }
        public Location Location { get; set; } = null!;
    }

    public class Location
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}