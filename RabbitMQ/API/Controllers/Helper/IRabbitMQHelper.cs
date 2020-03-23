using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRabbitMQHelper
{
    Task<string> GetMessageToId(string queueName, string Id);
    void SendMessage(string queueName, string message);
}