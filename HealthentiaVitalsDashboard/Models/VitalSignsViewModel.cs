namespace HealthentiaVitalsDashboard.Models;

public class VitalSignsViewModel
{
    public Patient Patient { get; set; } = new();
    public List<VitalSign> VitalSigns { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool FilterLast24h { get; set; }
}