using Meta.RabbitMQ.Generic;
using System;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Consumer
{
	public interface IConsumerSubscriber
	{
		/// <summary>
		/// 客户端信息
		/// </summary>
		ClientOptions ClientOption { get; }

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