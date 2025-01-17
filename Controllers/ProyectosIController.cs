using LandingData;
using LandingEntidades; // Contiene DTOs y entidades
using Microsoft.AspNetCore.Mvc;

namespace LandingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProyectosIController : ControllerBase
    {
        private readonly ProyectoIData _proyectoIData;

        public ProyectosIController(ProyectoIData proyectoIData)
        {
            _proyectoIData = proyectoIData;
        }

        /// <summary>
        /// Crea un nuevo proyecto inmobiliario, con imágenes opcionales en formato JSON.
        /// </summary>
        /// <param name="dto">DTO que contiene los datos del proyecto y la lista de imágenes.</param>
        /// <returns>Retorna el mensaje y el Id del proyecto creado.</returns>
        [HttpPost("crear")]
        public async Task<IActionResult> CrearProyecto([FromBody] ProyectoIDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (mensaje, idProyecto) = await _proyectoIData.CrearProyecto(dto);
            if (idProyecto.HasValue)
            {
                return Ok(new { Mensaje = mensaje, IdProyecto = idProyecto.Value });
            }
            else
            {
                return BadRequest(new { Mensaje = mensaje });
            }
        }

        /// <summary>
        /// Modifica un proyecto inmobiliario existente.
        /// </summary>
        /// <param name="id">Id del proyecto a modificar.</param>
        /// <param name="dto">Nuevos datos del proyecto.</param>
        /// <returns>Retorna un mensaje de éxito o error.</returns>
        [HttpPut("modificar/{id}")]
        public async Task<IActionResult> ModificarProyecto(int id, [FromBody] ProyectoIDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var mensaje = await _proyectoIData.ModificarProyecto(id, dto);
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
        /// Elimina un proyecto inmobiliario por su ID.
        /// </summary>
        /// <param name="id">ID del proyecto a eliminar.</param>
        /// <returns>Retorna un mensaje de éxito o error.</returns>
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarProyecto(int id)
        {
            var mensaje = await _proyectoIData.EliminarProyecto(id);
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
        /// Lista proyectos inmobiliarios de manera resumida, paginados.
        /// </summary>
        /// <param name="pageNumber">Número de página.</param>
        /// <param name="pageSize">Tamaño de página.</param>
        /// <returns>Retorna la lista paginada de proyectos en modo resumen.</returns>
        [HttpGet("listar-resumidos")]
        public async Task<IActionResult> ListarProyectosResumidos(int pageNumber = 1, int pageSize = 3)
        {
            var lista = await _proyectoIData.ListarProyectosResumidos(pageNumber, pageSize);
            return Ok(lista);
        }

        /// <summary>
        /// Obtiene el detalle completo de un proyecto inmobiliario (incluye las imágenes).
        /// </summary>
        /// <param name="id">ID del proyecto a obtener.</param>
        /// <returns>Retorna el proyecto en detalle o un error si no existe.</returns>
        ///

        [HttpGet("detalle/{id}")]
        public async Task<IActionResult> ObtenerProyectoDetalle(int id)
        {
            var proyecto = await _proyectoIData.ObtenerProyectoDetalle(id);
            if (proyecto == null)
                return NotFound(new { Mensaje = "Proyecto no encontrado o error en la base de datos." });

            return Ok(proyecto);
        }

        /// <summary>
        /// 
        /// Retorna el conteo total de proyectos inmobiliarios en la base de datos.
        /// </summary>
        /// <returns>Objeto con la cantidad de proyectos y un mensaje de error si aplica.</returns>
        /// 

        [HttpGet("contar")]
        public async Task<IActionResult> ContarProyectos()
        {
            var resultado = await _proyectoIData.ContarProyectos();
            return Ok(resultado);
        }
    }
}
