using System;
using System.Threading;
using Microsoft.Extensions.ObjectPool;  
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;  

public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>  
{  
    // private readonly ILogger _logger;

    private readonly RabbitOptions _options;  

    private readonly IConnection _connection;  
    
    public RabbitModelPooledObjectPolicy(IOptions<RabbitOptions> optionsAccs)  
    {  
        _options = optionsAccs.Value;
        _connection = GetConnection();  
        // _logger = logger;
    }  
    
    private IConnection GetConnection()  
    {  
        // logInfo(String.Format("HostName: {0}", _options.HostName));
        // logInfo(String.Format("UserName: {0}",_options.UserName));
        // logInfo(String.Format("Password: {0}",_options.Password));
        // logInfo(String.Format("Port: {0}", _options.Port));
        Console.WriteLine(String.Format("HostName: {0}", _options.HostName));
        Console.WriteLine(String.Format("UserName: {0}",_options.UserName));
        Console.WriteLine(String.Format("Password: {0}",_options.Password));
        Console.WriteLine(String.Format("Port: {0}", _options.Port));
        var factory = new ConnectionFactory()  
        {  
            HostName = _options.HostName,  
            UserName = _options.UserName,  
            Password = _options.Password,  
            Port = _options.Port,  
            VirtualHost = _options.VHost,  
        };  
    
        return factory.CreateConnection();  
    }  
    
    public IModel Create()  
    {  
        return _connection.CreateModel();  
    }  
    
    public bool Return(IModel obj)  
    {  
        if (obj.IsOpen)  
        {  
            return true;  
        }  
        else  
        {  
            obj?.Dispose();  
            return false;  
        }  
    }  

    // private void logInfo(String message)
    // {
    //     _logger.LogInformation("[INFO ][RabbitModelPooledObjectPolicy][{thread}]: {message}", Thread.CurrentThread.ManagedThreadId, message);
    // }

    // private void logError(String message)
    // {
    //     _logger.LogError("[ERROR][RabbitModelPooledObjectPolicy][{thread}]: {message}", Thread.CurrentThread.ManagedThreadId, message);
    // }
}  
