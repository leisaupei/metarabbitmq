using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Serialization;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Producer
{
	internal class DefaultMessageProducer : IMessageProducer
	{
		private readonly ILogger _logger;
		private readonly IConnectionChannelPoolCollection _connectionChannelPools;
		private readonly ISerializer _serializer;

		public DefaultMessageProducer(ILogger<DefaultMessageProducer> logger, IConnectionChannelPoolCollection connectionChannelPools, ISerializer serializer)
		{
			_logger = logger;
			_connectionChannelPools = connectionChannelPools;
			_serializer = serializer;
		}


		private Task<ProducerResult> SendAsync(Message<byte[]> message)
		{
			IModel channel = null;
			IConnectionChannelPool pool = null;
			try
			{
				_connectionChannelPools.TryGetValue(message.GetName() ?? "", out pool);
				channel = pool.GetChannel();
				//channel.BasicAcks += new EventHandler<BasicAckEventArgs>((o, basic) =>
				//{
				//	Console.WriteLine("d:/ack.txt", "\r\n调用了ack;DeliveryTag:" + basic.DeliveryTag.ToString() + ";Multiple:" + basic.Multiple.ToString() + "时间:" + DateTime.Now.ToString());
				//});
				//channel.BasicNacks += new EventHandler<BasicNackEventArgs>((o, basic) =>
				//{
				//	//MQ服务器出现了异常，可能会出现Nack的情况
				//	Console.WriteLine("d:/nack.txt", "\r\n调用了Nacks;DeliveryTag:" + basic.DeliveryTag.ToString() + ";Multiple:" + basic.Multiple.ToString() + "时间:" + DateTime.Now.ToString());
				//});

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
				props.Headers = message.Headers.ToDictionary(x => x.Key, x => (object)x.Value);
				// 发送消息
				channel.BasicPublish(exchage, routingKey, props, message.Body);

				//_logger.LogDebug($"RabbitMQ message [{exchage}-{routingKey}] has been published.");

				return Task.FromResult(ProducerResult.Success);
			}
			catch (Exception ex)
			{
				ProducerSentFailedException wrapperEx = new ProducerSentFailedException(ex.Message, ex);
				ProducerError errors = new ProducerError
				{
					Code = ex.HResult.ToString(),
					Description = ex.Message
				};

				return Task.FromResult(ProducerResult.Failed(wrapperEx, errors));
			}
			finally
			{
				if (channel != null && pool != null)
				{
					bool returned = pool.Return(channel);
					if (!returned)
					{
						channel.Dispose();
					}
				}
			}
		}

		public async Task<ProducerResult> SendAsync<T>(Message<T> message) where T : class, new()
		{
			return await SendAsync(await _serializer.SerializeAsync(message));
		}

		public async Task<ProducerResult> SendAsync(Message<string> message)
		{
			return await SendAsync(await _serializer.SerializeAsync(message));
		}
	}
}
