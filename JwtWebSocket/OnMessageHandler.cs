using System;
using System.Collections.Generic;

namespace JwtWebSocket
{
    public class OnMessageHandler
    {
        private Dictionary<string, Type> types;
        
        private Dictionary<string, EventHandler> events;

        public void SignTag(string tag, Type dataType)
        {
            
        }
        
        public void Handle(string json)
        {
            int indexStart = json.IndexOf("\"tag\":\"");
        }
    }
}