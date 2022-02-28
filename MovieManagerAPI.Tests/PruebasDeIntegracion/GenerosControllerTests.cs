using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieManagerAPI.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Tests.PruebasDeIntegracion
{
    public class GenerosControllerTests : BasePruebas
    {
        private static readonly string url = "/api/generos";

        [TestMethod]
        public async Task ObtenerTodosLosGenerosListadoVacio()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreBD);

            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);

            respuesta.EnsureSuccessStatusCode();

            var generos = JsonConvert.DeserializeObject<List<GeneroDTO>>(await respuesta.Content.ReadAsStringAsync());

            Assert.AreEqual(0, generos.Count);
        }

    }
}
