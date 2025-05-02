using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Domain.Services;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.Core.Tests.Domain.Services;

public class DispatchServiceShould
{
    [Fact]
    public void ThrowExceptionWhenOrderIsNull()
    {
        var service = new DispatchService();

        var exception = Assert.Throws<ArgumentNullException>(() => service.Dispatch(null!, null!));
        exception.ParamName.Should().Be("order");
    }

    [Fact]
    public void ThrowExceptionWhenOrderStatusIsNotCreated()
    {
        var service = new DispatchService();
        var order = new Order(Guid.NewGuid(), new Location(5, 5));
        var courier = new Courier("Courier", "Car", 3, Location.MinLocation);
        order.Assigne(courier);

        var exception = Assert.Throws<Exception>(() => service.Dispatch(order, null!));
        exception.Message.Should().Be($"The order must have the status 'created'.");
    }

    [Fact]
    public void ThrowExceptionWhenNoCouriersAreAvailable()
    {
        var service = new DispatchService();
        var order = new Order(Guid.NewGuid(), new Location(5, 5));
        var couriers = new List<Courier>();

        var exception = Assert.Throws<Exception>(() => service.Dispatch(order, couriers));
        exception.Message.Should().Be($"There are no couriers available.");
    }

    [Fact]
    public void ThrowExceptionWhenNoFreeCouriersAreAvailable()
    {
        var service = new DispatchService();
        var order = new Order(Guid.NewGuid(), new Location(5, 5));
        var courier = new Courier("Courier", "Car", 3, Location.MinLocation);
        courier.SetBusy();
        var couriers = new List<Courier> { courier };

        var exception = Assert.Throws<Exception>(() => service.Dispatch(order, couriers));
        exception.Message.Should().Be($"There are no free couriers available.");
    }

    [Fact]
    public void DispatchWalkingCouriersCorrectly()
    {
        var service = new DispatchService();
        var order = new Order(Guid.NewGuid(), new Location(5, 5));
        var couriers = new List<Courier>
        {
            new("Courier 1", "Legs", 1, new Location(1, 1)),
            new("Courier 2", "Legs", 1, new Location(2, 2)),
            new("Courier 3", "Legs", 1, new Location(2, 3))
        };

        var fasterCourier = service.Dispatch(order, couriers);
        
        fasterCourier.Name.Should().Be("Courier 3");
        order.CourierId.Should().Be(fasterCourier.Id);
    }
    
    [Fact]
    public void DispatchCorrectly()
    {
        var service = new DispatchService();
        var order = new Order(Guid.NewGuid(), new Location(5, 5));
        var couriers = new List<Courier>
        {
            new("Courier 1", "Car", 3, new Location(1, 1)),
            new("Courier 2", "Legs", 1, new Location(2, 2)),
            new("Courier 3", "Legs", 1, new Location(2, 3))
        };

        var fasterCourier = service.Dispatch(order, couriers);
        
        fasterCourier.Name.Should().Be("Courier 1");
        order.CourierId.Should().Be(fasterCourier.Id);
    }
}