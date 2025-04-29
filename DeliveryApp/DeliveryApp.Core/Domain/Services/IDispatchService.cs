using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;

namespace DeliveryApp.Core.Domain.Services;

public interface IDispatchService
{
    Courier Dispatch(Order order, List<Courier> couriers);
}