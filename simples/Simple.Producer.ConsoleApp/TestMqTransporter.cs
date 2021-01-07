using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Producer;
using Meta.RabbitMQ.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Meta.RabbitMQ;
namespace Simple.Producer.ConsoleApp
{
	public class TestMqTransporter : MessageProducerBase
	{
		public TestMqTransporter(IMessageProducer messageProducer) : base(messageProducer)
		{
		}
		public Task<ProducerResult> SendTestMessageAsync(TestModel model)
		{
			return SendMessageAsync(model, "test.ex.v1", "test.rk.v1");
		}
		public Task<ProducerResult> SendTestMessageAsync(TestModel[] model)
		{
			return SendMessagesAsync(model, "test.ex.v1", "test.rk.v1");
		}
	}
	public class TestModel
	{
		public Guid UserId { get; set; }
	}
}
