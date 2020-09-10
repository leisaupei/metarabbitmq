using Meta.RabbitMQ.Extension;
using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Producer;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Producer.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			IServiceCollection services = new ServiceCollection();
			services.AddOptions();
			services.AddLogging();
			services.Configure<RabbitMQOptionCollection>(a =>
			{
				a.Add(new RabbitMQOption
				{
					HostName = "localhost",
					Port = 5672,
					UserName = "guest",
					Password = "guest",
					VirtualHost = "/",
				});
			});
			services.AddRabbitMQProducerService();
			services.AddSingleton<TestMqTransporter>();
			ServiceProvider serviceProvider = services.BuildServiceProvider();
			TestMqTransporter transport = serviceProvider.GetService<TestMqTransporter>();
			Parallel.For(0, 1000000, new ParallelOptions { MaxDegreeOfParallelism = 10000 }, async (i) =>
			{
				await transport.SendTestMessageAsync(new TestModel { UserId = Guid.NewGuid() });
			});
			Console.ReadKey();
		}
	}
}
