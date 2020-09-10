using System;
using System.Collections.Generic;
using System.Text;

namespace Meta.RabbitMQ.Generic
{
	public class ClientOptions
	{
		public ClientOptions(string exchange, string routingKey, string exchangeType, string queueName, string name = "")
		{
			Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
			RoutingKey = routingKey ?? throw new ArgumentNullException(nameof(routingKey));
			ExchangeType = exchangeType ?? throw new ArgumentNullException(nameof(exchangeType));
			QueueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
			Name = name ?? throw new ArgumentNullException(nameof(name));
		}

		public string Exchange { get; }
		public string RoutingKey { get; }
		public string ExchangeType { get; }
		public string QueueName { get; }
		public string Name { get; }
		public override string ToString()
		{
			return $"exchange: {Exchange}, queue: {QueueName}, routing key: {RoutingKey}.";
		}
	}
}
