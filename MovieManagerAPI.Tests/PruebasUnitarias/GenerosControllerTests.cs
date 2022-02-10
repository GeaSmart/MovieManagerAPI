using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class GenerosControllerTests : BasePruebas
    {
        [TestMethod]
        public async Task ObtenerTodosLosGeneros()
        {
            //Preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutomapper();

            contexto.Generos.Add(new Genero { Nombre = "género 1" });
            contexto.Generos.Add(new Genero { Nombre = "género 2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD); //creamos otro contexto para asegurarnos que la información viene de la bd y no de la variable contexto, es decir en memoria

            //Prueba
            var controlador = new GenerosController(contexto2, mapper);
            var respuesta = await controlador.Get();

            //Verificación
            var generos = respuesta.Value;
            Assert.AreEqual(2, generos.Count);
        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdNoExistente()
        {
            //Preparación
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutomapper();

            //Prueba
            var controlador = new GenerosController(contexto, mapper);
            var respuesta = await controlador.Get(1);

            //Verificación
            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdExistente()
        {
            //Preparación
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutomapper();

            contexto.Generos.Add(new Genero { Nombre = "género 1" });
            contexto.Generos.Add(new Genero { Nombre = "género 2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);

            //Prueba
            int id = 1;
            var controlador = new GenerosController(contexto2, mapper);
            var respuesta = await controlador.Get(id);

            //Verificación
            var resultado = respuesta.Value;
            Assert.AreEqual(id, resultado.Id);
        }

        [TestMethod]
        public async Task CrearGenero()
        {
            //Preparación
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutomapper();

            //Prueba            
            var controlador = new GenerosController(contexto, mapper);
            var nuevoGenero = new GeneroCreacionDTO { Nombre = "nombre" };
            var respuesta = await controlador.Post(nuevoGenero);

            //Verificación
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.IsNotNull(resultado);

            var contexto2 = ConstruirContext(nombreBD);
            var cantidad = await contexto2.Generos.CountAsync();
            Assert.AreEqual(1, cantidad);
        }

        [TestMethod]
        public async Task ActualizarGenero()
        {
            //Preparación
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutomapper();

            //Prueba
            var genero = new Genero { Nombre = "nombre" };
            var generoCreacionDTO = new GeneroCreacionDTO { Nombre = "nuevo nombre" };
            contexto.Generos.Add(genero);
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);

            var controlador = new GenerosController(contexto2, mapper);
            int id = 1;
            var respuesta = await controlador.Put(id, generoCreacionDTO);

            //Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreBD);
            var existe = await contexto3.Generos.AnyAsync(x => x.Nombre == "nuevo nombre");
            Assert.IsTrue(existe);
        }

        [TestMethod]
        public async Task EliminarGeneroNoExistente()
        {
            //Preparación
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutomapper();

            //Prueba
            var controlador = new GenerosController(contexto, mapper);
            var id = 1;
            var respuesta = await controlador.Delete(id);

            //Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);           
        }

        [TestMethod]
        public async Task EliminarGeneroExistente()
        {
            //Preparación
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutomapper();

            //Prueba            
            var genero = new Genero { Nombre = "nombre" };
            contexto.Generos.Add(genero);
            await contexto.SaveChangesAsync();
                        
            var contexto2 = ConstruirContext(nombreBD);
            var controlador = new GenerosController(contexto2, mapper);
            var respuesta = await controlador.Delete(1);

            //Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreBD);
            var existe = await contexto3.Generos.AnyAsync();
            Assert.IsFalse(existe);
        }
    }
}
