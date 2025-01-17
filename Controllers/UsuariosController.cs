using LandingData;
using LandingEntidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly UsuariosData _usuariosData;
    private readonly IConfiguration _configuration;

    public UsuariosController(UsuariosData usuariosData, IConfiguration configuration)
    {
        _usuariosData = usuariosData;
        _configuration = configuration;
    }

    // Obtener todos los usuarios
    [HttpGet("lista")]
    public async Task<IActionResult> ObtenerListaUsuarios()
    {
        var usuarios = await _usuariosData.ListaUsuarios();
        return Ok(usuarios);
    }

    // Obtener un usuario específico
    [HttpGet("obtener")]
    public async Task<IActionResult> ObtenerUsuario(
     [FromQuery] string email,
     [FromQuery] string password)
    {
        // Obtener el usuario desde la base de datos
        var (mensaje, usuario) = await _usuariosData.ObtenerUsuario(email, password);

        if (usuario == null)
        {
            return NotFound(new { mensaje });
        }

        // Generar el token JWT si el usuario es válido
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // Clave desde appsettings.json

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, usuario.Email),
            new Claim("IdUsuario", usuario.IdUsuario.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1), // El token expira en 1 hora
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Retornar el token junto con el usuario y el mensaje
        return Ok(new
        {
            mensaje,
            usuario,
            token = tokenHandler.WriteToken(token)
        });
    }

    // Crear un nuevo usuario
    [HttpPost("crear")]
    public async Task<IActionResult> CrearUsuario([FromBody] Usuario objeto_usuario)
    {
        if (objeto_usuario == null || string.IsNullOrEmpty(objeto_usuario.Email))
        {
            return BadRequest("El usuario proporcionado no es válido.");
        }

        var resultado = await _usuariosData.Crear(objeto_usuario);

        if (resultado.mensaje.Contains("Error"))
        {
            return StatusCode(500, new { mensaje = resultado.mensaje });
        }

        return Ok(new { mensaje = resultado.mensaje, idUsuario = resultado.idUsuario });
    }

    // Modificar un usuario existente
    [HttpPut("modificar")]
    public async Task<IActionResult> ModificarUsuario([FromBody] Usuario objeto_usuario)
    {
        if (objeto_usuario == null || objeto_usuario.IdUsuario <= 0)
        {
            return BadRequest("El usuario proporcionado no es válido.");
        }

        var mensaje = await _usuariosData.Modificar(objeto_usuario);

        if (mensaje.Contains("Error"))
        {
            return StatusCode(500, new { mensaje });
        }

        return Ok(new { mensaje });
    }

    // Eliminar un usuario
    [HttpDelete("eliminar/{idUsuario}")]
    public async Task<IActionResult> EliminarUsuario(int idUsuario)
    {
        if (idUsuario <= 0)
        {
            return BadRequest("El ID del usuario no es válido.");
        }

        var mensaje = await _usuariosData.Eliminar(idUsuario);

        if (mensaje.Contains("Error"))
        {
            return StatusCode(500, new { mensaje });
        }

        return Ok(new { mensaje });
    }
}
