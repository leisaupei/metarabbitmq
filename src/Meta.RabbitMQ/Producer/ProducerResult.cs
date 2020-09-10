using System;
using System.Collections.Generic;
using System.Linq;

namespace Meta.RabbitMQ.Producer
{
	/// <summary>
	/// 发送消息结果
	/// </summary>
	public class ProducerResult
	{
		private readonly List<ProducerError> _errors = new List<ProducerError>();

		/// <summary>
		/// 是否成功
		/// </summary>
		public bool Succeeded { get; set; }

		public Exception Exception { get; set; }

		/// <summary>
		/// 错误集合
		/// </summary>
		public IEnumerable<ProducerError> Errors => _errors;

		/// <summary>
		/// 发送成功
		/// </summary>
		public static ProducerResult Success { get; } = new ProducerResult { Succeeded = true };

		/// <summary>
		/// 发送错误
		/// </summary>
		/// <param name="errors"></param>
		/// <returns></returns>
		public static ProducerResult Failed(params ProducerError[] errors)
		{
			var result = new ProducerResult { Succeeded = false };
			if (errors != null)
			{
				result._errors.AddRange(errors);
			}

			return result;
		}
		/// <summary>
		/// 发送错误
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="errors"></param>
		/// <returns></returns>
		public static ProducerResult Failed(Exception ex, params ProducerError[] errors)
		{
			var result = new ProducerResult
			{
				Succeeded = false,
				Exception = ex
			};
			if (errors != null)
			{
				result._errors.AddRange(errors);
			}

			return result;
		}

		public override string ToString()
		{
			return Succeeded
				? "Succeeded"
				: string.Format("{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
		}
	}

	/// <summary>
	/// 发送队列消息错误实体
	/// </summary>
	public class ProducerError
	{
		/// <summary>
		/// 错误code
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 错误描述
		/// </summary>
		public string Description { get; set; }
	}
}
