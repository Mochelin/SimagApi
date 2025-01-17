using LandingData;
using LandingEntidades; // Contiene DTOs y entidades
using Microsoft.AspNetCore.Mvc;

namespace LandingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProyectosTController : ControllerBase
    {
        private readonly ProyectoTData _proyectoTData;

        public ProyectosTController(ProyectoTData proyectoTData)
        {
            _proyectoTData = proyectoTData;
        }

        /// <summary>
        /// Crea un nuevo proyecto Terreno, con imágenes opcionales en formato JSON.
        /// </summary>
        /// <param name="dto">DTO que contiene los datos del proyecto y la lista de imágenes.</param>
        /// <returns>Retorna el mensaje y el Id del proyecto creado.</returns>
        [HttpPost("crear")]
        public async Task<IActionResult> CrearProyecto([FromBody] ProyectoTDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (mensaje, idProyecto) = await _proyectoTData.CrearProyecto(dto);
            if (idProyecto.HasValue)
            {
                return Ok(new { Mensaje = mensaje, IdProyectoTerreno = idProyecto.Value });
            }
            else
            {
                return BadRequest(new { Mensaje = mensaje });
            }
        }

        /// <summary>
        /// Modifica un proyecto Terreno existente.
        /// </summary>
        /// <param name="id">Id del proyecto a modificar.</param>
        /// <param name="dto">Nuevos datos del proyecto.</param>
        /// <returns>Retorna un mensaje de éxito o error.</returns>
        [HttpPut("modificar/{id}")]
        public async Task<IActionResult> ModificarProyecto(int id, [FromBody] ProyectoTDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var mensaje = await _proyectoTData.ModificarProyecto(id, dto);
            if (mensaje == "El proyecto fue modificado correctamente.")
            {
                return Ok(new { Mensaje = mensaje });
            }
            else
            {
                return BadRequest(new { Mensaje = mensaje });
            }
        }

        /// <summary>
        /// Elimina un proyecto Terreno por su ID.
        /// </summary>
        /// <param name="id">ID del proyecto a eliminar.</param>
        /// <returns>Retorna un mensaje de éxito o error.</returns>
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarProyecto(int id)
        {
            var mensaje = await _proyectoTData.EliminarProyecto(id);
            if (mensaje == "El proyecto fue eliminado correctamente.")
            {
                return Ok(new { Mensaje = mensaje });
            }
            else
            {
                return BadRequest(new { Mensaje = mensaje });
            }
        }

        /// <summary>
        /// Lista proyectos Terreno de manera resumida, paginados.
        /// </summary>
        /// <param name="pageNumber">Número de página.</param>
        /// <param name="pageSize">Tamaño de página.</param>
        /// <returns>Retorna la lista paginada de proyectos en modo resumen.</returns>
        [HttpGet("listar-resumidos")]
        public async Task<IActionResult> ListarProyectosResumidos(int pageNumber = 1, int pageSize = 3)
        {
            var lista = await _proyectoTData.ListarProyectosResumidos(pageNumber, pageSize);

            // Log temporal para inspeccionar el resultado
            Console.WriteLine($"Resultados: {System.Text.Json.JsonSerializer.Serialize(lista)}");

            return Ok(lista);
        }


        /// <summary>
        /// Obtiene el detalle completo de un proyecto Terreno (incluye las imágenes).
        /// </summary>
        /// <param name="id">ID del proyecto a obtener.</param>
        /// <returns>Retorna el proyecto en detalle o un error si no existe.</returns>
        [HttpGet("detalle/{id}")]
        public async Task<IActionResult> ObtenerProyectoDetalle(int id)
        {
            var proyecto = await _proyectoTData.ObtenerProyectoDetalle(id);
            if (proyecto == null)
                return NotFound(new { Mensaje = "Proyecto no encontrado o error en la base de datos." });

            return Ok(proyecto);
        }

        /// <summary>
        /// Retorna el conteo total de proyectos Terreno en la base de datos.
        /// </summary>
        /// <returns>Objeto con la cantidad de proyectos y un mensaje de error si aplica.</returns>
        [HttpGet("contar")]
        public async Task<IActionResult> ContarProyectos()
        {
            var resultado = await _proyectoTData.ContarProyectos();
            return Ok(resultado);
        }
    }
}
