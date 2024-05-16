using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace TechChallenge.Worker;

public class Worker(
    ILogger<Worker> logger,
    IServiceScopeFactory serviceScopeFactory
) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var connection = Connect();
                using var channel = connection.CreateModel();
                channel.ExchangeDeclare(exchange: "fiap.techchallenge", type: ExchangeType.Fanout);
                channel.QueueDeclare(queue: "definir_solucionador",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                channel.QueueBind(queue: "definir_solucionador",
                                  exchange: "fiap.techchallenge",
                                  routingKey: "demanda");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    byte[] body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var transacaoDTO = JsonSerializer.Deserialize<TransacaoDTO>(message);
                    if (transacaoDTO is not null)
                    {
                        using IServiceScope scope = _serviceScopeFactory.CreateScope();
                        try
                        {
                            
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, message);
                        }
                    }
                };
                channel.BasicConsume(queue: "definir_solucionador",
                                     autoAck: true,
                                     consumer: consumer);
                _logger.LogInformation("Consumer started at: {Time}", DateTimeOffset.Now);
            }
        }, stoppingToken);
    }

    private IConnection Connect()
    {
        IConnection? connection = null;
        while (connection is null)
        {
            try
            {
                ConnectionFactory factory = new()
                {
                    HostName = _appSettings.GetValue("RabbitMQ:HostName"),
                    UserName = _appSettings.GetValue("RabbitMQ:UserName"),
                    Password = _appSettings.GetValue("RabbitMQ:Password")
                };
                connection = factory.CreateConnection();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to connect to RabbitMQ {Message}:", e.Message);
                _logger.LogError("Retrying in 10 seconds...");
                Task.Delay(10000).Wait();
            }
        }
        return connection;
    }
}
