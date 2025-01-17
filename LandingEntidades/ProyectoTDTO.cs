namespace LandingEntidades
{
    public class ProyectoTDTO
    {
        // Datos generales
        public string? Titulo { get; set; }
        public string? SubTitulo { get; set; }
        public string? Cuerpo { get; set; }
        public string? TituloResumen { get; set; }
        public string? Resumen { get; set; }
        public string? LinkVideo { get; set; }
        public string? Descripcion { get; set; }
        public string? Area { get; set; }

        // Referencia al usuario creador
        public int IdNotificante { get; set; }

        // Lista de imágenes (URL o JSON)
        public List<ImagenProyectoICreateDto> Imagenes { get; set; } = new();
    }

    public class ImagenProyectoTCreateDto
    {
        public string? TipoImagen { get; set; }   // "Principal", "Secundaria", etc.
        public string? UrlImagen { get; set; }
    }



}
