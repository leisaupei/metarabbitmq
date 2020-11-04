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
			var header = new Dictionary<string, string> {
				{ "a", "a" } ,
				//{ Meta.RabbitMQ.Generic.Headers.QueueName, "test.queue.v1" },
				//{ Meta.RabbitMQ.Generic.Headers.ExchangeType, ExchangeType.Direct },
				//{ Meta.RabbitMQ.Generic.Headers.ProducerInitQueue, "1" }
			};
			return SendMessageAsync(model, "test.ex.v1", "test.rk.v1", header);
		}
	}
	public class TestModel
	{
		public Guid UserId { get; set; }
	}
}
