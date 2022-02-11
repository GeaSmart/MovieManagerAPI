using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieManagerAPI.Controllers;
using MovieManagerAPI.DTO;
using MovieManagerAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieManagerAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class ActoresControllerTests : BasePruebas
    {
        [TestMethod]
        public async Task ObtenerTodosLosAutoresPaginados()
        {
            //Preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutomapper();

            contexto.Actores.Add(new Actor { Nombre = "actor 1" });
            contexto.Actores.Add(new Actor { Nombre = "actor 2" });
            contexto.Actores.Add(new Actor { Nombre = "actor 3" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);

            //Prueba 1
            var controlador = new ActoresController(contexto2, mapper, null);

            //porque para usar la paginación se hace uso de InsertarParametrosPaginacion el cual necesita un httpcontext, si no agregamos esta línea tendremos un nullreference exception
            controlador.ControllerContext.HttpContext = new DefaultHttpContext();
            var paginacionDTO = new PaginacionDTO() { CantidadRegistrosPorPagina = 10, Pagina = 1 };
            var respuesta = await controlador.Get(paginacionDTO);

            //Verificación 1
            var actores = respuesta.Value;
            Assert.AreEqual(3, actores.Count);

            //prueba 2
            controlador.ControllerContext.HttpContext = new DefaultHttpContext();
            var paginacionDTO2 = new PaginacionDTO() { CantidadRegistrosPorPagina = 2, Pagina = 2 };
            var respuesta2 = await controlador.Get(paginacionDTO2);

            //Verificación 2
            var actores2 = respuesta2.Value;
            Assert.AreEqual(1, actores2.Count);

            //prueba 3
            controlador.ControllerContext.HttpContext = new DefaultHttpContext();
            var paginacionDTO3 = new PaginacionDTO() { CantidadRegistrosPorPagina = 2, Pagina = 3 };
            var respuesta3 = await controlador.Get(paginacionDTO3);

            //Verificación 3
            var actores3 = respuesta3.Value;
            Assert.AreEqual(0, actores3.Count);
        }

        
    }
}
