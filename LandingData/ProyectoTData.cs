using Dapper;
using LandingEntidades;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text.Json; // o Newtonsoft.Json

namespace LandingData
{
    public class ProyectoTData
    {
        private readonly ConnectionStrings con;

        public ProyectoTData(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        /// <summary>
        /// Crea un nuevo proyecto inmobiliario, con múltiples imágenes en JSON.
        /// </summary>
        /// <param name="proyectoDto">DTO que contiene todos los datos y la lista de imágenes.</param>
        /// <returns>Mensaje y el ID creado.</returns>
        public async Task<(string Mensaje, int? IdProyecto)> CrearProyecto(ProyectoTDTO proyectoDto)
        {
            string mensaje = "";
            int? idProyecto = null;

            // 1. Convertir la lista de imágenes a un JSON si existen
            string jsonImagenes = "[]";
            if (proyectoDto.Imagenes != null && proyectoDto.Imagenes.Count > 0)
            {
                // Serializar la lista de imágenes a JSON
                jsonImagenes = JsonSerializer.Serialize(proyectoDto.Imagenes);
            }

            try
            {
                using (var conexion = new SqlConnection(con.CadenaSQL))
                {
                    await conexion.OpenAsync();

                    using (var cmd = new SqlCommand("sp_CrearProyectoT", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parámetros principales
                        cmd.Parameters.AddWithValue("@Titulo", proyectoDto.Titulo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SubTitulo", proyectoDto.SubTitulo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Cuerpo", proyectoDto.Cuerpo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TituloResumen", proyectoDto.TituloResumen ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Resumen", proyectoDto.Resumen ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@LinkVideo", proyectoDto.LinkVideo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Descripcion", proyectoDto.Descripcion ?? (object)DBNull.Value);
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
                                // Capturamos el ID del proyecto creado
                                idProyecto = reader["IdProyectoTerreno"] != DBNull.Value
                                    ? Convert.ToInt32(reader["IdProyectoTerreno"])
                                    : (int?)null;
                            }
                        }

                        // Obtener el mensaje de salida
                        mensaje = paramMensaje.Value?.ToString() ?? "Error desconocido.";
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Ocurrió un error: {ex.Message}";
                idProyecto = null; // No se pudo crear el proyecto
            }

            return (mensaje, idProyecto);
        }


        /// <param name="idProyecto">ID del proyecto a modificar.</param>
        /// <param name="proyectoDto">DTO con la nueva información.</param>
        /// <returns>Mensaje de éxito o error.</returns>
        public async Task<string> ModificarProyecto(int idProyecto, ProyectoTDTO proyectoDto)
        {
            string mensaje = "";

            // Convertir la lista de imágenes a JSON
            string jsonImagenes = "[]";
            if (proyectoDto.Imagenes != null && proyectoDto.Imagenes.Count > 0)
            {
                jsonImagenes = JsonSerializer.Serialize(proyectoDto.Imagenes);
            }

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_ModificarProyectoT", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros
                cmd.Parameters.AddWithValue("@IdProyectoTerreno", idProyecto);
                cmd.Parameters.AddWithValue("@Titulo", proyectoDto.Titulo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SubTitulo", proyectoDto.SubTitulo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Cuerpo", proyectoDto.Cuerpo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TituloResumen", proyectoDto.TituloResumen ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Resumen", proyectoDto.Resumen ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LinkVideo", proyectoDto.LinkVideo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", proyectoDto.Descripcion ?? (object)DBNull.Value);
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

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_EliminarProyectoT", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProyectoTerreno", idProyecto);

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

        public async Task<IEnumerable<ProyectoTResumenDTO>> ListarProyectosResumidos(int pageNumber, int pageSize)
        {
            using var conexion = new SqlConnection(con.CadenaSQL);
            var parametros = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var lista = await conexion.QueryAsync<ProyectoTResumenDTO>(
                "sp_ListarProyectoTResumidas",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            Console.WriteLine($"Resultados obtenidos: {lista.Count()} registros encontrados.");
            return lista;
        }


        public async Task<ProyectoTCompletaDTO?> ObtenerProyectoDetalle(int idProyecto)
        {
            ProyectoTCompletaDTO? proyectoDetalle = null;
            using var conexion = new SqlConnection(con.CadenaSQL);
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("sp_ObtenerProyectoTDetalle", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdProyectoTerreno", idProyecto);

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
                        proyectoDetalle = new ProyectoTCompletaDTO
                        {
                            IdProyectoInmobiliario = Convert.ToInt32(dr["IdProyectoTerreno"]),
                            Titulo = dr["Titulo"]?.ToString(),
                            SubTitulo = dr["SubTitulo"]?.ToString(),
                            Cuerpo = dr["Cuerpo"]?.ToString(),
                            LinkVideo = dr["LinkVideo"]?.ToString(),
                            Descripcion = dr["Descripcion"]?.ToString(),
                            Area = dr["Area"]?.ToString(),
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"]),
                            UsuarioCreador = dr["UsuarioCreador"]?.ToString(),
                            Imagenes = new List<ImagenProyectoTDto>()
                        };
                    }

                    // Leer datos de la imagen
                    if (dr["IdImagen"] != DBNull.Value)
                    {
                        var img = new ImagenProyectoTDto
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
            using var conexion = new SqlConnection(con.CadenaSQL);
            var parametros = new DynamicParameters();
            parametros.Add("msgError", dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

            // Ejecutar sp_ContarProyectoI y mapear a ConteoDTO
            var conteo = await conexion.QueryFirstOrDefaultAsync<ConteoDTO>(
                "sp_ContarProyectoT",
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
