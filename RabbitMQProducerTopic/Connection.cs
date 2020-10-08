using RabbitMQ.Client;
using System;
using System.Text;
using System.Timers;

namespace RabbitMQProducerTopic
{
	public class Connection
	{
		private ConnectionFactory _connectionFactory;
		IConnection _connection;
		IModel _channelMachineStateProducer, _channelMachineStateLoggerProducer;
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

			MachineStateMessage mupm = new MachineStateMessage();
			mupm.Id = Guid.NewGuid();
			mupm.TimeStamp = DateTime.Now.ToUniversalTime();
			mupm.ExchangeName = "machinesstate";
			mupm.RountingKey = "MACHINE_PUMAM2402";
			mupm.MachineId = Guid.Parse("0044aa00-7c71-44e4-b49e-d83a11803fc7");
			mupm.Type = 003;
			mupm.MachineState = new MachineState();
			if (machineNumber % 2 == 0)
			{
				mupm.MachineState.State = MachineStates.Work;
				mupm.MachineState.TimeStamp = DateTime.Now;
			}
			else
			{
				mupm.MachineState.State = MachineStates.Stop;
				mupm.MachineState.TimeStamp = DateTime.Now;
			}
			message = Serializer.Serialize(mupm);

			Message msg1 = new Message();
			msg1.Payload = message;
			msg1.Type = 3;
			msg1.TimeStamp = DateTime.Now.ToUniversalTime();
			string finalMessage = Serializer.Serialize(msg1);
			Console.WriteLine(finalMessage);
			byte[] msg = Encoding.UTF8.GetBytes(finalMessage);

			var properties = _channelMachineStateProducer.CreateBasicProperties();
			properties.Persistent = true;

			try
			{
				_channelMachineStateProducer.BasicPublish("machinesstate", "MACHINE.PUMAM2402", basicProperties: properties, msg);
				_channelMachineStateLoggerProducer.BasicPublish("machinesstate", "MACHINE_STATE", basicProperties: properties, msg);
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
					_channelMachineStateProducer = _connection.CreateModel();
					_channelMachineStateProducer.ExchangeDeclare("machinesstate", ExchangeType.Fanout, durable: true);

					_channelMachineStateLoggerProducer = _connection.CreateModel();
					_channelMachineStateLoggerProducer.ExchangeDeclare("machinesstate", ExchangeType.Fanout, durable: true);
					// così se si riavvia Rabbit la coda muore
					_channelMachineStateLoggerProducer.QueueDeclare("RabbitMQTest_MACHINESTATE", durable: true, exclusive: false, autoDelete: false);
					// così la coda non muore nemmeno se si riavvia Rabbit
					// _channelMachineStateProducer.QueueDeclare("RabbitMQTest_MACHINE_PUMAM2401_MACHINESTATE_PRODUCER", durable: true, exclusive: false);
					_channelMachineStateProducer.QueueBind("RabbitMQTest_MACHINESTATE", "machinesstate", "MACHINE_STATE", null);

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
			_channelMachineStateProducer.Close();
			_channelMachineStateLoggerProducer.Close();


			_connection.Close();
		}
	}
}
