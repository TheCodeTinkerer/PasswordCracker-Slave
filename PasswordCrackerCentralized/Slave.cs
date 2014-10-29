using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PasswordCrackerCentralized.model;

namespace PasswordCrackerCentralized
{
    class Slave
    {
        public String MasterName { get; private set; }
        public int MasterPort { get; private set; }
        Cracking crack = new Cracking();
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="masterName"></param>
        /// <param name="masterPort"></param>
        public Slave(string masterName, int masterPort)
        {
            MasterName = masterName;
            MasterPort = masterPort;
        }
        /// <summary>
        /// Runs the server, gets a request and saves it into List<string>words, which as long as the list is not empty, it should pass the list
        /// into the RunCracking method in the Cracking class and process it, if the list is empty the loop will break and the process is 
        /// completed
        /// </summary>
        public void Run()
        {
            while (true)
            {
                List<String> words = SendGetRequest();
                if (words.Count != 0)
                {
                    var list = crack.RunCracking(words);
                    Console.WriteLine(list);
                    SendFoundRequest(null, list);
                }
                if (words.Count == 0)
                {
                    break;
                }
            }
            Console.WriteLine("Slave is done");
        }
        /// <summary>
        /// Sends the GET request to the server, to retrive the part of the dictionary it needs to perform the cracking process, also used to
        /// shutdown the connection it it retrieves the command from the server.
        /// </summary>
        /// <returns></returns>
        private List<String> SendGetRequest()
        {
            using (TcpClient connection = new TcpClient(MasterName, MasterPort))
            {
                List<String> words = new List<string>();
                try
                {
                    StreamWriter toServer = new StreamWriter(connection.GetStream());
                    toServer.WriteLine("GET");
                    toServer.Flush();
                    Console.WriteLine("Slave GET sent");
                    StreamReader fromServer = new StreamReader(connection.GetStream());
                    if (fromServer.ReadLine() == "Shutdown Sockets")
                    {
                        connection.Close();
                    }
                    while (!fromServer.EndOfStream)
                    {
                        String word = fromServer.ReadLine();
                        Console.WriteLine("Slave read " + word);
                        words.Add(word);
                    }
                    Console.WriteLine(String.Join(", ", words));
                }
                catch (Exception)
                {
                    Console.WriteLine("Server might be down or cracking procedure might already be completed, program will now exit.");
                }
                return words;
            }
        }
        /// <summary>
        /// Sends message to the server about found username and passwords
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void SendFoundRequest(String username, string password)
        {
            using (TcpClient connection = new TcpClient(MasterName, MasterPort))
            {
                StreamWriter toServer = new StreamWriter(connection.GetStream());
                toServer.WriteLine("FOUND");
                toServer.WriteLine(username);
                toServer.WriteLine(password);
                toServer.Flush();
            }
        }
    }
    }

