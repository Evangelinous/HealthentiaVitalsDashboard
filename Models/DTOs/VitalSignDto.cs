using System.ComponentModel.DataAnnotations;
namespace HealthentiaVitalsDashboard.Models.Dtos
{
    public class VitalSignDto
    {
        [Required]
        public int HeartRate { get; set; }

        [Required]
        public int BloodPressureSystolic { get; set; }

        [Required]
        public int BloodPressureDiastolic { get; set; }

        [Required]
        public int OxygenSaturation { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}