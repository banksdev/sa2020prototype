using System;
using System.Threading;

namespace FileMS
{
    public class Service
    {
        public string GetFile(Guid id)
        {
            Thread.Sleep(200);
            return "Hello JSON";
        }

        public Guid CreateFile(string json)
        {
            Thread.Sleep(1000);
            return Guid.NewGuid();
        }
    }
}
