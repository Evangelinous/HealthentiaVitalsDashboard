using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthentiaVitalsDashboard.Data;
using Microsoft.AspNetCore.Authorization;
using HealthentiaVitalsDashboard.Models;
using HealthentiaVitalsDashboard.Models.Dtos;
using Microsoft.AspNetCore.SignalR;
using HealthentiaVitalsDashboard.Hubs;

namespace HealthentiaVitalsDashboard.Controllers;

    [Authorize]
    public class PatientsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<VitalSignsHub> _hubContext;

        public PatientsController(AppDbContext context, IHubContext<VitalSignsHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<IActionResult> Index()
    {
        var patients = await _context.Patients.ToListAsync();
        return View(patients);
    }

    public async Task<IActionResult> Details(int id, int page = 1, bool last24h = false)
    {
        const int pageSize = 10;

        var patient = await _context.Patients
            .Include(p => p.VitalSigns)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            return NotFound();
        }

        var vitalsQuery = patient.VitalSigns.AsQueryable();

        if (last24h)
        {
            var cutoff = DateTime.UtcNow.AddHours(-24);
            vitalsQuery = vitalsQuery.Where(v => v.Timestamp >= cutoff);
        }

        var totalVitals = vitalsQuery.Count();
        var totalPages = (int)Math.Ceiling(totalVitals / (double)pageSize);

        var vitalsPaged = vitalsQuery
            .OrderByDescending(v => v.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var viewModel = new VitalSignsViewModel
        {
            Patient = patient,
            VitalSigns = vitalsPaged,
            CurrentPage = page,
            TotalPages = totalPages,
            FilterLast24h = last24h
        };

        ViewBag.Patient = patient; // You can also create a wrapper view model if needed
        return View(viewModel);
    }
    
    [HttpPost("/patients/{id}/vitals")]
    public async Task<IActionResult> PostVital(int id, [FromBody] VitalSignDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var patient = await _context.Patients.Include(p => p.VitalSigns).FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null)
            return NotFound();

        var vital = new VitalSign
        {
            HeartRate = dto.HeartRate,
            BloodPressureSystolic = dto.BloodPressureSystolic,
            BloodPressureDiastolic = dto.BloodPressureDiastolic,
            OxygenSaturation = dto.OxygenSaturation,
            Timestamp = dto.Timestamp,
            PatientId = id
        };

        _context.VitalSigns.Add(vital);
        await _context.SaveChangesAsync();

        // Notify via SignalR
        await _hubContext.Clients.Group($"patient-{id}").SendAsync("ReceiveVitalUpdate", new
        {
            patientId = id,
            timestamp = vital.Timestamp,
            heartRate = vital.HeartRate,
            bloodPressureSystolic = vital.BloodPressureSystolic,
            bloodPressureDiastolic = vital.BloodPressureDiastolic,
            oxygenSaturation = vital.OxygenSaturation
        });

        return Ok();
    }
}