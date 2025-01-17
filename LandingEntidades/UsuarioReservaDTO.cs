namespace LandingEntidades
{
    public class UsuarioReservaDTO
    {
        public int? IdUsuario { get; set; } // ID del usuario que aprobó (puede ser null)
        public string? Nombre { get; set; } // Nombre completo del usuario que aprobó
        public DateTime? FechaAprobacion { get; set; } // Fecha en que se aprobó la reserva (puede ser null)
    }

}
