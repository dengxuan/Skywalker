//// Licensed to the Gordon under one or more agreements.
//// Gordon licenses this file to you under the MIT license.

//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Reflection;
//using System.Runtime.Loader;
//using System.Text;
//using Microsoft.Extensions.DependencyModel;

//namespace Skywalker.Ddd.Parts;

///// <summary>
///// Manages the parts and features of an MVC application.
///// </summary>
//public class DddApplicationPartManager
//{
//    /// <summary>
//    /// Gets the list of <see cref="IApplicationFeatureProvider"/>s.
//    /// </summary>
//    public IList<IApplicationFeatureProvider> FeatureProviders { get; } =
//        new List<IApplicationFeatureProvider>();

//    /// <summary>
//    /// Gets the list of <see cref="DddApplicationPart"/> instances.
//    /// <para>
//    /// Instances in this collection are stored in precedence order. An <see cref="DddApplicationPart"/> that appears
//    /// earlier in the list has a higher precedence.
//    /// An <see cref="IApplicationFeatureProvider"/> may choose to use this an interface as a way to resolve conflicts when
//    /// multiple <see cref="DddApplicationPart"/> instances resolve equivalent feature values.
//    /// </para>
//    /// </summary>
//    public IList<DddApplicationPart> ApplicationParts { get; } = new List<DddApplicationPart>();

//    /// <summary>
//    /// Populates the given <paramref name="feature"/> using the list of
//    /// <see cref="IApplicationFeatureProvider{TFeature}"/>s configured on the
//    /// <see cref="DddApplicationPartManager"/>.
//    /// </summary>
//    /// <typeparam name="TFeature">The type of the feature.</typeparam>
//    /// <param name="feature">The feature instance to populate.</param>
//    public void PopulateFeature<TFeature>(TFeature feature)
//    {
//        if (feature == null)
//        {
//            throw new ArgumentNullException(nameof(feature));
//        }

//        foreach (var provider in FeatureProviders.OfType<IApplicationFeatureProvider<TFeature>>())
//        {
//            provider.PopulateFeature(ApplicationParts, feature);
//        }
//    }

//    internal void PopulateDefaultParts(string entryAssemblyName)
//    {
//        var assemblies = GetApplicationPartAssemblies(entryAssemblyName);
//        var seenAssemblies = new HashSet<Assembly>();

//        foreach (var assembly in assemblies)
//        {
//            if (!seenAssemblies.Add(assembly))
//            {
//                // "assemblies" may contain duplicate values, but we want unique ApplicationPart instances.
//                // Note that we prefer using a HashSet over Distinct since the latter isn't
//                // guaranteed to preserve the original ordering.
//                continue;
//            }

//            var partFactory = DddApplicationPartFactory.GetApplicationPartFactory(assembly);
//            foreach (var applicationPart in partFactory.GetApplicationParts(assembly))
//            {
//                ApplicationParts.Add(applicationPart);
//            }
//        }
//    }

//    private static IEnumerable<Assembly> GetApplicationPartAssemblies(string entryAssemblyName)
//    {
//        var entryAssembly = Assembly.Load(new AssemblyName(entryAssemblyName));
//        var directory = Path.GetDirectoryName(entryAssembly.Location);
//        var files = Directory.GetFiles(directory!, "*.dll");

//        var assemblies = new HashSet<Assembly>();
//        foreach (var item in files)
//        {
//            var assembly = Assembly.LoadFrom(item);
//            // 如果该程序集标记了 DddApplicationPartAttribute，则返回它
//            if (assembly.GetCustomAttributes<DddApplicationPartAttribute>().Any())
//            {
//                assemblies.Add(assembly);
//            }
//        }
//        return [.. assemblies];
//    }

//    private static bool IsDddPart(Assembly assembly)
//    {
//        var referencedAssemblies = assembly.GetReferencedAssemblies();
//        foreach (var item in referencedAssemblies)
//        {
//            if (IsDddPart(Assembly.Load(item)))
//            {
//                return true;
//            }
//        }
//        return false;
//    }

//    private static IEnumerable<Assembly> GetAssemblyClosure(Assembly assembly)
//    {
//        yield return assembly;

//        var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, throwOnError: false)
//            .OrderBy(assembly => assembly.FullName, StringComparer.Ordinal);

//        foreach (var relatedAssembly in relatedAssemblies)
//        {
//            yield return relatedAssembly;
//        }
//    }
//}
