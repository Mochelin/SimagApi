using LandingEntidades;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class SectoresController : ControllerBase
{
    private readonly SectoresData _sectoresData;

    public SectoresController(SectoresData sectoresData)
    {
        _sectoresData = sectoresData;
    }

    // Obtener todos los sectores
    [HttpGet("lista")]
    public async Task<IActionResult> ListarSectores()
    {
        var sectores = await _sectoresData.ListarSectores();
        return Ok(sectores);
    }

    // Obtener un sector específico
    [HttpGet("obtener/{idSector}")]
    public async Task<IActionResult> ObtenerSector(int idSector)
    {
        var (mensaje, sector) = await _sectoresData.ObtenerSector(idSector);

        if (sector == null)
        {
            return NotFound(new { mensaje });
        }

        return Ok(new { mensaje, sector });
    }

    // Crear un nuevo sector
    [HttpPost("crear")]
    public async Task<IActionResult> CrearSector([FromBody] Sector objeto_sector)
    {
        if (objeto_sector == null || string.IsNullOrEmpty(objeto_sector.Nombre))
        {
            return BadRequest("El sector proporcionado no es válido.");
        }

        var (mensaje, idSector) = await _sectoresData.CrearSector(objeto_sector);

        if (mensaje.Contains("Error"))
        {
            return StatusCode(500, new { mensaje });
        }

        return Ok(new { mensaje, idSector });
    }

    // Modificar un sector existente
    [HttpPut("modificar")]
    public async Task<IActionResult> ModificarSector([FromBody] Sector objeto_sector)
    {
        if (objeto_sector == null || objeto_sector.IdSector <= 0)
        {
            return BadRequest("El sector proporcionado no es válido.");
        }

        var mensaje = await _sectoresData.ModificarSector(objeto_sector);

        if (mensaje.Contains("Error"))
        {
            return StatusCode(500, new { mensaje });
        }

        return Ok(new { mensaje });
    }

    // Eliminar un sector por ID
    [HttpDelete("eliminar/{idSector}")]
    public async Task<IActionResult> EliminarSector(int idSector)
    {
        if (idSector <= 0)
        {
            return BadRequest("El ID del sector no es válido.");
        }

        var mensaje = await _sectoresData.EliminarSector(idSector);

        if (mensaje.Contains("Error"))
        {
            return StatusCode(500, new { mensaje });
        }

        return Ok(new { mensaje });
    }
}
