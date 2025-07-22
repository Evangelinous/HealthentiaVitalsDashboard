using HealthentiaVitalsDashboard.Data;
using HealthentiaVitalsDashboard.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using HealthentiaVitalsDashboard.Hubs;

namespace HealthentiaVitalsDashboard.Services;

public class VitalSignSimulator : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<VitalSignsHub> _hubContext;
    private readonly Random _random = new();

    public VitalSignSimulator(IServiceProvider serviceProvider, IHubContext<VitalSignsHub> hubContext)
    {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var patientIds = await db.Patients.Select(p => p.Id).ToListAsync();
            
            if (patientIds.Any())
            {
                var randomId = patientIds[_random.Next(patientIds.Count)];
                var patient = await db.Patients.FindAsync(randomId);

                var vital = new VitalSign
                {
                    PatientId = patient.Id,
                    Timestamp = DateTime.UtcNow,
                    HeartRate = _random.Next(60, 120),
                    BloodPressureSystolic = _random.Next(100, 160),
                    BloodPressureDiastolic = _random.Next(60, 100),
                    OxygenSaturation = _random.Next(85, 100)
                };

                db.VitalSigns.Add(vital);
                await db.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("ReceiveVitalUpdate", new
                {
                    patientId = vital.PatientId,
                    timestamp = vital.Timestamp,
                    heartRate = vital.HeartRate,
                    bloodPressureSystolic = vital.BloodPressureSystolic,
                    bloodPressureDiastolic = vital.BloodPressureDiastolic,
                    oxygenSaturation = vital.OxygenSaturation
                });
            }

            await Task.Delay(5000, stoppingToken); // every 5 seconds
        }
    }
}