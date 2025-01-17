using LandingEntidades;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace LandingData
{
    public class SolicitanteData
    {
        private readonly ConnectionStrings con;


        public SolicitanteData(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        // Listar Solicitantes
        public async Task<List<Solicitante>> ListarSolicitantes()
        {
            var lista = new List<Solicitante>();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_ListaSolicitantes", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Solicitante
                    {
                        IdSolicitante = Convert.ToInt32(dr["IdSolicitante"]),
                        RUT = dr["RUT"].ToString()!,
                        Nombre = dr["Nombre"].ToString()!,
                        Apellido = dr["Apellido"].ToString()!,
                        Correo = dr["Correo"].ToString()!,
                        Telefono = dr["Telefono"].ToString()!,
                  
                        FechaNacimiento = Convert.ToDateTime(dr["FechaNacimiento"])
                    });
                }
            }

            return lista;
        }

        // Crear Solicitante
        public async Task<(string mensaje, int? idSolicitante)> CrearSolicitante(SolicitanteDTO objeto_solicitante)
        {
            string mensaje = "";
            int? idSolicitante = null;

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_CrearSolicitantes", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@RUT", objeto_solicitante.RUT);
                cmd.Parameters.AddWithValue("@Nombre", objeto_solicitante.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", objeto_solicitante.Apellido);
                cmd.Parameters.AddWithValue("@Correo", objeto_solicitante.Correo);
                cmd.Parameters.AddWithValue("@Telefono", objeto_solicitante.Telefono);
                cmd.Parameters.AddWithValue("@IdEntidad", objeto_solicitante.IdEntidad);
                cmd.Parameters.AddWithValue("@FechaNacimiento", objeto_solicitante.FechaNacimiento);

                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        idSolicitante = Convert.ToInt32(reader["IdSolicitante"]);
                    }
                }

                mensaje = paramMensaje.Value.ToString()!;
            }

            return (mensaje, idSolicitante);
        }

        // Modificar Solicitante
        public async Task<string> ModificarSolicitante(SolicitanteDTO objeto_solicitante)
        {
            string mensaje = "";

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_ModificarSolicitantes", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdSolicitante", objeto_solicitante.IdSolicitante);
                cmd.Parameters.AddWithValue("@RUT", objeto_solicitante.RUT);
                cmd.Parameters.AddWithValue("@Nombre", objeto_solicitante.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", objeto_solicitante.Apellido);
                cmd.Parameters.AddWithValue("@Correo", objeto_solicitante.Correo);
                cmd.Parameters.AddWithValue("@Telefono", objeto_solicitante.Telefono);
                cmd.Parameters.AddWithValue("@IdEntidad", objeto_solicitante.IdEntidad);
                cmd.Parameters.AddWithValue("@FechaNacimiento", objeto_solicitante.FechaNacimiento);

                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                await cmd.ExecuteNonQueryAsync();
                mensaje = paramMensaje.Value.ToString()!;
            }

            return mensaje;
        }

        // Eliminar Solicitante
        public async Task<string> EliminarSolicitante(int idSolicitante)
        {
            string mensaje = "";

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_EliminarSolicitantes", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdSolicitante", idSolicitante);

                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                await cmd.ExecuteNonQueryAsync();
                mensaje = paramMensaje.Value.ToString()!;
            }

            return mensaje;
        }

        // Obtener Solicitante
        public async Task<(string mensaje, Solicitante? solicitante)> ObtenerSolicitante(string RUT)
        {
            string mensaje = "";
            Solicitante? solicitante = null;

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                using var cmd = new SqlCommand("sp_ObtenerSolicitantes", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@RUT", RUT);

                var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramMensaje);

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        solicitante = new Solicitante
                        {
                            IdSolicitante = Convert.ToInt32(dr["IdSolicitante"]),
                            RUT = dr["RUT"].ToString()!,
                            Nombre = dr["Nombre"].ToString()!,
                            Apellido = dr["Apellido"].ToString()!,
                            Correo = dr["Correo"].ToString()!,
                            Telefono = dr["Telefono"].ToString()!,
                         
                            FechaNacimiento = Convert.ToDateTime(dr["FechaNacimiento"])
                        };
                    }
                }

                mensaje = paramMensaje.Value.ToString()!;
            }

            return (mensaje, solicitante);
        }
    }



}

