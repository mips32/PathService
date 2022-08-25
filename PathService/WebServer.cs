using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PathService
{
    class Client
    {
        public Client(TcpClient client)
        {
            string request = String.Empty;
            byte[] buffer = new byte[1024];
            int count;

            while ((count = client.GetStream().Read(buffer, 0, buffer.Length)) > 0)
            {
                request += Encoding.ASCII.GetString(buffer, 0, count);
                if (request.IndexOf("\r\n\r\n", StringComparison.Ordinal) >= 0 && request.IndexOf("}", StringComparison.Ordinal) == -1)
                {
                    count = client.GetStream().Read(buffer, 0, buffer.Length);
                    request += Encoding.ASCII.GetString(buffer, 0, count);
                }

                break;
            }

            string[] parsedRequest = request.Split(new[]{"\r\n\r\n"}, StringSplitOptions.None);
            JObject parms = ((JObject)JsonConvert.DeserializeObject(parsedRequest[1]));

            string from = (string)parms["from"];
            string to = (string)parms["to"];

            PathFinder pf = new PathFinder(from, to, 100);
            List<string> path = pf.GetPath();

            string content = JsonConvert.SerializeObject(path);
            byte[] contentBuffer = Encoding.ASCII.GetBytes(content);

            string headers = "HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nConnection: close\r\nContent-Length: " + contentBuffer.Length + "\r\n\r\n";
            byte[] headersBuffer = Encoding.ASCII.GetBytes(headers);
            client.GetStream().Write(headersBuffer, 0, headersBuffer.Length);

            client.GetStream().Write(contentBuffer, 0, contentBuffer.Length);

            client.Close();
        }
    }

    class Server
    {
        TcpListener Listener;

        public Server(int port)
        {
            Listener = new TcpListener(IPAddress.Loopback, port);
            Listener.Start();
   
            while (true)
            {
                TcpClient client = Listener.AcceptTcpClient();
                Thread thread = new Thread(ClientThread);
                thread.Start(client);
            }
        }
   
        static void ClientThread(Object stateInfo)
        {
            new Client((TcpClient) stateInfo);
        }

        ~Server()
        {
            if (Listener != null)
            {
                Listener.Stop();
            }
        }

    }
}
