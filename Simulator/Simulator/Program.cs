using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net;

namespace Simulator
{
    class Program
    {
        public static ConcurrentBag<PrototypeResponse> _responses;
        public static Guid empty = Guid.Empty;
        public static string URI = $"http://localhost/api/file/{empty}";
        // public static string URI = $"https://www.google.com/";
        public static int MAX = 100;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            _responses = new ConcurrentBag<PrototypeResponse>();
            var threads = new List<Thread>();
            for (var i = 0; i < 4; i++)
            {
                var t = new Thread(() =>
                {
                    var j = MAX;
                    var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                    while (j-- > 0)
                    {
                        try
                        {
                            SoCallMeMaybe(client, URI);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                });
                threads.Add(t);
            }
            
            Console.WriteLine("Starting");
            foreach (var thread in threads)
            {
                thread.Start();
            }
            
            Console.WriteLine("Joining");
            foreach (var thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine("Result " + _responses.Count);
            Console.WriteLine($"AVG TIME {GetMeanTime()}");
        }

        public static void SoCallMeMaybe(HttpClient client,string uri)
        {
            var prev= DateTime.UtcNow;
            var response = client.GetAsync(new Uri(uri)).Result;
            var now = DateTime.UtcNow;
            var timeResponse = now - prev;
            // Console.WriteLine(response.StatusCode);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                var resp = new PrototypeResponse()
                {
                    ResponseCode = response.StatusCode,
                    TimeSpan = timeResponse
                };
                _responses.Add(resp);
            }
        }

        public static string GetMeanTime()
        {
            var r = _responses.Select(x => x.TimeSpan);
            TimeSpan totalSpan = new TimeSpan();
            foreach (var timeSpan in r)
            {
                totalSpan += timeSpan;
            }

            var res = totalSpan / _responses.Count;
            return res.ToString();
        }

      
    }
}
