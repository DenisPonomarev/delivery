using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.Core.Tests.Domain.Model.SharedKernel;

public class LocationShould
{
    [Fact]
    public void BeCorrectWhenIsCorrectOnCreated()
    {
        const byte x = 1, y = 1;

        var location = Location.Create(x, y);

        location.X.Should().Be(x);
        location.Y.Should().Be(y);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void ThrowExceptionWhenXCoordinateIsIncorrect(byte x)
    {
        const byte y = 1;
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Location.Create(x, y));
        exception.ParamName.Should().Be("x");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void ThrowExceptionWhenYCoordinateIsIncorrect(byte y)
    {
        const byte x = 1;
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Location.Create(x, y));
        exception.ParamName.Should().Be("y");
    }

    [Fact]
    public void BeEqualWhenCoordinatesAreEqual()
    {
        const byte x = 1, y = 1;

        var firstLocation = Location.Create(x, y);
        var secondLocation = Location.Create(x, y);

        var isEqual = firstLocation == secondLocation;

        isEqual.Should().BeTrue();
    }

    [Theory]
    [InlineData(1, 1, 1, 2)]
    [InlineData(1, 1, 2, 1)]
    [InlineData(1, 1, 2, 2)]
    public void BeNotEqualWhenCoordinatesAreNotEqual(byte x1, byte y1, byte x2, byte y2)
    {
        var firstLocation = Location.Create(x1, y1);
        var secondLocation = Location.Create(x2, y2);

        var isNotEqual = firstLocation != secondLocation;

        isNotEqual.Should().BeTrue();
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 0)]
    [InlineData(1, 1, 2, 1, 1)]
    [InlineData(1, 1, 2, 2, 2)]
    [InlineData(2, 1, 1, 1, 1)]
    [InlineData(2, 2, 1, 1, 2)]
    public void DistanceIsCorrect(byte x1, byte y1, byte x2, byte y2, int distance)
    {
        var firstLocation = Location.Create(x1, y1);
        var secondLocation = Location.Create(x2, y2);

        firstLocation.DistanceTo(secondLocation).Should().Be(distance);
    }

    [Fact]
    public void ThrowExceptionWhenOtherLocationIsNull()
    {
        var location = Location.Create(1, 1);
        var exception = Assert.Throws<ArgumentNullException>(() => location.DistanceTo(null!));
        exception.ParamName.Should().Be("location");
    }

    [Fact]
    public void BeRandomWhenRandomLocationsOnCreated()
    {
        const int sequenceLength = 5;
        var uniqueLocationsCount = Enumerable.Range(0, sequenceLength).Select(_ => Location.CreateRandom()).Distinct().Count();
        uniqueLocationsCount.Should().BeGreaterThan(1);
    }
}