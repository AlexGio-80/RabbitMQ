using System;

namespace RabbitMQDownloadProgramProducerConsumer
{
	/// <summary>
	///     Message with the machine program to upload.
	/// </summary>
	public class MachinePartProgramMessage : IMessage
	{
		public MachinePartProgramMessage()
		{
			Type = 4;
			TimeStamp = DateTime.Now;
			TimeStampUtc = DateTime.UtcNow;
		}

		public Guid MachineId { get; set; }
		public Guid Id { get; set; }
		public short Type { get; set; }
		public string ExchangeName { get; set; }
		public string RountingKey { get; set; }
		public DateTime TimeStamp { get; set; }
		public DateTime TimeStampUtc { get; set; }
		public string ProgramName { get; set; }
		public string ProgramBody { get; set; }
		public string ProgramPath { get; set; }
	}
}
