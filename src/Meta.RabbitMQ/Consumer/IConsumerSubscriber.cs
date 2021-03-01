using Meta.RabbitMQ.Generic;
using System;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Consumer
{
	public interface IConsumerSubscriber
	{
		/// <summary>
		/// 开启线程数量, 当值为0时取全局配置
		/// </summary>
		ushort ThreadCount { get; }

		/// <summary>
		/// 同时消费消息的数量, 同时消费线程数, 默认是1, 0则是无限制
		/// </summary>
		ushort PrefetchCount { get; }

		/// <summary>
		/// 抛出异常是否也确认消费消息, 默认: true, 选择false可能会产生大量日志, 清谨慎选择
		/// </summary>
		bool CommitIfAnyException { get; }

		/// <summary>
		/// 客户端信息
		/// </summary>
		ClientOption ClientOption { get; }

		/// <summary>
		/// 消息体类型
		/// </summary>
		/// <returns></returns>
		Type GetMessageType();

		/// <summary>
		/// 消息接收方法
		/// </summary>
		Task Invoke(Message message);
	}
}