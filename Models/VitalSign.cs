namespace HealthentiaVitalsDashboard.Models;

public class VitalSign
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int HeartRate { get; set; }
    public int BloodPressureSystolic { get; set; }
    public int BloodPressureDiastolic { get; set; }
    public int OxygenSaturation { get; set; }

    public VitalStatus GetStatus()
    {
        // Any critical reading makes the row critical
        bool critical =
            HeartRate > 120 ||
            BloodPressureSystolic > 139 || BloodPressureDiastolic > 90 ||
            OxygenSaturation < 90;

        bool warning =
            (HeartRate > 100 && HeartRate <= 120) ||
            (BloodPressureSystolic >= 120 && BloodPressureSystolic <= 139) ||
            (BloodPressureDiastolic >= 80 && BloodPressureDiastolic <= 89) ||
            (OxygenSaturation >= 90 && OxygenSaturation <= 95);

        if (critical) return VitalStatus.Critical;
        if (warning) return VitalStatus.Warning;
        return VitalStatus.Normal;
    }
}

public enum VitalStatus
{
    Normal,
    Warning,
    Critical
}
