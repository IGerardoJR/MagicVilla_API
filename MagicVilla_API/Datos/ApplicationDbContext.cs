using MagicVilla_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Datos
{
    public class ApplicationDbContext : DbContext
    {
        // Le mandamos las options por medio de base(o super como se quiera ver)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options) :base(options)
        {
            
        }
        //           modelo
        public DbSet<Villa> Villas { get; set; }

        public DbSet<NumeroVilla> NumeroVilla { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData(
                new Villa()
                {
                    Id=1,
                    Nombre="Villa Real",
                    Detalle="Detalle de la villa...",
                    ImagenUrl="",
                    Ocupantes=5,
                    MetrosCuadrados=50,
                    Tarifa=200,
                    Amenidad="",
                    FechaCreacion=DateTime.Now,
                    FechaActualizacion=DateTime.Now
                },
                new Villa()
                {
                    Id = 2,
                    Nombre = "Premium Vista a la piscina",
                    Detalle = "Detalle de la villa",
                    ImagenUrl = "",
                    Ocupantes=4,
                    MetrosCuadrados = 50,
                    Tarifa = 300,
                    Amenidad = "",
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                }

                );
        }


    }
}
