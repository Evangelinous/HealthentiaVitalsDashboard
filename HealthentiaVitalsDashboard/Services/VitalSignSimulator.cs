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

                if (patient == null)
                {
                    continue; // Skip if patient not found
                }

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
                
                // Check for critical values and send alert if needed
                if (vital.GetStatus() == VitalStatus.Critical)
                {
                    List<string> criticals = new();

                    if (vital.HeartRate > 120)
                        criticals.Add($"Heart Rate: {vital.HeartRate} bpm");

                    if (vital.BloodPressureSystolic > 139 || vital.BloodPressureDiastolic > 90)
                        criticals.Add($"Blood Pressure: {vital.BloodPressureSystolic}/{vital.BloodPressureDiastolic} mmHg");

                    if (vital.OxygenSaturation < 90)
                        criticals.Add($"Oxygen Saturation: {vital.OxygenSaturation}%");

                    var alert = new
                    {
                        patientId = vital.PatientId,
                        patientName = patient.Name,
                        room = patient.RoomNumber,
                        type = "VitalSign",
                        value = new
                        {
                            heartRate = vital.HeartRate,
                            systolic = vital.BloodPressureSystolic,
                            diastolic = vital.BloodPressureDiastolic,
                            oxygenSaturation = vital.OxygenSaturation
                        },
                        timestamp = vital.Timestamp,
                        message = $"⚠️ Critical values for {patient.Name} (Room {patient.RoomNumber}): {string.Join(", ", criticals)}"
                    };

                    await _hubContext.Clients.All.SendAsync("ReceiveCriticalAlert", alert);

                }
            }

            await Task.Delay(5000, stoppingToken); // every 5 seconds
        }
    }
}