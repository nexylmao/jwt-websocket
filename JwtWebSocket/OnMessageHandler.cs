using System;
using System.Collections.Generic;

namespace JwtWebSocket
{
    public class OnMessageHandler
    {
        private Dictionary<string, EventHandler> events;

        public void Handle(string json)
        {
            int indexStart = json.IndexOf("\"tag\":\"");
        }
    }
}