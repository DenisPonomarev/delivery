using System.Diagnostics.CodeAnalysis;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public class Courier : Aggregate<Guid>
{
    [ExcludeFromCodeCoverage]
    private Courier()
    {
    }

    public Courier(string name, string transportName, int transportSpeed, Location location) : this()
    {
        if (string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException(null, nameof(name));
        
        ArgumentNullException.ThrowIfNull(location);

        Id = Guid.NewGuid();
        Name = name;
        Location = location;
        Transport = new Transport(transportName, transportSpeed);
        Status = CourierStatus.Free;
    }

    public string Name { get; private set; }
    public Transport Transport { get; private set; }
    public Location Location { get; private set; }
    public CourierStatus Status { get; private set; }

    public void SetFree()
    {
        Status = CourierStatus.Free;
    }

    public void SetBusy()
    {
        if (Status == CourierStatus.Busy)
            throw new Exception("Already Busy");
        
        Status = CourierStatus.Busy;
    }

    public void Move(Location target)
    {
        ArgumentNullException.ThrowIfNull(target);

        Location = Transport.Move(Location, target);
    }

    public double CalculateTimeToLocation(Location target)
    {
        ArgumentNullException.ThrowIfNull(target);

        var distance = Location.DistanceTo(target);
        return (double)distance / Transport.Speed;
    }
}