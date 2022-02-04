using AutoMapper;
using MovieManagerAPI.DTO;
using MovieManagerAPI.Entidades;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();

            CreateMap<SalaDeCine, SalaDeCineDTO>()
                .ForMember(x=>x.Latitud,x=>x.MapFrom(y=>y.Ubicacion.Y))
                .ForMember(x=>x.Longitud,x=>x.MapFrom(y=>y.Ubicacion.X));

            CreateMap<SalaDeCineDTO, SalaDeCine>()
                .ForMember(x=>x.Ubicacion,x=>x.MapFrom(y=>
                geometryFactory.CreatePoint(new Coordinate(y.Longitud,y.Latitud))));

            CreateMap<SalaDeCineCreacionDTO, SalaDeCine>()
                .ForMember(x => x.Ubicacion, x => x.MapFrom(y =>
                geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));
                        

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());//se ignora porque los tipos son distintos
            CreateMap<Actor, ActorPatchDTO>().ReverseMap();

            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(x => x.Poster, options => options.Ignore())//se ignora porque los tipos son distintos
                .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.PeliculasActores, options => options.MapFrom(MapPeliculasActores));

            CreateMap<Pelicula, PeliculaDetallesDTO>()
                .ForMember(x=>x.Generos,options=>options.MapFrom(MapPeliculasGeneros))
                .ForMember(x=>x.Actores, options =>options.MapFrom(MapPeliculasActores));

            CreateMap<Pelicula, PeliculaPatchDTO>().ReverseMap();
        }

        private List<GeneroDTO> MapPeliculasGeneros(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<GeneroDTO>();
            if (pelicula.PeliculasGeneros == null)
                return resultado;

            foreach (var item in pelicula.PeliculasGeneros)
                resultado.Add(new GeneroDTO { Id = item.GeneroId, Nombre = item.Genero.Nombre });
            
            return resultado;
        }

        private List<ActorPeliculaDetalleDTO> MapPeliculasActores(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<ActorPeliculaDetalleDTO>();
            if (pelicula.PeliculasActores == null)
                return resultado;

            foreach (var item in pelicula.PeliculasActores)
                resultado.Add(new ActorPeliculaDetalleDTO{ ActorId = item.ActorId, Persona = item.Actor.Nombre, Personaje = item.Personaje });

            return resultado;
        }

        private List<PeliculasGeneros> MapPeliculasGeneros(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();
            if (peliculaCreacionDTO.GenerosIDs == null)
                return resultado;

            foreach (var id in peliculaCreacionDTO.GenerosIDs)
                resultado.Add(new PeliculasGeneros { GeneroId = id });

            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActores(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();
            if (peliculaCreacionDTO.Actores == null)
                return resultado;

            foreach (var actor in peliculaCreacionDTO.Actores)
                resultado.Add(new PeliculasActores { ActorId = actor.ActorId, Personaje = actor.Personaje });

            return resultado;
        }
    }
}
