using Challange.Models;
using Challange.RabitMQ.Interfaces;
using Challange.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Challange.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class GeocodificarController : Controller
    {
        private readonly IGeoService _geoService;
        private readonly IRabitMQDireccion _rabitMQDireccion;

        public GeocodificarController(IGeoService geoService, IRabitMQDireccion rabitMQDireccion)
        {
            _geoService = geoService;
            _rabitMQDireccion = rabitMQDireccion;
        }

        // GET: api/geocodificar?id=5
        [HttpGet("{id}")]
        public ActionResult<Result> GetDireccionById(int id)
        {
            Result result = new Result();
            try
            {
                var direccion2 = _rabitMQDireccion.RecieveDireccionMessage();
                if (direccion2 != null)
                {
                    if (direccion2.Latitud != "" && direccion2.Latitud != null)
                    {
                        _geoService.UpdateDireccion(direccion2);
                    }
                }           

                var direccion = _geoService.GetDireccionById((long)id);
                if (direccion != null)
                {
                    if ((direccion.Latitud != "") && (direccion.Longitud != "") && (direccion.Latitud != null) && (direccion.Longitud != null))
                    {
                        result.Id = direccion.IdDireccion;
                        result.Latitud = direccion.Latitud;
                        result.Longitud = direccion.Longitud;
                        result.Estado = "TERMINADO";
                    }
                    else
                    {
                        result.Id = direccion.IdDireccion;
                        result.Latitud = "xxxx";
                        result.Longitud = "xxxx";
                        result.Estado = "PROCESANDO";
                    }
                }else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return Ok(result);
        }
    }
}
