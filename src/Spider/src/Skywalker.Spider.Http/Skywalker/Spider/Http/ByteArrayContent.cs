using System;

namespace Skywalker.Spider.Http
{
    public class ByteArrayContent : IHttpContent
	{
		private readonly ContentHeaders _headers = new();
		private bool _disposed;

		public ContentHeaders Headers => _headers;

		/// <summary>
		/// 内容
		/// </summary>
		public byte[] Bytes { get; private set; }

		public ByteArrayContent(byte[] bytes)
		{
			Bytes = bytes;
		}

		public object Clone()
		{
			var bytes = new byte[Bytes!.Length];
			Bytes.CopyTo(bytes, 0);

			var content = new ByteArrayContent(bytes);

			if (_headers != null)
			{
				foreach (var header in _headers)
				{
					content.Headers.Add(header.Key, header.Value);
				}
			}

			return content;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed)
			{
				return;
			}

			_disposed = true;
			if (_headers != null)
			{
				_headers.Clear();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
