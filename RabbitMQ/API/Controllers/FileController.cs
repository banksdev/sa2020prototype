using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase, IFileController
    {

        // GET: api/File/75fc74cd-236e-46be-b01f-f3b149af5473
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(Guid id)
        {
            var queueName = "task_queue";
            var messageId = Guid.NewGuid();
            var factory = new ConnectionFactory() { HostName = "172.100.18.2" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);


                // SEND 
                var messageWithId = messageId + ";" + "GET" + ";" + id;
                SendMessage(channel, queueName, messageWithId);

            }

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                var responseQueueName = "task_queue_responses";
                channel.QueueDeclare(queue: responseQueueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                // RECEIVE
                var response = GetMessageToId(channel, "task_queue_responses", messageId.ToString());

                return Ok(response);
            }
        

        }

        // POST: api/File
        [HttpPost]
        public IActionResult Post([FromBody] ContentWrapper filetext)
        {
            var queueName = "task_queue";
            var textbytes = Encoding.ASCII.GetBytes(filetext.Content);
            var messageId = Guid.NewGuid();

            var factory = new ConnectionFactory() { HostName = "172.100.18.2" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                

                var messageWithId = messageId + ";" + "GET" + ";" + textbytes;
                SendMessage(channel, queueName, messageWithId);

            }

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                var responseQueueName = "task_queue_responses";
                channel.QueueDeclare(queue: responseQueueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                var messageWithId = messageId + ";" + "GET" + ";" + textbytes;
                SendMessage(channel, responseQueueName, messageWithId);

                // RECEIVE
                var response = GetMessageToId(channel, responseQueueName, messageId.ToString());

                return Ok(response);
            }

        }

        public string GetMessageToId(IModel channel, string queueName, string Id) 
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
                        
                        // extract message
                        return message.Split(";")[1];
                    }

                    channel.BasicAck(deliveryTag: result.DeliveryTag, multiple: false);
                }

                result = channel.BasicGet(queueName, noAck);

            }
            
            return null; // should not happen, just for compiler
        
        }

        public void SendMessage(IModel channel, string queueName, string message)
        {
            // var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                                routingKey: queueName,
                                basicProperties: properties,
                                body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }

        public (IModel, IModel) ConnectToQueue(string queueName)
        {
            var channels = new IModel[2];
            channels[0] = CreateQueue(queueName);
            channels[1] = CreateQueue(queueName + "responses");
            return (channels[0], channels[1]);

        }

        public IModel CreateQueue(string queueName)
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

        public class ContentWrapper
        {
            public string Content { get; set; }
        }
    }
}
