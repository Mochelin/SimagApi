
using LandingData;
using LandingEntidades;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace LandingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoController : ControllerBase
    {
        public readonly DocumentoData _documentosData;

        public DocumentoController(DocumentoData documentoData)
        {
            _documentosData = documentoData;
        }


        [HttpPost("crear")]
        public async Task<IActionResult> CrearDocumento([FromBody] DocumentoDTO objetoDocumento)
        {
            // Validaciones iniciales
            if (objetoDocumento == null || objetoDocumento.IdUsuario <= 0)
            {
                return BadRequest("Los datos del documento no son válidos.");
            }

            // Mostrar en consola la info recibida, si lo deseas (debug).
            Console.WriteLine(JsonConvert.SerializeObject(objetoDocumento));

            // Si viene el documento en Base64, procesarlo
            if (!string.IsNullOrEmpty(objetoDocumento.DocumentoBase64))
            {
                try
                {
                    // Extraer el encabezado y el contenido del Base64
                    var base64Parts = objetoDocumento.DocumentoBase64.Split(',');
                    if (base64Parts.Length == 2)
                    {
                        // Obtener el tipo de documento desde el encabezado
                        string encabezado = base64Parts[0]; // Ejemplo: "data:application/pdf;base64"
                        string tipoDocumento = encabezado.Split(':')[1].Split(';')[0]; // Ejemplo: "application/pdf"
                        objetoDocumento.TipoDocumento = tipoDocumento;

                        // Convertir el contenido Base64 a byte[]
                        objetoDocumento.Documento = Convert.FromBase64String(base64Parts[1]);
                    }
                    else
                    {
                        return BadRequest("El formato del DocumentoBase64 no es válido.");
                    }
                }
                catch (FormatException)
                {
                    return BadRequest("El formato de DocumentoBase64 no es válido.");
                }
            }

            // Validar que el tipo de documento y el contenido sean válidos
            if (objetoDocumento.Documento == null || string.IsNullOrEmpty(objetoDocumento.TipoDocumento))
            {
                return BadRequest("El documento no tiene un contenido o tipo válido.");
            }

            // Llamar a la capa de datos/servicio para crear el documento
            var (mensaje, idDocumento) = await _documentosData.CrearDocumento(objetoDocumento);

            // Si el mensaje indica error
            if (mensaje.Contains("Error", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(500, new { mensaje });
            }

            // Si fue exitoso, retornar OK con el mensaje y el IdDocumento
            return Ok(new { mensaje, idDocumento });
        }


        // 2) ELIMINAR DOCUMENTO
        [HttpDelete("eliminar/{idDocumento}")]
        public async Task<IActionResult> EliminarDocumento(int idDocumento)
        {
            if (idDocumento <= 0)
            {
                return BadRequest("IdDocumento inválido.");
            }

            var mensaje = await _documentosData.EliminarDocumento(idDocumento);
            if (mensaje.Contains("Error", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(500, new { mensaje });
            }

            return Ok(new { mensaje });
        }

        // 3) DESCARGAR DOCUMENTO
        [HttpGet("descargar/{idDocumento}")]
        public async Task<IActionResult> DescargarDocumento(int idDocumento)
        {
            if (idDocumento <= 0)
            {
                return BadRequest("IdDocumento inválido.");
            }

            var (mensaje, documento) = await _documentosData.DescargarDocumento(idDocumento);

            if (mensaje.Contains("Error", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(500, new { mensaje });
            }

            // Si no existe el documento en la BD
            if (documento == null)
            {
                return NotFound(new { mensaje = "Documento no encontrado." });
            }

            // Aquí decides cómo retornarlo.
            // Por ejemplo, puedes responder con el byte[] como base64:
            if (documento.Documento != null && documento.Documento.Length > 0)
            {
                // Convertimos a base64
                documento.DocumentoBase64 = "data:" + documento.TipoDocumento + ";base64,"
                                            + Convert.ToBase64String(documento.Documento);
            }

            return Ok(new
            {
                mensaje,
                documento // Incluye el contenido, tipo, etc.
            });
        }

        // 4) LISTAR DOCUMENTOS
        [HttpGet("listar")]
        public async Task<IActionResult> ListarDocumentos()
        {
            var (mensaje, documentos) = await _documentosData.ListarDocumentos();

            if (mensaje.Contains("Error", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(500, new { mensaje });
            }

            // Si deseas, podrías convertir cada documento en base64,
            // pero usualmente no se hace por performance (solo al descargar).
            // Lo dejo simple:
            return Ok(new
            {
                mensaje,
                documentos
            });
        }




    }
}
