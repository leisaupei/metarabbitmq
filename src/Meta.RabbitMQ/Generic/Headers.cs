using System;
using System.Collections.Generic;
using System.Text;

namespace Meta.RabbitMQ.Generic
{
	public static class Headers
	{
		/// <summary>
		/// 队列名称
		/// </summary>
		public const string QueueName = "meta-queue-name";

		/// <summary>
		/// routing kee
		/// </summary>
		public const string RoutingKey = "meta-routing-key";

		/// <summary>
		/// 交换机
		/// </summary>
		public const string Exchange = "meta-exchange";

		/// <summary>
		/// 交换机类型
		/// </summary>
		public const string ExchangeType = "meta-exchange-type";

		/// <summary>
		/// 连接名称
		/// </summary>
		public const string Name = "meta-pool-name";

		/// <summary>
		/// 这个参数用于是否在生产者创建队列初始化操作, null为执行, 否则不执行
		/// </summary>
		public const string ProducerInitQueue = "meta-producer-init";


		public const string Exception = "meta-exception";
	}
}
