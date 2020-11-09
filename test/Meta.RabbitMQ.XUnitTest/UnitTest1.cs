using Meta.RabbitMQ.Consumer;
using Meta.RabbitMQ.Extension;
using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Producer;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using RabbitMQHeaders = RabbitMQ.Client.Headers;
namespace Meta.RabbitMQ.XUnitTest
{
	public class UnitTest1
	{
		private readonly ITestOutputHelper _output;

		public UnitTest1(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void TestAddSameNameChannelPool()
		{
			ServiceCollection services = new ServiceCollection();
			services.AddOptions();
			services.Configure<RabbitMQOptionCollection>(a =>
			{
				a.TryAdd(new RabbitMQOption
				{
					HostName = "localhost",
					Port = 5672,
					UserName = "guest",
					Password = "guest",
					VirtualHost = "/",
					Name = "name1"
				});
			});
			services.Configure<RabbitMQOptionCollection>(a =>
			{
				a.TryAdd(new RabbitMQOption
				{
					HostName = "localhost",
					Port = 5672,
					UserName = "guest",
					Password = "guest",
					VirtualHost = "/",
					Name = "name1"
				});
			});
			services.AddRabbitMQProducerService();
			services.AddSingleton<TestMqTransporter>();
			ServiceProvider serviceProvider = services.BuildServiceProvider();

			TestMqTransporter transport = serviceProvider.GetService<TestMqTransporter>();
		}
		[Fact]
		public async Task SingleHost()
		{
			ServiceCollection services = new ServiceCollection();
			services.AddOptions();
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
			//IMessageProducer transport = serviceProvider.GetService<IMessageProducer>();

			TestMqTransporter transport = serviceProvider.GetService<TestMqTransporter>();
			var result = await transport.SendTestMessageAsync(new TestModel { UserId = Guid.NewGuid() });
			_output.WriteLine(result.ToString());
			//for (int i = 0; i < 1000000; i++)
			//{
			//	ProducerResult taskresult = await transport.SendAsync(new Message<string>(
			//	new Dictionary<string, string> {
			//			{ Generic.Headers.Exchange, "test.ex.v1" },
			//			{ Generic.Headers.RoutingKey, "test.rk.v1" },
			//	}, "你好呀"));
			//}
			//Console.ReadKey();
		}

		[Fact]
		public async Task Test()
		{
			IServiceCollection services = new ServiceCollection();
			services.AddLogging();
			services.AddOptions();
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

			services.Configure<RabbitMQOptionCollection>(a =>
			{
				a.Add(new RabbitMQOption
				{
					HostName = "localhost",
					Port = 5672,
					UserName = "guest",
					Password = "guest",
					VirtualHost = "test",
				});
			});
			services.AddRabbitMQProducerService();
			services.AddSingleton<TestMqTransporter>();
			ServiceProvider serviceProvider = services.BuildServiceProvider();
			TestMqTransporter transport = serviceProvider.GetService<TestMqTransporter>();

			await transport.SendTestMessageAsync(new TestModel { UserId = Guid.NewGuid() });
		}
		[Fact]
		public async Task MutipleHost()
		{
			IServiceCollection services = new ServiceCollection();
			services.AddLogging();
			services.Configure<RabbitMQOptionCollection>(o =>
			{
				o.Add(new RabbitMQOption
				{
					HostName = "localhost",
					Port = 5672,
					UserName = "guest",
					Password = "guest",
					VirtualHost = "test_mq",
					Name = "host1_test_mq" // required when using IchannelPoolCollection
				});

				o.Add(new RabbitMQOption
				{
					HostName = "localhost",
					Port = 5672,
					UserName = "guest",
					Password = "guest",
					VirtualHost = "test_mq",
					Name = "host1_test_mq1" // required when using IchannelPoolCollection
				});


			});
			services.AddRabbitMQProducerService();


			ServiceProvider serviceProvider = services.BuildServiceProvider();
			IMessageProducer transport = serviceProvider.GetService<IMessageProducer>();

			// send to 'host1_test_mq'
			await transport.SendAsync(new Message<string>(
					new Dictionary<string, string> {
					{ Generic.Headers.Exchange, "test.ex.v1" },
					{ Generic.Headers.QueueName, "test.queue.v1" },
					{ Generic.Headers.RoutingKey, "test.rk.v1" },
					{ Generic.Headers.ExchangeType, ExchangeType.Direct },
					{ Generic.Headers.Name, "host1_test_mq" }
					}, "你好呀"));

			// send to 'host1_test_mq1'
			await transport.SendAsync(new Message<string>(
				new Dictionary<string, string> {
					{ Generic.Headers.Exchange, "test.ex.v2 " },
					{ Generic.Headers.QueueName, "test.queue.v2" },
					{ Generic.Headers.RoutingKey, "test.rk.v1" },
					{ Generic.Headers.ExchangeType, ExchangeType.Direct },
					{ Generic.Headers.Name, "host1_test_mq1" }
				}, "你好呀"));

			// send to 'host2_first'
			//await transport.SendAsync(new TransportMessage(new Dictionary<string, string> { { Generic.Headers.QueueName, "test.queue" }, { Generic.Headers.Name, "host2_first" } }, Encoding.UTF8.GetBytes("你好呀")));

		}

	}
}
