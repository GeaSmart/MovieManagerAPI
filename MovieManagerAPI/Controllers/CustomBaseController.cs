using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManagerAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Controllers
{
    public class CustomBaseController: ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public CustomBaseController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class
        {
            var entidades = await context.Set<TEntidad>().AsNoTracking().ToListAsync();//AsNoTracking hace que nuestros querys sean más rápidos
            return mapper.Map<List<TDTO>>(entidades);
        }

        protected async Task<ActionResult<TDTO>> Get<TEntidad, TDTO>(int id) where TEntidad : class, IId
        {
            var entidad = await context.Set<TEntidad>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entidad == null)
                return NotFound("No se encontró la información solicitada");

            return mapper.Map<TDTO>(entidad);
        }

        protected async Task<ActionResult> Post<TCreacion, TEntidad, TLectura>([FromBody] TCreacion creacionDTO, string nombreRuta) where TEntidad : class, IId
        {
            var entidad = mapper.Map<TEntidad>(creacionDTO);
            await context.AddAsync(entidad);
            await context.SaveChangesAsync();

            var dtoLectura = mapper.Map<TLectura>(entidad);
            return new CreatedAtRouteResult(nombreRuta, new { id = entidad.Id }, dtoLectura);
        }

        protected async Task<ActionResult> Put<TCreacion, TEntidad>(int id, [FromBody] TCreacion creacionDTO) where TEntidad:class,IId
        {
            var existe = await context.Set<TEntidad>().AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound();

            var entidad = mapper.Map<TEntidad>(creacionDTO);
            entidad.Id = id;
            context.Set<TEntidad>().Update(entidad);
            await context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult> Delete<TEntidad>(int id) where TEntidad:class, IId , new()
        {
            var existe = await context.Set<TEntidad>().AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound();

            context.Set<TEntidad>().Remove(new TEntidad { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
