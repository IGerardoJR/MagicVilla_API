using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Models;
using MagicVilla_API.Models.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillaTodas()
        {
            _logger.LogInformation("Se obtuvieron todas las villas");
            //return Ok(VillaStore.villaList);

            //IEnumerable<Villa> villaList = await _context.Villas.ToListAsync();
            IEnumerable<Villa> villaList = await _villaRepo.ObtenerTodos();
            return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));
            //return Ok(await _context.Villas.ToListAsync());

        }


        // Se puede indicar el tipo de dato
        [HttpGet("id:int",Name = "GetVilla")]
        // Documentamos los Endpoints.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        // Endpoint para obtener solo una villa

        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            // Zona de validaciones.
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToString());
            }



            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            //var villa = await _context.Villas.FirstOrDefaultAsync(v => v.Id == id);
            var villa = await _villaRepo.Obtener(v => v.Id == id);


            if (id == 0)
            {
                _logger.LogError($"ERROR AL TRAER LA VILLA CON EL ID : {id}");
                return BadRequest();
            }
            if(villa == null)
            {
                return NotFound();
            }

           return Ok(_mapper.Map<VillaDto>(villa));
        }
//-------------------------------------------------------------------------------------------
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto createDto)
        {

            if (await _villaRepo.Obtener(v => v.Nombre.ToLower() == createDto.Nombre.ToLower())!=null)
            {
                ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe!");
                return BadRequest(ModelState);
            }

            if (createDto == null) {  return BadRequest(createDto); }

            //                                Recibe los datos de:
            Villa modelo = _mapper.Map<Villa>(createDto);
          

            //villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
            //VillaStore.villaList.Add(villaDto);


            // Creando modelo
            // Teniendo el automapper podremos comentar el modelo.
            //Villa modelo = new()
            //{
            //    Nombre =  createDto.Nombre,
            //    Detalle = createDto.Detalle,
            //    ImagenUrl = createDto.ImagenUrl,
            //    Amenidad = createDto.Amenidad,
            //    Ocupantes = createDto.Ocupantes,
            //    Tarifa = createDto.Tarifa,
            //    MetrosCuadrados =  createDto.MetrosCuadrados
            //};

            // Insercion de datos en base al modelo
            await _villaRepo.Crear(modelo);
            // Guardamos los cambios en la DB
            //await _context.SaveChangesAsync(); // Ya viene incluido en la metodo Crear

            return CreatedAtRoute("GetVilla",new {id = modelo.Id},createDto);

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
           // Si el id es menor a 1 
            if(id == 0)
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            var villa = await _villaRepo.Obtener(v => v.Id == id);
            
            // En caso de que no se encuentre el registro solicitado.
            if(villa == null)
            {
                return NotFound();
            }

            //VillaStore.villaList.Remove(villa);
            // Eliminamos la villa
            //_context.Villas.Remove(villa);
            await _villaRepo.Remover(villa);
            // Guardamos los cambios
            //await _context.SaveChangesAsync();

            return NoContent();   
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
                return BadRequest(); 
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;
            var modelo = _mapper.Map<Villa>(updateDto);


            // Con el automapper podremos commentar las siguientes lineas del modelo.
            //Villa modelo = new()
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    Tarifa = villaDto.Tarifa,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Amenidad = villaDto.Amenidad
            //};

            //_context.Villas.Update(modelo);
            await _villaRepo.Actualizar(modelo);
            //await _context.SaveChangesAsync();
            return NoContent();
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

            //VillaUpdateDto villaDto = new()
            //{
            //    Id = villa.Id,
            //    Nombre = villa.Nombre,
            //    Detalle = villa.Detalle,
            //    ImagenUrl = villa.ImagenUrl,
            //    Ocupantes = villa.Ocupantes,
            //    Tarifa = villa.Tarifa,
            //    MetrosCuadrados = villa.MetrosCuadrados,
            //    Amenidad = villa.Amenidad

            //};




            ////patchDto.ApplyTo(villa, ModelState);
            if (villa == null) { return BadRequest(); }

            patchDto.ApplyTo(villaDto, ModelState);


            Villa modelo = _mapper.Map<Villa>(villaDto);


            //Villa modelo = new()
            //{
            //    Id = villaDto.Id, 
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    Tarifa = villaDto.Tarifa,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Amenidad = villaDto.Amenidad
            //};


            //_context.Villas.Update(modelo);

           await _villaRepo.Actualizar(modelo);
           //await _context.SaveChangesAsync();


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();

          
        }
    }
}
