using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using HealthentiaVitalsDashboard.Controllers;
using HealthentiaVitalsDashboard.Data;
using HealthentiaVitalsDashboard.Hubs;
using HealthentiaVitalsDashboard.Models;
using HealthentiaVitalsDashboard.Models.Dtos;

public class PatientVitalsControllerTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IHubContext<VitalSignsHub>> _hubContextMock;
    private readonly PatientVitalsController _controller;

    public PatientVitalsControllerTests()
    {
        var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // fresh DB per test
            .Options;

        _context = new AppDbContext(dbOptions);
        _context.Database.EnsureCreated();

        _hubContextMock = new Mock<IHubContext<VitalSignsHub>>();

        // Setup Clients.Group().SendAsync
        var mockClients = new Mock<IHubClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);
        _hubContextMock.Setup(h => h.Clients).Returns(mockClients.Object);

        _controller = new PatientVitalsController(_context, _hubContextMock.Object);
    }

    [Fact]
    public async Task PostVital_ReturnsBadRequest_WhenModelStateInvalid()
    {
        _controller.ModelState.AddModelError("HeartRate", "Required");

        var result = await _controller.PostVital(1, new VitalSignDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PostVital_ReturnsNotFound_WhenPatientNotExists()
    {
        var dto = new VitalSignDto
        {
            HeartRate = 80,
            BloodPressureSystolic = 120,
            BloodPressureDiastolic = 80,
            OxygenSaturation = 98,
            Timestamp = DateTime.UtcNow
        };

        var result = await _controller.PostVital(999, dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PostVital_ReturnsOk_AndSavesVital_AndSendsSignalR()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);
        var patient = new Patient { Id = 1, Name = "Test", VitalSigns = new List<VitalSign>() };
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var vitalDto = new VitalSignDto
        {
            HeartRate = 80,
            BloodPressureSystolic = 120,
            BloodPressureDiastolic = 80,
            OxygenSaturation = 98,
            Timestamp = DateTime.UtcNow
        };

        var clientProxyMock = new Mock<IClientProxy>();
        var clientsMock = new Mock<IHubClients>();
        var hubContextMock = new Mock<IHubContext<VitalSignsHub>>();

        clientsMock.Setup(c => c.Group("patient-1")).Returns(clientProxyMock.Object);
        hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);

        var controller = new PatientVitalsController(context, hubContextMock.Object);

        var result = await controller.PostVital(1, vitalDto);

        Assert.IsType<OkResult>(result);

        clientProxyMock.Verify(proxy =>
            proxy.SendCoreAsync(
                "ReceiveVitalUpdate",
                It.Is<object[]>(o => o.Length == 1),
                default), Times.Once);
    }
}