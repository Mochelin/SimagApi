using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandingEntidades
{
    public class ImagenProyectoIDto
    {
        public int IdImagen { get; set; }
        public string? TipoImagen { get; set; }  // "Principal", "Plano", "Secundaria", etc.
        public string? UrlImagen { get; set; }
    }
}
