using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Entidades
{
    public class PeliculasActores
    {
        public int PeliculaId { get; set; }
        public int ActorId { get; set; }
        public string Personaje { get; set; } //para guardar el nombre del personaje en la película
        public int Orden { get; set; }//siempre se quiere que el protagonista aparezca primero
        public Pelicula Pelicula { get; set; }
        public Actor Actor { get; set; }
    }
}
