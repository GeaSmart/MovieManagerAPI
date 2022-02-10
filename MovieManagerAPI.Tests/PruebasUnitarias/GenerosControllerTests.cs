using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieManagerAPI.Controllers;
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
    }
}
