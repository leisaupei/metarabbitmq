using Meta.RabbitMQ.Generic;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Producer
{
	public interface IMessageProducer
	{
		/// <summary>
		/// Publish message.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		Task<ProducerResult> SendAsync(Message<byte[]> message);
	}
}