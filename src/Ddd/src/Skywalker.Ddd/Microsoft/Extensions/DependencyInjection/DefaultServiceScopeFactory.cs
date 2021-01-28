namespace Microsoft.Extensions.DependencyInjection
{
    public class DefaultServiceScopeFactory : IHybridServiceScopeFactory, ISingletonDependency
    {
        protected IServiceScopeFactory Factory { get; }

        public DefaultServiceScopeFactory(IServiceScopeFactory factory)
        {
            Factory = factory;
        }

        public IServiceScope CreateScope()
        {
            IServiceScope serviceScope = Factory.CreateScope();
            System.Console.WriteLine($"CreateScope:{serviceScope.GetHashCode()}");
            return serviceScope;
        }
    }
}
