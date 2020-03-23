using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase, IFileController
    {
        private readonly ILogger _logger;
        private IRabbitManager _manager;  
        private string queueName = "file_queue";
        private string responseQueueName = "file_queue_responses";

        public FileController(ILogger<FileController> logger, IRabbitManager manager)
        {
            _logger = logger;
            _manager = manager;
        }

        // GET: api/File/75fc74cd-236e-46be-b01f-f3b149af5473
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(Guid id)
        {
            logInfo(String.Format("Download file with ID: {0}", id));
            var num = new System.Random().Next(9000);
            _manager.Publish("myqueue", new 
            {  
                field1 = $"Hello-{num}",
                field2 = $"rabbit-{num}"
            });
  
            return Ok("response");
            // // SEND
            // var messageId = Guid.NewGuid();
            // var factory = new ConnectionFactory() { HostName = "rabbitmqserver" };
            // using(var connection = factory.CreateConnection())
            // using(var channel = connection.CreateModel())
            // {
            //     channel.QueueDeclare(queue: queueName,
            //                     durable: true,
            //                     exclusive: false,
            //                     autoDelete: false,
            //                     arguments: null);

            //     // SEND 
            //     var messageWithId = messageId + ";" + "GET" + ";" + id;
            //     RabbitMQHelper.SendMessage(channel, queueName, messageWithId);

            //     // RECEIVE
            //     channel.QueueDeclare(queue: responseQueueName,
            //                     durable: true,
            //                     exclusive: false,
            //                     autoDelete: false,
            //                     arguments: null);

            //     // RECEIVE
            //     var response = RabbitMQHelper.GetMessageToId(channel, responseQueueName, messageId.ToString());

            // return Ok(response);

        }

        // POST: api/File
        [HttpPost]
        public IActionResult Post([FromBody] ContentWrapper filetext)
        {
            var queueName = "task_queue";
            var textbytes = Encoding.ASCII.GetBytes(filetext.Content);
            var messageId = Guid.NewGuid();

            var factory = new ConnectionFactory() { HostName = "rabbitmqserver" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                var messageWithId = messageId + ";" + "GET" + ";" + textbytes;
                RabbitMQHelper.SendMessage(channel, queueName, messageWithId);

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
                RabbitMQHelper.SendMessage(channel, responseQueueName, messageWithId);

                // RECEIVE
                var response = RabbitMQHelper.GetMessageToId(channel, responseQueueName, messageId.ToString());

                return Ok(response);

            }

        }

        private void logInfo(String message)
        {
            _logger.LogInformation("[INFO ][FileController][{thread}]: {message}", Thread.CurrentThread.ManagedThreadId, message);
        }

        private void logError(String message)
        {
            _logger.LogError("[ERROR][FileController][{thread}]: {message}", Thread.CurrentThread.ManagedThreadId, message);
        }

        public class ContentWrapper
        {
            public string Content { get; set; }
        }
        
    }
}
