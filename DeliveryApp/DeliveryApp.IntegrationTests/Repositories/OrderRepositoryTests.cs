using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class OrderRepositoryShould : IAsyncLifetime
{
    /// <summary>
    ///     Настройка Postgres из библиотеки TestContainers
    /// </summary>
    /// <remarks>По сути это Docker контейнер с Postgres</remarks>
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:14.7")
        .WithDatabase("order")
        .WithUsername("username")
        .WithPassword("secret")
        .WithCleanUp(true)
        .Build();

    private ApplicationDbContext _context = null!;

    /// <summary>
    ///     Ctr
    /// </summary>
    /// <remarks>Вызывается один раз перед всеми тестами в рамках этого класса</remarks>
    public OrderRepositoryShould()
    {
    }

    /// <summary>
    ///     Инициализируем окружение
    /// </summary>
    /// <remarks>Вызывается перед каждым тестом</remarks>
    public async Task InitializeAsync()
    {
        //Стартуем БД (библиотека TestContainers запускает Docker контейнер с Postgres)
        await _postgreSqlContainer.StartAsync();

        //Накатываем миграции и справочники
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                _postgreSqlContainer.GetConnectionString(),
                sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); })
            .Options;
        _context = new ApplicationDbContext(contextOptions);
        _context.Database.Migrate();
    }

    /// <summary>
    ///     Уничтожаем окружение
    /// </summary>
    /// <remarks>Вызывается после каждого теста</remarks>
    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task CanAddOrder()
    {
        //Arrange
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, Location.MinLocation);

        //Act
        var orderRepository = new OrderRepository(_context);
        var unitOfWork = new UnitOfWork(_context);

        await orderRepository.AddAsync(order);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var orderFromDb = await orderRepository.GetAsync(order.Id);
        orderFromDb.Should().NotBeNull();
        order.Should().BeEquivalentTo(orderFromDb);
    }

    [Fact]
    public async Task CanUpdateOrder()
    {
        //Arrange
        var courier = new Courier("Иван", "Pedestrian", 1, new Location(1, 1));

        var orderId = Guid.NewGuid();
        var order = new Order(orderId, Location.MinLocation);

        var orderRepository = new OrderRepository(_context);
        await orderRepository.AddAsync(order);

        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync();

        //Act
        order.Assigne(courier);
        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var orderFromDb = await orderRepository.GetAsync(order.Id);
        orderFromDb.Should().NotBeNull();
        order.Should().BeEquivalentTo(orderFromDb);
    }

    [Fact]
    public async Task CanGetById()
    {
        //Arrange
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, Location.MinLocation);

        //Act
        var orderRepository = new OrderRepository(_context);
        await orderRepository.AddAsync(order);

        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var orderFromDb = await orderRepository.GetAsync(order.Id);
        orderFromDb.Should().NotBeNull();
        order.Should().BeEquivalentTo(orderFromDb);
    }

    [Fact]
    public async Task CanGetFirstInCreatedStatus()
    {
        //Arrange
        var courier = new Courier("Иван", "Pedestrian", 1, new Location(1, 1));

        var order1Id = Guid.NewGuid();
        var order1 = new Order(order1Id, Location.MinLocation);
        order1.Assigne(courier);

        var order2Id = Guid.NewGuid();
        var order2 = new Order(order2Id, Location.MinLocation);

        var orderRepository = new OrderRepository(_context);
        await orderRepository.AddAsync(order1);
        await orderRepository.AddAsync(order2);

        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync();

        //Act
        var orderFromDb = await orderRepository.GetFirstInCreatedStatusAsync();

        //Assert
        orderFromDb.Should().NotBeNull();
        order2.Should().BeEquivalentTo(orderFromDb);
    }
}