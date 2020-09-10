using RabbitMQ.Client;
using System;

namespace Meta.RabbitMQ.Generic
{
	public interface IConnectionChannelPool : IDisposable
	{
		/// <summary>
		/// 连接池连接的IP地址
		/// </summary>
		string HostAddress { get; }

		/// <summary>
		/// 获取连接
		/// </summary>
		/// <returns></returns>
		IConnection GetConnection();

		/// <summary>
		/// 获取channel
		/// </summary>
		/// <returns></returns>
		IModel GetChannel();

		/// <summary>
		/// 返还channel
		/// </summary>
		/// <param name="channel"></param>
		/// <returns></returns>
		bool Return(IModel channel);
	}
}