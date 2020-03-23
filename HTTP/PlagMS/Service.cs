using System;
using System.Threading;

namespace PlagMS
{
    public class Service
    {
        public bool IsPlag(string text)
        {
            Thread.Sleep(new Random().Next(200, 1000));
            return false;
        }
    }
}
