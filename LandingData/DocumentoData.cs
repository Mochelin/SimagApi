using LandingEntidades;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace LandingData
{
    public class DocumentoData
    {

        private readonly ConnectionStrings con;

        public DocumentoData(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }


        public async Task<(string mensaje, int? idDocumento)> CrearDocumento(DocumentoDTO objetoDocumento)
        {
            string mensaje = "";
            int? idDocumento = null;

            // Si viene el documento en Base64 y no en byte[], lo convertimos
            byte[] contenidoDocumento = objetoDocumento.Documento;
            string tipoDocumento = objetoDocumento.TipoDocumento;

            if (contenidoDocumento == null && !string.IsNullOrEmpty(objetoDocumento.DocumentoBase64))
            {
                // Extraemos el encabezado del Base64
                var base64Parts = objetoDocumento.DocumentoBase64.Split(',');
                if (base64Parts.Length == 2)
                {
                    // Obtenemos el tipo de documento desde el encabezado
                    string encabezado = base64Parts[0]; // Ejemplo: "data:application/pdf;base64"
                    var tipo = encabezado.Split(':')[1].Split(';')[0]; // Ejemplo: "application/pdf"
                    tipoDocumento = tipo;

                    // Convertimos el contenido Base64 en byte[]
                    contenidoDocumento = Convert.FromBase64String(base64Parts[1]);
                }
                else
                {
                    mensaje = "El formato del documento Base64 no es válido.";
                    return (mensaje, null);
                }
            }

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_CrearDocumento", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros
                cmd.Parameters.AddWithValue("@IdUsuario", objetoDocumento.IdUsuario);
                cmd.Parameters.AddWithValue("@Titulo", objetoDocumento.Titulo);
                cmd.Parameters.AddWithValue("@Descripcion", objetoDocumento.Descripcion);
                cmd.Parameters.AddWithValue("@Documento", objetoDocumento.Documento);
                cmd.Parameters.AddWithValue("@TipoDocumento", objetoDocumento.TipoDocumento);

                // Parámetro de salida para mensaje de error
                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                // Ejecutamos el SP y obtenemos el IdDocumento
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        idDocumento = Convert.ToInt32(reader["IdDocumento"]);
                    }
                }

                // Recuperamos el mensaje devuelto por el SP
                mensaje = paramMensaje.Value?.ToString() ?? "Error desconocido.";
            }

            return (mensaje, idDocumento);
        }



        public async Task<string> EliminarDocumento(int idDocumento)
        {
            string mensaje = "";

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_EliminarDocumento", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdDocumento", idDocumento);

                // Parámetro de salida para mensaje de error
                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                await cmd.ExecuteNonQueryAsync();

                // Recuperamos el mensaje devuelto por el SP
                mensaje = paramMensaje.Value?.ToString() ?? "Error desconocido.";
            }

            return mensaje;
        }



        // 2) DESCARGAR DOCUMENTO
        public async Task<(string mensaje, DocumentoDTO documento)> DescargarDocumento(int idDocumento)
        {
            string mensaje = "";
            DocumentoDTO documento = null;

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_DescargarDocumento", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdDocumento", idDocumento);

                // Parámetro de salida para mensaje de error
                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        documento = new DocumentoDTO
                        {
                            IdDocumento = Convert.ToInt32(reader["IdDocumento"]),
                            IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                            Documento = reader["Documento"] as byte[],
                            TipoDocumento = reader["TipoDocumento"]?.ToString()
                            // Agrega otros campos que necesites (Título, Descripción, etc.)
                        };
                    }
                }

                // Obtenemos el valor del parámetro de salida
                mensaje = paramMensaje.Value?.ToString() ?? "Error desconocido.";
            }

            return (mensaje, documento);
        }

        // 3) LISTAR DOCUMENTOS
        public async Task<(string mensaje, List<DocumentoDTO> documentos)> ListarDocumentos()
        {
            string mensaje = "";
            var documentos = new List<DocumentoDTO>();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_ListarDocumentosFormateados", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetro de salida para mensaje de error
                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var doc = new DocumentoDTO
                        {
                            IdDocumento = Convert.ToInt32(reader["IdDocumento"]),
                            NombreUsuario = reader["NombreUsuario"]?.ToString(), // Concatenación Nombre + Apellido
                            Titulo = reader["Titulo"]?.ToString(),
                            Descripcion = reader["Descripcion"]?.ToString(),
                            DocumentoBase64 = reader["DocumentoBase64"].ToString(),
                            FechaCreacion = reader["FechaCreacion"] != DBNull.Value
                            ? Convert.ToDateTime(reader["FechaCreacion"])
                            : DateTime.MinValue // O cualquier valor predeterminado que prefieras

                        };
                        documentos.Add(doc);
                    }
                }

                // Obtenemos el valor del parámetro de salida
                mensaje = paramMensaje.Value?.ToString() ?? "Error desconocido.";
            }

            return (mensaje, documentos);
        }


    }
}
