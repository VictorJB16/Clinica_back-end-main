using Clinica_back_end.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Clinica_back_end
{
    public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<TipoCita> TiposCita { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("identity");

            // Configuración de la relación uno a uno entre ApplicationUser y Patient
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Patient)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey<Paciente>(p => p.ApplicationUserId);

            modelBuilder.Entity<ApplicationUser>()
                .Property(us => us.FullName)
                .HasMaxLength(200);


            // Relaciones y restricciones
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Paciente)
                .WithMany(p => p.Citas)
                .HasForeignKey(c => c.PacienteId);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Sucursal)
                .WithMany(s => s.Citas)
                .HasForeignKey(c => c.SucursalId);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.TipoCita)
                .WithMany(tc => tc.Citas)
                .HasForeignKey(c => c.TipoCitaId);

        }
    }

}
