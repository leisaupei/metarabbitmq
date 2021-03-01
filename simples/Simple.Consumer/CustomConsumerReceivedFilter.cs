using Meta.RabbitMQ.Consumer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simple.Consumer
{
	public class CustomConsumerReceivedFilter : ConsumerReceivedFilter
	{
		private readonly ILogger<CustomConsumerReceivedFilter> _logger;

		public CustomConsumerReceivedFilter(ILogger<CustomConsumerReceivedFilter> logger, ILoggerFactory loggerFactory) : base(loggerFactory)
		{
			_logger = logger;
		}

		public override Task OnSubscriberExceptionAsync(ExceptionConsumerContext context)
		{
			_logger.LogError(context.Exception, "");
			return Task.CompletedTask;
			//	return Task.CompletedTask;
		}

		public override Task OnSubscriberInvokingAsync(ConsumerContext context)
		{
			//Console.WriteLine("OnSubscriberInvokingAsync, " + JsonConvert.SerializeObject(context.Message));
			return Task.CompletedTask;
		}
	}
}
