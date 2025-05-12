using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.Core.Tests.UseCases.Commands;

public class CreateOrderCommandShould
{
    private readonly IOrderRepository _orderRepositoryMock;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateOrderCommandShould()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
    }
    
    private Order? EmptyOrder()
    {
        return null;
    }
    
    private Result<Location> DefaultLocation()
    {
        return new Location(1, 1);
    }
    
    private Order ExistedOrder()
    {
        return new Order(Guid.NewGuid(), new Location(1, 1));
    }
    
    [Fact]
    public async Task ReturnTrueWhenOrderExists()
    {
        _orderRepositoryMock.GetAsync(Arg.Any<Guid>())!
            .Returns(Task.FromResult(ExistedOrder()));
        _unitOfWork.SaveChangesAsync()
            .Returns(Task.FromResult(true));

        var command = new CreateOrderCommand(Guid.NewGuid(), "улица");
        var handler = new CreateOrderHandler(_unitOfWork, _orderRepositoryMock);
        await handler.Handle(command, CancellationToken.None);
    }
    
    [Fact]
    public async Task ReturnTrueWhenOrderCreatedSuccessfully()
    {
        //Arrange
        _orderRepositoryMock.GetAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult(EmptyOrder()));
        _unitOfWork.SaveChangesAsync()
            .Returns(Task.FromResult(true));

        //Act
        var command = new CreateOrderCommand(Guid.NewGuid(), "улица");
        var handler = new CreateOrderHandler(_unitOfWork, _orderRepositoryMock);
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        _orderRepositoryMock.Received(1);
        _unitOfWork.Received(1);
    }
}