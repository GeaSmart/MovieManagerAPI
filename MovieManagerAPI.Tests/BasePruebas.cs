using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieManagerAPI.Helpers;
using NetTopologySuite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieManagerAPI.Tests
{
    public class BasePruebas
    {
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
    }
}
