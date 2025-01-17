namespace LandingEntidades
{
    public class SolicitanteDTO

    {
        public int? IdSolicitante { get; set; }
        public required string RUT { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public int? IdEntidad { get; set; } // Solo se pasa el ID de la entidad
        public DateTime FechaNacimiento { get; set; }
    }

}

