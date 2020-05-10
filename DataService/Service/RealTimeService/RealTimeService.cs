using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AttendanceSystemIPCamera.Services.RecordService
{
    public interface IRealTimeService
    {
    }

    public class HubMethods
    {
        public static string KEEP_ALIVE = "keepAlive";
    }

    public class RealTimeService : Hub, IRealTimeService
    {
        private readonly IHubContext<RealTimeService> hubContext;
        private readonly Timer timer;
        public RealTimeService(IHubContext<RealTimeService> hubContext)
        {
            this.hubContext = hubContext;
            timer = new Timer(TimeSpan.FromSeconds(30).TotalMilliseconds);
            timer.Elapsed += async (source, e) =>
            {
                await KeepAlive();
            };
            timer.AutoReset = true;
        }
        public async Task KeepAlive()
        {
            await hubContext.Clients.All.SendAsync(HubMethods.KEEP_ALIVE);
        }

        public override Task OnConnectedAsync()
        {
            timer.Start();
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            timer?.Stop();
            return Task.CompletedTask;
        }
    }
}
