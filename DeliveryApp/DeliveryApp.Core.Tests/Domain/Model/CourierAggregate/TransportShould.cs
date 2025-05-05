using System.Security.Cryptography;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;
using domainCourier = DeliveryApp.Core.Domain.Model.CourierAggregate;

namespace DeliveryApp.Core.Tests.Domain.Model.CourierAggregate;

public class TransportShould
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void BeCorrectWhenIsCorrectOnCreated(int speed)
    {
        const string name = "car";

        var transport = new domainCourier.Transport(name, speed);

        transport.Name.Should().Be(name);
        transport.Speed.Should().Be(speed);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(4)]
    public void ThrowExceptionWhenSpeedIsIncorrect(int speed)
    {
        const string name = "car";
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new domainCourier.Transport(name, speed));
        exception.ParamName.Should().Be("speed");
    }

    [Theory]
#pragma warning disable xUnit1012
    [InlineData(null)]
#pragma warning restore xUnit1012
    [InlineData("")]
    public void ThrowExceptionWhenNameIsIncorrect(string name)
    {
        const int speed = 1;
        var exception = Assert.Throws<ArgumentException>(() => new domainCourier.Transport(name, speed));
        exception.ParamName.Should().Be("name");
    }

    [Fact]
    public void ThrowExceptionWhenMoveWithNullCurrentLocation()
    {
        const string name = "car";
        const int speed = 3;

        var transport = new domainCourier.Transport(name, speed);

        var targetLocation = new Location(1, 1);

        var exception = Assert.Throws<NullReferenceException>(() => transport.Move(null!, targetLocation));
        exception.Message.Should().Be("current");
    }

    [Fact]
    public void ThrowExceptionWhenMoveWithNullTargetLocation()
    {
        const string name = "car";
        const int speed = 3;

        var transport = new domainCourier.Transport(name, speed);

        var currentLocation = new Location(1, 1);

        var exception = Assert.Throws<NullReferenceException>(() => transport.Move(currentLocation, null!));
        exception.Message.Should().Be("target");
    }

    [Theory]
    // Нет необходимости перемещаться
    [InlineData(1, 3, 3, 3, 3, 3, 3)]
    // Можем переместиться на одну клетку по горизонтали или вертикали
    [InlineData(1, 3, 3, 4, 3, 4, 3)]
    [InlineData(1, 3, 3, 3, 2, 3, 2)]
    [InlineData(1, 3, 3, 2, 3, 2, 3)]
    [InlineData(1, 3, 3, 3, 4, 3, 4)]
    // Можем переместиться, скорости не хватает чтобы добраться за один шаг
    [InlineData(1, 3, 3, 3, 1, 3, 2)]
    [InlineData(1, 3, 3, 4, 2, 4, 3)]
    [InlineData(1, 3, 3, 5, 3, 4, 3)]
    [InlineData(1, 3, 3, 4, 4, 4, 3)]
    [InlineData(1, 3, 3, 3, 5, 3, 4)]
    [InlineData(1, 3, 3, 2, 4, 2, 3)]
    [InlineData(1, 3, 3, 1, 3, 2, 3)]
    [InlineData(1, 3, 3, 2, 2, 2, 3)]
    // Можем переместиться, скорости хватает чтобы добраться за один шаг
    [InlineData(2, 3, 3, 3, 1, 3, 1)]
    [InlineData(2, 3, 3, 4, 2, 4, 2)]
    [InlineData(2, 3, 3, 5, 3, 5, 3)]
    [InlineData(2, 3, 3, 4, 4, 4, 4)]
    [InlineData(2, 3, 3, 3, 5, 3, 5)]
    [InlineData(2, 3, 3, 2, 4, 2, 4)]
    [InlineData(2, 3, 3, 1, 3, 1, 3)]
    [InlineData(2, 3, 3, 2, 2, 2, 2)]
    public void MovementIsCorrect(int speed, int currX, int currY, int targetX, int targetY, int expectedX,
        int expectedY)
    {
        const string name = "car";

        var transport = new domainCourier.Transport(name, speed);

        var currentLocation = new Location(currX, currY);
        var targetLocation = new Location(targetX, targetY);

        var expectedLocation = transport.Move(currentLocation, targetLocation);

        expectedLocation.X.Should().Be(expectedX);
        expectedLocation.Y.Should().Be(expectedY);
    }
}