using System;

namespace Meta.RabbitMQ.Consumer
{
	public interface IConsumerRegister : IDisposable
	{
		/// <summary>
		/// 取消服务
		/// </summary>
		void Cancel();

		/// <summary>
		/// 启动订阅服务
		/// </summary>
		/// <exception cref="NoSubscriberException">需要注入<see cref="IConsumerSubscriber"/></exception>
		/// <exception cref="ArgumentException">当<see cref="ConsumerOptions.SubscribeThreadCount"/>等于0</exception>
		void Start();
	}
}