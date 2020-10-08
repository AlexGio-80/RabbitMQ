using System;

namespace RabbitMQTest
{
    /// <summary>
    ///     Message sended by machines to the node containing the information that they are healty.
    /// </summary>
    public class MachineAliveMessage : IMessage
    {
        public MachineAliveMessage()
        {
            Type = 0;
            TimeStamp = DateTime.UtcNow;
            ;
        }

        public Guid MachineId { get; set; }
        public Guid Id { get; set; }
        public short Type { get; set; }
        public string ExchangeName { get; set; }
        public string RountingKey { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}