//// Licensed to the Gordon under one or more agreements.
//// Gordon licenses this file to you under the MIT license.

//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Resources;
//using System.Text;

//namespace Skywalker.Ddd.Parts;

///// <summary>
///// Specifies a contract for synthesizing one or more <see cref="DddApplicationPart"/> instances
///// from an <see cref="Assembly"/>.
///// <para>
///// By default, Mvc registers each application assembly that it discovers as an <see cref="AssemblyPart"/>.
///// Assemblies can optionally specify an <see cref="DddApplicationPartFactory"/> to configure parts for the assembly
///// by using <see cref="ProvideApplicationPartFactoryAttribute"/>.
///// </para>
///// </summary>
//public abstract class DddApplicationPartFactory
//{
//    /// <summary>
//    /// Gets one or more <see cref="DddApplicationPart"/> instances for the specified <paramref name="assembly"/>.
//    /// </summary>
//    /// <param name="assembly">The <see cref="Assembly"/>.</param>
//    public abstract IEnumerable<DddApplicationPart> GetApplicationParts(Assembly assembly);

//    /// <summary>
//    /// Gets the <see cref="DddApplicationPartFactory"/> for the specified assembly.
//    /// <para>
//    /// An assembly may specify an <see cref="DddApplicationPartFactory"/> using <see cref="ProvideApplicationPartFactoryAttribute"/>.
//    /// Otherwise, <see cref="DefaultApplicationPartFactory"/> is used.
//    /// </para>
//    /// </summary>
//    /// <param name="assembly">The <see cref="Assembly"/>.</param>
//    /// <returns>An instance of <see cref="DddApplicationPartFactory"/>.</returns>
//    public static DddApplicationPartFactory GetApplicationPartFactory(Assembly assembly)
//    {
//        if (assembly == null)
//        {
//            throw new ArgumentNullException(nameof(assembly));
//        }

//        var provideAttribute = assembly.GetCustomAttribute<ProvideApplicationPartFactoryAttribute>();
//        if (provideAttribute == null)
//        {
//            return DefaultApplicationPartFactory.Instance;
//        }

//        var type = provideAttribute.GetFactoryType();
//        if (!typeof(DddApplicationPartFactory).IsAssignableFrom(type))
//        {
//            throw new InvalidOperationException($"Type {type} specified by {nameof(ProvideApplicationPartFactoryAttribute)} is invalid. Type specified by {nameof(ProvideApplicationPartFactoryAttribute)} must derive from {typeof(DddApplicationPartFactory)}.");
//        }

//        return (DddApplicationPartFactory)Activator.CreateInstance(type)!;
//    }
//}
