using System;
using System.Threading;

namespace FileMS
{
    public class Service
    {
        public string GetFile(Guid id)
        {
            Thread.Sleep(new Random().Next(200, 1000));
            return "Hello JSON";
        }

        public Guid CreateFile(string json)
        {
            Thread.Sleep(new Random().Next(200, 1000));
            return Guid.NewGuid();
        }
    }
}
