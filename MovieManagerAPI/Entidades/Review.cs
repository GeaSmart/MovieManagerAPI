using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Entidades
{
    public class Review : IId
    {
        public int Id { get; set; }
        public string Comentario { get; set; }
        [Range(1,5)]
        public int Puntuacion { get; set; }
        public int PeliculaId { get; set; }
        public string UsuarioId { get; set; }

        //Propiedades de navegación
        public Pelicula Pelicula { get; set; }
        public IdentityUser Usuario { get; set; }
    }
}
