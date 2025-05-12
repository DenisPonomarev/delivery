using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;

public class MoveCouriersHandler : IRequestHandler<MoveCouriersCommand, Unit>
{
    private readonly ICourierRepository _courierRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MoveCouriersHandler(ICourierRepository courierRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _courierRepository = courierRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(MoveCouriersCommand request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllInAssignedStatusAsync();
        if (orders.Count == 0)
            return Unit.Value;

        foreach (var order in orders)
        {
            if (order.CourierId == null)
                throw new Exception("К заказу не привязан курьер.");
            
            var courier = await _courierRepository.GetAsync(order.CourierId.Value);
            if (courier == null)
                throw new Exception("Курьер, привязанный к заказу, не найден.");
            
            courier.Move(order.Location);

            if (courier.Location == order.Location)
            {
                order.Complete();
                courier.SetFree();
            }
            
            _courierRepository.Update(courier);
            _orderRepository.Update(order);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}