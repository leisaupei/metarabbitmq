using Meta.RabbitMQ.Consumer;
using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Producer;
using Meta.RabbitMQ.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
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
		/// 创建生产者服务, 需要 AddConfigure<see cref="RabbitMQOptionCollection"/>
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddRabbitMQProducerService(this IServiceCollection services)
		{
			services.AddLogging();
			services.TryAddSingleton<IChannelPoolCollection, ChannelPoolCollection>();
			services.TryAddSingleton<ISerializer, DefaultSerializer>();
			services.TryAddSingleton<IMessageProducer, MessageProducer>();
			return services;
		}

		/// <summary>
		/// 注入消费者服务, 需要 AddConfigure <see cref="RabbitMQOptionCollection"/>
		/// 用于需要手动注入 <see cref="IConsumerRegister"/>;
		/// </summary>
		/// <param name="services"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static IServiceCollection AddRabbitMQConsumer(this IServiceCollection services, Action<ConsumerOptions> action = null)
		{
			services.AddLogging();
			if (action != null)
				services.Configure(action);

			var options = new ConsumerOptions();
			action?.Invoke(options);

			if (options.ConsumerReceiveFilterType != null)
				services.TryAddSingleton(typeof(IConsumerReceivedFilter), options.ConsumerReceiveFilterType);
			services.TryAddSingleton<IConsumerReceivedFilter, ConsumerReceivedFilter>();
			services.TryAddSingleton<IChannelPoolCollection, ChannelPoolCollection>();
			services.TryAddSingleton<IConsumerClientFactory, ConsumerClientFactory>();
			services.TryAddSingleton<ISerializer, DefaultSerializer>();
			return services;
		}

		/// <summary>
		/// 注入消费者服务, 需要 AddConfigure<see cref="RabbitMQOptionCollection"/>
		/// </summary>
		/// <param name="services"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static IServiceCollection AddRabbitMQConsumerHostedService(this IServiceCollection services, Action<ConsumerOptions> action = null)
		{
			services.AddRabbitMQConsumer(action);
			services.TryAddSingleton<IConsumerRegister, ConsumerRegister>();
			services.AddHostedService<ConsumerRegisterHostedService>();
			return services;
		}
	}
}