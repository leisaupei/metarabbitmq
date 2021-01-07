using Meta.RabbitMQ.Generic;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Serialization
{
	/// <summary>
	/// default serializer with utf-8 encoding
	/// </summary>
	public class DefaultSerializer : ISerializer
	{
		public virtual async Task<Message<byte[]>> SerializeAsync(Message message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			var body = await SerializeObjectToBytesAsync(message.Body);

			return new Message<byte[]>(message.Headers, body);
		}

		public virtual Task<Message> DeserializeAsync(Message<byte[]> transportMessage, Type valueType)
		{
			if (valueType == null || transportMessage.Body == null)
			{
				return Task.FromResult(new Message(transportMessage.Headers, null));
			}
			var json = Encoding.UTF8.GetString(transportMessage.Body);

			if (valueType == typeof(string))
			{
				return Task.FromResult(new Message(transportMessage.Headers, json));
			}
			return Task.FromResult(new Message(transportMessage.Headers, JsonConvert.DeserializeObject(json, valueType)));
		}

		//private Task<Messages<T>> DeserializeAsync<T>(Messages<byte[]> transportMessage)
		//{
		//	if (!(transportMessage.Body?.Any() ?? false))
		//	{
		//		return Task.FromResult(new Messages<T>(transportMessage.Headers, new T[0]));
		//	}
		//	int length = transportMessage.Body.Count();

		//	T[] messages = new T[length];
		//	var isString = typeof(T) == typeof(string);
		//	for (int i = 0; i < length; i++)
		//	{
		//		object json = Encoding.UTF8.GetString(transportMessage.Body.ElementAt(i));
		//		if (isString)
		//		{
		//			messages[i] = (T)json;
		//		}
		//		else
		//		{
		//			messages[i] = JsonConvert.DeserializeObject<T>((string)json);
		//		}

		//	}
		//	return Task.FromResult(new Messages<T>(transportMessage.Headers, messages));
		//}

		public virtual Task<byte[]> SerializeObjectToBytesAsync(object messageBody)
		{
			if (messageBody == null)
			{
				return null;
			}

			string content = messageBody is string ? messageBody as string : JsonConvert.SerializeObject(messageBody);

			return Task.FromResult(Encoding.UTF8.GetBytes(content));
		}

		public virtual async Task<string> ChangeMessageToStringAsync(Message<byte[]> transportMessage)
		{
			var message = await DeserializeAsync(transportMessage, typeof(string));

			return await SerializeObjectToStringAsync(message);
		}

		public virtual Task<string> ChangeMessageToStringAsync(Messages<byte[]> transportMessages)
		{
			Message message;
			if (transportMessages.Body?.Any() ?? false)
			{
				int length = transportMessages.Body.Count();

				string[] bodies = new string[length];

				for (int i = 0; i < length; i++)
					bodies[i] = Encoding.UTF8.GetString(transportMessages.Body.ElementAt(i));
				message = new Message(transportMessages.Headers, bodies);
			}
			else
				message = transportMessages;

			return SerializeObjectToStringAsync(message);
		}

		public virtual Task<string> SerializeObjectToStringAsync(object messageBody)
		{
			if (messageBody is string)
				return Task.FromResult(messageBody.ToString());

			return Task.FromResult(JsonConvert.SerializeObject(messageBody));
		}
	}
}