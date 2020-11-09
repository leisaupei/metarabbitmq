using Meta.RabbitMQ.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meta.RabbitMQ.Consumer
{
	public class ConsumerContext
	{
		public ConsumerContext(string hostAddress, ClientOption clientOption, Message message)
		{
			HostAddress = hostAddress;
			ClientOption = clientOption;
			Message = message;
		}

		public string HostAddress { get; }
		public ClientOption ClientOption { get; }
		public Message Message { get; }
	}
	public class ExceptionConsumerContext : ConsumerContext
	{
		public ExceptionConsumerContext(string hostAddress, ClientOption clientOption, Message message, Exception exception) : base(hostAddress, clientOption, message)
		{
			Exception = exception;
		}

		public Exception Exception { get; }
	}
}
