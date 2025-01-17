namespace LandingEntidades
{
    public class ProyectoHabitacional
    {
        public int IdNoticia { get; set; }
        public byte[]? ImagenPrincipal { get; set; } // Imagen principal
        public required string Titulo { get; set; } // Título de la noticia
        public string? SubTitulo { get; set; } // Subtítulo opcional
        public string? Cuerpo { get; set; } // Contenido de la noticia
        public byte[]? ImagenResumen { get; set; } // Imagen del resumen
        public string? TituloResumen { get; set; } // Título del resumen
        public string? Resumen { get; set; } // Descripción del resumen
        public DateTime FechaCreacion { get; set; } // Fecha de creación
        public Usuario? UsuarioCreador { get; set; } // Relación con la tabla Usuarios
    }

}
