using MagicVilla_API.Datos;
using MagicVilla_API.Models;
using MagicVilla_API.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_API.Controllers
{

    
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        // Creamos variable para la inyeccion de dependencias
        private readonly ILogger<VillaController> _logger;
            

        
        // Constructor
        public VillaController(ILogger<VillaController> logger ) => this._logger = logger;
        
        


        [HttpGet]
        // Creacion de endpoint para obtener todas las villas
        // Retornamos los datos genericos de la Lista villaList
        public ActionResult<IEnumerable<VillaDto>> GetVillaTodas()
        {
            _logger.LogInformation("Se obtuvieron todas las villas");
            return Ok(VillaStore.villaList);
        }
           

        // Se puede indicar el tipo de dato
        [HttpGet("id:int",Name = "GetVilla")]
        // Documentamos los Endpoints.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        // Endpoint para obtener solo una villa

        public ActionResult<VillaDto> GetVilla(int id)
        {
            // Zona de validaciones.
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToString());
            }

           

            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);


            if (id == 0)
            {
                _logger.LogError($"ERROR AL TRAER LA VILLA CON EL ID : {id}");
                return BadRequest();
            }
            if(villa == null)
            {
                return NotFound();
            }

           return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villaDto)
        {

            if (VillaStore.villaList.FirstOrDefault(v => v.Nombre.ToLower() == villaDto.Nombre.ToLower())!=null)
            {
                ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe!");
                return BadRequest(ModelState);
            }

            if (villaDto == null) {  return BadRequest(villaDto); }
            if(villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDto);

            return CreatedAtRoute("GetVilla",new {id = villaDto.Id},villaDto);

        }


        // Eliminacion de elementos
        [HttpDelete("{id:int}")]
        // Documentamos el endpoint
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult DeleteVilla(int id)
        {
           // Si el id es menor a 1 
            if(id == 0)
            {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            
            // En caso de que no se encuentre el registro solicitado.
            if(villa == null)
            {
                return NotFound();
            }

            VillaStore.villaList.Remove(villa);

            return NoContent();   
        }




        // PUT / ACTUALIZAR
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        
        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto)
        { 
            if (villaDto == null || id != villaDto.Id)
            {
                return BadRequest(); 
            }

            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            villa.Nombre = villaDto.Nombre;
            villa.Ocupantes = villaDto.Ocupantes;
            villa.MetrosCuadrados = villaDto.MetrosCuadrados;

            return NoContent();
        }


        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id == 0 )
            {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            patchDto.ApplyTo(villa, ModelState);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();

            return NoContent();
        }
    }
}
