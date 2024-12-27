using TaskManagement.Contracts.ConsumerInterfaces;
using TaskManagement.ServiceBus;

namespace TaskManagement.Consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private ITaskManagerConsumerService _consumerService;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                if (!ServiceBusHandler.ConsumerConnected)
                {
                    _logger.LogInformation("Trying to connect consumer with topic");

                    var scope = _serviceProvider.CreateScope();

                    _consumerService = scope.ServiceProvider.GetRequiredService<ITaskManagerConsumerService>();

                    try
                    {
                        await _consumerService.RegisterConsumer();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("An error occurred in the with the consumer connection with service bus, Error: {Error}", ex.Message);

                        _logger.LogInformation("Retrying to establish connection in 5 sedonds");

                        //Needs to be configurable parameter
                        await Task.Delay(5000, stoppingToken);
                    }
                }

                //Needs to be configurable parameter
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
