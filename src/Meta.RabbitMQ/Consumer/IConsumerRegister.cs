using System;

namespace Meta.RabbitMQ.Consumer
{
	public interface IConsumerRegister : IDisposable
	{
		/// <summary>
        /// 订阅服务是否正常
        /// </summary>
        /// <returns></returns>
		bool IsHealthy();

		/// <summary>
        /// 取消服务
        /// </summary>
		void Cancel();

		/// <summary>
        /// 启动订阅服务
        /// </summary>
		void Start();

		/// <summary>
		/// 重新启动订阅服务
		/// </summary>
		/// <param name="force"></param>
		void ReStart(bool force = false);
	}
}