using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Producer;
using System;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Serialization
{
	public interface ISerializer
	{
		Task<Message> DeserializeAsync(Message<byte[]> transportMessage, Type valueType);
		Task<Message<byte[]>> SerializeAsync(Message message);
		Task<Message<T>> DeserializeAsync<T>(Message<byte[]> transportMessage);
	}
}