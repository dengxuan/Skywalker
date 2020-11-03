//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using Skywalker.EventBus.Distributed;
//using Skywalker.EventBus.Local;
//using Skywalker.Modularity;
//using Skywalker.Reflection;

//namespace Skywalker.EventBus
//{
//    public class AbpEventBusModule : AbpModule
//    {
//        public override void PreConfigureServices(ServiceConfigurationContext context)
//        {
//            AddEventHandlers(context.Services);
//        }

//        private static void AddEventHandlers(IServiceCollection services)
//        {
//            var localHandlers = new List<Type>();
//            var distributedHandlers = new List<Type>();

//            services.OnRegistred(context =>
//            {
//                if (ReflectionHelper.IsAssignableToGenericType(context.ImplementationType, typeof(ILocalEventHandler<>)))
//                {
//                    localHandlers.Add(context.ImplementationType);
//                }
//                else if (ReflectionHelper.IsAssignableToGenericType(context.ImplementationType, typeof(IDistributedEventHandler<>)))
//                {
//                    distributedHandlers.Add(context.ImplementationType);
//                }
//            });

//            services.Configure<AbpLocalEventBusOptions>(options =>
//            {
//                options.Handlers.AddIfNotContains(localHandlers);
//            });

//            services.Configure<AbpDistributedEventBusOptions>(options =>
//            {
//                options.Handlers.AddIfNotContains(distributedHandlers);
//            });
//        }
//    }
//}
