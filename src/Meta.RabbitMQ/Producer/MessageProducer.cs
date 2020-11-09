using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Serialization;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Producer
{
	internal class MessageProducer : IMessageProducer
	{
		private readonly ILogger _logger;
		private readonly IChannelPoolCollection _channelPools;
		private readonly ISerializer _serializer;

		public MessageProducer(ILogger<MessageProducer> logger, IChannelPoolCollection channelPools, ISerializer serializer)
		{
			_logger = logger;
			_channelPools = channelPools;
			_serializer = serializer;
		}

		private async Task<ProducerResult> SendAsync(Message<byte[]> message)
		{
			IModel channel = null;
			IChannelPool pool = null;
			try
			{
				_channelPools.TryGetValue(message.GetName() ?? "", out pool);
				channel = pool.GetChannel();
				IBasicProperties props = channel.CreateBasicProperties();
				props.DeliveryMode = 2;

				string exchage = message.GetExchange() ?? throw new ArgumentNullException("the exchange is null.");
				string routingKey = message.GetRoutingKey() ?? "";

				if (message.IsInitQueue())
				{
					// 声明一个交换机
					string exchangeType = message.GetExchangeType() ?? throw new ArgumentNullException("the exchange type is null.");
					channel.ExchangeDeclare(exchage, exchangeType, true);
					// 声明一个队列 然后把队列和交换机绑定起来
					string queue = message.GetQueueName();
					channel.QueueDeclare(queue, true, false, false);
					channel.QueueBind(queue, exchage, routingKey);
				}
				message.Remove();

				TransferHeaderProperties(props, message.Headers);

				// 发送消息
				channel.BasicPublish(exchage, routingKey, props, message.Body);

				_logger.LogDebug($"RabbitMQ message [{exchage}-{routingKey}] has been published.");

				return ProducerResult.Success;
			}
			catch (Exception ex)
			{
				ProducerSentFailedException wrapperEx = new ProducerSentFailedException(ex.Message, ex);
				ProducerError errors = new ProducerError
				{
					Code = ex.HResult.ToString(),
					Description = ex.Message,
					HostAddress = pool?.HostAddress,
					Body = (await _serializer.DeserializeAsync(message, typeof(string))).Body as string
				};

				return ProducerResult.Failed(wrapperEx, errors);
			}
			finally
			{
				if (channel != null && pool != null)
					if (!pool.Return(channel))
						channel.Dispose();
			}
		}

		private void TransferHeaderProperties(IBasicProperties props, IDictionary<string, string> msgHeader)
		{
			if (msgHeader.Count == 0) return;
			props.Headers = new Dictionary<string, object>();
			foreach (var item in msgHeader)
			{
				switch (item.Key)
				{
					case Generic.Headers.Expiration:
						props.Expiration = item.Value;
						break;
					default:
						props.Headers.Add(item.Key, item.Value);
						break;
				}
			}
		}

		public async Task<ProducerResult> SendAsync<T>(Message<T> message)
		{
			return await SendAsync(await _serializer.SerializeAsync(message));
		}

		public async Task<ProducerResult> SendAsync(Message<string> message)
		{
			return await SendAsync(await _serializer.SerializeAsync(message));
		}
	}
}
