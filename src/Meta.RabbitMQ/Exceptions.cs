using Meta.RabbitMQ.Consumer;
using Meta.RabbitMQ.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meta.RabbitMQ
{
	public class MetaRabbitMQException : Exception
	{
		public MetaRabbitMQException(string message) : base(message) { }

		public MetaRabbitMQException(string message, Exception innerException) : base(message, innerException) { }
	}

	public class BrokerConnectionException : MetaRabbitMQException
	{
		public BrokerConnectionException(Exception innerException) : base("Broker Unreachable", innerException) { }
	}

	public class NoSubscriberException : MetaRabbitMQException
	{
		public NoSubscriberException() : base($"No subscriber of the type implement {nameof(IConsumerSubscriber)}") { }
	}

	public class ProducerSentFailedException : MetaRabbitMQException
	{
		public ProducerSentFailedException(string message) : base(message) { }

		public ProducerSentFailedException(string message, Exception ex) : base(message, ex) { }
	}

	public class ChannelPoolNotFoundException : MetaRabbitMQException
	{
		public ChannelPoolNotFoundException(string name) : base($"Can not found {nameof(IChannelPool)} the pool name of '{name}'") { }
	}

	public class ChannelPoolNameAlreadyExistsException : MetaRabbitMQException
	{
		public ChannelPoolNameAlreadyExistsException(string name) : base($"{nameof(IChannelPool)} the pool name of '{name}' already exists.") { }
	}

	public class MessageBodyNullException : MetaRabbitMQException
	{
		public MessageBodyNullException() : base("Message body is null") { }
	}
}
