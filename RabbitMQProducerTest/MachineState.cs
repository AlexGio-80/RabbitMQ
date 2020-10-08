using System;

namespace RabbitMQProducerTest
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
        public MachineStates State { get; set; }
        public int PiecesCounter { get; set; }
        public UploadStates uploadStates { get; set; }
        public string uploadMessage { get; set; }

        public DateTime TimeStamp { get; set; }

        public void TimeStampNowUTC()
        {
            TimeStamp = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return "(" + TimeStamp.ToString("HH:mm:ss") + ") State:" + State + " Pieces:" + PiecesCounter;
        }
    }
}