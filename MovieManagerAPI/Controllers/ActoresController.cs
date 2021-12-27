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
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public ActoresController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<List<ActorDTO>> Get()
        {
            var actores = await context.Actores.ToListAsync();
            return mapper.Map<List<ActorDTO>>(actores);
        }

        [HttpGet("{id:int}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var existe = await context.Actores.AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound("El actor solicitado no existe");

            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            return mapper.Map<ActorDTO>(actor);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = mapper.Map<Actor>(actorCreacionDTO);
            await context.Actores.AddAsync(actor);
            await context.SaveChangesAsync();

            var actorDto = mapper.Map<ActorDTO>(actor);//este dto es distinto del actorCreacionDTO ya que contiene también el id generado al crear y es necesario para devolver createdatrouteresult
            return new CreatedAtRouteResult("obtenerActor", new { id = actor.Id }, actorDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] ActorCreacionDTO actorCreacionDTO)
        {
            var existe = await context.Actores.AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound("El actor solicitado no existe");

            var actor = mapper.Map<Actor>(actorCreacionDTO);
            actor.Id = id;
            context.Actores.Update(actor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Actores.AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound("El actor solicitado no existe");

            context.Actores.Remove(new Actor { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
