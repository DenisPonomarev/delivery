using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Model.SharedKernel;

public class Location : ValueObject
{
    private const byte MIN_X = 1;
    private const byte MAX_X = 10;
    private const byte MIN_Y = 1;
    private const byte MAX_Y = 10;

    [ExcludeFromCodeCoverage]
    private Location()
    {
    }

    private Location(byte x, byte y)
    {
        X = x;
        Y = y;
    }

    public byte X { get; }
    public byte Y { get; }

    public int DistanceTo(Location location)
    {
        return Math.Abs(X - location.X) + Math.Abs(Y - location.Y);
    }

    public static Location Create(byte x, byte y)
    {
        if (x is < MIN_X or > MAX_X)
            throw new ArgumentOutOfRangeException(nameof(x));

        if (y is < MIN_Y or > MAX_Y)
            throw new ArgumentOutOfRangeException(nameof(y));

        return new(x, y);
    }

    public static Location CreateRandom()
    {
        var random = new Random();
        var x = (byte)random.Next(MIN_X, MAX_X + 1);
        var y = (byte)random.Next(MIN_Y, MAX_Y + 1);
        return new(x, y);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}