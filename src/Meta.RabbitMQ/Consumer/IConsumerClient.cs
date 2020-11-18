using Meta.RabbitMQ.Generic;
using System;
using System.Threading;

namespace Meta.RabbitMQ.Consumer
{
	/// <summary>
	/// 消费者客户端
	/// </summary>
	public interface IConsumerClient : IDisposable
	{
		/// <summary>
		/// 客户端host
		/// </summary>
		string HostAddress { get; }

		/// <summary>
		/// 队列监听
		/// </summary>
		void Listening(TimeSpan timeout, CancellationToken cancellationToken, ushort prefetchCount);

		/// <summary>
		/// 消息确认接收
		/// </summary>
		/// <param name="sender">not null</param>
		void Commit(object sender);

		/// <summary>
		/// 消息拒收
		/// </summary>
		/// <param name="sender">can be null</param>
		void Reject(object sender);

		/// <summary>
		/// 消息接收事件
		/// </summary>
		event EventHandler<Message<byte[]>> OnMessageReceived;

		/// <summary>
		/// 日志事件
		/// </summary>
		event EventHandler<LogMessageEventArgs> OnLog;
	}
}