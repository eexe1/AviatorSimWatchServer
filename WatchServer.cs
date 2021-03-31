using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Timers;

namespace Simvars
{

    public class Data : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data == "BALUS"
                      ? "I've been balused already..."
                      : "I'm not available now.";

            Console.WriteLine("Message is:" + e.Data);

            Send(msg);
        }
    }

    public class WatchServer
    {

        private static Timer mockTimer;

        /// SimConnect object
        public bool isConnected = false;


        /// SimConnect object
        public WebSocketServer wssv = null;

        public WatchServer()
        {
        }

        public void StartServer()
        {
            wssv = new WebSocketServer(12345);
            wssv.AddWebSocketService<Data>("/");
            wssv.Start();
            isConnected = true;

            Console.WriteLine("Start Server");

            if (false)
            {
                // Create a timer with a two second interval.
                mockTimer = new System.Timers.Timer(2000);
                // Hook up the Elapsed event for the timer. 
                mockTimer.Elapsed += OnTimedEvent;
                mockTimer.AutoReset = true;
                mockTimer.Enabled = true;
            }
        }

        public void StopServer()
        {
            if (wssv != null)
            {
                wssv.Stop();
                isConnected = false;
                Console.WriteLine("Stop Server");
            }
        }

        public void SendMessage(Dictionary<string, dynamic> dict)
        {
            if (wssv != null && wssv.WebSocketServices["/"] != null)
            {
                var sessions = wssv.WebSocketServices["/"].Sessions;
                if (sessions.Count > 0)
                {
                    foreach (dynamic value in dict.Values)
                    {
                        Console.WriteLine(value);
                    }
                    string json = JsonConvert.SerializeObject(dict, Formatting.None);
                    sessions.Broadcast(json);
                }
            }
        }

        private int mockHsi = 0;
        private int mockGps = 0;

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Send a mock message");
            var dict = new Dictionary<string, dynamic>
                    {
                        {"key", "HSI BEARING"},
                        {"value", mockHsi}
                    };

            SendMessage(dict);

            var dict2 = new Dictionary<string, dynamic>
                    {
                        {"key", "GPS WP BEARING"},
                        {"value", mockGps}
                    };

            SendMessage(dict2);

            mockHsi += 5;
            mockGps -= 5;
        }
    }
}
