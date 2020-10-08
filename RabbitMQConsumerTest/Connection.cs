using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQConsumerTest
{
	public class Connection
	{
		private ConnectionFactory _connectionFactory;
		IConnection _connection;
		IModel _channelMachineStateConsumer;
		String consumerMachineStateTag;
		
		public Connection()
		{
			Connect();
		}

		private void Connect()
		{
			var factories = new Dictionary<short, MessageDeserializer>
			{
                /* 006 */ {new MachinesStateMessage().Type, new MessageDeserializer(() => typeof(MachinesStateMessage))}
			};

			_connectionFactory = new ConnectionFactory();
			_connectionFactory.UserName = "guest";
			_connectionFactory.Password = "guest";
			_connectionFactory.VirtualHost = "/";
			_connectionFactory.HostName = "localhost";
			_connectionFactory.Port = 5672;

			Dictionary<string, Guid> keyValuePairs = new Dictionary<string, Guid>();
			MachinesStateMessage machinesStateMessage;

			if (_connectionFactory != null)
			{
				_connection = _connectionFactory.CreateConnection();
				if (_connection != null)
				{
					_channelMachineStateConsumer = _connection.CreateModel();
					// l'exchange lo crea già il producer
					_channelMachineStateConsumer.ExchangeDeclare("machinesstatecurrent", ExchangeType.Topic, durable: true);
					_channelMachineStateConsumer.QueueDeclare("MACHINES_STATE_CURRENT", exclusive: false, durable: true, autoDelete: false);
					_channelMachineStateConsumer.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
					_channelMachineStateConsumer.QueueBind("MACHINES_STATE_CURRENT", "machinesstatecurrent", "MACHINE_STATE");

					var consumer = new EventingBasicConsumer(_channelMachineStateConsumer);
					consumer.Received += (model, ea) =>
					{
						var body = ea.Body;
						var payload = Encoding.UTF8.GetString(body.ToArray());

						//Console.WriteLine(Encoding.UTF8.GetString(body.ToArray()));
						Console.WriteLine(payload);

						GetMessage(
							payload, 
							msg =>
							{
								machinesStateMessage = msg;
								foreach (var machine in msg.machineStates)
								{
									if (!keyValuePairs.ContainsKey(machine.Code))
										keyValuePairs.Add(machine.Code, machine.MachineId);
								}
							},
							null);

						_channelMachineStateConsumer.BasicAck(ea.DeliveryTag, true);
					};
					// this consumer tag identifies the subscription
					// when it has to be cancelled
					consumerMachineStateTag = _channelMachineStateConsumer.BasicConsume("MACHINES_STATE_CURRENT", false, consumer);
				}
			}
		}

		private void GetMessage(string payload, Action<MachinesStateMessage> done, Action<Exception> fail)
		{
			Deserializer.Deserialize<Message>(
				payload,
				message =>
				{

					if (message.Type != 6)
					{
						Console.WriteLine("decodifica sbagliata");
					}

					Deserializer.Deserialize(
						message.Payload,
						typeof(MachinesStateMessage),
						message =>
						{
							var completeMessage = (MachinesStateMessage)message;
							completeMessage.TimeStampUtc = DateTime.UtcNow;
							completeMessage.TimeStamp = DateTime.Now;

							done(completeMessage);
						},
						null);
				},
				null);
		}

		private void _channel_CallbackException(object sender, CallbackExceptionEventArgs e)
		{
			throw new NotImplementedException();
		}

		public void Disconnect()
		{
			//_channelMachineStateConsumer.BasicCancel(consumerMachineStateTag);
			//_channelMachineStateConsumer.Close();
			
			_connection.Close();
		}
	}
}
