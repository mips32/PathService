using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PathService
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Server started");

            new Task(() => { new Server(8888); }).Start();

            Console.WriteLine("Press ENTER");
            Console.ReadLine();

            WebClient wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            string json_data = "{\"from\": \"e2\", \"to\": \"h8\"}";
            Console.WriteLine("{0}\n", json_data);

            string response = wc.UploadString("http://127.0.0.1:8888/", json_data);

            List<string> answer = ((JArray)JsonConvert.DeserializeObject(response)).ToObject<List<string>>();

            for (var i = 0; i < answer.Count; i++)
            {
                var t = answer[i];
                Console.WriteLine("{0}", t);
            }

            Console.WriteLine("\nPress ENTER to exit");
            Console.ReadLine();
        }
    }
}
