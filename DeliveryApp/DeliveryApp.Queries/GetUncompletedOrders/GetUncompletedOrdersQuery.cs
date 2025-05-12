using MediatR;

namespace DeliveryApp.Queries.GetUncompletedOrders;

public class GetUncompletedOrdersQuery : IRequest<GetUncompletedOrdersResponse>;