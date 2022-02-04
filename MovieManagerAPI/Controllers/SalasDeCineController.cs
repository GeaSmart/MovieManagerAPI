using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManagerAPI.DTO;
using MovieManagerAPI.Entidades;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class SalasDeCineController : CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly GeometryFactory geometryFactory;

        public SalasDeCineController(ApplicationDBContext context, IMapper mapper, GeometryFactory geometryFactory) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.geometryFactory = geometryFactory;
        }

        [HttpGet]
        public async Task<List<SalaDeCineDTO>> Get()
        {
            return await Get<SalaDeCine, SalaDeCineDTO>();
        }

        [HttpGet("{id:int}", Name = "obtenerSalaDeCine")]
        public async Task<ActionResult<SalaDeCineDTO>> Get(int id)
        {
            return await Get<SalaDeCine, SalaDeCineDTO>(id);
        }

        [HttpGet("Cercanos")]
        public async Task<ActionResult<List<SalaDeCineCercanoDTO>>> Cercanos([FromQuery] SalaDeCineCercanoFiltroDTO filtro)
        {
            var puntoReferencia = geometryFactory.CreatePoint(new Coordinate(filtro.Longitud, filtro.Latitud));
            var salasDeCine = await context.SalasDeCine
                .OrderBy(x => x.Ubicacion.Distance(puntoReferencia))
                .Where(x => x.Ubicacion.IsWithinDistance(puntoReferencia, filtro.DistanciaEnKms * 1000))
                .Select(x => new SalaDeCineCercanoDTO
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Longitud = x.Ubicacion.X,
                    Latitud = x.Ubicacion.Y,
                    DistanciaEnMetros = Math.Round(x.Ubicacion.Distance(puntoReferencia))
                }).ToListAsync();
            return salasDeCine;
        }
            

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalaDeCineCreacionDTO salaDeCineCreacionDTO)
        {
            return await Post<SalaDeCineCreacionDTO, SalaDeCine, SalaDeCineDTO>(salaDeCineCreacionDTO, "obtenerSalaDeCine");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] SalaDeCineCreacionDTO salaDeCineCreacionDTO)
        {
            return await Put<SalaDeCineCreacionDTO, SalaDeCine>(id, salaDeCineCreacionDTO);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<SalaDeCine>(id);
        }
    }
}
