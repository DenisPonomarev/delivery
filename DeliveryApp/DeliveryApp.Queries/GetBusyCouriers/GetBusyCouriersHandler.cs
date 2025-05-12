using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Queries.GetBusyCouriers;

public class GetBusyCouriersHandler : IRequestHandler<GetBusyCouriersQuery, GetBusyCouriersResponse>
{
    private readonly ApplicationDbContext _dbContext;

    public GetBusyCouriersHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<GetBusyCouriersResponse> Handle(GetBusyCouriersQuery request, CancellationToken cancellationToken)
    {
        var couriers = await _dbContext.Couriers
            .AsNoTracking()
            .AsSplitQuery()
            .Where(c => c.Status.Name == CourierStatus.Busy.Name)
            .Select(c => new GetBusyCouriersResponse.Courier
            {
                Id = c.Id,
                Name = c.Name,
                Location = new GetBusyCouriersResponse.Location
                {
                    X = c.Location.X,
                    Y = c.Location.Y
                }
            })
            .ToListAsync(cancellationToken: cancellationToken);

        return new GetBusyCouriersResponse
        {
            Couriers = couriers
        };
    }
}