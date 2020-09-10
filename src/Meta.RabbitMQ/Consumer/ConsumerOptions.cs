using System;
using System.Collections.Generic;
using System.Text;

namespace Meta.RabbitMQ.Consumer
{
	public class ConsumerOptions
	{
		public const ushort DefaultSubscribeThreadCount = 1;
		public const bool DefaultShowDebugReceivedMessage = false;
		public const bool DefaultCommitIfAnyException = true;
		/// <summary>
		/// 消费线程数量, 默认: 1
		/// </summary>
		public ushort SubscribeThreadCount { get; set; } = DefaultSubscribeThreadCount;
		/// <summary>
		/// 调试接收消息, ilogger使用debug级别打印消息, 默认: false
		/// </summary>
		public bool ShowDebugReceivedMessage { get; set; } = DefaultShowDebugReceivedMessage;
		/// <summary>
		/// 处理消息出现异常是否确认, 默认: true
		/// </summary>
		public bool CommitIfAnyException { get; set; } = DefaultCommitIfAnyException;
	}
}
