using System;

namespace Skywalker.Aspects.DynamicProxy
{
    /// <summary>
    ///   Describes the <see cref="IInvocation.Proceed"/> operation for an <see cref="IInvocation"/>
    ///   at a specific point during interception.
    /// </summary>
    public interface IInvocationProceedInfo
	{
		/// <summary>
		///   Executes the <see cref="IInvocation.Proceed"/> operation described by this instance.
		/// </summary>
		/// <exception cref="NotImplementedException">There is no interceptor, nor a proxy target object, to proceed to.</exception>
		void Invoke();
	}
}
