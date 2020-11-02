using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Meta.RabbitMQ.Generic
{
	public class DefaultChannelPoolCollection : IChannelPoolCollection
	{
		private readonly IDictionary<string, IChannelPool> _pools = new Dictionary<string, IChannelPool>();
		private readonly ILogger _logger;
		private readonly IOptions<RabbitMQOptionCollection> _optionsAccessor;
		private static readonly object _reactivateLock = new object();

		public int Count => _pools.Count;

		public DefaultChannelPoolCollection(ILogger<DefaultChannelPoolCollection> logger, IOptions<RabbitMQOptionCollection> optionsAccessor)
		{
			_logger = logger;
			_optionsAccessor = optionsAccessor;
			foreach (var option in optionsAccessor.Value.Options)
			{
				if (option.Name == null)
					throw new ArgumentNullException(nameof(option.Name));
				if (_pools.ContainsKey(option.Name))
					throw new ChannelPoolNameAlreadyExistsException(option.Name);
				_pools.Add(option.Name, new DefaultChannelPool(logger, option));
			}
		}

		public void Dispose()
		{
			foreach (IChannelPool p in _pools.Values)
				p.Dispose();
			_pools.Clear();
		}
		void IDisposable.Dispose() => Dispose();

		public bool TryGetValue(string name, out IChannelPool value)
		{
			if (!_pools.TryGetValue(name, out value))
				throw new ChannelPoolNotFoundException(name);
			return true;
		}
	}
}