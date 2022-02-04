using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.DTO
{
    public class SalaDeCineCercanoFiltroDTO
    {
        [Range(-90, 90)]
        public double Latitud { get; set; }
        [Range(-180, 180)]
        public double Longitud { get; set; }

        private int distanciaEnKms = 10; //distancia por defecto
        private int distanciaEnKmsMaxima = 500;
        public int DistanciaEnKms {
            get{
                return distanciaEnKms;
            }
            set{
                distanciaEnKms = (value > distanciaEnKmsMaxima) ? distanciaEnKmsMaxima : value;
            } 
        }
    }
}
