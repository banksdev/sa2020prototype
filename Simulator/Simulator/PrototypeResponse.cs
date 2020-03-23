using System;
using System.Net;

namespace Simulator
{
    public class PrototypeResponse
    {
        public HttpStatusCode ResponseCode { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}