using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;

    public CreateOrderHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
    }

    public async Task<Unit> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var dbOrder = await _orderRepository.GetAsync(request.BasketId);
        if (dbOrder != null)
            return Unit.Value;
        
        var location = Location.CreateRandom();
        var order = new Order(request.BasketId, location);
        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}