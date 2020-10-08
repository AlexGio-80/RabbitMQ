using System;

namespace RabbitMQTest
{
	public class MachineUploadProgramMessage : IMessage
	{
		public MachineUploadProgramMessage()
		{
			Type = 4;
			TimeStamp = DateTime.UtcNow;
		}

		public Guid MachineId { get; set; }
		public Guid Id { get; set; }
		public short Type { get; set; }
		public string ExchangeName { get; set; }
		public string RountingKey { get; set; }
		public DateTime TimeStamp { get; set; }
		public string ProgramName { get; set; }
		public string ProgramBody { get; set; }
		public string ProgramPath { get; set; }
	}
}
