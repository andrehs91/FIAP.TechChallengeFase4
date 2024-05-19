using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TechChallenge.Dominio.Entities;
using TechChallenge.Dominio.Enums;
using TechChallenge.Dominio.Interfaces;
using TechChallenge.Infraestrutura.Settings;

namespace TechChallenge.Worker;

public class Worker(
    ILogger<Worker> logger,
    IAppSettings appSettings,
    IServiceScopeFactory serviceScopeFactory
) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IAppSettings _appSettings = appSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private IConnection? Connection;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Connection ??= Connect();
            using var channel = Connection.CreateModel();
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
            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                if (int.TryParse(message, out int idDemanda))
                {
                    using IServiceScope scope = _serviceScopeFactory.CreateScope();
                    var demandaRepository = scope.ServiceProvider.GetRequiredService<IDemandaRepository>();
                    var solucionadorPolicy = scope.ServiceProvider.GetRequiredService<ISolucionadorPolicy>();
                    try
                    {
                        Demanda? demanda = demandaRepository.BuscarPorId(idDemanda);
                        if (demanda is not null && demanda.Situacao == Situacoes.AguardandoDistribuicao)
                        {
                            Usuario? solucionador =
                                solucionadorPolicy.IdentificarSolucionadorMenosAtarefado(demanda.AtividadeId);
                            if (solucionador is not null)
                            {
                                demanda.DefinirSolucionador(solucionador);
                                demandaRepository.Editar(demanda);
                            }
                        }
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception e)
                    {
                        channel.BasicNack(ea.DeliveryTag, false, true);
                        _logger.LogError(e, message);
                    }
                }
            };
            channel.BasicConsume(queue: "definir_solucionador",
                                    autoAck: false,
                                    consumer: consumer);
            _logger.LogInformation("Consumer started at: {Time}", DateTimeOffset.Now);
            await Task.Delay(5000, stoppingToken);
        }
    }

    private IConnection Connect()
    {
        int tries = 0;
        IConnection? connection = null;
        while (connection is null)
        {
            try
            {
                ConnectionFactory factory = new()
                {
                    HostName = _appSettings.GetValue("RabbitMQ:HostName"),
                    UserName = _appSettings.GetValue("RabbitMQ:UserName"),
                    Password = _appSettings.GetValue("RabbitMQ:Password"),
                };
                connection = factory.CreateConnection();
            }
            catch (Exception e)
            {
                tries++;
                _logger.LogWarning("Failed to connect to RabbitMQ: {Message}", e.Message);
                _logger.LogWarning("Retrying in 20 seconds...");
                if (tries < 10) Task.Delay(20000).Wait();
                else throw;
            }
        }
        _logger.LogInformation("Connected to RabbitMQ.");
        return connection;
    }
}
