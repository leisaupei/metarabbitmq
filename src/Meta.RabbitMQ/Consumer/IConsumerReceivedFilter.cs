﻿using Meta.RabbitMQ.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Consumer
{
	public interface IConsumerReceivedFilter
	{
		/// <summary>
		/// 执行subscriber.invoke方法之前执行
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		Task OnSubscriberInvokingAsync(ConsumerContext message);

		/// <summary>
		/// 当subscriber报错时执行
		/// </summary>
		/// <param name="exceptionConsumerContext"></param>
		/// <returns></returns>
		Task OnSubscriberExceptionAsync(ExceptionConsumerContext exceptionConsumerContext);
	}
}
