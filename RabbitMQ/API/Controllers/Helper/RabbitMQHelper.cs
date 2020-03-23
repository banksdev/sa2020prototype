using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

public static class RabbitMQHelper
{

    public static string GetMessageToId(IModel channel, string queueName, string Id) 
    {
        var watch = new Stopwatch();
        watch.Start();
        Console.WriteLine(String.Format(" [*] Waiting for messages on channel{0}.", channel));
        
        bool noAck = false;
        bool run = true;
        BasicGetResult result = channel.BasicGet(queueName, noAck);
        while(run) 
        {
            if (watch.ElapsedMilliseconds > 1000) run = false;

            // Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds);

            if (result == null) {
                Thread.Sleep(100);
                // No message available at this time.
                // dont ack, meant for other api

            } else {
                // var response = channel.QueueDeclarePassive(queueName);
                // Console.WriteLine("Currently In Queue: " + response.MessageCount);

                IBasicProperties props = result.BasicProperties;
                byte[] body = result.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                var msgPieces = message.Split(";");
                if(msgPieces[0] == Id)
                {
                    // we found our message, return to caller
                    run = false;
                    channel.BasicAck(deliveryTag: result.DeliveryTag, multiple: false);
                    // extract message
                    return msgPieces[1];
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

}
