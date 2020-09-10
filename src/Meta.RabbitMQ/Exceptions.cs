using Meta.RabbitMQ.Consumer;
using Meta.RabbitMQ.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meta.RabbitMQ
{
	public class BrokerConnectionException : Exception
	{
		public BrokerConnectionException(Exception innerException) : base("Broker Unreachable", innerException)
		{

		}
	}

	public class NoSubscriberException : Exception
	{
		public NoSubscriberException() : base($"No subscriber of the type implement {nameof(IConsumerSubscriber)}")
		{

		}
	}
	public class ProducerSentFailedException : Exception
	{
		public ProducerSentFailedException(string message) : base(message)
		{
		}

		public ProducerSentFailedException(string message, Exception ex) : base(message, ex)
		{
		}
	}

	public class ConnectionChannelPoolNotFoundException : Exception
	{
		public ConnectionChannelPoolNotFoundException(string name) : base($"Can not found {nameof(IConnectionChannelPool)} the pool name of '{name}'")
		{

		}
	}
}
