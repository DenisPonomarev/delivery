namespace DeliveryApp.Queries.GetBusyCouriers;

public class GetBusyCouriersResponse
{
    public List<Courier> Couriers { get; set; } = null!;
    
    public class Courier
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Location Location { get; set; } = null!;
    }

    public class Location
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}