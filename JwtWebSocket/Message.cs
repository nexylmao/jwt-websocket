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
        public string tag, message;
        public T data;

        public SocketMessage(string tag, string message, T data)
        {
            this.tag = tag;
            this.message = message;
            this.data = data;
        }
    }
}