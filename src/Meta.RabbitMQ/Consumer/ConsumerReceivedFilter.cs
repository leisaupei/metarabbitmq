using Meta.RabbitMQ.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Consumer
{
	public class ConsumerReceivedFilter : IConsumerReceivedFilter
	{
		protected readonly ILogger _logger;

		public ConsumerReceivedFilter(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<ConsumerReceivedFilter>();
		}

		public virtual Task OnSubscriberExceptionAsync(ExceptionConsumerContext context)
		{
			_logger.LogError(context.Exception, "An exception occurred when process received message. host: '{0}', client:'{1}' Message:'{2}'.",
				context.HostAddress,
				context.ClientOption.ToString(),
				JsonConvert.SerializeObject(context.Message));
			return Task.CompletedTask;
		}

		public virtual Task OnSubscriberInvokingAsync(ConsumerContext context)
		{
			_logger.LogDebug($"Received message. host: {context.HostAddress} client: {context.ClientOption}, body: {JsonConvert.SerializeObject(context)}");
			return Task.CompletedTask;
		}
	}
}
