using Meta.RabbitMQ;
using Meta.RabbitMQ.Generic;
using Microsoft.Extensions.Options;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Meta.RabbitMQ.Consumer
{
	public sealed class DefaultConsumerClientFactory : IConsumerClientFactory
	{
		private readonly IConnectionChannelPoolCollection _connectionChannelPoolCollection;
		private readonly RabbitMQOptionCollection _rabbitMQOptions;

		public DefaultConsumerClientFactory(IOptions<RabbitMQOptionCollection> rabbitMQOptions, IConnectionChannelPoolCollection connectionChannelPoolCollection)
		{

			_rabbitMQOptions = rabbitMQOptions.Value;
			_connectionChannelPoolCollection = connectionChannelPoolCollection;
		}

		/// <summary>
		/// 创建订阅者
		/// </summary>
		/// <param name="clientOption"></param>
		/// <returns></returns>
		public IConsumerClient Create(ClientOptions clientOption)
		{
			try
			{
				_connectionChannelPoolCollection.TryGetValue(clientOption.Name, out IConnectionChannelPool pool);
				DefaultConsumerClient client = new DefaultConsumerClient(clientOption, pool, _rabbitMQOptions.Options.First(a => a.Name == clientOption.Name));
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