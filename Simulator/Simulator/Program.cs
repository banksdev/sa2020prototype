using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Simulator
{
    class Program
    {


        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var client = new HttpClient {Timeout = TimeSpan.FromSeconds(10)};
            var empty = Guid.Empty;
            var uri = $"http://localhost/file/{empty}";
            var dic = new Dictionary<bool, int> {{true, 0}, {false, 0}};
            //var i = 10_000;
            var i = 10;
            while (i --> 0)
            {
                var res = await SoCallMeMaybe(client, uri);
                var prevVal = dic.GetValueOrDefault(res);
                dic.Remove(res);
                dic.Add(res, prevVal + 1);
            }

            var resTrue = dic.GetValueOrDefault(true);
            var resFalse = dic.GetValueOrDefault(false);
            Console.WriteLine("True " + resTrue);
            Console.WriteLine("False " + resFalse);
        }

        public static async Task<bool> SoCallMeMaybe(HttpClient client,string uri)
        {
            var response = await client.GetAsync(new Uri(uri));
            return response.IsSuccessStatusCode;
        }
    }
}
