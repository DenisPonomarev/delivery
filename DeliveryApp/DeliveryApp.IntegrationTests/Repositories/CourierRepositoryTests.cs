using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class CourierRepositoryShould : IAsyncLifetime
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
    public CourierRepositoryShould()
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
        await _context.Database.MigrateAsync();
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
    public async Task CanAddCourier()
    {
        //Arrange
        var courier = new Courier("Иван", "Pedestrian", 1, new Location(1, 1));

        //Act
        var courierRepository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        await courierRepository.AddAsync(courier);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var courierFromDb = await courierRepository.GetAsync(courier.Id);
        courierFromDb.Should().NotBeNull();
        courier.Should().BeEquivalentTo(courierFromDb);
    }

    [Fact]
    public async Task CanUpdateCourier()
    {
        //Arrange
        var courier = new Courier("Иван", "Pedestrian", 1, new Location(1, 1));

        var courierRepository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        await courierRepository.AddAsync(courier);
        await unitOfWork.SaveChangesAsync();

        //Act
        courier.SetBusy();
        courierRepository.Update(courier);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var courierFromDb = await courierRepository.GetAsync(courier.Id);
        courierFromDb.Should().NotBeNull();
        courier.Should().BeEquivalentTo(courierFromDb);
        courierFromDb.Status.Should().Be(CourierStatus.Busy);
    }

    [Fact]
    public async Task CanGetById()
    {
        //Arrange
        var courier = new Courier("Иван", "Pedestrian", 1, new Location(1, 1));

        var courierRepository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        await courierRepository.AddAsync(courier);
        await unitOfWork.SaveChangesAsync();

        //Act
        var courierFromDb = await courierRepository.GetAsync(courier.Id);

        //Assert
        courierFromDb.Should().NotBeNull();
        courier.Should().BeEquivalentTo(courierFromDb);
    }

    [Fact]
    public async Task CanGetFirstInCreatedStatus()
    {
        //Arrange
        var courier1 = new Courier("Иван", "Pedestrian", 1, new Location(1, 1));
        courier1.SetBusy();

        var courier2 = new Courier("Борис", "Pedestrian", 1, new Location(1, 1));

        var courierRepository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        await courierRepository.AddAsync(courier1);
        await courierRepository.AddAsync(courier2);
        await unitOfWork.SaveChangesAsync();

        //Act
        var couriersFromDb = await courierRepository.GetAllInFreeStatusAsync();

        //Assert
        couriersFromDb.Should().NotBeEmpty();
        couriersFromDb.Count.Should().Be(1);
        couriersFromDb.First().Should().BeEquivalentTo(courier2);
    }
}