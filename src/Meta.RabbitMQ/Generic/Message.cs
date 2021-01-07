using System;
using System.Collections.Generic;
using System.Text;

namespace Meta.RabbitMQ.Generic
{
	/// <summary>
	/// 传输消息
	/// </summary>
	public class Message
	{
		public Message(IDictionary<string, string> headers, object value)
		{
			Headers = headers ?? throw new ArgumentNullException(nameof(headers));
			Body = value;
		}

		public IDictionary<string, string> Headers { get; }

		public object Body { get; }

		/// <summary>
		/// 转换成泛型
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public Message<T> ToMessage<T>()
		{ 
			return new Message<T>(Headers, (T)Body);
		}
	}

	/// <summary>
	/// 泛型消息重载
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Message<T> : Message
	{
		public Message(IDictionary<string, string> headers, T value) : base(headers, value) { }

		public new T Body => (T)base.Body;
	}

	/// <summary>
	/// 泛型消息重载
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Messages<T> : Message
	{
		public Messages(IDictionary<string, string> headers, IList<T> value) : base(headers, value) { }

		public new IList<T> Body => (IList<T>)base.Body;
	}

	public static class MessageExtensions
	{
		/// <summary>
		/// Get queue name.
		/// </summary>
		/// <returns></returns>
		public static string GetQueueName(this Message message) => message.Headers.TryGetValue(Meta.RabbitMQ.Generic.Headers.QueueName, out var value) ? value : null;

		/// <summary>
		/// Get exchange.
		/// </summary>
		/// <returns></returns>
		public static string GetExchange(this Message message) => message.Headers.TryGetValue(Meta.RabbitMQ.Generic.Headers.Exchange, out var value) ? value : null;

		/// <summary>
		/// Get routing key.
		/// </summary>
		/// <returns></returns>
		public static string GetRoutingKey(this Message message) => message.Headers.TryGetValue(Meta.RabbitMQ.Generic.Headers.RoutingKey, out var value) ? value : null;

		/// <summary>
		/// Get routing key.
		/// </summary>
		/// <returns></returns>
		public static string GetExchangeType(this Message message) => message.Headers.TryGetValue(Meta.RabbitMQ.Generic.Headers.ExchangeType, out var value) ? value : null;

		/// <summary>
		/// 获取pool name, 当你注入了此服务<see cref="IChannelPoolCollection"/>
		/// </summary>
		/// <returns></returns>
		public static string GetName(this Message message) => message.Headers.TryGetValue(Meta.RabbitMQ.Generic.Headers.Name, out var value) ? value : null;

		/// <summary>
		/// 是否在生产者创建队列初始化操作
		/// </summary>
		/// <returns></returns>
		public static bool IsInitQueue(this Message message) => message.Headers.TryGetValue(Meta.RabbitMQ.Generic.Headers.ProducerInitQueue, out _);

		/// <summary>
		/// 
		/// </summary>
		public static void Remove(this Message message)
		{
			message.Headers.Remove(Meta.RabbitMQ.Generic.Headers.QueueName);
			message.Headers.Remove(Meta.RabbitMQ.Generic.Headers.RoutingKey);
			message.Headers.Remove(Meta.RabbitMQ.Generic.Headers.Exchange);
			message.Headers.Remove(Meta.RabbitMQ.Generic.Headers.ExchangeType);
			message.Headers.Remove(Meta.RabbitMQ.Generic.Headers.Name);
			message.Headers.Remove(Meta.RabbitMQ.Generic.Headers.ProducerInitQueue);
		}

	}
}
