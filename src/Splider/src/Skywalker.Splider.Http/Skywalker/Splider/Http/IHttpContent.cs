using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Splider.Http
{
	public interface IHttpContent : IDisposable, ICloneable
	{
		ContentHeaders Headers { get; }
	}
}
