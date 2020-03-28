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
        public static int MAX = 1_000;

        static void Main(string[] args)
        {
            var start = DateTime.UtcNow.AddHours(1);
            Console.WriteLine($"Starting benchmark at {start}");

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
                        Guid guid = Guid.NewGuid();
                        string URI = $"http://localhost/api/file/{guid}";

                        try
                        {
                            SoCallMeMaybe(client, URI, guid);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                });
                threads.Add(t);
            }
            
            Console.WriteLine($"Starting {threads.Count} threads. Each will send {MAX} requests");
            foreach (var thread in threads)
            {
                thread.Start();
            }
            
            Console.WriteLine("Joining threads...");
            foreach (var thread in threads)
            {
                thread.Join();
            }

            var end = DateTime.UtcNow.AddHours(1);

            Console.WriteLine("Result " + _responses.Count);
            Console.WriteLine($"AVG TIME {GetMeanTime()}");

            Console.WriteLine($"Total running time: {end-start}");
            Console.WriteLine($"Finished benchmark at {end}");

        }

        public static void SoCallMeMaybe(HttpClient client, string uri, Guid guid)
        {
            var prev = DateTime.UtcNow;
            var response = client.GetAsync(new Uri(uri)).Result;
            var now = DateTime.UtcNow;
            var timeResponse = now - prev;
            if(response.StatusCode == HttpStatusCode.OK && response.Content.ReadAsStringAsync().Result == guid.ToString())
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
