using System;
using JWT;
using JWT.Algorithms;
using Newtonsoft.Json;
using WebSocketSharp;

namespace JwtWebSocket
{
    public class Connection
    {
        private string path;
        private string secret;
        private WebSocket client; // = new WebSocket(*path*);

        private IJwtAlgorithm algorithm;
        private IJsonSerializer serializer; // to be JwtWebSocket.JsonParser
        private IBase64UrlEncoder urlEncoder;
        private IJwtEncoder encoder; // = new JwtEncoder(algorithm, serializer, urlEncoder);

        private IDateTimeProvider provider;
        private IJwtValidator validator; // = new JwtValidator(serializer, provider);
        private IJwtDecoder decoder; // = new JwtDecoder(serializer, validator, urlEncoder);

        private OnMessageHandler messageHandler;

        public OnMessageHandler MessageHandler
        {
            get { return messageHandler; }
            set { messageHandler = value; }
        }

        public Connection(string path, string secret)
        {
            this.path = path;
            this.secret = secret;
            algorithm = new HMACSHA256Algorithm();
            serializer = new JsonParser();
            urlEncoder = new JwtBase64UrlEncoder();
            encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            provider = new UtcDateTimeProvider();
            validator = new JwtValidator(serializer, provider);
            decoder = new JwtDecoder(serializer, validator, urlEncoder);
            client = new WebSocket(path);

            client.OnMessage += (sender, args) =>
            {
                // 1. part -> args to json if token
                string message = args.Data;
                try
                {
                    if (message.IndexOf("ey") == 0 && message.Split('.').Length == 3)
                    {
                        message = Verify(message);    
                    }
                }
                catch (TokenExpiredException)
                {
                    throw new Exception(@"Token has expired");
                }
                catch (SignatureVerificationException)
                {
                    throw new Exception(@"Token can't be verified");
                }
                // 2. part -> handle json
                if (messageHandler == null)
                {
                    throw new Exception(@"No handler is defined");
                }
                messageHandler.Handle(message);
            };
        }

        public void SubscribeOnOpen(EventHandler onOpenEvent)
        {
            client.OnOpen += onOpenEvent;
        }

        public void UnsubscribeOnOpen(EventHandler onOpenEvent)
        {
            client.OnOpen -= onOpenEvent;
        }

        public void SubscribeOnMessage(EventHandler<MessageEventArgs> onMessageEvent)
        {
            client.OnMessage += onMessageEvent;
        }

        public void UnsubscribeOnMessage(EventHandler<MessageEventArgs> onMessageEvent)
        {
            client.OnMessage -= onMessageEvent;
        }
        
        public void SubscribeOnEvent(EventHandler<ErrorEventArgs> onErrorEvent)
        {
            client.OnError += onErrorEvent;
        }

        public void UnsubscribeOnEvent(EventHandler<ErrorEventArgs> onErrorEvent)
        {
            client.OnError -= onErrorEvent;
        }

        public void SubscribeOnClose(EventHandler<CloseEventArgs> onCloseEvent)
        {
            client.OnClose += onCloseEvent;
        }

        public void UnsubscribeOnClose(EventHandler<CloseEventArgs> onCloseEvent)
        {
            client.OnClose -= onCloseEvent;
        }
        
        public void Send(EventArgs data, bool jwt)
        {
            if (client.ReadyState != WebSocketState.Open)
            {
                throw new Exception(@"The connection is not open");
            }
            string json;
            if (jwt)
            {
                json = Sign(data);
            }
            else
            {
                json = JsonConvert.SerializeObject(data);
            }
            client.Send(json);
        }
        
        public void Start()
        {
            client.Connect();
        }

        public void Close()
        {
            client.Close();
        }
        
        private string Sign(object json)
        {
            return encoder.Encode(json, secret);
        }

        private string Verify(string token)
        {
            return decoder.Decode(token, secret, true);
        }
    }
}