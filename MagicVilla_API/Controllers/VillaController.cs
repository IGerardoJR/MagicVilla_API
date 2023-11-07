using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Models;
using MagicVilla_API.Models.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Net;

namespace MagicVilla_API.Controllers
{

    
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        // Creamos variable para la inyeccion de dependencias
        private readonly ILogger<VillaController> _logger;
       /* private readonly ApplicationDbContext _context;*/ // Instancia para la 
        private readonly IMapper _mapper; // Variable para usar el automapper

        private readonly IVillaRepositorio _villaRepo;

        protected ApiResponse _response;



        // Constructor
        public VillaController(ILogger<VillaController> logger, IVillaRepositorio villaRepo, IMapper mapper)
        //{
        { 
        //    this._context = db;
            _villaRepo = villaRepo;
            this._logger = logger;
            this._mapper = mapper;
            this._response = new();
        }
        


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // Creacion de endpoint para obtener todas las villas
        // Retornamos los datos genericos de la Lista villaList
        public async Task<ActionResult<ApiResponse>> GetVillaTodas()
        {

            try
            {
                _logger.LogInformation("Se obtuvieron todas las villas");
                //return Ok(VillaStore.villaList);

                //IEnumerable<Villa> villaList = await _context.Villas.ToListAsync();
                IEnumerable<Villa> villaList = await _villaRepo.ObtenerTodos();

                _response.Resultado = _mapper.Map<IEnumerable<VillaDto>>(villaList);

                _response._statusCode = System.Net.HttpStatusCode.OK;
                return Ok(_response);

                //return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));
                //return Ok(await _context.Villas.ToListAsync());

            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                // Guardamos los errores en una lista
                _response.ErrorMessages = new List<String>() { ex.ToString()}; 
            }

            return _response;

            
        }


        // Se puede indicar el tipo de dato
        [HttpGet("id:int",Name = "GetVilla")]
        // Documentamos los Endpoints.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        // Endpoint para obtener solo una villa

        public async Task<ActionResult<ApiResponse>> GetVilla(int id)
        {

            try
            {
                // Zona de validaciones.
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.ToString());
                }



                //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                //var villa = await _context.Villas.FirstOrDefaultAsync(v => v.Id == id);
                var villa = await _villaRepo.Obtener(v => v.Id == id);


                if (id == 0)
                {
                    _logger.LogError($"ERROR AL TRAER LA VILLA CON EL ID : {id}");
                    _response._statusCode = System.Net.HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                if (villa == null)
                {
                    _response._statusCode = System.Net.HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Resultado = _mapper.Map<VillaDto>(villa);
                _response._statusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {

                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
           //return Ok(_mapper.Map<VillaDto>(villa));
        }
//-------------------------------------------------------------------------------------------
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> CrearVilla([FromBody] VillaCreateDto createDto)
        {

            try
            {
                if (await _villaRepo.Obtener(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe!");
                    _response._statusCode = HttpStatusCode.BadRequest;

                    return BadRequest(ModelState);
                }

                if (createDto == null) { return BadRequest(createDto); }

                //                                Recibe los datos de:
                Villa modelo = _mapper.Map<Villa>(createDto);




                // Insercion de datos en base al modelo
                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;


                await _villaRepo.Crear(modelo);
                _response.Resultado = modelo;
                _response._statusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<String>() { ex.ToString() };
            }

            return _response;

        }

//-------------------------------------------------------------------------------------
        // Eliminacion de elementos
        [HttpDelete("{id:int}")]
        // Documentamos el endpoint
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteVilla(int id)
        {

            try
            {
                // Si el id es menor a 1 
                if (id == 0)
                {
                    _response.IsExitoso = false;
                    _response._statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }


                var villa = await _villaRepo.Obtener(v => v.Id == id);

                // En caso de que no se encuentre el registro solicitado.
                if (villa == null)
                {
                    _response.IsExitoso = false;
                    _response._statusCode = HttpStatusCode.BadRequest;
                    return NotFound(_response);
                }

                await _villaRepo.Remover(villa);

                _response._statusCode = HttpStatusCode.NoContent;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
              
            }
            return Ok(_response);
        }

//-------------------------------------------------------------------------------------
        // PUT / ACTUALIZAR
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        { 
            if (updateDto == null || id != updateDto.Id)
            {
                _response._statusCode  = HttpStatusCode.BadRequest;
                _response.IsExitoso = false;
                return BadRequest(_response); 
            }

            var modelo = _mapper.Map<Villa>(updateDto);

            modelo.FechaActualizacion = DateTime.Now;
            await _villaRepo.Actualizar(modelo);
            _response._statusCode = HttpStatusCode.NoContent;
       
            return Ok(_response);
        }

//------------------------------------------------------------------------------------
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0 )
            {
                return BadRequest();
            }

            // AsNoTracking permite consultar un permiso sin tenerlo "abierto" todo el tiempo.
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            //var villa = await _context.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);


            var villa = await _villaRepo.Obtener(v => v.Id == id, tracked : false);
            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            
            if (villa == null) { return BadRequest(); }

            patchDto.ApplyTo(villaDto, ModelState);


            Villa modelo = _mapper.Map<Villa>(villaDto);

            await _villaRepo.Actualizar(modelo);
            _response._statusCode = HttpStatusCode.NoContent;
        


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_response);

          
        }
    }
}
