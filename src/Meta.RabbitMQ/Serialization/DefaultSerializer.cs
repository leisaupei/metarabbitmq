using Meta.RabbitMQ.Generic;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Serialization
{
	/// <summary>
	/// default serializer with utf-8 encoding
	/// </summary>
	public class DefaultSerializer : ISerializer
	{
		public Task<Message<byte[]>> SerializeAsync(Message message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			if (message.Body == null)
			{
				return Task.FromResult(new Message<byte[]>(message.Headers, null));
			}
			string content = message.Body is string ? message.Body as string : JsonConvert.SerializeObject(message.Body);
			return Task.FromResult(new Message<byte[]>(message.Headers, Encoding.UTF8.GetBytes(content)));
		}

		public Task<Message> DeserializeAsync(Message<byte[]> transportMessage, Type valueType)
		{
			if (valueType == null || transportMessage.Body == null)
			{
				return Task.FromResult(new Message(transportMessage.Headers, null));
			}
			var json = Encoding.UTF8.GetString(transportMessage.Body);

			if (valueType == typeof(string))
				return Task.FromResult(new Message(transportMessage.Headers, json));
			return Task.FromResult(new Message(transportMessage.Headers, JsonConvert.DeserializeObject(json, valueType)));
		}
	}
}