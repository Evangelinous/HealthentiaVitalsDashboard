namespace HealthentiaVitalsDashboard.Models.Dtos;
public class AlertNotification
{
    public int PatientId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Value { get; set; }
    public DateTime Timestamp { get; set; }
    public string Message { get; set; } = string.Empty;
}
