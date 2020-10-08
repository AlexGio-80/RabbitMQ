using System;

namespace RabbitMQProducerMachineStateCurrent
{
    public interface IMessage
    {
        public Guid Id { get; set; }
        public short Type { get; set; }
        public string ExchangeName { get; set; }
        public string RountingKey { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}