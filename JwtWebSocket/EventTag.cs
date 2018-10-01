using System;
using Newtonsoft.Json;

namespace JwtWebSocket
{
    public interface ET
    {
        void Trigger(string message);
    }

    public class EventTag<T> : ET
    {
        private string tag;

        public event EventHandler<SocketMessage<T>> Event;

        public EventTag(string tag)
        {
            this.tag = tag;
        }

        public void Trigger(string json)
        {
            SocketMessage<T> data = JsonConvert.DeserializeObject<SocketMessage<T>>(json);
            Event?.Invoke(this, data);
        }
    }
}