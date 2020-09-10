﻿using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Serialization;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Producer
{
	/// <summary>
	/// 一个virtual host,一个类的抽象
	/// </summary>
	public abstract class MessageProducerBase
	{
		protected readonly IMessageProducer _messageProducer;
		protected readonly ISerializer _serializer;

		/// <summary>
		/// 如果使用<see cref="IConnectionChannelPoolCollection"/>注入, 此项需要override
		/// </summary>
		protected virtual string Name { get; } = string.Empty;
		protected MessageProducerBase(IMessageProducer messageProducer, ISerializer serializer)
		{
			_messageProducer = messageProducer;
			_serializer = serializer;
		}
		/// <summary>
		/// 发送消息
		/// </summary>
		/// <param name="model">消息模型 按照<see cref="ISerializer"/>规则序列化</param>
		/// <param name="exchange">交换机</param>
		/// <param name="routingKey"></param>
		/// <param name="header">消息头部参数<see cref="IBasicProperties.Headers"/> 或者 <see cref="Generic.Headers"/></param>
		/// <returns></returns>
		public async Task<ProducerResult> SendMessageAsync<T>(T model, string exchange, string routingKey, IDictionary<string, string> header = null) where T : class, new()
		{
			IDictionary<string, string> innerHeader = GetMessageHeader(exchange, routingKey, header);
			var producerMsg = await _serializer.SerializeAsync(new Message<T>(innerHeader, model));
			return await _messageProducer.SendAsync(producerMsg);
		}

		/// <summary>
		/// 发送消息
		/// </summary>
		/// <param name="content">消息字符串</param>
		/// <param name="exchange">交换机</param>
		/// <param name="routingKey"></param>
		/// <param name="header">消息头部参数<see cref="IBasicProperties.Headers"/> 或者 <see cref="Generic.Headers"/></param>
		/// <returns></returns>
		public async Task<ProducerResult> SendMessageAsync(string content, string exchange, string routingKey, IDictionary<string, string> header = null)
		{
			IDictionary<string, string> innerHeader = GetMessageHeader(exchange, routingKey, header);
			var producerMsg = await _serializer.SerializeAsync(new Message<string>(innerHeader, content));
			return await _messageProducer.SendAsync(producerMsg);
		}

		private IDictionary<string, string> GetMessageHeader(string exchange, string routingKey, IDictionary<string, string> header)
		{
			IDictionary<string, string> innerHeader = new Dictionary<string, string>
			{
				{ Generic.Headers.Exchange, exchange },
				{ Generic.Headers.RoutingKey, routingKey },
				{ Generic.Headers.Name, Name }
			};
			if (header != null)
				foreach (var item in header)
					innerHeader.Add(item);
			return innerHeader;
		}
	}
}