using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using CSharpFunctionalExtensions;
using DeliveryApp.Api;
using DeliveryApp.Core.Application.UseCases.Commands.AssignCourier;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using DeliveryApp.Queries.GetBusyCouriers;
using DeliveryApp.Queries.GetUncompletedOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Primitives;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin();
    });
});

builder.Services.ConfigureOptions<SettingsSetup>();

var connectionString = builder.Configuration["CONNECTION_STRING"];
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionString,
            sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); });
        options.EnableSensitiveDataLogging();
    }
);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IDispatchService, DispatchService>();

builder.Services.AddScoped<IRequestHandler<CreateOrderCommand, Unit>, CreateOrderHandler>();
builder.Services.AddScoped<IRequestHandler<MoveCouriersCommand, Unit>, MoveCouriersHandler>();
builder.Services.AddScoped<IRequestHandler<AssignCourierCommand, Unit>, AssignCourierHandler>();

builder.Services.AddScoped<IRequestHandler<GetBusyCouriersQuery, GetBusyCouriersResponse>, GetBusyCouriersHandler>();
builder.Services.AddScoped<IRequestHandler<GetUncompletedOrdersQuery, GetUncompletedOrdersResponse>, GetUncompletedOrdersHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHealthChecks("/health");
app.UseRouting();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();