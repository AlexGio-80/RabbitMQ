using RabbitMQ.Client;
using System;
using System.Text;
using System.Timers;

namespace RabbitMQProducerMachineStateCurrent
{
	public class Connection
	{
		private ConnectionFactory _connectionFactory;
		IConnection _connection;
		IModel _channelMachineStateLoggerProducer;
		Timer _timerSendMachineState;
		int machineNumber;

		public Connection()
		{
			machineNumber = 1;

			Connect();
		}

		private void _timerSendMachineState_Elapsed(object sender, ElapsedEventArgs e)
		{
			string message;

			MachinesStateMessage mss = new MachinesStateMessage();
			mss.Id = Guid.NewGuid();
			mss.Type = 006;
			mss.ExchangeName = "machinesstatecurrent";
			mss.RountingKey = "MACHINE_STATE";
			mss.TimeStamp = DateTime.Now;
			mss.TimeStampUtc = DateTime.UtcNow;
			mss.machineStates = new System.Collections.Generic.List<MachineState>();
			mss.machineStates.Add(MachineState.Create("PUMAM2401", Guid.Parse("0044aa00-7c71-44e4-b49e-d83a11803fc6"), MachineStates.Work, 1, DateTime.Now, ""));
			mss.machineStates.Add(MachineState.Create("PUMAM2402", Guid.Parse("88aceebc-4a52-45d1-96f8-5af158488e72"), MachineStates.Stop, 32, DateTime.Now, ""));
			mss.machineStates.Add(MachineState.Create("PUMAM2403", Guid.Parse("88aceebc-4a52-45d1-96f8-5af158488e66"), MachineStates.Stop, 32, DateTime.Now, ""));
			MachineState ms = new MachineState();
			message = Serializer.Serialize(mss);

			Message msg1 = new Message();
			msg1.Payload = message;
			msg1.Type = 3;
			msg1.TimeStamp = DateTime.Now.ToUniversalTime();
			string finalMessage = Serializer.Serialize(msg1);
			Console.WriteLine(finalMessage);
			byte[] msg = Encoding.UTF8.GetBytes(finalMessage);

			var properties = _channelMachineStateLoggerProducer.CreateBasicProperties();
			properties.Persistent = true;

			try
			{
				_channelMachineStateLoggerProducer.BasicPublish("machinesstatecurrent", "MACHINE_STATE", basicProperties: properties, msg);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			machineNumber++;
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
					_channelMachineStateLoggerProducer = _connection.CreateModel();
					_channelMachineStateLoggerProducer.ExchangeDeclare("machinesstatecurrent", ExchangeType.Topic, durable: true);
					_channelMachineStateLoggerProducer.QueueDeclare("MACHINES_STATE_CURRENT", durable: true, exclusive: false, autoDelete: false);
					_channelMachineStateLoggerProducer.QueueBind("MACHINES_STATE_CURRENT", "machinesstatecurrent", "MACHINE_STATE", null);

					_timerSendMachineState = new Timer();
					_timerSendMachineState.Elapsed += _timerSendMachineState_Elapsed;
					_timerSendMachineState.Interval = 5000;
					_timerSendMachineState.Start();
				}
			}
		}

		public void Disconnect()
		{
			_channelMachineStateLoggerProducer.Close();
			_connection.Close();
		}
	}
}
