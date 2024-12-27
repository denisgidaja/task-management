using Microsoft.EntityFrameworkCore;
using TaskManagement.Consumer;
using TaskManagement.Contracts.ConsumerInterfaces;
using TaskManagement.Infrastructure;
using TaskManagement.Contracts.RepositoryInterfaces;
using TaskManagement.ServiceBus;
using TaskManagement.Contracts.ServiceBusInterfaces;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<RepositoryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

builder.Services.AddSingleton<IServiceBusHandler>(x => new ServiceBusHandler(
    builder.Configuration.GetValue<string>("ServiceBus:ConnectionString"),
    builder.Configuration.GetValue<string>("ServiceBus:TopicName")));

builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

builder.Services.AddScoped<ITaskManagerConsumerService, TaskManagerConsumerService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
