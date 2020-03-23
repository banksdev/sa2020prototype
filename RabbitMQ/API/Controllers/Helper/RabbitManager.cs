using System.Security.AccessControl;
using System;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

public class RabbitManager : IRabbitManager  
{  
    private readonly DefaultObjectPool<IModel> _objectPool;  
    
    public RabbitManager(IPooledObjectPolicy<IModel> objectPolicy)  
    {  
        _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);  
    }  
    
    public void Publish<T>(string queueName, T message)   
        where T : class  
    {  
        if (message == null)  
            return;  
    
        var channel = _objectPool.Get();
    
        try  
        {  
            channel.QueueDeclare(queue: queueName,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

            var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));  
            var properties = channel.CreateBasicProperties();  
            properties.Persistent = true;
            channel.BasicPublish("", queueName, properties, sendBytes);
        }  
        catch (Exception ex)  
        {  
            throw ex;  
        }  
        finally  
        {  
            _objectPool.Return(channel);                  
        }  
    }  
}  
