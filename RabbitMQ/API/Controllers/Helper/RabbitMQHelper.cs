using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class RabbitMQHelper
{

    public static string GetMessageToId(IModel channel, string queueName, string Id) 
    {
        Console.WriteLine(String.Format(" [*] Waiting for messages on channel{0}.", channel));
        
        bool noAck = false;
        bool run = true;
        BasicGetResult result = channel.BasicGet(queueName, noAck);
        while(run) 
        {
            Thread.Sleep(100);

            if (result == null) {
                
                // No message available at this time.
                // dont ack, meant for other api
            } else {
                IBasicProperties props = result.BasicProperties;
                byte[] body = result.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                if(message.Contains(Id))
                {
                    // we found our message, return to caller
                    run = false;
                    channel.BasicAck(deliveryTag: result.DeliveryTag, multiple: false);
                    // extract message
                    return message.Split(";")[1];
                }

                
            }

            result = channel.BasicGet(queueName, noAck);

        }
        
        return null; // should not happen, just for compiler
    
    }

    public static void SendMessage(IModel channel, string queueName, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish(exchange: "",
                            routingKey: queueName,
                            basicProperties: properties,
                            body: body);
        Console.WriteLine(" [x] Sent {0}", message);
    }

    public static (IModel, IModel) ConnectToQueue(string queueName)
    {
        var channels = new IModel[2];
        channels[0] = CreateQueue(queueName);
        channels[1] = CreateQueue(queueName + "responses");
        return (channels[0], channels[1]);
    }

    public static IModel CreateQueue(string queueName)
    {
        var factory = new ConnectionFactory() { HostName = "172.100.18.2" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            return channel;
        }
    }

}