using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GeoCodificador
{
    public class CodificadorServ
    {
        public LocationModel GetLatLong(string data)
        {
            LocationModel model = JsonConvert.DeserializeObject<LocationModel>(data);
            using var client = new HttpClient();
            string url = String.Format("https://nominatim.openstreetmap.org/search?street={0}&city={1}&state={2}&country={3}&postalcode={4}&format=json&q=", new string[] {
                                    model.Calle + "+"+ model.Numero,
                                    model.Ciudad.Replace(" ","+"),
                                    model.Provincia.Replace(" ","+"),
                                    model.Pais,
                                    model.CodigoPostal
            }); 
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            //"lat":"-38.00952667346939","lon":"-57.572001816326534"
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/2.0)");
            var content = client.SendAsync(requestMessage).Result;
            var stringResponse = content.Content.ReadAsStringAsync().Result;
            if (content.IsSuccessStatusCode)
            {
                var arrayObject = JArray.Parse(stringResponse);
                model.Latitud = arrayObject[0]["lat"].ToString();
                model.Longitud = arrayObject[0]["lon"].ToString();
            }
            if (model.Latitud != "" && model.Longitud != "")
            {
                SendQueue(model);
            }
            return model;          
        }

        public bool SendQueue(LocationModel model)
        {
            bool response = false;
            var factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "guest", Password = "guest", Port = 5672 };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "LocationResult", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    var json = JsonConvert.SerializeObject(model);
                    var body = Encoding.UTF8.GetBytes(json);
                    channel.BasicPublish(exchange: "", routingKey: "LocationResult", basicProperties: null, body: body);
                    response = true;
                }
            }
            return response;
        }
    }
}
