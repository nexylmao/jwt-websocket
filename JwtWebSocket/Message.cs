using System;

namespace JwtWebSocket
{
    /// <summary>
    /// This is the payload class. In this format, the messages received from server should be in this format.
    /// The messages you send, should also be sent in this format.
    /// </summary>
    /// <typeparam name="T">This is the type of the data field of the message.</typeparam>
    public class SocketMessage<T> : EventArgs
    {
        private string tag, message;
        private T data;

        public SocketMessage(string tag, string message, T data)
        {
            this.tag = tag;
            this.message = message;
            this.data = data;
        }

        public string Tag
        {
            get => tag;
            set => tag = value;
        }

        public string Message
        {
            get => message;
            set => message = value;
        }

        public T Data
        {
            get => data;
            set => data = value;
        }
    }
}