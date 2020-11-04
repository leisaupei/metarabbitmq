﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meta.RabbitMQ.Generic;
using Meta.RabbitMQ.Producer;
using Meta.RabbitMQ.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Meta.RabbitMQ.Consumer
{

	public class DefaultConsumerRegister : IConsumerRegister
	{
		private readonly ILogger _logger;

		private readonly IConsumerClientFactory _consumerClientFactory;
		private readonly ISerializer _serializer;
		private readonly IEnumerable<IConsumerSubscriber> _subscribers;
		private readonly TimeSpan _pollingDelay = TimeSpan.FromSeconds(1);
		private readonly ConsumerOptions _options;
		private CancellationTokenSource _cts;
		private Task _compositeTask;
		private bool _disposed;
		public DefaultConsumerRegister(ILogger<DefaultConsumerRegister> logger, IServiceProvider serviceProvider)
		{
			_logger = logger;
			_consumerClientFactory = serviceProvider.GetService<IConsumerClientFactory>();
			_serializer = serviceProvider.GetService<ISerializer>();
			_subscribers = serviceProvider.GetService<IEnumerable<IConsumerSubscriber>>();
			_options = serviceProvider.GetService<IOptions<ConsumerOptions>>().Value;
			_cts = new CancellationTokenSource();
		}

		public void Start()
		{
			if (_subscribers.Count() == 0)
				throw new NoSubscriberException();
			if (_options.SubscribeThreadCount == 0)
				throw new ArgumentException("the parameter SubscribeThreadCount must be great than 0.");

			foreach (var subscriber in _subscribers)
			{
				var count = subscriber.ThreadCount > 0 ? subscriber.ThreadCount : _options.SubscribeThreadCount;
				for (int i = 0; i < count; i++)
				{
					Task.Factory.StartNew(() =>
					{
						try
						{
							using (var client = _consumerClientFactory.Create(subscriber.ClientOption))
							{
								RegisterMessageProcessor(client, subscriber);

								client.Listening(_pollingDelay, _cts.Token);
							}
						}
						catch (OperationCanceledException)
						{
							//ignore
						}
						catch (BrokerConnectionException e)
						{
							_logger.LogError(e, e.Message);
						}
						catch (Exception e)
						{
							_logger.LogError(e, e.Message);
						}
					}, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
				}
			}
			_compositeTask = Task.CompletedTask;
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			try
			{
				Cancel();

				_compositeTask?.Wait(TimeSpan.FromSeconds(2));
			}
			catch (AggregateException ex)
			{
				var innerEx = ex.InnerExceptions[0];
				if (!(innerEx is OperationCanceledException))
				{
					_logger.LogWarning(ex, $"Expected an OperationCanceledException, but found '{ex.Message}'.");
				}
			}
		}

		public void Cancel()
		{
			_cts?.Cancel();
		}

		private void RegisterMessageProcessor(IConsumerClient client, IConsumerSubscriber subscriber)
		{
			client.OnMessageReceived += async (sender, transportMessage) =>
			{
				if (_options.ShowReceivedMessage)
					_logger.LogInformation($"Received message.host: {client.HostAddress} client: {subscriber.ClientOption}, body: {(await _serializer.DeserializeAsync(transportMessage, typeof(string))).Body}");

				else
					_logger.LogDebug($"Received message.host: {client.HostAddress} client: {subscriber.ClientOption}, body: {(await _serializer.DeserializeAsync(transportMessage, typeof(string))).Body}");

				try
				{
					try
					{
						Message message = await _serializer.DeserializeAsync(transportMessage, subscriber.GetMessageType());
						await subscriber.Invoke(message);
					}
					catch (Exception e)
					{
						throw e;
					}

					client.Commit(sender);
				}
				catch (Exception e)
				{
					_logger.LogError(e, "An exception occurred when process received message.host: '{0}', client:'{1}' Message:'{2}'.", client.HostAddress, subscriber.ClientOption.ToString(), (await _serializer.DeserializeAsync(transportMessage, typeof(string))).Body);

					if (subscriber.CommitIfAnyException)
						client.Commit(sender);
					else
						client.Reject(sender);
				}
			};

			client.OnLog += WriteLog;
		}

		private void WriteLog(object sender, LogMessageEventArgs logmsg)
		{
			switch (logmsg.EventType)
			{
				case EventType.ConsumerCancelled:
					_logger.LogWarning("RabbitMQ consumer cancelled. --> " + logmsg.Reason);
					break;
				case EventType.ConsumerRegistered:
					_logger.LogInformation("RabbitMQ consumer registered. --> " + logmsg.Reason);
					break;
				case EventType.ConsumerUnregistered:
					_logger.LogWarning("RabbitMQ consumer unregistered. --> " + logmsg.Reason);
					break;
				case EventType.ConsumerShutdown:
					_logger.LogWarning("RabbitMQ consumer shutdown. --> " + logmsg.Reason);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}