using GeoCodificador;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;


try
{
    var factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "guest", Password = "guest", Port = 5672 };
    string content = null;
    using (var _connection = factory.CreateConnection())
    {
        using (var _channel = _connection.CreateModel())
        {
            while (_channel.IsOpen && content == null)
            {
                _channel.QueueDeclare(queue: "GeoCodeResult", durable: true, exclusive: false, autoDelete: false, arguments: null);
                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (ch, ea) =>
                {
                    var body = ea.Body.ToArray();
                    content = Encoding.UTF8.GetString(body);
                    _channel.BasicAck(ea.DeliveryTag, false);
                };

                _channel.BasicConsume(queue: "GeoCode", autoAck: true, consumer: consumer);
            }
        }
    }
    Console.WriteLine(content);
    CodificadorServ service = new CodificadorServ();
    service.GetLatLong(content);
}
catch (Exception ex)
{
    Console.WriteLine("Error" + ex.Message.ToString());
}