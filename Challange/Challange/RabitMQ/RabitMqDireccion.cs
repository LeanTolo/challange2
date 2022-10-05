using Challange.Config;
using Challange.Models;
using Challange.RabitMQ.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Challange.RabitMQ
{
    public class RabitMqDireccion : IRabitMQDireccion
    {
        private readonly RabbitConfig _config;
        private readonly ConnectionFactory connectionFactory;
        public RabitMqDireccion(RabbitConfig config)
        {
            _config = config;
            //creamos la connection con la config
            connectionFactory = new ConnectionFactory
            {
                UserName = _config.UserName,
                Password = _config.Password,
                HostName = _config.Hostname,
                Port = Convert.ToInt32(_config.Port)
            };
        }
        public void SendDireccionMessage<T>(T message)
        {
            using (var conn = connectionFactory.CreateConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: _config.QueueName,
                        durable: true, 
                        exclusive: false, 
                        autoDelete: false, 
                        arguments: null
                    );
                    var json = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(json);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    channel.BasicPublish(exchange: "",
                        routingKey: _config.QueueName,
                        basicProperties: properties,
                        body: body
                    );
                }
            }
        }
        public Direccion RecieveDireccionMessage()
        {
            Direccion direccionWithLatLong = null;
            using (var conn = connectionFactory.CreateConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    while (channel.IsOpen && direccionWithLatLong == null)
                    {
                        channel.QueueDeclare(
                            queue: "LocationUpdated",
                            exclusive: false,
                            durable: true,
                            autoDelete: false,
                            arguments: null
                            );
                        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (ch, ea) =>
                        {
                            var body = ea.Body.ToArray();
                            var response = Encoding.UTF8.GetString(body);
                            Console.WriteLine(response);
                            direccionWithLatLong = JsonConvert.DeserializeObject<Direccion>(response);
                            channel.BasicAck(ea.DeliveryTag, false);
                        };
                        channel.BasicConsume("LocationResult", true, consumer);
                    }
                }
            }
            return direccionWithLatLong;
        }
    }
}
