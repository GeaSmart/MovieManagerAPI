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
        public async Task ObtenerSalasDeCine5km()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            //creamos el contexto, pero no el de basepruebas sino el de localdb ya que es en localdb donde estamos probando
            using (var context = LocalDbInitializer.GetDbContextLocalDb(false))
            {
                var salasDeCine = new List<SalaDeCine>()
                {
                    new SalaDeCine{ Nombre = "Real plaza Trujillo", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-79.031337, -8.131762)) },//el primer numero es el segundo que aparece en google maps: longitud, el otro latitud
                    new SalaDeCine{ Nombre = "Real plaza Lima", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-77.023628, -12.099039)) }
                };

                context.AddRange(salasDeCine);
                await context.SaveChangesAsync();
            }

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
                Assert.AreEqual(2, valor.Count);
            }
        }
    }
}
