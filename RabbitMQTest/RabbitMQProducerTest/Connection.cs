using RabbitMQ.Client;
using System;
using System.Text;
using System.Timers;

namespace RabbitMQProducerTest
{
	public class Connection
	{
		private ConnectionFactory _connectionFactory;
		IConnection _connection;
		IModel _channelUpload;
		Timer _timerSendMachineState;
		int machineUpload;

		public Connection()
		{
			machineUpload = 1;

			Connect();
		}

		private void _timerSendMachineState_Elapsed(object sender, ElapsedEventArgs e)
		{
			string message;
			string programBody;
			programBody = $"0 BEGIN PGM COMMESSA"+machineUpload.ToString()+"NOPEZZI MM\n";
			programBody += $"1  Q1700 = 0\n";
			programBody += $"2  FN 38: SEND / '#BARCODE=000YU'\n";
			programBody += $"3  LBL 'Inizio'\n";
			programBody += $"4  L X+1000 R0 FMAX M31\n";
			programBody += $"5  L X+1100 R0 FMAX\n";
			programBody += $"6  Q1700 = Q1700 + 1\n";
			programBody += $"7  CALL LBL 'Inizio' REP2\n";
			programBody += $"8  M99\n";
			programBody += $"9  Q227 = 0\n";
			programBody += $"10 M9\n";
			programBody += $"11 END PGM COMMESSA2NOPEZZI MM\n";


			MachineUploadProgramMessage mupm = new MachineUploadProgramMessage();
			mupm.Id = Guid.NewGuid();
			mupm.TimeStamp = DateTime.Now.ToUniversalTime();
			mupm.ProgramName = "O010" + machineUpload.ToString();
			mupm.ProgramBody = programBody;
			mupm.ProgramPath = "via masera di sotto";
			mupm.ExchangeName = "machinesupprogram";
			mupm.RountingKey = "MACHINE_UP_PROGRAM.PUMAM2401";
			mupm.MachineId = Guid.Parse("0044aa00-7c71-44e4-b49e-d83a11803fc6");
			mupm.Type = 004;
			message = Serializer.Serialize(mupm);

			Message msg1 = new Message();
			msg1.Payload = message;
			msg1.Type = 4;
			msg1.TimeStamp = DateTime.Now.ToUniversalTime();
			string finalMessage = Serializer.Serialize(msg1);
			byte[] msg = Encoding.UTF8.GetBytes(finalMessage);

			try
			{
				_channelUpload.BasicPublish("machinesupprogram", "MACHINE_UP_PROGRAM.PUMAM2401", mandatory: false, basicProperties: null, msg);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			machineUpload++;
		}

		private void Connect()
		{
			_connectionFactory = new ConnectionFactory();
			_connectionFactory.UserName = "guest";
			_connectionFactory.Password = "guest";
			_connectionFactory.VirtualHost = "/";
			_connectionFactory.HostName = "localhost";
			_connectionFactory.Port = 5672;

			if (_connectionFactory != null)
			{
				_connection = _connectionFactory.CreateConnection();
				if (_connection != null)
				{
					_channelUpload = _connection.CreateModel();
					_channelUpload.ExchangeDeclare("machinesupprogram", ExchangeType.Topic, durable: true);
					_channelUpload.QueueDeclare("MACHINE_UP_PROGRAM.PUMAM2401", durable: true, exclusive: false, autoDelete: false);
					_channelUpload.QueueBind("MACHINE_UP_PROGRAM.PUMAM2401", "machinesupprogram", "MACHINE_UP_PROGRAM.PUMAM2401", null);

					_timerSendMachineState = new Timer();
					_timerSendMachineState.Elapsed += _timerSendMachineState_Elapsed;
					_timerSendMachineState.Interval = 5000;
					_timerSendMachineState.Start();
				}
			}
		}

		public void Disconnect()
		{
			//_channelMachineStateProducer.QueueDelete("RabbitMQTest_MACHINE_PUMAM2401_MACHINESTATE_PRODUCER");
			_channelUpload.Close();
			_connection.Close();
		}
	}
}
