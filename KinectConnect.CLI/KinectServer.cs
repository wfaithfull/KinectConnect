using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KinectConnect.CLI
{
    public class KinectServer
    {
        private static List<TcpClient> clients = new List<TcpClient>();

        public static void Start(int port)
        {
            TcpListener server = null;
            try
            {
                server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                server.Start();

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    clients.Add(client);
                    Console.WriteLine("Accepted client " + client.Client.RemoteEndPoint.ToString());     
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                if(server != null)
                    server.Stop();
            }

        }

        public static void WriteToAll(byte[] data)
        {
            for (int i = clients.Count - 1; i >= 0; i--)
            {
                var client = clients[i];
                if(!client.Client.IsConnected())
                {
                    Console.WriteLine("Closing dead client " + client.Client.RemoteEndPoint.ToString());
                    client.Close();
                    clients.RemoveAt(i);
                    continue;
                }
                Console.WriteLine("Writing..");
                try
                {
                    var stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                    var delim = Encoding.UTF8.GetBytes("\r\n");
                    stream.Write(delim, 0, delim.Length);
                }
                catch (IOException)
                {
                    // TODO: log exception
                }
            }
        }
    }

    static class SocketExtensions
    {
        public static bool IsConnected(this Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }
    }
}
