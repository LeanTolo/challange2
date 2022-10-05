using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Challange.Models
{
    public class Direccion
    {
        [Key]
        [JsonIgnore]
        public long IdDireccion { get; set; }
        public string Calle { get; set; }
        public int Numero { get; set; }
        public string Ciudad { get; set; }
        public string Provincia { get; set; }
        public string Pais { get; set; }
        public string CodigoPostal { get; set; }

        [JsonIgnore]
        public string? Latitud { get; set; }
        [JsonIgnore]
        public string? Longitud { get; set; }


    }
}
