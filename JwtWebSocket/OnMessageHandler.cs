using System;
using System.Collections.Generic;
using System.Reflection;

namespace JwtWebSocket
{
    public class OnMessageHandler
    {
        private Dictionary<string, object> events;

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
        }

        private void Handler(object arg1, EventArgs arg2) { }

        public void Handle(string json)
        {
            const string tagHead = "\"tag\":\"";
            var indexStart = json.IndexOf(tagHead, StringComparison.Ordinal) + tagHead.Length;
        }
    }
}