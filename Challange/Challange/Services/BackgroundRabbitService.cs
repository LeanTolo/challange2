using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Channels;
using Challange.RabitMQ.Interfaces;
using Challange.Services.Interfaces;

namespace Challange.Services
{
    public class BackgroundRabbitService : BackgroundService
    {
        private readonly IRabitMQDireccion _rabitMQDireccion;
        private readonly IGeoService _geoService;

        public BackgroundRabbitService(IRabitMQDireccion rabitMQDireccion, IGeoService geoService)
        {
            _rabitMQDireccion = rabitMQDireccion;
            _geoService = geoService;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //token campurador de evento 
            stoppingToken.ThrowIfCancellationRequested();
            try
            {
                var direccion = _rabitMQDireccion.RecieveDireccionMessage();
                _geoService.UpdateDireccion(direccion);
            }
            catch (Exception)
            {
                throw;
            }
            return Task.CompletedTask;
        }
    }
}
