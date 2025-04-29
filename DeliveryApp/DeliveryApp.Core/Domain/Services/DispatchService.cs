using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;

namespace DeliveryApp.Core.Domain.Services;

public class DispatchService : IDispatchService
{
    public Courier Dispatch(Order order, List<Courier> couriers)
    {
        ArgumentNullException.ThrowIfNull(order);
        if (order.Status != OrderStatus.Created)
            throw new Exception($"The order must have the status '{OrderStatus.Created.Name}'.");
        
        if (couriers == null || couriers.Count == 0)
            throw new Exception("There are no couriers available.");
        
        var freeCouriers = couriers.Where(c => c.Status == CourierStatus.Free).ToList();
        if (freeCouriers.Count == 0)
            throw new Exception("There are no free couriers available.");

        var fasterCourier = freeCouriers[0];
        var bestTime = fasterCourier.CalculateTimeToLocation(order.Location);

        foreach (var courier in freeCouriers)
        {
            var time = courier.CalculateTimeToLocation(order.Location);
            if (time < bestTime)
            {
                bestTime = time;
                fasterCourier = courier;
            }
        }
        
        return fasterCourier;
    }
}