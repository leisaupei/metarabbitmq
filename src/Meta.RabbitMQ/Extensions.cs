using Meta.RabbitMQ.Consumer;
using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Producer;
using Meta.RabbitMQ.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
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
			services.AddSingleton<IConnectionChannelPoolCollection, DefaultConnectionChannelPoolCollection>();
			services.AddSingleton<ISerializer, DefaultSerializer>();
			services.AddSingleton<IMessageProducer, DefaultMessageProducer>();
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
			if (action == null)
				services.Configure<ConsumerOptions>(option =>
				{
					option.CommitIfAnyException = ConsumerOptions.DefaultCommitIfAnyException;
					option.ShowDebugReceivedMessage = ConsumerOptions.DefaultShowDebugReceivedMessage;
					option.SubscribeThreadCount = ConsumerOptions.DefaultSubscribeThreadCount;
				});
			else
				services.Configure(action);
			services.AddSingleton<IConnectionChannelPoolCollection, DefaultConnectionChannelPoolCollection>();
			services.AddSingleton<IConsumerClientFactory, DefaultConsumerClientFactory>();
			services.AddSingleton<ISerializer, DefaultSerializer>();
			services.AddSingleton<IConsumerRegister, DefaultConsumerRegister>();
			return services;
		}
		private static FileConfigurationProvider ProviderCreator { get; set; } = new JsonConfigurationProvider(new JsonConfigurationSource { Optional = true });

		/// <summary>
		/// 注入json字符串
		/// </summary>
		/// <typeparam name="TOptions"></typeparam>
		/// <param name="services"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		public static IServiceCollection ConfigureJsonValue<TOptions>(this IServiceCollection services, IConfigurationSection section) where TOptions : class
		{
			return services.ConfigureJsonValue<TOptions>(Options.DefaultName, section);
		}

		public static IServiceCollection ConfigureJsonValue<TOptions>(this IServiceCollection services, string name, IConfigurationSection section) where TOptions : class
		{
			services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(new ConfigurationChangeTokenSource<TOptions>(name, section))
				.Configure<TOptions>(name, options =>
				{
					var root = new ConfigurationRoot(new List<IConfigurationProvider> { ProviderCreator });

					using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(section.Value)))
						ProviderCreator.Load(stream);

					root.Bind(options);
				});
			return services;
		}
	}
}