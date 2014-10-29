using System;
using System.Net;
using System.Net.Sockets;

namespace PasswordCrackerCentralized
{
    class Program
    {
        private static readonly Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main()
        {
            Console.Title = "Client";
            //ConnectToServer();
            Slave slave = new Slave("10.154.1.141", 8080);
            slave.Run();
        }

        private static void ConnectToServer()
        {
            int attempts = 0;
            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);
                    ClientSocket.Connect(IPAddress.Loopback, 1234);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }
            Console.Clear();
            Console.WriteLine("Connected");
        }
    }
}
