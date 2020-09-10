using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Producer;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Meta.RabbitMQ.Consumer
{
	internal sealed class DefaultConsumerClient : IConsumerClient
	{
		private readonly static SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
		private readonly ClientOptions _clientOption;
		private readonly IConnectionChannelPool _connectionChannelPool;

		private readonly RabbitMQOption _rabbitMQOptions;
		private IConnection _connection;
		private IModel _channel;

		public DefaultConsumerClient(ClientOptions clientOption, IConnectionChannelPool connectionChannelPool, RabbitMQOption options)
		{
			_clientOption = clientOption;
			_connectionChannelPool = connectionChannelPool;
			_rabbitMQOptions = options;
		}

		public event EventHandler<Message<byte[]>> OnMessageReceived;

		public event EventHandler<LogMessageEventArgs> OnLog;


		public string HostAddress => _connectionChannelPool.HostAddress;

		public void Listening(TimeSpan timeout, CancellationToken cancellationToken)
		{
			Connect();

			_channel.QueueBind(_clientOption.QueueName, _clientOption.Exchange, _clientOption.RoutingKey);

			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += OnConsumerReceived;
			consumer.Shutdown += OnConsumerShutdown;
			consumer.Registered += OnConsumerRegistered;
			consumer.Unregistered += OnConsumerUnregistered;
			consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

			_channel.BasicConsume(_clientOption.QueueName, false, consumer);

			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();
				cancellationToken.WaitHandle.WaitOne(timeout);
			}
		}

		/// <summary>
		/// 确认接收
		/// </summary>
		/// <param name="sender"></param>
		public void Commit(object sender)
		{
			_channel.BasicAck((ulong)sender, true);
		}

		/// <summary>
		/// 拒绝接收, 重新放回队列
		/// </summary>
		/// <param name="sender"></param>
		public void Reject(object sender)
		{
			_channel.BasicReject((ulong)sender, true);
		}

		public void Dispose()
		{
			_channel?.Dispose();
			_connection?.Dispose();
		}

		public void Connect()
		{
			if (_channel != null)
			{
				return;
			}

			_connectionLock.Wait();

			try
			{
				if (_channel == null)
				{

					_connection = _connectionChannelPool.GetConnection();

					_channel = _connection.CreateModel();

					// 声明交换机
					_channel.ExchangeDeclare(_clientOption.Exchange, _clientOption.ExchangeType, true);
					var arguments = new Dictionary<string, object>();
					if (_rabbitMQOptions.QueueMessageExpires > 0)
						arguments.Add("x-message-ttl", _rabbitMQOptions.QueueMessageExpires);
					// 声明队列
					_channel.QueueDeclare(_clientOption.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
				}
			}
			finally
			{
				_connectionLock.Release();
			}
		}

		#region events

		private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
		{
			var args = new LogMessageEventArgs
			{
				EventType = EventType.ConsumerCancelled,
				Reason = string.Join(",", e.ConsumerTags)
			};
			OnLog?.Invoke(sender, args);
		}

		private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
		{
			var args = new LogMessageEventArgs
			{
				EventType = EventType.ConsumerUnregistered,
				Reason = string.Join(",", e.ConsumerTags)
			};
			OnLog?.Invoke(sender, args);
		}

		private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
		{
			var args = new LogMessageEventArgs
			{
				EventType = EventType.ConsumerRegistered,
				Reason = string.Join(",", e.ConsumerTags)
			};
			OnLog?.Invoke(sender, args);
		}

		private void OnConsumerReceived(object sender, BasicDeliverEventArgs e)
		{
			var headers = new Dictionary<string, string>();
			if (e.BasicProperties.Headers != null)
				foreach (var header in e.BasicProperties.Headers)
				{
					headers.Add(header.Key, header.Value == null ? null : Encoding.UTF8.GetString((byte[])header.Value));
				}
			headers.Add(Meta.RabbitMQ.Generic.Headers.QueueName, _clientOption.QueueName);

			var message = new Message<byte[]>(headers, e.Body.ToArray());

			OnMessageReceived?.Invoke(e.DeliveryTag, message);
		}

		private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
		{
			var args = new LogMessageEventArgs
			{
				EventType = EventType.ConsumerShutdown,
				Reason = e.ReplyText
			};

			OnLog?.Invoke(sender, args);
		}

		#endregion
	}
}