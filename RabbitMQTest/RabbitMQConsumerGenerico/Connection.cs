using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQConsumerGenerico
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
						_channelMachineStateConsumer = _connection.CreateModel();
						// l'exchange lo crea già il producer
						_channelMachineStateConsumer.ExchangeDeclare("machinesstate", ExchangeType.Topic, durable: true);
						_channelMachineStateConsumer.QueueDeclare("RabbitMQTest_MACHINESTATE_CONSUMER", exclusive: false);
						_channelMachineStateConsumer.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
						_channelMachineStateConsumer.QueueBind("RabbitMQTest_MACHINESTATE_CONSUMER", "machinesstate", "MACHINE.PUMAM2401");

						var consumer = new EventingBasicConsumer(_channelMachineStateConsumer);
						consumer.Received += (model, ea) =>
						{
							var body = ea.Body;
							Console.WriteLine(Encoding.UTF8.GetString(body.ToArray()));

							_channelMachineStateConsumer.BasicAck(ea.DeliveryTag, true);
							//_channelMachineStateConsumer.BasicNack(ea.DeliveryTag, true, false);
						};
						// this consumer tag identifies the subscription
						// when it has to be cancelled
						consumerMachineStateTag = _channelMachineStateConsumer.BasicConsume("RabbitMQTest_MACHINESTATE_CONSUMER", false, consumer);
						//_channelMachineStateConsumer.BasicConsume("RabbitMQTest_MACHINE_PUMAM2401_CONSUMER", true, consumer);
						//_channelMachineStateConsumer.BasicConsume("RabbitMQTest_MACHINE_PUMAM2401_CONSUMER", true, consumer);
				}
			}
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
