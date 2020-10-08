using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RabbitMQDownloadProgramProducerConsumer
{
	public partial class frmConsumerProducer : Form
	{
		private ConnectionFactory _connectionFactory;
		IConnection _connection;
		IModel _channelDownloadProgramProducer, _channelDownloadProgramConsumer;
		string _consumerDownloadProgramTag;
		bool _init;
		private Thread thread2 = null;
		private delegate void SafeCallDelegate(string text);

		public frmConsumerProducer()
		{
			_init = true;
			InitializeComponent();
		}

		private void frmConsumerProducer_Load(object sender, EventArgs e)
		{
			_init = false;
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
					_channelDownloadProgramProducer = _connection.CreateModel();
					_channelDownloadProgramProducer.ExchangeDeclare("machinesdwprogram", ExchangeType.Topic, durable: true);
					_channelDownloadProgramProducer.QueueDeclare("MACHINE_DW_PROGRAM.REQUEST.PUMAM2401", durable: true, exclusive: false, autoDelete: false);
					_channelDownloadProgramProducer.QueueBind("MACHINE_DW_PROGRAM.REQUEST.PUMAM2401", "machinesdwprogram", "MACHINE_DW_PROGRAM.REQUEST.PUMAM2401", null);
				}
			}
		}

		public void Disconnect()
		{
			_channelDownloadProgramProducer.Close();
			_connection.Close();
		}

		private void cmdRequestProgram_Click(object sender, EventArgs e)
		{
			string message;

			if (_init)
			{
				_init = false;
				Connect();
				CreateConsumer();
				thread2 = new Thread(new ThreadStart(SetText));
				thread2.Start();
				Thread.Sleep(1000);
				return;
			}

			MachinePartProgramMessage mppm = new MachinePartProgramMessage();
			mppm.MachineId = Guid.Parse("0044aa00-7c71-44e4-b49e-d83a11803fc6");
			mppm.Id = Guid.NewGuid();
			mppm.Type = 004;
			mppm.ExchangeName = "machinesdwprogram";
			mppm.RountingKey = "MACHINE_DW_PROGRAM.REQUEST.PUMAM2401";
			mppm.TimeStamp = DateTime.Now;
			mppm.TimeStampUtc = DateTime.UtcNow;
			mppm.ProgramName = txtProgramName.Text;
			mppm.ProgramPath = txtProgramPath.Text;
			message = Serializer.Serialize(mppm);

			Message msg1 = new Message();
			msg1.Payload = message;
			msg1.Type = 4;
			msg1.TimeStamp = DateTime.Now.ToUniversalTime();
			string finalMessage = Serializer.Serialize(msg1);
			Console.WriteLine(finalMessage);
			byte[] msg = Encoding.UTF8.GetBytes(finalMessage);

			var properties = _channelDownloadProgramProducer.CreateBasicProperties();
			properties.Persistent = true;

			try
			{
				_channelDownloadProgramProducer.BasicPublish("machinesdwprogram", "MACHINE_DW_PROGRAM.REQUEST.PUMAM2401", basicProperties: properties, msg);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private void frmConsumerProducer_FormClosing(object sender, FormClosingEventArgs e)
		{
			Disconnect();
		}

		private void CreateConsumer()
		{
			_channelDownloadProgramConsumer = _connection.CreateModel();
			// l'exchange lo crea già il producer
			_channelDownloadProgramConsumer.ExchangeDeclare("machinesdwprogram", ExchangeType.Topic, durable: true);
			_channelDownloadProgramConsumer.QueueDeclare("MACHINE_DW_PROGRAM.REPLY.PUMAM2401", exclusive: false, durable: true, autoDelete: false);
			_channelDownloadProgramConsumer.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
			_channelDownloadProgramConsumer.QueueBind("MACHINE_DW_PROGRAM.REPLY.PUMAM2401", "machinesstate", "MACHINE_DW_PROGRAM.REPLY.PUMAM2401");

			var consumer = new EventingBasicConsumer(_channelDownloadProgramConsumer);
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
						File.WriteAllText("E:\\OSL\\PROGRAMMI\\TEST.H", msg.ProgramBody);
						WriteTextSafe(msg.ProgramBody);
					},
					null);

				_channelDownloadProgramConsumer.BasicAck(ea.DeliveryTag, true);
				//_channelMachineStateConsumer.BasicNack(ea.DeliveryTag, true, false);
			};
			// this consumer tag identifies the subscription
			// when it has to be cancelled
			_consumerDownloadProgramTag = _channelDownloadProgramConsumer.BasicConsume("MACHINE_DW_PROGRAM.REPLY.PUMAM2401", false, consumer);
		}

		private void GetMessage(string payload, Action<MachinePartProgramMessage> done, Action<Exception> fail)
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
						typeof(MachinePartProgramMessage),
						message =>
						{
							var completeMessage = (MachinePartProgramMessage)message;
							completeMessage.TimeStampUtc = DateTime.UtcNow;
							completeMessage.TimeStamp = DateTime.Now;

							done(completeMessage);
						},
						null);
				},
				null);
		}

		private void WriteTextSafe(string text)
		{
			if (txtProgramBody.InvokeRequired)
			{
				var d = new SafeCallDelegate(WriteTextSafe);
				txtProgramBody.Invoke(d, new object[] { text });
			}
			else
			{
				txtProgramBody.Text = text;
			}
		}

		private void SetText()
		{
			WriteTextSafe("This text was set safely.");
		}
	}
}
