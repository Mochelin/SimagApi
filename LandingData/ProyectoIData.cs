using Dapper;
using LandingEntidades;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text.Json; // o Newtonsoft.Json

namespace LandingData
{
    public class ProyectoIData
    {
        private readonly ConnectionStrings _con;

        public ProyectoIData(IOptions<ConnectionStrings> options)
        {
            _con = options.Value;
        }

        /// <summary>
        /// Crea un nuevo proyecto inmobiliario, con múltiples imágenes en JSON.
        /// </summary>
        /// <param name="proyectoDto">DTO que contiene todos los datos y la lista de imágenes.</param>
        /// <returns>Mensaje y el ID creado.</returns>
        public async Task<(string Mensaje, int? IdProyecto)> CrearProyecto(ProyectoIDTO proyectoDto)
        {
            string mensaje = "";
            int? idProyecto = null;

            // 1. Convertir la lista de imágenes a un JSON si existen
            string jsonImagenes = "[]";
            if (proyectoDto.Imagenes != null && proyectoDto.Imagenes.Count > 0)
            {
                // Usar System.Text.Json o Newtonsoft.Json
                jsonImagenes = JsonSerializer.Serialize(proyectoDto.Imagenes);
            }

            using (var conexion = new SqlConnection(_con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_CrearProyectoI", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros principales
                cmd.Parameters.AddWithValue("@Titulo", proyectoDto.Titulo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SubTitulo", proyectoDto.SubTitulo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Cuerpo", proyectoDto.Cuerpo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TituloResumen", proyectoDto.TituloResumen ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Resumen", proyectoDto.Resumen ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LinkVideo", proyectoDto.LinkVideo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", proyectoDto.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Habitaciones", proyectoDto.Habitaciones ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Banios", proyectoDto.Banios ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Area", proyectoDto.Area ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IdNotificante", proyectoDto.IdNotificante);

                // Lista de imágenes en JSON
                cmd.Parameters.AddWithValue("@ImagenesJSON", jsonImagenes);

                // Parámetro de salida para el mensaje
                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                // Ejecutar el procedimiento y capturar el ID creado
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        // El sp_CrearProyectoI hace: SELECT SCOPE_IDENTITY() AS IdProyectoInmobiliario
                        idProyecto = Convert.ToInt32(reader["IdProyectoInmobiliario"]);
                    }
                }

                mensaje = paramMensaje.Value?.ToString() ?? "Error desconocido.";
            }

            return (mensaje, idProyecto);
        }

