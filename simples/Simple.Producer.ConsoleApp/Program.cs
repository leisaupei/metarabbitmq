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
		static async Task Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			IServiceCollection services = new ServiceCollection();
			services.AddOptions();
			services.AddLogging();
			services.Configure<RabbitMQOptionCollection>(a =>
			{
				a.Add(new RabbitMQOption
				{
					HostName = "172.16.1.219",
					Port = 5672,
					UserName = "lgx",
					Password = "123456",
					VirtualHost = "test_mq",
				});
			});
			services.AddRabbitMQProducerService();
			services.AddSingleton<TestMqTransporter>();
			ServiceProvider serviceProvider = services.BuildServiceProvider();

			TestMqTransporter transport = serviceProvider.GetService<TestMqTransporter>();

			var tests = new List<TestModel>();
			for (int i = 0; i < 1000000; i++)
			{
				tests.Add(new TestModel { UserId = Guid.NewGuid() });
			}
			await transport.SendTestMessageAsync(tests.ToArray());

			//Parallel.For(0, 1000000, new ParallelOptions { MaxDegreeOfParallelism = 10000 }, async (i) =>
			//{
			//	await transport.SendTestMessageAsync(new TestModel { UserId = Guid.NewGuid() });
			//});

			Console.ReadKey();
		}
	}
}
