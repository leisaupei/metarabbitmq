using Meta.RabbitMQ.Generic;
using System;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Consumer
{
	/// <summary>
	/// 泛型实现接口
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ConsumerSubscriberBase<T> : IConsumerSubscriber
	{
		/// <summary>
		/// 抽象实现客户端配置
		/// </summary>
		public abstract ClientOptions ClientOption { get; }

		/// <summary>
		/// 使用泛型转换的语法糖, 若不需要, 直接实现<see cref="IConsumerSubscriber"/>即可
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public abstract Task Invoke(Message<T> message);

		/// <summary>
		/// 获取typeof(T)
		/// </summary>
		/// <returns></returns>
		public Type GetMessageType() => typeof(T);

		/// <summary>
		/// 转换消息类型
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public Task Invoke(Message message) => Invoke(message.ToMessage<T>());

	}

}