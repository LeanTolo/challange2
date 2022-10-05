using Challange.Models;

namespace Challange.Services.Interfaces
{
    public interface IGeoService
    {
        public Direccion GetDireccionById(long id);
        public Direccion AddDireccion(Direccion direccion);
        public void UpdateDireccion(Direccion direccion);

    }
}
