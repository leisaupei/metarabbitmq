using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Meta.RabbitMQ.Generic
{
	public class DefaultConnectionChannelPoolCollection : IConnectionChannelPoolCollection
	{
		private readonly Dictionary<string, IConnectionChannelPool> _pools = new Dictionary<string, IConnectionChannelPool>();
		public DefaultConnectionChannelPoolCollection(ILogger<DefaultConnectionChannelPoolCollection> logger, IOptions<RabbitMQOptionCollection> optionsAccessor)
		{
			foreach (var option in optionsAccessor.Value.Options)
			{
				if (option.Name == null)
					throw new ArgumentNullException(nameof(option.Name));
				if (_pools.ContainsKey(option.Name))
					throw new ArgumentException($"the name '{option.Name}' already exists");
				_pools.Add(option.Name, new DefaultConnectionChannelPool(logger, option));
			}
		}

		public void Dispose()
		{
			foreach (var p in _pools.Values)
			{
				p.Dispose();
			}
		}
		void IDisposable.Dispose() => Dispose();

		public bool TryGetValue(string key, out IConnectionChannelPool value)
		{
			if (!_pools.TryGetValue(key, out value))
				throw new ConnectionChannelPoolNotFoundException(key);
			return true;
		}

	}
}
