using System.ComponentModel.DataAnnotations;

namespace LandingEntidades
{
    public class Sector
    {
        [Key]
        public int IdSector { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int Capacidad { get; set; }
        public bool Activo { get; set; }

    }
}
