#pragma warning disable 1591
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System;

namespace ServerStatus.Hubs
{
    public class ServerStatusHub : Hub
    {
        private static DateTime lastTime;
        private static TimeSpan lastTotalProcessorTime;
        private static DateTime curTime;
        private static TimeSpan curTotalProcessorTime;
        public Task timerCallback()
        {
            var proc = Process.GetCurrentProcess();
            var mem = proc.WorkingSet64;
            var cpu = proc.TotalProcessorTime;

            return Clients.All.InvokeAsync("broadcastStatus", new
            {
                ram = mem / (1024 * 1024),
                cpu = CPUUsage(proc)
            });
        }

        public double CPUUsage(Process p)
        {
            if (lastTime == null || lastTime == new DateTime())
            {
                lastTime = DateTime.Now;
                lastTotalProcessorTime = p.TotalProcessorTime;
            }

            curTime = DateTime.Now;
            curTotalProcessorTime = p.TotalProcessorTime;

            double CPUUsage = (curTotalProcessorTime.TotalMilliseconds - lastTotalProcessorTime.TotalMilliseconds) / curTime.Subtract(lastTime).TotalMilliseconds / Convert.ToDouble(Environment.ProcessorCount);

            lastTime = curTime;
            lastTotalProcessorTime = curTotalProcessorTime;

            return CPUUsage * 100;

        }
    }
}