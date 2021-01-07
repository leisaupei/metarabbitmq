using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Meta.RabbitMQ.Generic
{
	public class ChannelPoolCollection : IChannelPoolCollection
	{
		private readonly IDictionary<string, IChannelPool> _pools = new ConcurrentDictionary<string, IChannelPool>();

		public int Count => _pools.Count;

		public ChannelPoolCollection(ILogger<ChannelPoolCollection> logger, IOptions<RabbitMQOptionCollection> optionsAccessor)
		{
			foreach (var option in optionsAccessor.Value.Options)
			{
				if (option.Name == null)
					throw new ArgumentNullException(nameof(option.Name));

				if (_pools.ContainsKey(option.Name))
					throw new ChannelPoolNameAlreadyExistsException(option.Name);

				_pools.Add(option.Name, new ChannelPool(logger, option));
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