using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using FileMS;
using System.Threading;

class RPCServer
{

    private static string receive_channel_name = "file_queue";
    // private static string send_channel_name = "file_queue_responses";

    public static void Main()
    {
        // WAIT A MINUT! (for rabbitmq server)
        Console.WriteLine("Waiting 15 secs for RabbitMQ");
        Thread.Sleep(15*1000);
        Console.WriteLine("Done waiting");

        var factory = new ConnectionFactory() { HostName = "rabbitmqserver" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: receive_channel_name, durable: false,
              exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: receive_channel_name,
              autoAck: false, consumer: consumer);
            Console.WriteLine(" [x] Awaiting RPC requests");

            consumer.Received += (model, ea) =>
            {
                string response = null;

                var body = ea.Body;
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [.] recieved ({0})", message);

                    var fileService = new Service();
                    // response = fib(n).ToString();
                    response = fileService.GetFile(Guid.Parse(message));
                }
                catch (Exception e)
                {
                    Console.WriteLine(" [.] " + e.Message);
                    response = "";
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                      basicProperties: replyProps, body: responseBytes);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag,
                      multiple: false);
                }
            };

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }

    /// 

    /// Assumes only valid positive integer input.
    /// Don't expect this one to work for big numbers, and it's
    /// probably the slowest recursive implementation possible.
    /// 
    private static int fib(int n)
    {
        if (n == 0 || n == 1)
        {
            return n;
        }

        return fib(n - 1) + fib(n - 2);
    }
}