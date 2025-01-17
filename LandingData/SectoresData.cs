using LandingData;
using LandingEntidades;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
public class SectoresData
{
    private readonly ConnectionStrings con;

    public SectoresData(IOptions<ConnectionStrings> options)
    {
        con = options.Value;
    }

    // Listar Sectores
    public async Task<List<Sector>> ListarSectores()
    {
        var lista = new List<Sector>();

        using (var conexion = new SqlConnection(con.CadenaSQL))
        {
            await conexion.OpenAsync();
            using var cmd = new SqlCommand("sp_ListarSectores", conexion);
            cmd.CommandType = CommandType.StoredProcedure;

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add(new Sector
                {
                    IdSector = Convert.ToInt32(dr["IdSector"]),
                    Nombre = dr["Nombre"].ToString()!,
                    Descripcion = dr["Descripcion"]?.ToString(),
                    Capacidad = Convert.ToInt32(dr["Capacidad"]),
                    Activo = Convert.ToBoolean(dr["Activo"])
                });
            }
        }

        return lista;
    }

    // Crear Sector
    public async Task<(string mensaje, int? idSector)> CrearSector(Sector sector)
    {
        string mensaje = "";
        int? idSector = null;

        using (var conexion = new SqlConnection(con.CadenaSQL))
        {
            await conexion.OpenAsync();
            using var cmd = new SqlCommand("sp_CrearSector", conexion);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nombre", sector.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", sector.Descripcion);
            cmd.Parameters.AddWithValue("@Capacidad", sector.Capacidad);
            cmd.Parameters.AddWithValue("@Activo", sector.Activo);

            var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(paramMensaje);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    idSector = Convert.ToInt32(reader["IdSector"]);
                }
            }

            mensaje = paramMensaje.Value.ToString()!;
        }

        return (mensaje, idSector);
    }

    // Modificar Sector
    public async Task<string> ModificarSector(Sector sector)
    {
        string mensaje = "";

        using (var conexion = new SqlConnection(con.CadenaSQL))
        {
            await conexion.OpenAsync();
            using var cmd = new SqlCommand("sp_ModificarSector", conexion);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdSector", sector.IdSector);
            cmd.Parameters.AddWithValue("@Nombre", sector.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", sector.Descripcion);
            cmd.Parameters.AddWithValue("@Capacidad", sector.Capacidad);
            cmd.Parameters.AddWithValue("@Activo", sector.Activo);

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

    // Eliminar Sector
    public async Task<string> EliminarSector(int idSector)
    {
        string mensaje = "";

        using (var conexion = new SqlConnection(con.CadenaSQL))
        {
            await conexion.OpenAsync();
            using var cmd = new SqlCommand("sp_EliminarSector", conexion);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdSector", idSector);

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

    // Obtener Sector
    public async Task<(string mensaje, Sector? sector)> ObtenerSector(int idSector)
    {
        string mensaje = "";
        Sector? sector = null;

        using (var conexion = new SqlConnection(con.CadenaSQL))
        {
            await conexion.OpenAsync();
            using var cmd = new SqlCommand("sp_ObtenerSector", conexion);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdSector", idSector);

            var paramMensaje = new SqlParameter("@msgError", SqlDbType.NVarChar, 200)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(paramMensaje);

            using (var dr = await cmd.ExecuteReaderAsync())
            {
                if (await dr.ReadAsync())
                {
                    sector = new Sector
                    {
                        IdSector = Convert.ToInt32(dr["IdSector"]),
                        Nombre = dr["Nombre"].ToString()!,
                        Descripcion = dr["Descripcion"]?.ToString(),
                        Capacidad = Convert.ToInt32(dr["Capacidad"]),
                        Activo = Convert.ToBoolean(dr["Activo"])
                    };
                }
            }

            mensaje = paramMensaje.Value.ToString()!;
        }

        return (mensaje, sector);
    }
}
