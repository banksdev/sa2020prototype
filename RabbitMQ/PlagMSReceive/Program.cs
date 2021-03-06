﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using PlagMS;

namespace PlagMSReceive
{
    class Program
    {
        private static string receive_channel_name = "plag_queue";
        private static string send_channel_name = "plag_queue_responses";

        static void Main(string[] args)
        {
            // WAIT A MINUT! (for rabbitmq server)
            Console.WriteLine("Waiting 20 secs for RabbitMQ");
            Thread.Sleep(20*1000);
            Console.WriteLine("Done waiting");

            var factory = new ConnectionFactory() { HostName = "rabbitmqserver" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {

                channel.QueueDeclare(queue: receive_channel_name,
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    var ms = new Service();
                    // propagate either GET or POST to MS
                    var messagePieces = message.Split(";");

                    var res = ms.IsPlag(messagePieces[1]);

                    // send answer out on "queueName" + "_responses"
                    var msg = messagePieces[0] + ";" + res;
                    var msgBody = Encoding.UTF8.GetBytes(msg);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                        routingKey: send_channel_name,
                                        basicProperties: properties,
                                        body: msgBody);
                    Console.WriteLine(" [x] Sent {0}", message);

                    // Note: it is possible to access the channel via
                    //       ((EventingBasicConsumer)sender).Model here
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(queue: receive_channel_name,
                                    autoAck: false,
                                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();

            }
            
        }
    }
}
