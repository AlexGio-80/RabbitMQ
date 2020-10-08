using System;
using System.Collections.Generic;

namespace RabbitMQConsumerTest
{
    /// <summary>
    ///     Message with the last machine state.
    /// </summary>
    public class MachinesStateMessage : IMessage
    {
        public MachinesStateMessage()
        {
            Type = 6;
            TimeStamp = DateTime.Now;
            TimeStampUtc = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public short Type { get; set; }
        public string ExchangeName { get; set; }
        public string RountingKey { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime TimeStampUtc { get; set; }
        public List<MachineState> machineStates { get; set; }
    }
}