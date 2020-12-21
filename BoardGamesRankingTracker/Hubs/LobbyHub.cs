using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGamesRankingTracker.Hubs
{
    public class LobbyHub : Hub
    {
        public void Hello(string name)
        {
            Clients.Others.hello(name);
        }

        public void Goodbye(string name)
        {
            Clients.All.goodbye(name);
        }

        public void Send(string Name, string message)
        {
            Clients.All.addNewMessageToPage(Name, message);
        }

    }
}