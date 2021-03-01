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

	/// <summary>
	/// 连接断开错误
	/// </summary>
	public class BrokerConnectionException : MetaRabbitMQException
	{
		public BrokerConnectionException(Exception innerException) : base("Broker Unreachable", innerException) { }
	}

	/// <summary>
	/// 没有订阅者
	/// </summary>
	public class NoSubscriberException : MetaRabbitMQException
	{
		public NoSubscriberException() : base($"No subscriber of the type implement {nameof(IConsumerSubscriber)}") { }
	}

	/// <summary>
	/// 消息发送失败错误
	/// </summary>
	public class ProducerSentFailedException : MetaRabbitMQException
	{
		public ProducerSentFailedException(string message) : base(message) { }

		public ProducerSentFailedException(string message, Exception ex) : base(message, ex) { }
	}

	/// <summary>
	/// 没有找到连接池错误
	/// </summary>
	public class ChannelPoolNotFoundException : MetaRabbitMQException
	{
		public ChannelPoolNotFoundException(string name) : base($"Can not found {nameof(IChannelPool)} the pool name of '{name}'") { }
	}

	/// <summary>
	/// 连接池名称已存在错误
	/// </summary>
	public class ChannelPoolNameAlreadyExistsException : MetaRabbitMQException
	{
		public ChannelPoolNameAlreadyExistsException(string name) : base($"{nameof(IChannelPool)} the pool name of '{name}' already exists.") { }
	}

	/// <summary>
	/// 消息body是空的
	/// </summary>
	public class MessageBodyNullException : MetaRabbitMQException
	{
		public MessageBodyNullException() : base("Message body is null") { }
	}

	/// <summary>
	/// 执行订阅者方法时出现错误
	/// </summary>
	public class SubscriberInvokeException : MetaRabbitMQException
	{
		public SubscriberInvokeException(Exception exception) : base("Subscriber invoke error.", exception) { }
	}
}
