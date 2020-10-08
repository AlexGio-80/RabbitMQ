using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQConsumerTest
{
	public class UploadState
	{
		public UploadStates UploadStates { get; set; }
		public string Message { get; set; }
		public DateTime TimeStamp { get; set; }
		public void TimeStampNowUTC()
		{
			TimeStamp = DateTime.UtcNow;
		}
	}
}
