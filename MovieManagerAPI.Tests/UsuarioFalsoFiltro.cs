using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieManagerAPI.Tests
{
    public class UsuarioFalsoFiltro : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "ejemplo@ejemplo.com"),
                new Claim(ClaimTypes.Email, "ejemplo@ejemplo.com"),
                new Claim(ClaimTypes.NameIdentifier,"940533e0-e9fe-442f-9184-8a27d7b2911b")
            }));

            await next();
        }
    }
}
