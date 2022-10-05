using Challange.Models;
using Challange.RabitMQ.Interfaces;
using Challange.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Challange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeolocalizarController : Controller
    {
        private readonly IGeoService _geoService;
        private readonly IRabitMQDireccion _rabitMQDireccion;
        public GeolocalizarController(IGeoService geoService, IRabitMQDireccion rabitMQDireccion)
        {
            _geoService = geoService;
            _rabitMQDireccion = rabitMQDireccion;
        }
        
        [HttpPost("geolocalizar")]
        public async Task<ActionResult> AddDireccion(Direccion direccion)
        {
            var directionWithId = _geoService.AddDireccion(direccion);
            //envia la data insertada a la queue y el consumidor la recibe
            _rabitMQDireccion.SendDireccionMessage(directionWithId);
            return StatusCode(202, new { id = directionWithId.IdDireccion });
        }

    }
}
