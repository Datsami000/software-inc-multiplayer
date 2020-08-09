﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Multiplayer.Debugging;
using WatsonTcp;

namespace Multiplayer.Networking
{

    public static class ServerClass
    {
        private static ushort Port = 52512;
        public static ushort MaxPlayers = 0;
        private static string Password = "";
        static WatsonTcpServer server;

        public static void Start(ushort port = 52512)
        {
            Port = port;
            server = new WatsonTcpServer(null, Port);
            server.ClientConnected += ClientConnected;
            server.ClientDisconnected += ClientDisconnected;
            server.MessageReceived += MessageReceived;
            server.SyncRequestReceived = SyncRequestReceived;
            server.Start();
            Logging.Info($"[Server] Server started.");
        }
        public static void Stop()
        {
            server.Dispose();
            Logging.Info($"[Server] Server disposed.");
        }

        static void ClientConnected(object sender, ClientConnectedEventArgs args)
        {
            Logging.Info("[Server] Client connected: " + args.IpPort);
        }

        static void ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
        {
            Logging.Info("[Server] Client disconnected: " + args.IpPort + ": " + args.Reason.ToString());
        }

        static void MessageReceived(object sender, MessageReceivedFromClientEventArgs args)
        {
            Logging.Info("[Server] Message received from " + args.IpPort + ": " + Encoding.UTF8.GetString(args.Data));
        }

        static SyncResponse SyncRequestReceived(SyncRequest req)
        {
            return new SyncResponse(req, "Hello back at you!");
        }
    }
}