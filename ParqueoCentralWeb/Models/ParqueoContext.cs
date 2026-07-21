using System.Data.Entity;

namespace ParqueoCentralWeb.Models
{
    /// <summary>
    /// Contexto de Entity Framework (Code First) utilizado para el acceso a datos
    /// de todo el sistema de gestión de estacionamiento.
    /// </summary>
    public class ParqueoContext : DbContext
    {
        public ParqueoContext() : base("name=ParqueoContext")
        {
        }

        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<EspacioEstacionamiento> Espacios { get; set; }
        public DbSet<MovimientoEstacionamiento> Movimientos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehiculo>().ToTable("Vehiculos");
            modelBuilder.Entity<EspacioEstacionamiento>().ToTable("EspaciosEstacionamiento");
            modelBuilder.Entity<MovimientoEstacionamiento>().ToTable("MovimientosEstacionamiento");

            base.OnModelCreating(modelBuilder);
        }
    }
}
