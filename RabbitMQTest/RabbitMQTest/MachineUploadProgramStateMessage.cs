using System;

namespace RabbitMQTest
{
    /// <summary>
    ///     Message with the last machine state.
    /// </summary>
    public class MachineUploadProgramStateMessage : IMessage
    {
        public MachineUploadProgramStateMessage()
        {
            Type = 5;
            TimeStamp = DateTime.UtcNow;
            ;
        }

        public Guid MachineId { get; set; }
        public MachineState UploadeState { get; set; }
        public Guid Id { get; set; }
        public short Type { get; set; }
        public string ExchangeName { get; set; }
        public string RountingKey { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}