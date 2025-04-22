using DeliveryApp.Core.Domain.Model.OrderAggregate;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.Core.Tests.Domain.Model.OrderAggregate;

public class OrderStatusShould
{
    [Fact]
    public void StatusesNameIsCorrect()
    {
        var created = OrderStatus.Created;
        var assigned = OrderStatus.Assigned;
        var completed = OrderStatus.Completed;

        created.Name.Should().Be("created");
        assigned.Name.Should().Be("assigned");
        completed.Name.Should().Be("completed");
    }

    [Fact]
    public void BeEqualWhenNamesAreEqual()
    {
        var firstCreated = OrderStatus.Created;
        var secondCreated = OrderStatus.Created;

        var isEqual = firstCreated == secondCreated;

        isEqual.Should().BeTrue();
    }

    [Fact]
    public void BeNotEqualWhenNamesAreNotEqual()
    {
        var created = OrderStatus.Created;
        var assigned = OrderStatus.Assigned;

        var isNotEqual = created != assigned;

        isNotEqual.Should().BeTrue();
    }
}