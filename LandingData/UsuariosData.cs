using LandingData.Utilidad;
using LandingEntidades;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace LandingData
{
    public class UsuariosData
    {
        private readonly ConnectionStrings con;
        public UsuariosData(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<(string mensaje, int? idUsuario)> Crear(Usuario objeto_usuario)
        {
            string mensaje = "";
            int? idUsuario = null;

            try
            {
                using var conexion = new SqlConnection(con.CadenaSQL);
                await conexion.OpenAsync();

                using var cmd = new SqlCommand("sp_CrearUsuarios", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // 1) Convertir Base64 a byte[], si viene algo
                byte[]? imagenBinaria = null;
                if (!string.IsNullOrEmpty(objeto_usuario.ImagenBase64))
                {
                    // Asumiendo que el string ya NO incluye el prefijo "data:image/jpeg;base64,"
                    // Si lo incluye, se debe removerlo antes:
                    // objeto_usuario.ImagenBase64 = objeto_usuario.ImagenBase64.Replace("data:image/jpeg;base64,", "");
                    imagenBinaria = Convert.FromBase64String(objeto_usuario.ImagenBase64);
                }

                // 2) Generar el hash de la contraseña
                var passwordHashBytes = PasswordHelper.CrearPasswordHash(objeto_usuario.PasswordHash);

                // 3) Asignar parámetros
                cmd.Parameters.AddWithValue("@RUT", objeto_usuario.RUT);
                cmd.Parameters.AddWithValue("@ImagenUsuario", (object?)imagenBinaria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Nombre", objeto_usuario.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", objeto_usuario.Apellido);
                cmd.Parameters.AddWithValue("@Email", objeto_usuario.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHashBytes);
                cmd.Parameters.AddWithValue("@Rol", objeto_usuario.Rol);

                // Parámetro de salida para el mensaje
                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                // Ejecutar el SP y leer el IdUsuario creado (SCOPE_IDENTITY)
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        idUsuario = Convert.ToInt32(reader["IdUsuario"]);
                    }
                }

                mensaje = paramMensaje.Value.ToString()!;
            }
            catch (Exception ex)
            {
                mensaje = $"Error: {ex.Message}";
            }

            return (mensaje, idUsuario);
        }



        public async Task<string> Modificar(Usuario objeto_usuario)
        {
            string mensaje = "";

            try
            {
                using var conexion = new SqlConnection(con.CadenaSQL);
                await conexion.OpenAsync();

                using var cmd = new SqlCommand("sp_ModificarUsuarios", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // Convertir Base64 a byte[]
                byte[]? imagenBinaria = null;
                if (!string.IsNullOrEmpty(objeto_usuario.ImagenBase64))
                {
                    imagenBinaria = Convert.FromBase64String(objeto_usuario.ImagenBase64);
                }

                // Generar el hash de la contraseña
                var passwordHashBytes = PasswordHelper.CrearPasswordHash(objeto_usuario.PasswordHash);

                // Parámetros de entrada
                cmd.Parameters.AddWithValue("@IdUsuario", objeto_usuario.IdUsuario);
                cmd.Parameters.AddWithValue("@RUT", objeto_usuario.RUT);
                cmd.Parameters.AddWithValue("@ImagenUsuario", (object?)imagenBinaria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Nombre", objeto_usuario.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", objeto_usuario.Apellido);
                cmd.Parameters.AddWithValue("@Email", objeto_usuario.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHashBytes);
                cmd.Parameters.AddWithValue("@Rol", objeto_usuario.Rol);

                // Parámetro de salida
                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                // Ejecutar el SP
                await cmd.ExecuteNonQueryAsync();

                mensaje = paramMensaje.Value.ToString()!;
            }
            catch (Exception ex)
            {
                mensaje = $"Error: {ex.Message}";
            }

            return mensaje;
        }


        public async Task<string> Eliminar(int idUsuario)
        {
            string mensaje = "";

            try
            {
                using var conexion = new SqlConnection(con.CadenaSQL);
                await conexion.OpenAsync();

                using var cmd = new SqlCommand("sp_EliminarUsuarios", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetro de entrada
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                // Parámetro de salida
                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                // Ejecutar el procedimiento
                await cmd.ExecuteNonQueryAsync();

                // Capturar el mensaje de salida
                mensaje = paramMensaje.Value.ToString()!;
            }
            catch (Exception ex)
            {
                mensaje = $"Error: {ex.Message}";
            }

            return mensaje;
        }






        public async Task<(string mensaje, Usuario? datosUsuario)> ObtenerUsuario(string email, string password)
        {
            string mensaje = "";
            Usuario? usuario = null;

            try
            {
                using var conexion = new SqlConnection(con.CadenaSQL);
                await conexion.OpenAsync();

                using var cmd = new SqlCommand("sp_ObtenerUsuarios", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // Generar el hash de la contraseña ingresada
                var passwordHashBytes = PasswordHelper.CrearPasswordHash(password);

                // Parámetros de entrada
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHashBytes);

                // Parámetros de salida
                var paramNombreCompleto = new SqlParameter("@NombreCompleto", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramNombreCompleto);

                var paramRol = new SqlParameter("@Rol", SqlDbType.NVarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramRol);

                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                var paramIdUsuario = new SqlParameter("@IdUsuario", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramIdUsuario);

                // Este parámetro es la imagen en Base64
                var paramImagenUsuario = new SqlParameter("@ImagenUsuario", SqlDbType.NVarChar, -1)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramImagenUsuario);

                // Ejecutar el procedimiento
                await cmd.ExecuteNonQueryAsync();

                // Capturar los mensajes y datos de salida
                mensaje = paramMensaje.Value?.ToString() ?? "Error desconocido.";
                if (mensaje == "Usuario encontrado.")
                {
                    var nombreCompleto = paramNombreCompleto.Value?.ToString() ?? "";
                    var rol = paramRol.Value?.ToString() ?? "";

                    // El sp_ObtenerUsuarios retorna la imagen en base64 (prefijo + datos)
                    var imagenBase64 = paramImagenUsuario.Value?.ToString();

                    // Reconstruimos la entidad
                    usuario = new Usuario
                    {
                        IdUsuario = (int)paramIdUsuario.Value,
                        Email = email,
                        Rol = rol
                    };

                    // Dividir el NombreCompleto
                    var partes = nombreCompleto.Split(' ', 2);
                    if (partes.Length >= 2)
                    {
                        usuario.Nombre = partes[0];
                        usuario.Apellido = partes[1];
                    }
                    else
                    {
                        usuario.Nombre = nombreCompleto;
                    }

                            // Asignar la imagen base64 tal cual la devolvió el SP.
                            // (por ej, 'data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAA...')
                    usuario.ImagenBase64 = imagenBase64;
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error: {ex.Message}";
            }

            return (mensaje, usuario);
        }





        public async Task<List<Usuario>> ListaUsuarios()
        {
            var lista = new List<Usuario>();

            try
            {
                using var conexion = new SqlConnection(con.CadenaSQL);
                await conexion.OpenAsync();

                using var cmd = new SqlCommand("sp_ListaUsuarios", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Usuario
                    {
                        IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                        RUT = dr["RUT"].ToString()!,
                        Nombre = dr["Nombre"].ToString()!,
                        Apellido = dr["Apellido"].ToString()!,
                        Email = dr["Email"].ToString()!,
                        FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"]),
                        Rol = dr["Rol"].ToString()!
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return lista;
        }

    }
}
