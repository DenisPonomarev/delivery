using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.Core.Tests.Domain.Model.CourierAggregate;

public class CourierStatusShould
{
    [Fact]
    public void StatusesNameIsCorrect()
    {
        var free = CourierStatus.Free;
        var busy = CourierStatus.Busy;

        free.Name.Should().Be("free");
        busy.Name.Should().Be("busy");
    }
    
    [Fact]
    public void BeEqualWhenNamesAreEqual()
    {
        var firstFree = CourierStatus.Free;
        var secondFree = CourierStatus.Free;

        var isEqual = firstFree == secondFree;

        isEqual.Should().BeTrue();
    }
    
    [Fact]
    public void BeNotEqualWhenNamesAreNotEqual()
    {
        var free = CourierStatus.Free;
        var busy = CourierStatus.Busy;

        var isNotEqual = free != busy;

        isNotEqual.Should().BeTrue();
    }
}