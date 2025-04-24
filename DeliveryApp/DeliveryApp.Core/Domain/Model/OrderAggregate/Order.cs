using System.Diagnostics.CodeAnalysis;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate;

public class Order : Aggregate<Guid>
{
    [ExcludeFromCodeCoverage]
    private Order() { }

    public Order(Guid basketId, Location location)
    {
        if (basketId == Guid.Empty)
            throw new ArgumentException(null, nameof(basketId));

        ArgumentNullException.ThrowIfNull(location);

        Id = basketId;
        Location = location;
        Status = OrderStatus.Created;
    }

    public Guid? CourierId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Location Location { get; private set; }

    public void Assigne(Courier courier)
    {
        ArgumentNullException.ThrowIfNull(courier);

        if (CourierId.HasValue && CourierId != Guid.Empty)
            throw new Exception("Courier already assigned");

        if (Status != OrderStatus.Created)
            throw new Exception($"The order must have the status '{OrderStatus.Created.Name}'");
        
        CourierId = courier.Id;
        Status = OrderStatus.Assigned;
    }

    public void Complete()
    {
        if (Status != OrderStatus.Assigned)
            throw new Exception($"The order must have the status '{OrderStatus.Assigned.Name}'");
        
        Status = OrderStatus.Completed;
        CourierId = null;
    }
}