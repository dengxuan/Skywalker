//// Licensed to the Gordon under one or more agreements.
//// Gordon licenses this file to you under the MIT license.

//using Skywalker.Extensions.DependencyInjection;
//using Skywalker.Extensions.DependencyInjection.Abstractions;

//namespace Microsoft.Extensions.DependencyInjection;

///// <summary>
///// 
///// </summary>
//public static class ServiceCollectionObjectAccessorExtensions
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="services"></param>
//    /// <returns></returns>
//    public static ObjectAccessor<T> TryAddObjectAccessor<T>(this IServiceCollection services)
//    {
//        if (services.Any(s => s.ServiceType == typeof(ObjectAccessor<T>)))
//        {
//            return services.GetSingletonInstance<ObjectAccessor<T>>();
//        }

//        return services.AddObjectAccessor<T>();
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="services"></param>
//    /// <returns></returns>
//    public static ObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services)
//    {
//        return services.AddObjectAccessor(new ObjectAccessor<T>());
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="services"></param>
//    /// <param name="obj"></param>
//    /// <returns></returns>
//    public static ObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services, T obj)
//    {
//        return services.AddObjectAccessor(new ObjectAccessor<T>(obj));
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="services"></param>
//    /// <param name="accessor"></param>
//    /// <returns></returns>
//    /// <exception cref="Exception"></exception>
//    public static ObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services, ObjectAccessor<T> accessor)
//    {
//        if (services.Any(s => s.ServiceType == typeof(ObjectAccessor<T>)))
//        {
//            throw new Exception("An object accessor is registered before for type: " + typeof(T).AssemblyQualifiedName);
//        }

//        //Add to the beginning for fast retrieve
//        services.Insert(0, ServiceDescriptor.Singleton(typeof(ObjectAccessor<T>), accessor));
//        services.Insert(0, ServiceDescriptor.Singleton(typeof(IObjectAccessor<T>), accessor));

//        return accessor;
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="services"></param>
//    /// <returns></returns>
//    public static T? GetObjectOrNull<T>(this IServiceCollection services)
//        where T : class
//    {
//        return services.GetSingletonInstanceOrNull<IObjectAccessor<T>>()?.Value;
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="services"></param>
//    /// <returns></returns>
//    /// <exception cref="Exception"></exception>
//    public static T GetObject<T>(this IServiceCollection services)
//        where T : class
//    {
//        return services.GetObjectOrNull<T>() ?? throw new Exception($"Could not find an object of {typeof(T).AssemblyQualifiedName} in services. Be sure that you have used AddObjectAccessor before!");
//    }
//}
