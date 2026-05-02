using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using UPHC.SurveillanceDashboard.Models;

namespace UPHC.SurveillanceDashboard.Hubs
{
    public class NotificationHub : Hub 
    { 

        // analysts will use this
        public async Task JoinAnalystGroup() 
        {
             await Groups.AddToGroupAsync(Context.ConnectionId, "Analysts"); 
           
        }


        //Adding this for Dashboard auto update..
        public async Task JoinDashboardGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Dashboard");
        }
    }
}
