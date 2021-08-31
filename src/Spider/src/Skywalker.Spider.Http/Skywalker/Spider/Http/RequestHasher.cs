using System;
using System.Security.Cryptography;

namespace Skywalker.Spider.Http
{
    /// <summary>
    ///请求哈希编译器
    /// </summary>
    public class RequestHasher : IRequestHasher
	{

		public async void ComputeHash(Request request)
		{
			var bytes = await new
			{
				request.Owner,
				request.RequestUri!.AbsoluteUri,
				request.Method,
				request.RequestedTimes,
				request.Content
			}.ToBytesAsync();
			request.Hash = bytes.ToMd5().ToHex();
		}
	}
}
