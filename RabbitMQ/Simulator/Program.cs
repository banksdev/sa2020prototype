using System;
using RabbitMQ.Client;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
namespace Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            // WAIT A MINUT! (for rabbitmq server)
            Thread.Sleep(60*1000);
            
            var factory = new ConnectionFactory() { HostName = "172.100.18.2" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var msgList = new List<String>(){
                    "First message.",
                    "Second message..",
                    "Third message...",
                    "Fourth message....",
                    "Fifth message....."
                };

                foreach(var msg in msgList){
                    // var message = GetMessage(args);
                    var message = msg;
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                        routingKey: "task_queue",
                                        basicProperties: properties,
                                        body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }

            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }
    }
}
