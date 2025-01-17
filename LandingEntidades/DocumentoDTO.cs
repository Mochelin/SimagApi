namespace LandingEntidades
{
    public class DocumentoDTO
    {
        public int IdDocumento { get; set; }
        public int IdUsuario { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Titulo { get; set; }
        public  string? Descripcion { get; set; }
        public string? DocumentoBase64 { get; set; }
        public byte[]? Documento { get; set; }
        public string? TipoDocumento { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
