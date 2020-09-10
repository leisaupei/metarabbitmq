using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Meta.RabbitMQ.Generic
{

	public class DefaultConnectionChannelPool : IConnectionChannelPool
	{
		public string HostAddress { get; }
		private readonly Func<IConnection> _connectionActivator;
		private readonly ILogger _logger;
		private readonly ConcurrentQueue<IModel> _pool;
		private IConnection _connection;
		private static readonly object SLock = new object();

		private readonly static SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
		private int _count;
		private int _maxSize;

		//public DefaultConnectionChannelPool(ILogger<DefaultConnectionChannelPool> logger, IOptions<RabbitMQOption> optionsAccessor) : this(logger, optionsAccessor.Value)
		//{
		//}
		internal DefaultConnectionChannelPool(ILogger logger, RabbitMQOption options)
		{
			_logger = logger;
			_maxSize = options.MaxChannelPoolSize;
			_pool = new ConcurrentQueue<IModel>();

			_connectionActivator = CreateConnection(options);

			HostAddress = $"{options.HostName}:{options.Port}";

			_logger.LogDebug($"RabbitMQ configuration:'HostName:{options.HostName}, Port:{options.Port}");
		}

		IModel IConnectionChannelPool.GetChannel()
		{
			lock (SLock)
			{
				//_connectionLock.Wait();
				//try
				//{
				while (_count > _maxSize)
				{
					Thread.SpinWait(1);
				}
				return GetChannel();
				//}
				//finally
				//{
				//	_connectionLock.Release();
				//}
			}
		}

		bool IConnectionChannelPool.Return(IModel connection)
		{
			return Return(connection);
		}


		public IConnection GetConnection()
		{
			if (_connection != null && _connection.IsOpen)
			{
				return _connection;
			}

			_connection = _connectionActivator();
			_connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
			return _connection;
		}

		public void Dispose()
		{
			_maxSize = 0;

			while (_pool.TryDequeue(out var context))
			{
				context.Dispose();
			}
		}

		private static Func<IConnection> CreateConnection(RabbitMQOption options)
		{
			var serviceName = Assembly.GetEntryAssembly()?.GetName().Name.ToLower();

			var factory = new ConnectionFactory
			{
				UserName = options.UserName,
				Port = options.Port,
				Password = options.Password,
				VirtualHost = options.VirtualHost
			};

			if (options.HostName.Contains(","))
			{
				options.ConnectionFactoryOptions?.Invoke(factory);
				return () => factory.CreateConnection(options.HostName.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries), serviceName);
			}

			factory.HostName = options.HostName;
			options.ConnectionFactoryOptions?.Invoke(factory);
			return () => factory.CreateConnection(serviceName);
		}

		private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
		{
			_logger.LogWarning($"RabbitMQ client connection closed! --> {e.ReplyText}");
		}

		public virtual IModel GetChannel()
		{
			if (_pool.TryDequeue(out var model))
			{
				Interlocked.Decrement(ref _count);

				Debug.Assert(_count >= 0);

				return model;
			}

			try
			{
				model = GetConnection().CreateModel();
			}
			catch (Exception e)
			{
				_logger.LogError(e, "RabbitMQ channel model create failed!");
				Console.WriteLine(e);
				throw;
			}

			return model;
		}

		public virtual bool Return(IModel channel)
		{
			if (Interlocked.Increment(ref _count) <= _maxSize && channel.IsOpen)
			{
				_pool.Enqueue(channel);

				return true;
			}

			Interlocked.Decrement(ref _count);

			Debug.Assert(_maxSize == 0 || _pool.Count <= _maxSize);

			return false;
		}
	}
}