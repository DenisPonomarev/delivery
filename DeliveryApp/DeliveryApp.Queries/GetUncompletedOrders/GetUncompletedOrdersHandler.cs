using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Queries.GetUncompletedOrders;

public class GetUncompletedOrdersHandler : IRequestHandler<GetUncompletedOrdersQuery, GetUncompletedOrdersResponse>
{
    private readonly ApplicationDbContext _dbContext;

    public GetUncompletedOrdersHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetUncompletedOrdersResponse> Handle(GetUncompletedOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _dbContext.Orders
            .Where(o => o.Status.Name != OrderStatus.Completed.Name)
            .Select(o => new GetUncompletedOrdersResponse.Order
            {
                Id = o.Id,
                Location = new GetUncompletedOrdersResponse.Location
                {
                    X = o.Location.X,
                    Y = o.Location.Y
                }
            })
            .ToListAsync(cancellationToken);
        
        return new GetUncompletedOrdersResponse
        {
            Orders = orders
        };
    }
}