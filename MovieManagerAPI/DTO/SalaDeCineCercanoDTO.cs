using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.DTO
{
    public class SalaDeCineCercanoDTO : SalaDeCineDTO
    {
        public double DistanciaEnMetros { get; set; }
    }
}
