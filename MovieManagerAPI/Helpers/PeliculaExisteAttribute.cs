using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Helpers
{
    public class PeliculaExisteAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDBContext dbcontext;

        public PeliculaExisteAttribute(ApplicationDBContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var peliculaIdObject = context.HttpContext.Request.RouteValues["peliculaId"];

            if (peliculaIdObject == null)
                return;

            var peliculaId = int.Parse(peliculaIdObject.ToString());

            var existePelicula = await dbcontext.Peliculas.AnyAsync(x => x.Id == peliculaId);
            if (!existePelicula)
            {
                context.Result = new NotFoundResult();//esto corta el pipeline de ejecución
            }
            else
            {
                await next();//permite que continúe
            }
        }
    }
}
