using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Entidades
{
    public class SalaDeCine
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }

        //Propiedades de navegacion
        public List<PeliculasSalasDeCine> PeliculasSalasDeCines { get; set; }        
    }
}
