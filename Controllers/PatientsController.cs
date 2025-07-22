using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthentiaVitalsDashboard.Data;
using Microsoft.AspNetCore.Authorization;

namespace HealthentiaVitalsDashboard.Controllers;
[Authorize]
public class PatientsController : Controller
{
    private readonly AppDbContext _context;

 
    public PatientsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var patients = await _context.Patients.ToListAsync();
        return View(patients);
    }

    public async Task<IActionResult> Details(int id)
    {
        var patient = await _context.Patients
            .Include(p => p.VitalSigns)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
            return NotFound();

        return View(patient);
    }
}