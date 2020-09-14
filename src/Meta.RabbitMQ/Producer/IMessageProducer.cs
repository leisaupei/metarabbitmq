using Meta.RabbitMQ.Generic;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Producer
{
	public interface IMessageProducer
	{
		/// <summary>
		/// 发送class类型消息, 使用<see cref="Serialization.ISerializer"/>序列化
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		Task<ProducerResult> SendAsync<T>(Message<T> message) where T : class, new();

		/// <summary>
		/// 发送字符串类型消息, 使用<see cref="Serialization.ISerializer"/>序列化
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		Task<ProducerResult> SendAsync(Message<string> message);
	}
}