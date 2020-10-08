using System;

namespace RabbitMQConsumerTest
{
    /// <summary>
    ///     State 0: Disconnected
    ///     State 1: Connected
    ///     State 2: Stop
    ///     State 3: Work
    ///     State 4: Emergency
    ///     State 5: DriverError
    /// </summary>
    public class MachineState
    {
        public string Code { get; set; }
        public Guid MachineId { get; set; }
        public MachineStates State { get; set; }
        public int PiecesCounter { get; set; }
        public UploadStates uploadStates { get; set; }
        public string uploadMessage { get; set; }
        public string programComment { get; set; }

        public DateTime TimeStamp { get; set; }

        public void TimeStampNowUTC()
        {
            TimeStamp = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return "(" + TimeStamp.ToString("HH:mm:ss") + ") State:" + State + " Pieces:" + PiecesCounter;
        }

		public static MachineState Create(Guid machineId, MachineStates state, int piecesCounter, DateTime timeStamp, string programComment)
		{
			var machineState = new MachineState();
            machineState.MachineId = machineId;
            machineState.State = state;
            machineState.PiecesCounter = piecesCounter;
            machineState.TimeStamp = timeStamp;
            machineState.programComment = programComment;

            return machineState;
		}
	}

    public static class MachineStateExtensions
	{
        public static void Update(this MachineState state)
		{

		}
	}
}