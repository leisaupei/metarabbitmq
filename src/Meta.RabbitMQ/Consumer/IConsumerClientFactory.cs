using Meta.RabbitMQ.Generic;

namespace Meta.RabbitMQ.Consumer
{
	/// <summary>
	/// Consumer client factory to create consumer client instance.
	/// </summary>
	public interface IConsumerClientFactory
	{
		/// <summary>
		/// Create a new instance of <see cref="IConsumerClient" />.
		/// </summary>
		/// <param name="clientOption">消费者客户端配置</param>
		IConsumerClient Create(ClientOptions clientOption);
	}
}