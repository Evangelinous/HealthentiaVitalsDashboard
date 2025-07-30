using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthentiaVitalsDashboard.Data;
using Microsoft.AspNetCore.Authorization;
using HealthentiaVitalsDashboard.Models;
using System.Text;

namespace HealthentiaVitalsDashboard.Controllers;


public class PatientsController : Controller
{
    private readonly AppDbContext _context;

    public PatientsController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var patients = await _context.Patients.ToListAsync();
        return View(patients);
    }

    [Authorize]
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

    [HttpGet]
public async Task<IActionResult> ExportVitalsToCsv(int id)
{
    var patient = await _context.Patients
        .Include(p => p.VitalSigns)
        .FirstOrDefaultAsync(p => p.Id == id);

    if (patient == null || patient.VitalSigns == null || !patient.VitalSigns.Any())
        return NotFound("No vitals found for this patient.");

    var builder = new StringBuilder();
    builder.AppendLine("Timestamp,HeartRate,BloodPressureSystolic,BloodPressureDiastolic,OxygenSaturation");

    foreach (var v in patient.VitalSigns.OrderBy(v => v.Timestamp))
    {
        builder.AppendLine($"{v.Timestamp:u},{v.HeartRate},{v.BloodPressureSystolic},{v.BloodPressureDiastolic},{v.OxygenSaturation}");
    }

    var csvBytes = Encoding.UTF8.GetBytes(builder.ToString());
    var filename = $"vital_signs_patient_{id}.csv";

    return File(csvBytes, "text/csv", filename);
}
}
