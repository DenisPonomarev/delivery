using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Microsoft.VisualBasic.FileIO;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public class Transport : Entity<Guid>
{
    private const int MIN_SPEED = 1;
    private const int MAX_SPEED = 3;
    
    [ExcludeFromCodeCoverage]
    private Transport()
    {
    }

    private Transport(string name, int speed)
    {
        Id = Guid.NewGuid();
        Name = name;
        Speed = speed;
    }

    public string Name { get; private set; } = null!;
    public int Speed { get; private set; }
    
    public static Transport Create(string name, int speed)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(null, nameof(name));
        if (speed is < MIN_SPEED or > MAX_SPEED)
            throw new ArgumentOutOfRangeException(nameof(speed));
        return new(name, speed);
    }

    public Location Move(Location current, Location target)
    {
        if (current == null) throw new NullReferenceException(nameof(current));
        if (target == null) throw new NullReferenceException(nameof(target));
        if (current == target) return current;
        
        var difX = target.X - current.X;
        var difY = target.Y - current.Y;
        var cruisingRange = Speed;
        
        var moveX = Math.Clamp(difX, -cruisingRange, cruisingRange);
        cruisingRange -= Math.Abs(moveX);
        
        var moveY = Math.Clamp(difY, -cruisingRange, cruisingRange);
        
        return Location.Create(current.X + moveX, current.Y + moveY);
    }
}