//// Licensed to the Gordon under one or more agreements.
//// Gordon licenses this file to you under the MIT license.

//namespace Skywalker.Ddd.Parts;

///// <summary>
///// Marker interface for <see cref="IApplicationFeatureProvider"/>
///// implementations.
///// </summary>
//public interface IApplicationFeatureProvider
//{
//}

///// <summary>
///// A provider for a given <typeparamref name="TFeature"/> feature.
///// </summary>
///// <typeparam name="TFeature">The type of the feature.</typeparam>
//public interface IApplicationFeatureProvider<TFeature> : IApplicationFeatureProvider
//{
//    /// <summary>
//    /// Updates the <paramref name="feature"/> instance.
//    /// </summary>
//    /// <param name="parts">The list of <see cref="DddApplicationPart"/> instances in the application.
//    /// </param>
//    /// <param name="feature">The feature instance to populate.</param>
//    /// <remarks>
//    /// <see cref="DddApplicationPart"/> instances in <paramref name="parts"/> appear in the same ordered sequence they
//    /// are stored in <see cref="DddApplicationPartManager.ApplicationParts"/>. This ordering may be used by the feature
//    /// provider to make precedence decisions.
//    /// </remarks>
//    void PopulateFeature(IEnumerable<DddApplicationPart> parts, TFeature feature);
//}
