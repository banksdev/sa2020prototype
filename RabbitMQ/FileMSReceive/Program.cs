using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using FileMS;

namespace FileMSReceive
{
    class Program
    {

        static void Main(string[] args)
        {
            // WAIT A MINUT! (for rabbitmq server)
            Console.WriteLine("Waiting 10 secs for RabbitMQ");
            Thread.Sleep(10*1000);
            Console.WriteLine("Done waiting");

            var factory = new ConnectionFactory() { HostName = "172.100.18.2" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {

                channel.QueueDeclare(queue: "task_queue",
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

                    // extract id from message

                    var res = "";
                    var ms = new Service();
                    // propagate either GET or POST to MS
                    var messagePieces = message.Split(";");
                    if(messagePieces[1] == "GET")
                    {
                        res = ms.GetFile(Guid.Parse(messagePieces[2]));
                    } 
                    else 
                    {
                        res = ms.CreateFile(message).ToString();
                    }

                    // send answer out on "queueName" + "_responses"
                    var msg = messagePieces[0] + ";" + res;
                    var msgBody = Encoding.UTF8.GetBytes(msg);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                        routingKey: "task_queue_responses",
                                        basicProperties: properties,
                                        body: msgBody);
                    Console.WriteLine(" [x] Sent {0}", message);

                    // Note: it is possible to access the channel via
                    //       ((EventingBasicConsumer)sender).Model here
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };


                channel.BasicConsume(queue: "task_queue",
                                    autoAck: false,
                                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();

            }

            
        }
    }
}
