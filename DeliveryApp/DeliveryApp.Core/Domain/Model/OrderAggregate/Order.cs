using System.Diagnostics.CodeAnalysis;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate;

public class Order : Aggregate<Guid>
{
    [ExcludeFromCodeCoverage]
    private Order() { }

    private Order(Location location)
    {
        Id = Guid.NewGuid();
        Location = location;
    }

    public Guid CourierId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Location Location { get; private set; }
}