using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Unit>
{
    public CreateOrderCommand(Guid basketId, string street)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(basketId, Guid.Empty);
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentNullException(nameof(street));
        
        BasketId = basketId;
        Street = street;
    }
    
    public Guid BasketId { get; set; }
    public string Street { get; set; }
}