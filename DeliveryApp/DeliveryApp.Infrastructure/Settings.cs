namespace DeliveryApp.Infrastructure;

public class Settings
{
    public string ConnectionString { get; set; } = null!;
    public string GeoServiceGrpcHost { get; set; } = null!;
    public string MessageBrokerHost { get; set; } = null!;
    public string OrderStatusChangedTopic { get; set; } = null!;
    public string BasketConfirmedTopic { get; set; } = null!;
}
