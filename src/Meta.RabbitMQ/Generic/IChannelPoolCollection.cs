using System;
using System.Collections.Generic;

namespace Meta.RabbitMQ.Generic
{
	/// <summary>
	/// 连接池集合
	/// </summary>
	public interface IChannelPoolCollection : IDisposable
	{
		/// <summary>
		/// 连接池数量
		/// </summary>
		int Count { get; }

		/// <summary>
		/// 取出连接池
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <exception cref="ChannelPoolNotFoundException">没有找到连接池</exception>
		/// <exception cref="ChannelPoolRestartTimeoutException">连接池报错</exception>
		/// <returns></returns>
		bool TryGetValue(string name, out IChannelPool value);
	}
}