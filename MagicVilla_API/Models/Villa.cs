using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_API.Models
// Modelo.Villa
{
    public class Villa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Como controlaremos el ID
        public int Id { get; set; } // PK
        public string Nombre { get; set; }

        public string Detalle { get; set; }

        [Required]
        public Double Tarifa { get; set; }

        public int Ocupantes { get; set; }

        public Double MetrosCuadrados { get; set; }

        public String ImagenUrl { get; set; }

        public String Amenidad { get; set; }

        public DateTime FechaActualizacion { get; set; }


        public DateTime FechaCreacion { get; set; }


    }
}
