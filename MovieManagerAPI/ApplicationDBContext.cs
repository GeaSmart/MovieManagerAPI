using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieManagerAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }
        //Configuración de api fluente
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PeliculasGeneros>().HasKey(x => new { x.PeliculaId, x.GeneroId });
            modelBuilder.Entity<PeliculasActores>().HasKey(x => new { x.PeliculaId, x.ActorId });
            modelBuilder.Entity<PeliculasSalasDeCine>().HasKey(x => new { x.PeliculaId, x.SalaDeCineId });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; }
        public DbSet<PeliculasActores> PeliculasActores { get; set; }
        public DbSet<SalaDeCine> SalasDeCine { get; set; }
        public DbSet<PeliculasSalasDeCine> PeliculasSalasDeCine { get; set; }
    }
}
