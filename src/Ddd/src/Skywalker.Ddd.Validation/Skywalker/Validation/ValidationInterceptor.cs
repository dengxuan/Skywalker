//using System.Threading.Tasks;
//using Skywalker.DependencyInjection;
//using Skywalker.DynamicProxy;

//namespace Skywalker.Validation
//{
//    public class ValidationInterceptor : AbpInterceptor, ITransientDependency
//    {
//        private readonly IMethodInvocationValidator _methodInvocationValidator;

//        public ValidationInterceptor(IMethodInvocationValidator methodInvocationValidator)
//        {
//            _methodInvocationValidator = methodInvocationValidator;
//        }

//        public override async Task InterceptAsync(IAbpMethodInvocation invocation)
//        {
//            Validate(invocation);
//            await invocation.ProceedAsync();
//        }

//        protected virtual void Validate(IAbpMethodInvocation invocation)
//        {
//            _methodInvocationValidator.Validate(
//                new MethodInvocationValidationContext(
//                    invocation.TargetObject,
//                    invocation.Method,
//                    invocation.Arguments
//                )
//            );
//        }
//    }
//}
