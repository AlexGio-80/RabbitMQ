using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQTest
{
	public class Connection
	{
		private ConnectionFactory _connectionFactory;
		IConnection _connection;
		IModel _channelMachineStateConsumer;
		IModel _channelMachineStateProducer;
		IModel _channelUpload;
		IModel _channelUploadAckConsumer;
		Timer _timerSendMachineState;
		Timer _timerUploadProgram;
		int machineNumber, machineUpload;
		String consumerUploadAckTag, consumerMachineStateTag;
		bool sendMessageUpload, sendMessageMachineState, readMessageMachineState, readUploadAck;

		public Connection()
		{
			sendMessageUpload = false;
			sendMessageMachineState = false;
			readMessageMachineState = true;
			readUploadAck = false;

			machineNumber = 1;
			machineUpload = 0;

			Connect();

			// *****************************************
			// Invio programma
			// *****************************************

			if (sendMessageUpload)
			{
				SendProgram();

				_timerUploadProgram = new Timer();
				_timerUploadProgram.Elapsed += _timerUploadProgram_Elapsed;
				_timerUploadProgram.Interval = 5000;
				_timerUploadProgram.Start();
			}

			// *****************************************
			// Invio programma
			// *****************************************

			// *****************************************
			// Invio machine state
			// *****************************************

			if (sendMessageMachineState)
			{
				if (_connection != null)
				{
					_channelMachineStateProducer = _connection.CreateModel();
					_channelMachineStateProducer.ExchangeDeclare("machinesstate", ExchangeType.Topic);
					_channelMachineStateProducer.QueueDeclare("RabbitMQTest_MACHINE_PUMAM2401_MACHINESTATE_PRODUCER", durable: false, exclusive: false);
					// così la coda non muore nemmeno se si riavvia Rabbit
					// _channelMachineStateProducer.QueueDeclare("RabbitMQTest_MACHINE_PUMAM2401_MACHINESTATE_PRODUCER", durable: true, exclusive: false);
					_channelMachineStateProducer.QueueBind("RabbitMQTest_MACHINE_PUMAM2401_MACHINESTATE_PRODUCER", "machinesstate", "MACHINE.PUMAM2401", null);
					_channelMachineStateProducer.CallbackException += _channel_CallbackException;
				}

				_timerSendMachineState = new Timer();
				_timerSendMachineState.Elapsed += _timerSendMachineState_Elapsed;
				_timerSendMachineState.Interval = 5000;
				_timerSendMachineState.Start();
			}

			// *****************************************
			// Invio machine state
			// *****************************************
		}

		private void SendProgram()
		{
			string message;

			MachineUploadProgramMessage mupm = new MachineUploadProgramMessage();
			mupm.Id = Guid.NewGuid();
			mupm.TimeStamp = DateTime.Now.ToUniversalTime();
			mupm.ProgramName = "O010" + machineUpload.ToString();
			mupm.ProgramBody = "programma di esempio";
			mupm.ProgramPath = "via masera di sotto";
			mupm.ExchangeName = "machinesupload";
			mupm.RountingKey = "MACHINE.PUMAM2401";
			mupm.MachineId = Guid.Parse("0044aa00-7c71-44e4-b49e-d83a11803fc7");
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
				_channelUpload.BasicPublish("machinesupload", "MACHINE.PUMAM2401", mandatory: false, basicProperties: null, msg);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			machineUpload++;
		}

		private void _timerSendMachineState_Elapsed(object sender, ElapsedEventArgs e)
		{
			string message;

			MachineStateMessage mupm = new MachineStateMessage();
			mupm.Id = Guid.NewGuid();
			mupm.TimeStamp = DateTime.Now.ToUniversalTime();
			mupm.ExchangeName = "machinesstate";
			mupm.RountingKey = "MACHINE_PUMAM2401";
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
			byte[] msg = Encoding.UTF8.GetBytes(finalMessage);

			var properties = _channelMachineStateProducer.CreateBasicProperties();
			properties.Persistent = true;

			try
			{
				_channelMachineStateProducer.BasicPublish("machinesstate", "MACHINE.PUMAM2401", basicProperties: properties, msg);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			machineNumber++;
		}

		private void _timerUploadProgram_Elapsed(object sender, ElapsedEventArgs e)
		{
			string message;
			MachineUploadProgramMessage mupm = new MachineUploadProgramMessage();

			mupm.Id = Guid.NewGuid();
			mupm.TimeStamp = DateTime.Now.ToUniversalTime();
			mupm.ProgramName = "O010" + machineUpload.ToString();
			mupm.ProgramBody = "programma di esempio";
			mupm.ProgramPath = "via masera di sotto";
			mupm.ExchangeName = "machinesupload";
			mupm.RountingKey = "MACHINE.PUMAM2401";
			mupm.MachineId = Guid.Parse("0044aa00-7c71-44e4-b49e-d83a11803fc7");
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
				_channelUpload.BasicPublish("machinesupload", "MACHINE.PUMAM2401", mandatory: false, basicProperties: null, msg);
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
					if (sendMessageUpload)
					{
						_channelUpload = _connection.CreateModel();
						_channelUpload.ExchangeDeclare("machinesupload", ExchangeType.Topic);
						_channelUpload.QueueDeclare("RabbitMQTest_UPLOAD_PROGRAM_PRODUCER", exclusive: false);
						_channelUpload.QueueBind("RabbitMQTest_UPLOAD_PROGRAM_PRODUCER", "machinesupload", "MACHINE.PUMAM2401", null);
						_channelUpload.CallbackException += _channel_CallbackException;
					}

					if (readMessageMachineState)
					{
						_channelMachineStateConsumer = _connection.CreateModel();
						// l'exchange lo crea già il producer
						_channelMachineStateConsumer.ExchangeDeclare("machinesstate", ExchangeType.Topic);
						_channelMachineStateConsumer.QueueDeclare("RabbitMQTest_MACHINE_PUMAM2401_CONSUMER", exclusive: false);
						_channelMachineStateConsumer.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
						_channelMachineStateConsumer.QueueBind("RabbitMQTest_MACHINE_PUMAM2401_CONSUMER", "machinesstate", "MACHINE.*");

						var consumer = new EventingBasicConsumer(_channelMachineStateConsumer);
						consumer.Received += (model, ea) =>
						{
							var body = ea.Body;
							Console.WriteLine(Encoding.UTF8.GetString(body.ToArray()));

							_channelMachineStateConsumer.BasicAck(ea.DeliveryTag, true);
						};
						// this consumer tag identifies the subscription
						// when it has to be cancelled
						consumerMachineStateTag = _channelMachineStateConsumer.BasicConsume("RabbitMQTest_MACHINE_PUMAM2401_CONSUMER", false, consumer);
						//_channelMachineStateConsumer.BasicConsume("RabbitMQTest_MACHINE_PUMAM2401_CONSUMER", true, consumer);
						//_channelMachineStateConsumer.BasicConsume("RabbitMQTest_MACHINE_PUMAM2401_CONSUMER", true, consumer);
					}

					if (readUploadAck)
					{
						_channelUploadAckConsumer = _connection.CreateModel();
						_channelUploadAckConsumer.ExchangeDeclare("machinesuploadack", "topic");
						_channelUploadAckConsumer.QueueDeclare("RabbitMQTest_MACHINE_PUMAM2401_UPLACK_CONSUMER", exclusive: false);
						_channelUploadAckConsumer.QueueBind("RabbitMQTest_MACHINE_PUMAM2401_UPLACK_CONSUMER", "machinesuploadack", "MACHINE.*");

						var consumer = new EventingBasicConsumer(_channelUploadAckConsumer);
						consumer.Received += (model, ea) =>
						{
							var body = ea.Body;
							Console.WriteLine(Encoding.UTF8.GetString(body.ToArray()));
							_channelUploadAckConsumer.BasicAck(ea.DeliveryTag, false);
						};
						// this consumer tag identifies the subscription
						// when it has to be cancelled
						consumerUploadAckTag = _channelUploadAckConsumer.BasicConsume("RabbitMQTest_MACHINE_PUMAM2401_UPLACK_CONSUMER", false, consumer);
					}
				}
			}
		}

		private void _channel_CallbackException(object sender, CallbackExceptionEventArgs e)
		{
			throw new NotImplementedException();
		}

		public void Disconnect()
		{
			if (readUploadAck)
			{
				_channelUploadAckConsumer.BasicCancel(consumerUploadAckTag);
				_channelUploadAckConsumer.Close();
			}

			if (sendMessageUpload)
			{
				_channelUpload.QueueDelete("RabbitMQTest_UPLOAD_PROGRAM_PRODUCER");
				_channelUpload.Close();
			}

			if (readMessageMachineState)
			{
				_channelMachineStateConsumer.BasicCancel(consumerMachineStateTag);
				_channelMachineStateConsumer.Close();
			}

			if (sendMessageMachineState)
			{
				//_channelMachineStateProducer.QueueDelete("RabbitMQTest_MACHINE_PUMAM2401_MACHINESTATE_PRODUCER");
				_channelMachineStateProducer.Close();
			}
			
			_connection.Close();
		}
	}
}
