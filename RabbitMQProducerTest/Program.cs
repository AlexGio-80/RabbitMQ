using System;

namespace RabbitMQProducerTest
{
	class Program
	{
		static Connection connection;
		static void Main(string[] args)
		{
			connection = new Connection();

			Console.WriteLine("Hello World!");
			ConsoleKeyInfo info = Console.ReadKey();
			connection.Disconnect();
		}
	}
}
