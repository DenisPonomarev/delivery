using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public class CourierStatus : ValueObject
{
    public static readonly CourierStatus Free = new(nameof(Free).ToLowerInvariant());
    public static readonly CourierStatus Busy = new(nameof(Busy).ToLowerInvariant());
    
    [ExcludeFromCodeCoverage]
    private CourierStatus()
    {
    }

    private CourierStatus(string name)
    {
        Name = name;
    }
    
    public string Name { get; private set; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}