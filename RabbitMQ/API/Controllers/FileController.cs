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

            var rpcClient = new RpcClient();

            Console.WriteLine($" [x] Requesting file {id}");
            var response = rpcClient.Call(id.ToString());
            Console.WriteLine(" [.] Got '{0}'", response);

            rpcClient.Close();

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
