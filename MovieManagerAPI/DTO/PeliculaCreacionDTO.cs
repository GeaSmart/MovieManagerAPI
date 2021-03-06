using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieManagerAPI.Helpers;
using MovieManagerAPI.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.DTO
{
    public class PeliculaCreacionDTO
    {
        [Required]
        [StringLength(300)]
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        [PesoArchivoValidacion(4)]
        [TipoArchivoValidacion(GrupoTipoArchivo.Imagen)]
        public IFormFile Poster { get; set; }
        [ModelBinder(binderType: typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIDs { get; set; }
        [ModelBinder(binderType: typeof(TypeBinder<List<PeliculasActoresCreacionDTO>>))]
        public List<PeliculasActoresCreacionDTO> Actores { get; set; }
    }
}
