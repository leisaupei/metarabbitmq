using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace Meta.RabbitMQ.Generic
{
	public class RabbitMQOptionCollection
	{
		public List<RabbitMQOption> Options { get; private set; } = new List<RabbitMQOption>();
		public void Add(RabbitMQOption option)
		{
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