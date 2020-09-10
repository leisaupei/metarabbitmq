
using System;

namespace Meta.RabbitMQ.Generic
{
	/// <summary>
	/// 连接池集合
	/// </summary>
	public interface IConnectionChannelPoolCollection : IDisposable
	{
		/// <summary>
		/// 获取某个连接池
		/// </summary>
		/// <param name="name">连接池名称</param>
		/// <param name="value"></param>
		/// <returns></returns>
		bool TryGetValue(string name, out IConnectionChannelPool value);
	}
}