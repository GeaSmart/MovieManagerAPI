using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
    public class PeliculasControllerTests : BasePruebas
    {
        //método auxiliar para crear data de prueba
        private string CrearDataPrueba()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            
            var genero = new Genero() { Nombre = "genero 1" };

            var peliculas = new List<Pelicula>()
            {
                new Pelicula(){ Titulo = "Tiburón", FechaEstreno = new DateTime(2020,1,1), EnCines = false},
                new Pelicula(){ Titulo = "Gol", FechaEstreno = DateTime.Today.AddDays(1), EnCines = true},//se estrenará en el futuro
                new Pelicula(){ Titulo = "El preso", FechaEstreno = DateTime.Today.AddDays(-1), EnCines = true}//ya se estrenó
            };

            var peliculaConGenero = new Pelicula()
            {
                Titulo = "Jonas",
                FechaEstreno = new DateTime(2022, 02, 02),
                EnCines = false
            };

            peliculas.Add(peliculaConGenero);

            contexto.Add(genero);
            contexto.AddRange(peliculas);
            contexto.SaveChanges();

            var peliculaGenero = new PeliculasGeneros() { GeneroId = genero.Id, PeliculaId = peliculaConGenero.Id };
            contexto.Add(peliculaGenero);
            contexto.SaveChanges();

            return nombreBD;
        }

        [TestMethod]
        public async Task FiltrarPorTitulo()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutomapper();
            var contexto = ConstruirContext(nombreBD);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var tituloPelicula = "Gol";
            var filtroDTO = new PeliculasFiltroDTO()
            {
                Titulo = tituloPelicula,
                CantidadRegistrosPorPagina = 10
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            //verificación
            Assert.AreEqual(1, peliculas.Count);//ingresé en la data de prueba una peli titulada así
            Assert.AreEqual(tituloPelicula, peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarEnCines()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutomapper();
            var contexto = ConstruirContext(nombreBD);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var enCines = true;
            var filtroDTO = new PeliculasFiltroDTO()
            {
                EnCines = enCines,
                CantidadRegistrosPorPagina = 10
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            //verificación
            Assert.AreEqual(2, peliculas.Count);//porque al crear la data ingresé 2 peliculas en cine.
            Assert.AreEqual(enCines, peliculas[0].EnCines);
            Assert.AreEqual("Gol", peliculas[0].Titulo);//la primera peli en cine se llama así
        }

        [TestMethod]
        public async Task FiltrarProximosEstrenos()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutomapper();
            var contexto = ConstruirContext(nombreBD);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var proximosEstrenos = true;
            var filtroDTO = new PeliculasFiltroDTO()
            {
                ProximosEstrenos = proximosEstrenos,
                CantidadRegistrosPorPagina = 10
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            //verificación
            Assert.AreEqual(1, peliculas.Count);//porque al crear la data ingresé 1 pelicula para prox estreno.            
            Assert.AreEqual("Gol", peliculas[0].Titulo);//la primera peli para proximo estreno se llama así
        }

        [TestMethod]
        public async Task FiltrarPeliculaConGenero()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutomapper();
            var contexto = ConstruirContext(nombreBD);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var generoId = contexto.Generos.Select(x => x.Id).First();

            var filtroDTO = new PeliculasFiltroDTO()
            {
                GeneroId = generoId
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            //verificación
            Assert.AreEqual(1, peliculas.Count);//porque al crear la data ingresé 1 pelicula con genero.            
            Assert.AreEqual("Jonas", peliculas[0].Titulo);//la primera peli con género se llama así
        }

        [TestMethod]
        public async Task FiltrarOrdenaTitulosAscendente()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutomapper();
            var contexto = ConstruirContext(nombreBD);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            
            var filtroDTO = new PeliculasFiltroDTO()
            {
                CampoOrdenar = "titulo",
                IsAscendente = true
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreBD);
            var peliculasDB = contexto2.Peliculas.OrderBy(x => x.Titulo).ToList();

            //verificación
            Assert.AreEqual(peliculasDB.Count, peliculas.Count);
            
            for(int i = 0; i < peliculasDB.Count; i++)
            {
                var peliculaFiltrada = peliculas[i];
                var peliculaDB = peliculasDB[i];

                Assert.AreEqual(peliculaDB.Id, peliculaFiltrada.Id);//verificamos que ambos listados tengan el mismo orden
            }                        
        }

        [TestMethod]
        public async Task FiltrarOrdenaTitulosDescendente()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutomapper();
            var contexto = ConstruirContext(nombreBD);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new PeliculasFiltroDTO()
            {
                CampoOrdenar = "titulo",
                IsAscendente = false
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreBD);
            var peliculasDB = contexto2.Peliculas.OrderByDescending(x => x.Titulo).ToList();

            //verificación
            Assert.AreEqual(peliculasDB.Count, peliculas.Count);

            for (int i = 0; i < peliculasDB.Count; i++)
            {
                var peliculaFiltrada = peliculas[i];
                var peliculaDB = peliculasDB[i];

                Assert.AreEqual(peliculaDB.Id, peliculaFiltrada.Id);//verificamos que ambos listados tengan el mismo orden
            }
        }

        [TestMethod]
        public async Task FiltrarPorCampoIncorrectoDevuelvePeliculas()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutomapper();
            var contexto = ConstruirContext(nombreBD);

            var mock = new Mock<ILogger<PeliculasController>>();
            
            var controller = new PeliculasController(contexto, mapper, null, mock.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new PeliculasFiltroDTO()
            {
                CampoOrdenar = "noexisteestecampo",
                IsAscendente = true
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreBD);
            var peliculasDB = contexto2.Peliculas.ToList();

            //verificación
            Assert.AreEqual(peliculasDB.Count, peliculas.Count);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
    }
}
