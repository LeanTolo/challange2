using Challange.Data;
using Challange.Models;
using Challange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Challange.Services
{
    public class GeoService : IGeoService
    {
        private readonly AppDbContext _dbContext;
        public GeoService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Direccion GetDireccionById(long id)
        {
            return _dbContext.Direcciones.Where(x => x.IdDireccion == id).FirstOrDefault();
        }

        public Direccion AddDireccion(Direccion direccion)
        {
            var result = _dbContext.Direcciones.Add(direccion);
            _dbContext.SaveChanges();
            return result.Entity;
        }

        public void UpdateDireccion(Direccion direccion)
        {
            _dbContext.Entry(direccion).State = EntityState.Modified;
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
    }
}
