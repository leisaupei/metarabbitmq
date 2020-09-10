﻿using Meta.RabbitMQ.Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Meta.RabbitMQ.Generic;

namespace Simple.Consumer
{
	public class SimpleSubscriber : ConsumerSubscriberBase<string>
	{
		public string Exchange => "test.ex.v1";

		public string RoutingKey => "test.rk.v1";

		public string ExchangeType => RabbitMQ.Client.ExchangeType.Direct;

		public string Queue => "test.queue.v1";

		public string Name => "v1";

		public override ClientOptions ClientOption => new ClientOptions(Exchange, RoutingKey, ExchangeType, Queue, Name);

		public override Task Invoke(Message<string> message)
		{
			Console.WriteLine(message.Body);
			return Task.CompletedTask;
		}
	}
	public class SimpleSubscriber2 : ConsumerSubscriberBase<string>
	{
		public string Exchange => "test.ex.v1";

		public string RoutingKey => "test.rk.v2";

		public string ExchangeType => RabbitMQ.Client.ExchangeType.Direct;

		public string Queue => "test.queue.v2";

		public string Name => "v1";

		public override ClientOptions ClientOption => new ClientOptions(Exchange, RoutingKey, ExchangeType, Queue, Name);
		public override Task Invoke(Message<string> message)
		{
			return Task.Run(() => Console.WriteLine(message.Body));
		}
	}
}
