using System;
using System.Security.Cryptography;

namespace Skywalker.Spider.Http
{
    /// <summary>
    ///请求哈希编译器
    /// </summary>
    public class RequestHasher : IRequestHasher
	{

		public void ComputeHash(Request request)
		{
			var bytes = new
			{
				request.Owner,
				request.RequestUri!.AbsoluteUri,
				request.Method,
				request.RequestedTimes,
				request.Content
			}.ToBytes();
			request.Hash = bytes.ToMd5().ToHex();
		}
	}
}
