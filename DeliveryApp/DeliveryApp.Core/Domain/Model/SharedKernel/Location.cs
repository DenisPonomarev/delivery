using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Model.SharedKernel;

public class Location : ValueObject
{
    private const int MIN_X = 1;
    private const int MAX_X = 10;
    private const int MIN_Y = 1;
    private const int MAX_Y = 10;

    [ExcludeFromCodeCoverage]
    private Location()
    {
    }

    private Location(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }

    public int DistanceTo(Location location)
    {
        ArgumentNullException.ThrowIfNull(location);
        return Math.Abs(X - location.X) + Math.Abs(Y - location.Y);
    }

    public static Location Create(int x, int y)
    {
        if (x is < MIN_X or > MAX_X) throw new ArgumentOutOfRangeException(nameof(x));
        if (y is < MIN_Y or > MAX_Y) throw new ArgumentOutOfRangeException(nameof(y));

        return new(x, y);
    }

    public static Location CreateRandom()
    {
        var random = new Random();
        var x = random.Next(MIN_X, MAX_X + 1);
        var y = random.Next(MIN_Y, MAX_Y + 1);
        return new(x, y);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}