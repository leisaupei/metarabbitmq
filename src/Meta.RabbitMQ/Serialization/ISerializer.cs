using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Producer;
using System;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Serialization
{
	public interface ISerializer
	{
		/// <summary>
		/// 反序列化二进制位message对象, 用于订阅者接收消息
		/// </summary>
		/// <param name="transportMessage"></param>
		/// <param name="valueType"></param>
		/// <returns></returns>
		Task<Message> DeserializeAsync(Message<byte[]> transportMessage, Type valueType);

		/// <summary>
		/// 序列化传入对象, 用于生产者发送消息到RabbitMQ
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		Task<Message<byte[]>> SerializeAsync(Message message);

		/// <summary>
		/// 序列化类型为二进制
		/// </summary>
		/// <param name="messageBody"></param>
		/// <returns></returns>
		Task<byte[]> SerializeObjectToBytesAsync(object messageBody);

		/// <summary>
		/// 转化二进制message to string, 用于打印日志
		/// </summary>
		/// <param name="transportMessage"></param>
		/// <returns></returns>
		Task<string> ChangeMessageToStringAsync(Message<byte[]> transportMessage);

		/// <summary>
		/// 转化二进制messages to string, 用于打印日志
		/// </summary>
		/// <param name="transportMessages"></param>
		/// <returns></returns>
		Task<string> ChangeMessageToStringAsync(Messages<byte[]> transportMessages);

		/// <summary>
		/// 转化二进制messageBody to string
		/// </summary>
		/// <param name="messageBody"></param>
		/// <returns></returns>
		Task<string> SerializeObjectToStringAsync(object messageBody);
	}
}