using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace JwtWebSocket
{
    public class OnMessageHandler
    {
        private Dictionary<string, ET> events;
        private event EventHandler<SocketMessage<object>> defaultTag;

        public OnMessageHandler()
        {
            events = new Dictionary<string, ET>();
            defaultTag = Handler;
        }

        public OnMessageHandler(EventHandler<SocketMessage<object>> defaultTag)
        {
            events = new Dictionary<string, ET>();
            this.defaultTag = defaultTag;
        }

        public object SignTag(string tag, Type type)
        {
            if (events.ContainsKey(tag))
            {
                throw new Exception("Tag already exists");
            }
            var e = typeof(EventTag<>).MakeGenericType(type);
            ConstructorInfo ctor = e.GetConstructors()[0];
            var eventHandler = ctor.Invoke(new object[] {tag});
            events.Add(tag, (ET) eventHandler);
            return eventHandler;
        }

        public ET GetHandler(string tag)
        {
            if (!events.ContainsKey(tag))
            {
                throw new Exception("Tag doesn\'t exist");
            }
            return events[tag];
        }

        private static void Handler(object arg1, EventArgs arg2)
        {
        }

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
            ET e = events[tag];
            e.Trigger(json);
        }
    }
}