using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignCourier;

public class AssignCourierHandler : IRequestHandler<AssignCourierCommand, Unit>
{
    private readonly ICourierRepository _courierRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IDispatchService _dispatchService;
    private readonly IUnitOfWork _unitOfWork;

    public AssignCourierHandler(ICourierRepository courierRepository, IOrderRepository orderRepository, IDispatchService dispatchService, IUnitOfWork unitOfWork)
    {
        _courierRepository = courierRepository;
        _orderRepository = orderRepository;
        _dispatchService = dispatchService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AssignCourierCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetFirstInCreatedStatusAsync();
        if (order == null)
            throw new Exception("Нет заказов, для назначения");
        
        var couriers = await _courierRepository.GetAllInFreeStatusAsync();
        if (couriers.Count == 0)
            throw new Exception("Нет доступных курьеров");
        
        var courier = _dispatchService.Dispatch(order, couriers);
        
        _orderRepository.Update(order);
        _courierRepository.Update(courier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}