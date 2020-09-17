using Meta.RabbitMQ.Extension;
using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Producer;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

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
		public void Test1()
		{
		}
		[Fact]
		public async Task SingleHost()
		{
			ServiceCollection services = new ServiceCollection();
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
			services.AddRabbitMQProducerService();
			ServiceProvider serviceProvider = services.BuildServiceProvider();
			IMessageProducer transport = serviceProvider.GetService<IMessageProducer>();
			for (int i = 0; i < 1000000; i++)
			{
				ProducerResult taskresult = await transport.SendAsync(new Message<string>(
					new Dictionary<string, string> {
						{ Generic.Headers.Exchange, "test.ex.v1" },
						{ Generic.Headers.RoutingKey, "test.rk.v1" }, }, "你好呀" + i));
			}
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
					Name = "host1_test_mq" // required when using IConnectionChannelPoolCollection
				});

				o.Add(new RabbitMQOption
				{
					HostName = "localhost",
					Port = 5672,
					UserName = "guest",
					Password = "guest",
					VirtualHost = "test_mq",
					Name = "host1_test_mq1" // required when using IConnectionChannelPoolCollection
				});

				//collection.Options[2].HostName = "172.16.1.220";
				//collection.Options[2].Port = 5672;
				//collection.Options[2].UserName = "lgx";
				//collection.Options[2].Password = "123456";
				//collection.Options[2].VirtualHost = "first"; // same host name with index 1 but different virtual host
				//collection.Options[2].Name = "host2_first"; // required when using IConnectionChannelPoolCollection

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
