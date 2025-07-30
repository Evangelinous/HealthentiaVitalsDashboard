using HealthentiaVitalsDashboard.Models;

public class VitalSignTests
{
    [Theory]
    [InlineData(130, 150, 95, 89, VitalStatus.Critical)]
    [InlineData(110, 135, 85, 92, VitalStatus.Warning)]
    [InlineData(80, 120, 80, 98, VitalStatus.Warning)]
    public void GetStatus_ReturnsExpectedStatus(int hr, int sys, int dia, int spo2, VitalStatus expected)
    {
        var vital = new VitalSign
        {
            Timestamp = DateTime.UtcNow,
            HeartRate = hr,
            BloodPressureSystolic = sys,
            BloodPressureDiastolic = dia,
            OxygenSaturation = spo2
        };

        var result = vital.GetStatus();

        Assert.Equal(expected, result);
    }
}