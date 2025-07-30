using Microsoft.AspNetCore.SignalR;

namespace HealthentiaVitalsDashboard.Hubs
{
    public class VitalSignsHub : Hub
    {
        public async Task SendCriticalAlert(object alert)
        {
            await Clients.All.SendAsync("ReceiveCriticalAlert", alert);
        }
    }
}