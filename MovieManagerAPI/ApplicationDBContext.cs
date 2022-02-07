using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieManagerAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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


        private void SeedData(ModelBuilder modelBuilder)
        {
            //creamos un usuario como data semilla

            //Usuario            
            var usuarioAdminId = "4ecc4de5-27ba-4012-bdce-7dfd8d3c3bff"; //GUIDs generados en https://www.guidgenerator.com/
            var usuarioUsername = "eldevelopermacho@bravedeveloper.com";
            var passwordHasher = new PasswordHasher<IdentityUser>();

            var usuarioAdmin = new IdentityUser()
            {
                Id = usuarioAdminId,
                UserName = usuarioUsername,
                NormalizedUserName = usuarioUsername,
                Email = usuarioUsername,
                NormalizedEmail = usuarioUsername,
                PasswordHash = passwordHasher.HashPassword(null, "Aa123456!")
            };

            //Rol
            var rolAdminId = "7a0b978d-d515-4e38-89a3-dea1dbec6275";

            var rolAdmin = new IdentityRole()
            {
                Id = rolAdminId,
                Name = "Admin",
                NormalizedName = "Admin"
            };

            //Insertando data
            modelBuilder.Entity<IdentityUser>().HasData(usuarioAdmin);
            modelBuilder.Entity<IdentityRole>().HasData(rolAdmin);
            modelBuilder.Entity<IdentityUserClaim<string>>().HasData(new IdentityUserClaim<string>()
            {
                Id = 1,
                ClaimType = ClaimTypes.Role,
                UserId = usuarioAdminId,
                ClaimValue = "Admin"
            });

        }
    }
}
