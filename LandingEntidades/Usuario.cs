using System.ComponentModel.DataAnnotations;

namespace LandingEntidades
{
    public class Usuario
    {
        
        public int IdUsuario { get; set; }
        public string? RUT { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }

        public string? Rol { get; set; }
        public DateTime FechaCreacion { get; set; }

      
        public byte[]? ImagenUsuario { get; set; }


        public string? ImagenBase64 { get; set; }
    }
}
