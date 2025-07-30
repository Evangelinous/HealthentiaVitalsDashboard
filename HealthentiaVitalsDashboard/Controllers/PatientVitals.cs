using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthentiaVitalsDashboard.Data;
using Microsoft.AspNetCore.Authorization;
using HealthentiaVitalsDashboard.Models;
using HealthentiaVitalsDashboard.Models.Dtos;
using Microsoft.AspNetCore.SignalR;
using HealthentiaVitalsDashboard.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HealthentiaVitalsDashboard.Controllers;

/// <summary>
/// Controller responsible for handling vital sign submissions for patients.
/// </summary>
/// <remarks>
/// Requires JWT Bearer authentication to access endpoints.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PatientVitalsController : Controller
{
    private readonly AppDbContext _context;
    private readonly IHubContext<VitalSignsHub> _hubContext;

    public PatientVitalsController(AppDbContext context, IHubContext<VitalSignsHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }
    
    /// <summary>
    /// Submits a new vital sign record for a specific patient.
    /// </summary>
    /// <param name="id">The ID of the patient to whom the vital sign belongs.</param>
    /// <param name="dto">The data transfer object containing vital sign measurements.</param>
    /// <returns>
    /// Returns <see cref="OkResult"/> if the vital sign was saved successfully,  
    /// <see cref="BadRequestObjectResult"/> if the model state is invalid,  
    /// or <see cref="NotFoundResult"/> if the patient does not exist.
    /// </returns>
    /// <response code="200">Vital sign submitted successfully.</response>
    /// <response code="400">The submitted data was invalid.</response>
    /// <response code="404">The specified patient was not found.</response>

    [HttpPost("/patientvitals/{id}/vitals")]
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