using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.Core.Tests.Domain.Model.CourierAggregate;

public class CourierShould
{
    private const string CourierName = "Courier";
    private const string TransportName = "Car";
    private const int TransportSpeed = 3;
    private readonly Location _courierLocation = Location.MinLocation;

    [Fact]
    public void BeCorrectWhenIsCorrectOnCreated()
    {
        var courier = new Courier(CourierName, TransportName, TransportSpeed, _courierLocation);

        courier.Name.Should().Be("Courier");
        courier.Status.Should().Be(CourierStatus.Free);
        courier.Transport.Name.Should().Be("Car");
        courier.Transport.Speed.Should().Be(3);
        courier.Location.X.Should().Be(1);
        courier.Location.Y.Should().Be(1);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ThrowExceptionWhenNameIsIncorrectOnCreated(string name)
    {
        var exception =
            Assert.Throws<ArgumentException>(() => new Courier(name, TransportName, TransportSpeed, _courierLocation));
        exception.ParamName.Should().Be("name");
    }

    [Fact]
    public void ThrowExceptionWhenLocationIsIncorrectOnCreated()
    {
        var exception =
            Assert.Throws<ArgumentNullException>(() => new Courier(CourierName, TransportName, TransportSpeed, null!));
        exception.ParamName.Should().Be("location");
    }

    [Fact]
    public void BeFreeWhenSetFree()
    {
        var courier = new Courier(CourierName, TransportName, TransportSpeed, _courierLocation);
        courier.SetFree();
        courier.Status.Should().Be(CourierStatus.Free);
    }

    [Fact]
    public void BeBusyWhenSetBusy()
    {
        var courier = new Courier(CourierName, TransportName, TransportSpeed, _courierLocation);
        courier.SetBusy();
        courier.Status.Should().Be(CourierStatus.Busy);
    }

    [Fact]
    public void ThrowExceptionWhenIsBusyAndSetBusy()
    {
        var courier = new Courier(CourierName, TransportName, TransportSpeed, _courierLocation);
        courier.SetBusy();

        var exception = Assert.Throws<Exception>(() => courier.SetBusy());
        exception.Message.Should().Be("Already Busy");
    }

    [Fact]
    public void ThrowExceptionWhenMoveAndTargetLocationIsIncorrect()
    {
        var courier = new Courier(CourierName, TransportName, TransportSpeed, _courierLocation);
        var exception = Assert.Throws<ArgumentNullException>(() => courier.Move(null!));
        exception.ParamName.Should().Be("target");
    }

    [Fact]
    public void WasAbleMoveToTargetLocation()
    {
        var courier = new Courier(CourierName, TransportName, TransportSpeed, _courierLocation);
        var targetLocation = new Location(2, 2);

        courier.Move(targetLocation);

        courier.Location.X.Should().Be(2);
        courier.Location.Y.Should().Be(2);
    }

    [Fact]
    public void ThrowExceptionWhenCalculateTimeToIncorrectLocation()
    {
        var courier = new Courier(CourierName, TransportName, TransportSpeed, _courierLocation);
        var exception = Assert.Throws<ArgumentNullException>(() => courier.CalculateTimeToLocation(null!));
        exception.ParamName.Should().Be("target");
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 0.0)]
    [InlineData(1, 1, 2, 2, 0.66666666666666663)]
    [InlineData(2, 2, 1, 1, 0.66666666666666663)]
    public void CalculatedTimeIsCorrect(int currX, int currY, int targetX, int targetY, double expectedTime)
    {
        var currentLocation = new Location(currX, currY);
        var courier = new Courier(CourierName, TransportName, TransportSpeed, currentLocation);
        var targetLocation = new Location(targetX, targetY);

        var time = courier.CalculateTimeToLocation(targetLocation);
        time.Should().Be(expectedTime);
    }
}