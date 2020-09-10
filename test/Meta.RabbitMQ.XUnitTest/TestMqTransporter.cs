using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Producer;
using Meta.RabbitMQ.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.XUnitTest
{
	public class TestMqTransporter : MessageProducerBase
	{
		public TestMqTransporter(IMessageProducer messageProducer, ISerializer serializer) : base(messageProducer, serializer)
		{
		}
		public Task<ProducerResult> SendTestMessageAsync(TestModel model)
		{
			return SendMessageAsync(model, "test.ex.v1", "test.rk.v1", new Dictionary<string, string> { { "a", "a" } });
		}

	}
	public class TestModel
	{
		public Guid UserId { get; set; }
	}
}
