using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace JwtWebSocket
{
    public class OnMessageHandler
    {
        private Dictionary<string, object> events;
        private EventHandler<SocketMessage<object>> defaultTag;
        
        public OnMessageHandler()
        {
            events = new Dictionary<string, object>();
            defaultTag = Handler;
        }

        public OnMessageHandler(EventHandler<SocketMessage<object>> defaultTag)
        {
            events = new Dictionary<string, object>();
            this.defaultTag = defaultTag;
        }

        public void SignTag(string tag, Type type)
        {
            if (events.ContainsKey(tag))
            {
                throw new Exception("Tag already exists");
            }
            var sm = typeof(SocketMessage<>).MakeGenericType(type);
            var eh = typeof(EventHandler<>).MakeGenericType(sm);
            ConstructorInfo ctor = eh.GetConstructors()[0];
            Action<object, EventArgs> handler = Handler;
            var eventHandler = ctor.Invoke(new object[]{handler});
            events.Add(tag, eventHandler);
        }

        public object GetHandler(string tag)
        {
            if (!events.ContainsKey(tag))
            {
                throw new Exception("Tag doesn\'t exist");
            }
            return events[tag];
        }
        
        private static void Handler(object arg1, EventArgs arg2) { }

        public void Handle(string json)
        {
            const string tagHead = "\"tag\":\"";
            var indexStart = json.IndexOf(tagHead, StringComparison.Ordinal) + tagHead.Length;
            var indexEnd = json.IndexOf('"', indexStart);
            var tag = json.Substring(indexStart, indexEnd - indexStart);

            if (!events.ContainsKey(tag))
            {
                defaultTag?.Invoke(this, JsonConvert.DeserializeObject<SocketMessage<object>>(json));
                return;
            }
            dynamic eventHandler = events[tag];
            eventHandler(this, JsonConvert.DeserializeObject(json));
        }
    }
}