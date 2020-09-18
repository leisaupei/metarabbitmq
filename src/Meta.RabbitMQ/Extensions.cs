using Meta.RabbitMQ.Consumer;
using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Producer;
using Meta.RabbitMQ.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Meta.RabbitMQ.Extension
{
	public static class Extensions
	{
		/// <summary>
		/// 创建生产者服务
		/// 需要 AddConfigure<see cref="RabbitMQOptionCollection"/>
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddRabbitMQProducerService(this IServiceCollection services)
		{
			services.AddLogging();
			services.TryAddSingleton<IConnectionChannelPoolCollection, DefaultConnectionChannelPoolCollection>();
			services.TryAddSingleton<ISerializer, DefaultSerializer>();
			services.TryAddSingleton<IMessageProducer, DefaultMessageProducer>();
			return services;
		}
		/// <summary>
		/// 注入消费者服务
		/// 需要 AddConfigure<see cref="RabbitMQOptionCollection"/>
		/// </summary>
		/// <param name="services"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static IServiceCollection AddRabbitMQConsumerService(this IServiceCollection services, Action<ConsumerOptions> action = null)
		{
			services.AddOptions();
			services.AddLogging();
			if (action != null)
				services.Configure(action);
			services.TryAddSingleton<IConnectionChannelPoolCollection, DefaultConnectionChannelPoolCollection>();
			services.TryAddSingleton<IConsumerClientFactory, DefaultConsumerClientFactory>();
			services.TryAddSingleton<ISerializer, DefaultSerializer>();
			services.TryAddSingleton<IConsumerRegister, DefaultConsumerRegister>();
			return services;
		}
	}
}