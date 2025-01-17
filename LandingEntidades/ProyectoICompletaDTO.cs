﻿namespace LandingEntidades
{
    public class ProyectoICompletaDTO
    {
        public int IdProyectoInmobiliario { get; set; }

        public string? Titulo { get; set; }
        public string? SubTitulo { get; set; }
        public string? Cuerpo { get; set; }
        public string? LinkVideo { get; set; }
        public string? Descripcion { get; set; }
        public string? Habitaciones { get; set; }
        public string? Banios { get; set; }
        public string? Area { get; set; }

        public DateTime FechaCreacion { get; set; }
        public string? UsuarioCreador { get; set; }

        // Lista completa de imágenes relacionadas con este proyecto
        public List<ImagenProyectoIDto> Imagenes { get; set; } = new();
    }

}
