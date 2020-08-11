﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer.Debugging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;
using WatsonTcp;

namespace Multiplayer.Networking
{
    public class ClientClass : IDisposable
    {
        public static ClientClass Instance;
        bool isLoggedin = false;
        static WatsonTcpClient client;
        private bool disposedValue;

        public ClientClass()
        {
            Instance = this;
        }

        public void Connect(string ip, ushort port = 52512, string password = "", Helpers.UserRole userRole = Helpers.UserRole.Client)
        {
            client = new WatsonTcpClient(ip, port);
            client.ServerConnected += ServerConnected;
            client.ServerDisconnected += ServerDisconnected;
            client.MessageReceived += MessageReceived;
            client.SyncRequestReceived = SyncRequestReceived;
            client.Start();
            if (client.Connected)
                Login(password);
            else
                Logging.Warn("[Client] Couldn't connect to the server!");
        }

        public async void Login(string password = "")
        {
            string username = "";
            try
            {
                username = SteamFriends.GetPersonaName();
            }
            catch (Exception ex)
            {
                Logging.Warn("[Client] Couldn't fetch Steam Username, will use username from server! => " + ex.Message);
            }
            Helpers.LoginMessage lm = new Helpers.LoginMessage(username, password);
            await client.SendAsync(lm.Meta, lm.Data);
        }

        void MessageReceived(object sender, MessageReceivedFromServerEventArgs args)
        {
            string datastr = Encoding.UTF8.GetString(args.Data);
            if (datastr == "login_response")
                LoginResponseReceived(args);
            else
                Logging.Warn("Unknown ServerMessage => " + datastr);
        }

        void LoginResponseReceived(MessageReceivedFromServerEventArgs args)
        {
            Logging.Info("[Client] Getting login response from server");
            string message = (string)args.Metadata["message"];
            if (message == "ok")
            {
                isLoggedin = true;
                Logging.Info("[Client] You're now logged in to the server!");
            }
            else if (message == "max_players")
                Logging.Warn("[Client] You can't login to the server because max player count reached");
            else if (message == "wrong_password")
                Logging.Warn("[Client] You can't login because you did enter the wrong password!");
            else
                Logging.Warn("[Client] Server says: " + message);
        }

        void ServerConnected(object sender, EventArgs args)
        {
            Logging.Info("[Client] Server connected");
        }

        void ServerDisconnected(object sender, EventArgs args)
        {
            Logging.Error("[Client] Server disconnected");
        }

        SyncResponse SyncRequestReceived(SyncRequest req)
        {
            return new SyncResponse(req, "Hello back at you!");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    client.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
