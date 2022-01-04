using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.DTO
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;
        private int cantidadRegistrosPorPagina { get; set; } = 10;
        private readonly int CantidadMaximaRegistrosPorPagina = 50;
            
        public int CantidadRegistrosPorPagina
        {
            get => cantidadRegistrosPorPagina;
            set
            {
                cantidadRegistrosPorPagina = (value > CantidadMaximaRegistrosPorPagina) ? CantidadMaximaRegistrosPorPagina : value;
            }
        }
    }
}
