
public interface IRabbitManager  
{  
    void Publish<T>(string queueName, T message)   
        where T : class;  
}  
