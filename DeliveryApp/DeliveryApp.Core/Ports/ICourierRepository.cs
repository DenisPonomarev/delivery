using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;

namespace DeliveryApp.Core.Ports;

public interface ICourierRepository
{
    Task AddAsync(Courier courier);
    void Update(Courier courier);
    Task<Courier?> GetAsync(Guid courierId);
    IEnumerable<Courier> GetAllInFreeStatus();
}