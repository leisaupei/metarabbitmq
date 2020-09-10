using System;
using System.Collections.Generic;
using System.Text;

namespace Meta.RabbitMQ.Generic
{
	public enum EventType
	{
		/// <summary>
		/// 消费者取消事件
		/// </summary>
		ConsumerCancelled,
		/// <summary>
		/// 消费者注册事件
		/// </summary>
		ConsumerRegistered,
		/// <summary>
		/// 消费者注销事件
		/// </summary>
		ConsumerUnregistered,
		/// <summary>
		/// 消费者关闭事件
		/// </summary>
		ConsumerShutdown,
	}

	/// <summary>
	/// 日志消息时间
	/// </summary>
	public class LogMessageEventArgs : EventArgs
	{
		public string Reason { get; set; }

		public EventType EventType { get; set; }
	}
}
