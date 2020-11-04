﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Meta.RabbitMQ.Consumer
{
	public class ConsumerOptions
	{
		public const ushort DefaultSubscribeThreadCount = 1;
		public const bool DefaultShowReceivedMessage = false;

		/// <summary>
		/// 消费线程数量, 默认: 1
		/// </summary>
		public ushort SubscribeThreadCount { get; set; } = DefaultSubscribeThreadCount;
		/// <summary>
		/// 打印接收消息, ilogger使用information级别打印消息, 默认: false
		/// </summary>
		public bool ShowReceivedMessage { get; set; } = DefaultShowReceivedMessage;
		/// <summary>
		/// 处理消息出现异常是否确认, 默认: true
		/// </summary>
		[Obsolete("取消全局配置, 改为实现IConsumerSubscriber.CommitIfAnyException变量")]
		public bool CommitIfAnyException { get; set; } = true;
	}
}
