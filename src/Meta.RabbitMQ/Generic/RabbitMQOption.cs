using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client;

namespace Meta.RabbitMQ.Generic
{
	public class RabbitMQOptionCollection
	{
		public List<RabbitMQOption> Options { get; private set; } = new List<RabbitMQOption>();
		/// <summary>
		/// 添加rabbitmq连接配置
		/// </summary>
		/// <param name="option"></param>
		/// <exception cref="ChannelPoolNameAlreadyExistsException">若存在名称相同的<see cref="RabbitMQOption.Name"/>则抛出此异常</exception>
		public void Add(RabbitMQOption option)
		{
			if (Options.Any(a => a.Name == option.Name))
				throw new ChannelPoolNameAlreadyExistsException(option.Name);
			Options.Add(option);
		}

		/// <summary>
		/// 添加rabbitmq连接配置, 若存在相同<see cref="RabbitMQOption.Name"/>则不添加
		/// </summary>
		/// <param name="option"></param>
		public void TryAdd(RabbitMQOption option)
		{
			if (!Options.Any(a => a.Name == option.Name))
				Options.Add(option);
		}
	}
	public class RabbitMQOption
	{
		public const string DefaultPassword = "guest";
		public const string DefaultUser = "guest";
		public const string DefaultVHost = "/";
		public const int DefaultPort = 5672;
		public const string DefaultHost = "localhost";
		public const string DefaultName = "";
		public const ushort DefaultChannelPoolSize = 50;
		public const int DefaultQueueMessageExpires = 864000000;
		public string HostName { get; set; } = DefaultHost;
		public string Password { get; set; } = DefaultPassword;
		public string UserName { get; set; } = DefaultUser;
		public int Port { get; set; } = DefaultPort;
		public string VirtualHost { get; set; } = DefaultVHost;

		/// <summary>
		/// 获取poolname 默认:""
		/// </summary>
		public string Name { get; set; } = DefaultName;

		/// <summary>
		/// 最大请求channel数量
		/// </summary>
		public ushort MaxChannelPoolSize { get; set; } = DefaultChannelPoolSize;

		/// <summary>
		/// 队列消息过期时间. Default 864000000 ms (10 天). 0 表示不添加TTL
		/// </summary>
		public int QueueMessageExpires { get; set; } = DefaultQueueMessageExpires;

		/// <summary>
		/// RabbitMQ native connection factory options
		/// </summary>
		public Action<ConnectionFactory> ConnectionFactoryOptions { get; set; }
	}
}