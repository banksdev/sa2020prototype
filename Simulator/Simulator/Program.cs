using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Simulator
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var benchmark_running_time_watch = new Stopwatch();
            benchmark_running_time_watch.Start();

            Console.WriteLine("Hello World!");
            var client = new HttpClient {Timeout = TimeSpan.FromSeconds(10)};
            var empty = Guid.Empty;
            var uri = $"http://127.0.0.1/api/file/{empty}";
            var dic = new Dictionary<bool, int> {{true, 0}, {false, 0}};
            var i = 10_000;
            //var i = 10;
            var watch = new Stopwatch();
            while (i --> 0)
            {   
                watch.Restart();
                if(i % 100 == 0) Console.WriteLine(i);

                try
                {
                    var res = await SoCallMeMaybe(client, uri);
                    var prevVal = dic.GetValueOrDefault(res);
                    dic.Remove(res);
                    dic.Add(res, prevVal + 1);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    var prevVal = dic.GetValueOrDefault(false);
                    dic.Remove(false);
                    dic.Add(false, prevVal + 1);
                }
                watch.Stop();

                Console.WriteLine("recieved response in " + watch.ElapsedMilliseconds + " ms");
            }

            var resTrue = dic.GetValueOrDefault(true);
            var resFalse = dic.GetValueOrDefault(false);
            Console.WriteLine("True " + resTrue);
            Console.WriteLine("False " + resFalse);

            Console.WriteLine($"Finished benchmark in {benchmark_running_time_watch.ElapsedMilliseconds / 1000} seconds");
            
        }

        public static async Task<bool> SoCallMeMaybe(HttpClient client, string uri)
        {
            var response = await client.GetAsync(new Uri(uri));
            return response.IsSuccessStatusCode;
        }
    }
}
