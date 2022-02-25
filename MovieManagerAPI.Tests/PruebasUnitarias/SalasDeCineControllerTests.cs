using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieManagerAPI.Controllers;
using MovieManagerAPI.DTO;
using MovieManagerAPI.Entidades;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieManagerAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class SalasDeCineControllerTests : BasePruebas
    {
        [TestMethod]
        public async Task ObtenerSalasDeCine1km()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var filtro = new SalaDeCineCercanoFiltroDTO()
            {
                DistanciaEnKms = 1,
                Latitud = -8.109588,
                Longitud = -79.028108
            };

            using (var context = LocalDbInitializer.GetDbContextLocalDb(false))
            {
                var mapper = ConfigurarAutomapper();
                var controller = new SalasDeCineController(context, mapper, geometryFactory);
                var respuesta = await controller.Cercanos(filtro);
                var valor = respuesta.Value;
                Assert.AreEqual(0, valor.Count);//A 1km no hay ninguno
            }
        }

        [TestMethod]
        public async Task ObtenerSalasDeCine5km()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var filtro = new SalaDeCineCercanoFiltroDTO()
            {
                DistanciaEnKms = 5,
                Latitud = -8.109588,
                Longitud = -79.028108
            };

            using (var context = LocalDbInitializer.GetDbContextLocalDb(false))
            {
                var mapper = ConfigurarAutomapper();
                var controller = new SalasDeCineController(context, mapper, geometryFactory);
                var respuesta = await controller.Cercanos(filtro);
                var valor = respuesta.Value;
                Assert.AreEqual(1, valor.Count);//Solo Real Plaza Trujillo está a menos de 5km
            }
        }

        [TestMethod]
        public async Task ObtenerSalasDeCine500km()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);                       

            var filtro = new SalaDeCineCercanoFiltroDTO()
            {
                DistanciaEnKms = 500,
                Latitud = -8.109588,
                Longitud = -79.028108
            };

            using (var context = LocalDbInitializer.GetDbContextLocalDb(false))
            {
                var mapper = ConfigurarAutomapper();
                var controller = new SalasDeCineController(context, mapper, geometryFactory);
                var respuesta = await controller.Cercanos(filtro);
                var valor = respuesta.Value;
                Assert.AreEqual(2, valor.Count);//Ambas están dentro de los 500km
            }
        }
    }
}
