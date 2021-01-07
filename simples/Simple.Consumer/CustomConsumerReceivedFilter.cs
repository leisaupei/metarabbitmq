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
		public CustomConsumerReceivedFilter(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
		}

		public override Task OnSubscriberExceptionAsync(ExceptionConsumerContext context)
		{
			Console.WriteLine("OnSubscriberExceptionAsync, " + context.Exception.Message);
			return Task.CompletedTask;
		}

		public override Task OnSubscriberInvokingAsync(ConsumerContext context)
		{
			//Console.WriteLine("OnSubscriberInvokingAsync, " + JsonConvert.SerializeObject(context.Message));
			return Task.CompletedTask;
		}
	}
}
