namespace HealthentiaVitalsDashboard.Models;

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string RoomNumber { get; set; } = string.Empty;

    public List<VitalSign> VitalSigns { get; set; } = new();
}