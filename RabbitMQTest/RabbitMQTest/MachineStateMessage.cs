using System;

namespace RabbitMQTest
{
    /// <summary>
    ///     Message with the last machine state.
    /// </summary>
    public class MachineStateMessage : IMessage
    {
        public MachineStateMessage()
        {
            Type = 3;
            TimeStamp = DateTime.UtcNow;
            ;
        }

        public Guid MachineId { get; set; }
        public MachineState MachineState { get; set; }
        public Guid Id { get; set; }
        public short Type { get; set; }
        public string ExchangeName { get; set; }
        public string RountingKey { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}