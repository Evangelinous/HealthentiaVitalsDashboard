using Microsoft.EntityFrameworkCore;
using HealthentiaVitalsDashboard.Data;
using HealthentiaVitalsDashboard.Models;

namespace HealthentiaVitalsDashboard.Services;

public static class DatabaseSeeder
{
    public static void Seed(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Database.Migrate(); // ensure DB is created

        // Seed Patients
        if (!context.Patients.Any())
        {
            context.Patients.AddRange(
                new Patient { Id = 1, Name = "John Doe", Age = 45, RoomNumber = "101" },
                new Patient { Id = 2, Name = "Jane Smith", Age = 32, RoomNumber = "102" },
                new Patient { Id = 3, Name = "Bob Johnson", Age = 67, RoomNumber = "103" }
            );
            context.SaveChanges();
        }

        // Seed VitalSigns
        if (!context.VitalSigns.Any())
        {
            var now = DateTime.UtcNow;

            context.VitalSigns.AddRange(
                // John Doe
                new VitalSign { PatientId = 1, Timestamp = now.AddMinutes(-30), HeartRate = 72, BloodPressureSystolic = 118, BloodPressureDiastolic = 76, OxygenSaturation = 97 },
                new VitalSign { PatientId = 1, Timestamp = now.AddMinutes(-15), HeartRate = 75, BloodPressureSystolic = 120, BloodPressureDiastolic = 80, OxygenSaturation = 96 },

                // Jane Smith
                new VitalSign { PatientId = 2, Timestamp = now.AddMinutes(-40), HeartRate = 85, BloodPressureSystolic = 135, BloodPressureDiastolic = 88, OxygenSaturation = 92 },
                new VitalSign { PatientId = 2, Timestamp = now.AddMinutes(-20), HeartRate = 89, BloodPressureSystolic = 140, BloodPressureDiastolic = 90, OxygenSaturation = 90 },

                // Bob Johnson
                new VitalSign { PatientId = 3, Timestamp = now.AddMinutes(-25), HeartRate = 102, BloodPressureSystolic = 145, BloodPressureDiastolic = 95, OxygenSaturation = 85 },
                new VitalSign { PatientId = 3, Timestamp = now.AddMinutes(-5), HeartRate = 110, BloodPressureSystolic = 150, BloodPressureDiastolic = 98, OxygenSaturation = 82 }
            );

            context.SaveChanges();
        }
    }
}