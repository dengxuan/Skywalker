namespace Skywalker.AspNetCore.Permissions;

//public class AuthorizationInterceptor : AbpInterceptor, ITransientDependency
//{
//    private readonly IMethodInvocationAuthorizationService _methodInvocationAuthorizationService;

//    public AuthorizationInterceptor(IMethodInvocationAuthorizationService methodInvocationAuthorizationService)
//    {
//        _methodInvocationAuthorizationService = methodInvocationAuthorizationService;
//    }

//    public override async Task InterceptAsync(IAbpMethodInvocation invocation)
//    {
//        await AuthorizeAsync(invocation);
//        await invocation.ProceedAsync();
//    }

//    protected virtual async Task AuthorizeAsync(IAbpMethodInvocation invocation)
//    {
//        await _methodInvocationAuthorizationService.CheckAsync(
//            new MethodInvocationAuthorizationContext(
//                invocation.Method
//            )
//        );
//    }
//}
