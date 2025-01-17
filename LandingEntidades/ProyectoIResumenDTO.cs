namespace LandingEntidades
{
    public class ProyectoIResumenDTO
    {
        public int IdProyectoInmobiliario { get; set; }        // Id del proyecto
        public string? TituloResumen { get; set; }
        public string? Resumen { get; set; }

        // Podemos usar el nombre "UrlImagenResumen" para la imagen principal o destacada.
        public string? UrlImagenResumen { get; set; }

        public DateTime FechaCreacion { get; set; }
        public string? UsuarioCreador { get; set; }

        // Si requieres en el resumen la info de habitaciones, baños y área (tal como
        // se incluyó en el SP), también puedes exponerlos:
        public string? Area { get; set; }
        public string? Habitaciones { get; set; }
        public string? Banios { get; set; }
    }
}
