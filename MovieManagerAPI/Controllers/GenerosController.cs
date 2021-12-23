using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManagerAPI.DTO;
using MovieManagerAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class GenerosController:ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public GenerosController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<List<GeneroDTO>> Get()
        {
            var generos = await context.Generos.ToListAsync();
            return mapper.Map<List<GeneroDTO>>(generos);
        }

        [HttpGet("{id:int}", Name = "obtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);
            if (genero == null)
                return NotFound("No se encontró el género solicitado");

            return mapper.Map<GeneroDTO>(genero);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var genero = mapper.Map<Genero>(generoCreacionDTO);
            await context.Generos.AddAsync(genero);
            await context.SaveChangesAsync();

            var generoDTO = mapper.Map<GeneroDTO>(genero);
            return new CreatedAtRouteResult("obtenerGenero", new { id = generoDTO.Id }, generoDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var existe = await context.Generos.AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound("El genero indicado no existe");

            var genero = mapper.Map<Genero>(generoCreacionDTO);
            genero.Id = id;
            context.Generos.Update(genero);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Generos.AnyAsync(x=>x.Id == id);
            if (!existe)
                return NotFound("El genero indicado no existe");

            context.Generos.Remove(new Genero { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
