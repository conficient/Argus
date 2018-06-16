using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Argus.Server.Hubs
{
    public class TestHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}

