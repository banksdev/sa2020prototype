﻿using System;
using System.Threading;

namespace FileMS
{
    public class Service
    {
        public string GetFile(Guid id)
        {
            Thread.Sleep(200);
            return id.ToString();
        }

        public Guid CreateFile(string json)
        {
            Thread.Sleep(1000);
            return Guid.NewGuid();
        }
    }
}
