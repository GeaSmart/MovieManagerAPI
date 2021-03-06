using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Entidades
{
    public class Genero :IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        public string Nombre { get; set; }

        //Propiedades de navegacion
        public List<PeliculasGeneros> PeliculasGeneros { get; set; }
    }
}
