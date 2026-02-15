using System.Text;
using System.Text.Json;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Models.Dtos;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ManagerTask;

public class RabbitMqService : IRabbitMqService
{
    private readonly RabbitOptions _rabbitOptions;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqService(IOptions<RabbitOptions> options)
    {
        _rabbitOptions = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = _rabbitOptions.HostName,
            UserName = _rabbitOptions.UserName,
            Password = _rabbitOptions.Password
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.QueueDeclareAsync(
            queue: _rabbitOptions.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        ).GetAwaiter().GetResult();

        _channel.QueueBindAsync(
            queue: _rabbitOptions.QueueName,
            exchange: _rabbitOptions.Exchange,
            routingKey: RoutingKeys.TaskNotificationKey,
            arguments: null
        ).GetAwaiter().GetResult();
        
        _channel.QueueDeclareAsync(
            queue: _rabbitOptions.NotificationQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        ).GetAwaiter().GetResult();

        _channel.QueueBindAsync(
            queue: _rabbitOptions.NotificationQueueName,
            exchange: _rabbitOptions.Exchange,
            routingKey: RoutingKeys.NotificationKey,
            arguments: null
        ).GetAwaiter().GetResult();
    }

    public async Task Publish(TaskCreatedNotification notification)
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(notification, options);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties();

        properties.Persistent = true;
        properties.ContentType = "application/json";

        await _channel.BasicPublishAsync(
            exchange: _rabbitOptions.Exchange,
            routingKey: RoutingKeys.TaskNotificationKey,
            mandatory: false,
            basicProperties: properties,
            body: body
        );
    }

    public async Task NotificationPublish(NotificationCreatedNotification notification)
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(notification, options);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties();

        properties.Persistent = true;
        properties.ContentType = "application/json";

        await _channel.BasicPublishAsync(
            exchange: _rabbitOptions.Exchange,
            routingKey: RoutingKeys.NotificationKey,
            mandatory: false,
            basicProperties: properties,
            body: body
        );
    }

    public void Dispose()
    {
        _channel.CloseAsync().GetAwaiter().GetResult();
        _connection.CloseAsync().GetAwaiter().GetResult();
    }

}
