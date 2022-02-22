using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieManagerAPI.Controllers;
using MovieManagerAPI.DTO;
using MovieManagerAPI.Entidades;
using MovieManagerAPI.Servicios;
using System;
using System.Collections.Generic;
using System.IO;
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

        [TestMethod]
        public async Task CrearActorSinFoto()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutomapper();

            var actor = new ActorCreacionDTO { Nombre = "Gerson", FechaNacimiento = DateTime.Now };

            //para trabajar con el IAlmacenadorArchivos
            var mock = new Mock<IAlmacenadorArchivos>();
            mock.Setup(x => x.GuardarArchivo(null, null, null, null)).Returns(Task.FromResult("url"));

            var controller = new ActoresController(contexto, mapper, mock.Object);

            var respuesta = await controller.Post(actor);
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.AreEqual(201, resultado.StatusCode);

            var contexto2 = ConstruirContext(nombreBD);
            var listado = await contexto2.Actores.ToListAsync();
            Assert.AreEqual(1, listado.Count);
            Assert.IsNull(listado[0].Foto);
            Assert.AreEqual(0,mock.Invocations.Count);
        }

        [TestMethod]
        public async Task CrearActorConFoto()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutomapper();

            var content = Encoding.UTF8.GetBytes("Imagen de prueba");
            var archivo = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "imagen.jpg");
            archivo.Headers = new HeaderDictionary();
            archivo.ContentType = "image/jpg";

            var actor = new ActorCreacionDTO()
            {
                Nombre = "nuevo actor",
                FechaNacimiento = DateTime.Now,
                Foto = archivo
            };

            //para trabajar con el IAlmacenadorArchivos
            var mock = new Mock<IAlmacenadorArchivos>();
            mock.Setup(x => x.GuardarArchivo(content, ".jpg", "actores", archivo.ContentType)).Returns(Task.FromResult("url"));

            var controller = new ActoresController(contexto, mapper, mock.Object);

            var respuesta = await controller.Post(actor);
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.AreEqual(201, resultado.StatusCode);

            var contexto2 = ConstruirContext(nombreBD);
            var listado = await contexto2.Actores.ToListAsync();
            Assert.AreEqual(1, listado.Count);
            //Assert.IsNull(listado[0].Foto);
            Assert.AreEqual("url", listado[0].Foto);
            Assert.AreEqual(1, mock.Invocations.Count);

        }

        [TestMethod]
        public async Task PatchRetorna404SiActorNoExiste()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutomapper();

            var controller = new ActoresController(contexto, mapper, null);
            var patchDoc = new JsonPatchDocument<ActorPatchDTO>();
            var respuesta = await controller.Patch(1, patchDoc);
            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task PatchActualizaUnSoloCampo()
        {

        }

    }
}
