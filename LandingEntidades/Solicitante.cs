using System.ComponentModel.DataAnnotations;

namespace LandingEntidades
{
    public class Solicitante
    {
        [Key]
        public int IdSolicitante { get; set; }
        public required string RUT { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }

        public DateTime? FechaNacimiento { get; set; }

    }
}
