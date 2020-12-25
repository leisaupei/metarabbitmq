using Meta.RabbitMQ.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meta.RabbitMQ.Consumer
{
	public class ConsumerContext
	{
		public ConsumerContext(string hostAddress, ClientOption clientOption, Message message, Message<byte[]> transportMessage)
		{
			HostAddress = hostAddress;
			ClientOption = clientOption;
			Message = message;
			TransportMessage = transportMessage;
		}

		public string HostAddress { get; }
		public ClientOption ClientOption { get; }
		public Message Message { get; }
		public Message<byte[]> TransportMessage { get; }
	}
	public class ExceptionConsumerContext : ConsumerContext
	{
		public ExceptionConsumerContext(string hostAddress, ClientOption clientOption, Message message, Message<byte[]> transportMessage, Exception exception)
			: base(hostAddress, clientOption, message, transportMessage) => Exception = exception;

		public Exception Exception { get; }
	}
}
