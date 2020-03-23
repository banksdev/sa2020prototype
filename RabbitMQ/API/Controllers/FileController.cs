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
using RabbitMQ.Client.Events;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase, IFileController
    {
        private string queueName = "file_queue";
        private string responseQueueName = "file_queue_responses";

        // GET: api/File/75fc74cd-236e-46be-b01f-f3b149af5473
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(Guid id)
        {
            // SEND
            // var messageId = Guid.NewGuid();
            // var factory = new ConnectionFactory() { HostName = "rabbitmqserver" };
            // var connection = factory.CreateConnection();
            // var channel = connection.CreateModel();
            // using(connection)
            // using(channel)
            // {
            //     var messageWithId = messageId + ";" + "GET" + ";" + id;
                
            //     // SEND 
            //     channel.QueueDeclare(queue: queueName,
            //                     durable: true,
            //                     exclusive: false,
            //                     autoDelete: false,
            //                     arguments: null);

            //     // RECEIVE
            //     channel.QueueDeclare(queue: responseQueueName,
            //                     durable: true,
            //                     exclusive: false,
            //                     autoDelete: false,
            //                     arguments: null);

            //     channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                
            //     int maxAttempts = 3;
            //     int attempts = 0;
            //     string response = null;
            //     while(attempts < maxAttempts)
            //     {
            //         RabbitMQHelper.SendMessage(channel, queueName, messageWithId);

            //         // RECEIVE
            //         response = RabbitMQHelper.GetMessageToId(channel, responseQueueName, messageId.ToString());
            //         if(response != null){
            //             break;
            //         } else {
            //             attempts++;
            //         }

            //     }

                var rpcClient = new RpcClient();

                Console.WriteLine($" [x] Requesting file {id}");
                var response = rpcClient.Call(id.ToString());
                Console.WriteLine(" [.] Got '{0}'", response);

                rpcClient.Close();

                // var consumer = new EventingBasicConsumer(channel);
                // consumer.Received += (sender, ea) =>
                // {
                //     var body = ea.Body;
                //     var message = Encoding.UTF8.GetString(body);
                //     Console.WriteLine(" [x] Received {0}", message);

                //     var msgPieces = message.Split(";");
                //     if(msgPieces[0] == id.ToString())
                //     {
                //         // we found our message, return to caller
                //         channel.BasicAck(deliveryTag: result.DeliveryTag, multiple: false);
                //         // extract message
                //         msgPieces[1];
                //     }
                // };

            //     Console.WriteLine(response);

            //     channel.Close();
            //     connection.Close();
            //     if(response = null){
            //         return BadRequest();
            //     }

            //     return Ok(response);

            // }

            return Ok(response);

        }

        // POST: api/File
        [HttpPost]
        public IActionResult Post([FromBody] ContentWrapper filetext)
        {
            var textbytes = Encoding.ASCII.GetBytes(filetext.Content);
            var messageId = Guid.NewGuid();

            var factory = new ConnectionFactory() { HostName = "rabbitmqserver" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            using(connection)
            using(channel)
            {
                channel.QueueDeclare(queue: queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                var messageWithId = messageId + ";" + "GET" + ";" + textbytes;
                RabbitMQHelper.SendMessage(channel, queueName, messageWithId);

                channel.QueueDeclare(queue: responseQueueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                messageWithId = messageId + ";" + "GET" + ";" + textbytes;
                RabbitMQHelper.SendMessage(channel, responseQueueName, messageWithId);

                // RECEIVE
                var response = RabbitMQHelper.GetMessageToId(channel, responseQueueName, messageId.ToString());

                channel.Close();
                connection.Close();
                return Ok(response);

            }

        }

        public class ContentWrapper
        {
            public string Content { get; set; }
        }
        
    }
}
