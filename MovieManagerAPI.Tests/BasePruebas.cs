using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieManagerAPI.Helpers;
using NetTopologySuite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieManagerAPI.Tests
{
    public class BasePruebas
    {
        protected string usuarioPorDefectoId = "940533e0-e9fe-442f-9184-8a27d7b2911b";
        protected string usuarioPorDefectoEmail = "ejemplo@ejemplo.com";

        protected ApplicationDBContext ConstruirContext(string nombreDb)
        {
            var opciones = new DbContextOptionsBuilder<ApplicationDBContext>().UseInMemoryDatabase(nombreDb).Options;
            var dbContext = new ApplicationDBContext(opciones);
            return dbContext;
        }

        protected IMapper ConfigurarAutomapper()
        {
            var config = new MapperConfiguration(options => {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                options.AddProfile(new AutoMapperProfiles(geometryFactory));
            });

            return config.CreateMapper();
        }

        protected ControllerContext ConstruirControllerContext()
        {
            var usuario = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, usuarioPorDefectoEmail),
                new Claim(ClaimTypes.Email, usuarioPorDefectoEmail),
                new Claim(ClaimTypes.NameIdentifier,usuarioPorDefectoId)
            }));

            return new ControllerContext() { HttpContext = new DefaultHttpContext() { User = usuario } };
        }

        //método auxiliar para construir peticiones http
        protected WebApplicationFactory<Startup> ConstruirWebApplicationFactory(string nombreBD, bool ignorarSeguridad = true)
        {
            var factory = new WebApplicationFactory<Startup>();
            factory = factory.WithWebHostBuilder(builder => {
                builder.ConfigureTestServices(services => {
                    var descriptorDBContext = services.SingleOrDefault( d => d.ServiceType == typeof(DbContextOptions<ApplicationDBContext>));

                    if (descriptorDBContext != null)
                        services.Remove(descriptorDBContext);

                    services.AddDbContext<ApplicationDBContext>(options =>
                        options.UseInMemoryDatabase(nombreBD)
                    );

                    if (ignorarSeguridad) //configuraremos un servicio y un filtro que nos ayudará a saltarnos la seguridad de asp.net core
                    {
                        services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();

                        services.AddControllers(options =>
                        {
                            options.Filters.Add(new UsuarioFalsoFiltro());
                        });
                    }
                });
            });
            return factory;
        }
    }
}
