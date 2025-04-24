using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.Core.Tests.Domain.Model.OrderAggregate;

public class OrderShould
{
    private readonly Guid _basketId = new("223455c4-1c4d-463e-93ff-40611f5051d3");
    private readonly Location _orderLocation = Location.MinLocation;
    private readonly Courier _courier = new("Courier", "Car", 3, Location.MaxLocation);

    [Fact]
    public void BeCorrectWhenParamsIsCorrectOnCreated()
    {
        var order = new Order(_basketId, _orderLocation);

        order.Id.Should().Be(_basketId);
        order.Location.Should().Be(_orderLocation);
        order.Status.Should().Be(OrderStatus.Created);
        order.CourierId.Should().BeNull();
    }

    [Fact]
    public void BeIncorrectWhenParamsIsIncorrectOnCreated()
    {
        var exception = Assert.Throws<ArgumentException>(() => new Order(Guid.Empty, _orderLocation));
        exception.ParamName.Should().Be("basketId");

        exception = Assert.Throws<ArgumentNullException>(() => new Order(_basketId, null!));
        exception.ParamName.Should().Be("location");
    }

    [Fact]
    public void CanAssignToCourier()
    {
        var order = new Order(_basketId, _orderLocation);
        order.Assigne(_courier);
        
        order.CourierId.Should().Be(_courier.Id);
        order.Status.Should().Be(OrderStatus.Assigned);
    }
    
    [Fact]
    public void ThrowExceptionWhenAssignToCourierTwice()
    {
        var order = new Order(_basketId, _orderLocation);
        order.Assigne(_courier);
        
        var exception = Assert.Throws<Exception>(() => order.Assigne(_courier));
        exception.Message.Should().Be("Courier already assigned");
    }
    
    [Fact]
    public void ThrowExceptionWhenAssignToCourierAndOrderStatusIsNotCreated()
    {
        var order = new Order(_basketId, _orderLocation);
        order.Assigne(_courier);
        order.Complete();
        
        var exception = Assert.Throws<Exception>(() => order.Assigne(_courier));
        exception.Message.Should().Be($"The order must have the status 'created'");
    }

    [Fact]
    public void CanCompleteOrder()
    {
        var order = new Order(_basketId, _orderLocation);
        order.Assigne(_courier);
        order.Complete();

        order.CourierId.Should().BeNull();
        order.Status.Should().Be(OrderStatus.Completed);
    }
    
    [Fact]
    public void ThrowExceptionWhenTryCompleteAndStatusIsNotAssigned()
    {
        var order = new Order(_basketId, _orderLocation);

        var exception = Assert.Throws<Exception>(() => order.Complete());
        exception.Message.Should().Be($"The order must have the status 'assigned'");
    }
    
    [Fact]
    public void ThrowExceptionWhenTryCompleteAndCourierNotAssigned()
    {
        var order = new Order(_basketId, _orderLocation);

        var exception = Assert.Throws<Exception>(() => order.Complete());
        exception.Message.Should().Be($"The order must have the status 'assigned'");
    }
}