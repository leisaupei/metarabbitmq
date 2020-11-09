using Meta.RabbitMQ;
using Meta.RabbitMQ.Generic;
using Microsoft.Extensions.Options;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Meta.RabbitMQ.Consumer
{
	public sealed class ConsumerClientFactory : IConsumerClientFactory
	{
		private readonly IChannelPoolCollection _channelPoolCollection;
		private readonly RabbitMQOptionCollection _rabbitMQOptions;

		public ConsumerClientFactory(IOptions<RabbitMQOptionCollection> rabbitMQOptions, IChannelPoolCollection channelPoolCollection)
		{
			_rabbitMQOptions = rabbitMQOptions.Value;
			_channelPoolCollection = channelPoolCollection;
		}

		/// <summary>
		/// 创建订阅者
		/// </summary>
		/// <param name="clientOption"></param>
		/// <returns></returns>
		public IConsumerClient Create(ClientOption clientOption)
		{
			try
			{
				_channelPoolCollection.TryGetValue(clientOption.Name, out IChannelPool pool);
				ConsumerClient client = new ConsumerClient(clientOption, pool, _rabbitMQOptions.Options.First(a => a.Name == clientOption.Name));
				client.Connect();
				return client;
			}
			catch (System.Exception e)
			{
				throw new BrokerConnectionException(e);
			}
		}
	}
}