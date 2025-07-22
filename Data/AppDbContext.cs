using Microsoft.EntityFrameworkCore;
using HealthentiaVitalsDashboard.Models;

namespace HealthentiaVitalsDashboard.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<VitalSign> VitalSigns => Set<VitalSign>();
    }
}