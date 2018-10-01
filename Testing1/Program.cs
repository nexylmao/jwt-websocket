using System;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using JwtWebSocket;
using Newtonsoft.Json;

namespace Testing1
{
    class Status
    {
        public string databaseState, databaseNotice, serverName, serverVersion, onlineNotice, gameServerNotice;
        public bool onlineState, gameServerState;
    }

    class LoginData
    {
        public string identification, password;

        public LoginData(string identification, string password)
        {
            this.identification = identification;
            this.password = password;
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.Clear();
            Connection loginServer = new Connection(@"path", @"secret");

            OnMessageHandler loginHandler = new OnMessageHandler((sender, message) =>
            {
                Console.WriteLine();
                Console.WriteLine("TAG : " + message.tag);
                Console.WriteLine("Message : " + message.message);
                Console.WriteLine("Data : " + JsonConvert.SerializeObject(message.data));
            });

            EventTag<Status> handler = (EventTag<Status>) loginHandler.SignTag("status", typeof(Status));
            handler.Event += (sender, message) =>
            {
                Console.WriteLine();
                Console.WriteLine("This is a 'status' message");
                Console.WriteLine("TAG : " + message.tag);
                Console.WriteLine("Message : " + message.message);
                Console.WriteLine("Data : " + JsonConvert.SerializeObject(message.data));
            };

            loginServer.MessageHandler = loginHandler;

            loginServer.Start();
            loginServer.Send(new SocketMessage<string>("whoami", "", ""), true);

            System.Threading.Thread.Sleep(2000);

            loginServer.Send(new SocketMessage<LoginData>("login", "", new LoginData(@"username", @"password")), true);

            Console.ReadKey(true);
        }
    }
}