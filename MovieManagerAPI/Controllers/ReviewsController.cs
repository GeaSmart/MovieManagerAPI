using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManagerAPI.DTO;
using MovieManagerAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieManagerAPI.Controllers
{
    [ApiController]
    [Route("api/peliculas/{peliculaId:int}/reviews")]
    public class ReviewsController:CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public ReviewsController(ApplicationDBContext context, IMapper mapper) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReviewDTO>>> Get(int peliculaId, [FromQuery] PaginacionDTO paginacionDTO)
        {
            //Aquí podríamos validar si la película existe, pero lo haremos mejor a través de los filtros

            var queryable = context.Reviews.Include(x => x.Usuario).Where(x => x.PeliculaId == peliculaId).AsQueryable();            
            return await Get<Review, ReviewDTO>(paginacionDTO, queryable);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int peliculaId, [FromBody] ReviewCreacionDTO reviewCreacionDTO)
        {
            //Aquí podríamos validar si la película existe, pero lo haremos mejor a través de los filtros

            //traemos de forma segura el UsuarioId
            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var reviewExiste = await context.Reviews.AnyAsync(x => x.PeliculaId == peliculaId && x.UsuarioId == usuarioId);
            if (reviewExiste)
                return BadRequest("El usuario ya dejó un review para esta pelicula");

            var review = mapper.Map<Review>(reviewCreacionDTO);
            review.PeliculaId = peliculaId;
            review.UsuarioId = usuarioId;

            context.Add(review);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(int reviewId, [FromBody] ReviewCreacionDTO reviewCreacionDTO)
        {
            //Aquí podríamos validar si la película existe, pero lo haremos mejor a través de los filtros

            var reviewDB = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (reviewDB == null)
                return NotFound();

            //traemos de forma segura el UsuarioId
            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (reviewDB.UsuarioId != usuarioId)
                return BadRequest("Su usuario actual no coincide con el usuario que envió el review");

            reviewDB = mapper.Map(reviewCreacionDTO, reviewDB);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete(int reviewId)
        {
            var reviewDB = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (reviewDB == null)
                return NotFound();

            //traemos de forma segura el UsuarioId
            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (reviewDB.UsuarioId != usuarioId)
                return BadRequest("Su usuario actual no coincide con el usuario que envió el review");

            context.Remove(reviewDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
