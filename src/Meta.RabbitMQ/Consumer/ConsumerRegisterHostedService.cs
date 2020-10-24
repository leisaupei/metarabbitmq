using Meta.RabbitMQ.Consumer;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meta.RabbitMQ.Consumer
{
	public class ConsumerRegisterHostedService : IHostedService
	{
		private readonly IConsumerRegister _consumerRegister;

		public ConsumerRegisterHostedService(IConsumerRegister consumerRegister)
		{
			_consumerRegister = consumerRegister;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_consumerRegister.Start();
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_consumerRegister.Cancel();
			return Task.CompletedTask;
		}
	}
}
