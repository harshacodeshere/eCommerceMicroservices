namespace BusinessLogicLayer.RabbitMQ
{
    public interface IRabbitMQProductNameUpdateConsumer
    {
        void Dispose();
        void Consume();
    }
}