        /// <param name="idProyecto">ID del proyecto a modificar.</param>
        /// <param name="proyectoDto">DTO con la nueva información.</param>
        /// <returns>Mensaje de éxito o error.</returns>
        public async Task<string> ModificarProyecto(int idProyecto, ProyectoIDTO proyectoDto)
        {
            string mensaje = "";

            // Convertir la lista de imágenes a JSON
            string jsonImagenes = "[]";
            if (proyectoDto.Imagenes != null && proyectoDto.Imagenes.Count > 0)
            {
                jsonImagenes = JsonSerializer.Serialize(proyectoDto.Imagenes);
            }

            using (var conexion = new SqlConnection(_con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_ModificarProyectoI", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros
                cmd.Parameters.AddWithValue("@IdProyectoInmobiliario", idProyecto);
                cmd.Parameters.AddWithValue("@Titulo", proyectoDto.Titulo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SubTitulo", proyectoDto.SubTitulo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Cuerpo", proyectoDto.Cuerpo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TituloResumen", proyectoDto.TituloResumen ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Resumen", proyectoDto.Resumen ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LinkVideo", proyectoDto.LinkVideo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", proyectoDto.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Habitaciones", proyectoDto.Habitaciones ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Banios", proyectoDto.Banios ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Area", proyectoDto.Area ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IdNotificante", proyectoDto.IdNotificante);
                cmd.Parameters.AddWithValue("@ImagenesJSON", jsonImagenes);

                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                // Ejecutar el sp
                await cmd.ExecuteNonQueryAsync();
                mensaje = paramMensaje.Value?.ToString() ?? "Error desconocido.";
            }

            return mensaje;
        }

        public async Task<string> EliminarProyecto(int idProyecto)
        {
            string mensaje = "";

            using (var conexion = new SqlConnection(_con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_EliminarProyectoI", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProyectoInmobiliario", idProyecto);

                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                await cmd.ExecuteNonQueryAsync();
                mensaje = paramMensaje.Value?.ToString() ?? "Error desconocido.";
            }

            return mensaje;
        }

        public async Task<IEnumerable<ProyectoIResumenDTO>> ListarProyectosResumidos(int pageNumber, int pageSize)
        {
            using var conexion = new SqlConnection(_con.CadenaSQL);
            // Dapper simplifica el mapeo directamente al DTO
            var parametros = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var lista = await conexion.QueryAsync<ProyectoIResumenDTO>(
                "sp_ListarProyectoIResumidas",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return lista;
        }

        public async Task<ProyectoICompletaDTO?> ObtenerProyectoDetalle(int idProyecto)
        {
            ProyectoICompletaDTO? proyectoDetalle = null;
            using var conexion = new SqlConnection(_con.CadenaSQL);
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("sp_ObtenerProyectoIDetalle", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdProyectoInmobiliario", idProyecto);

            var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(paramMensaje);

            using (var dr = await cmd.ExecuteReaderAsync())
            {
                // Este SP devuelve varias filas si hay múltiples imágenes.
                while (await dr.ReadAsync())
                {
                    // Si es la primera fila, creamos el DTO principal
                    if (proyectoDetalle == null)
                    {
                        proyectoDetalle = new ProyectoICompletaDTO
                        {
                            IdProyectoInmobiliario = Convert.ToInt32(dr["IdProyectoInmobiliario"]),
                            Titulo = dr["Titulo"]?.ToString(),
                            SubTitulo = dr["SubTitulo"]?.ToString(),
                            Cuerpo = dr["Cuerpo"]?.ToString(),
                            LinkVideo = dr["LinkVideo"]?.ToString(),
                            Descripcion = dr["Descripcion"]?.ToString(),
                            Habitaciones = dr["Habitaciones"]?.ToString(),
                            Banios = dr["Banios"]?.ToString(),
                            Area = dr["Area"]?.ToString(),
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"]),
                            UsuarioCreador = dr["UsuarioCreador"]?.ToString(),
                            Imagenes = new List<ImagenProyectoIDto>()
                        };
                    }

                    // Leer datos de la imagen
                    if (dr["IdImagen"] != DBNull.Value)
                    {
                        var img = new ImagenProyectoIDto
                        {
                            IdImagen = Convert.ToInt32(dr["IdImagen"]),
                            TipoImagen = dr["TipoImagen"]?.ToString(),
                            UrlImagen = dr["UrlImagen"]?.ToString()
                        };
                        proyectoDetalle.Imagenes.Add(img);
                    }
                }
            }

            var mensaje = paramMensaje.Value?.ToString() ?? "Error desconocido.";
            if (mensaje != "Proyecto encontrado.")
            {
                // Podrías retornar null o lanzar excepción
                // throw new Exception(mensaje);
                return null;
            }

            return proyectoDetalle;
        }

        /// <summary>
        /// Retorna el conteo total de proyectos registrados.
        /// </summary>
        public async Task<ConteoDTO> ContarProyectos()
        {
            using var conexion = new SqlConnection(_con.CadenaSQL);
            var parametros = new DynamicParameters();
            parametros.Add("msgError", dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

            // Ejecutar sp_ContarProyectoI y mapear a ConteoDTO
            var conteo = await conexion.QueryFirstOrDefaultAsync<ConteoDTO>(
                "sp_ContarProyectoI",
                parametros,
                commandType: CommandType.StoredProcedure
            ) ?? new ConteoDTO();

            // Obtener msgError
            var mensaje = parametros.Get<string>("msgError");
            conteo.MensajeError = mensaje;

            return conteo;
        }
    }
}
