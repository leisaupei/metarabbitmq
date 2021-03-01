using System;
using System.Collections.Generic;
using System.Text;

namespace Meta.RabbitMQ.Consumer
{
	public class ConsumerOptions
	{
		public const ushort DefaultSubscribeThreadCount = 1;
		internal Type ConsumerReceiveFilterType { get; private set; }
		/// <summary>
		/// 消费线程数量, 默认: 1
		/// </summary>
		public ushort SubscribeThreadCount { get; set; } = DefaultSubscribeThreadCount;

		/// <summary>
		/// 消费者过滤器
		/// </summary>
		/// <typeparam name="TFilter"></typeparam>
		public void ConsumerReceiveFilter<TFilter>() where TFilter : IConsumerReceivedFilter
		{
			ConsumerReceiveFilterType = typeof(TFilter);
		}
	}
}
