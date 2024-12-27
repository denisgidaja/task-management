using TaskManagement.ServiceBus;
using TaskManagement.Contracts.ServiceBusInterfaces;
using TaskManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Contracts.RepositoryInterfaces;
using TaskManagment.Middlewares;
using TaskManagment.Consumer.Notifications;
using TaskManagement.Contracts.ConsumerInterfaces;
using TaskManagement.FeedbackConsumer;
using SignalRHub;
using TaskManagement.Contracts.Services;
using TaskManagment.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

string connectionString = builder.Configuration.GetValue<string>("ServiceBus:ConnectionString");
string topicName = builder.Configuration.GetValue<string>("ServiceBus:TopicName");

builder.Services.AddSingleton<IServiceBusHandler>(x => new ServiceBusHandler(connectionString, topicName));

builder.Services.AddSingleton(await ServiceBusAdministrationService.CreateAsync(connectionString, topicName));

builder.Services.AddDbContext<RepositoryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

builder.Services.AddSingleton<IServiceBusHandler>(x => new ServiceBusHandler(
    builder.Configuration.GetValue<string>("ServiceBus:ConnectionString"),
    builder.Configuration.GetValue<string>("ServiceBus:TopicName")));

builder.Services.AddSingleton<ServiceBusConsumer>();

builder.Services.AddScoped<IServiceManager, ServiceManager>();

builder.Services.AddScoped<ITaskManagerConsumerService, TaskManagerConsumerService>();

builder.Services.AddSignalR();

//signalr
builder.Services.AddTransient<NotificationService>();


var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<SignalRhub>("/signalHub");

var serviceBusConsumer = app.Services.GetRequiredService<ServiceBusConsumer>();

using var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;

// Start the consumer with retry logic
Task.Run(() => serviceBusConsumer.StartProcessingWithRetryAsync(cancellationToken));

app.Run();

