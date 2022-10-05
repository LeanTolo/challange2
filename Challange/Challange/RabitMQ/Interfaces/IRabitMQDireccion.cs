using Challange.Models;

namespace Challange.RabitMQ.Interfaces
{
    public interface IRabitMQDireccion
    {
        public void SendDireccionMessage<T>(T message);
        public Direccion RecieveDireccionMessage();

    }
}
