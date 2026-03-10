//// Licensed to the Gordon under one or more agreements.
//// Gordon licenses this file to you under the MIT license.

//using System.Reflection;

//namespace Skywalker.Ddd.Parts;

///// <summary>
///// Default <see cref="DddApplicationPartFactory"/>.
///// </summary>
//public class DefaultApplicationPartFactory : DddApplicationPartFactory
//{
//    /// <summary>
//    /// Gets an instance of <see cref="DefaultApplicationPartFactory"/>.
//    /// </summary>
//    public static DefaultApplicationPartFactory Instance { get; } = new DefaultApplicationPartFactory();

//    /// <summary>
//    /// Gets the sequence of <see cref="DddApplicationPart"/> instances that are created by this instance of <see cref="DefaultApplicationPartFactory"/>.
//    /// <para>
//    /// Applications may use this method to get the same behavior as this factory produces during MVC's default part discovery.
//    /// </para>
//    /// </summary>
//    /// <param name="assembly">The <see cref="Assembly"/>.</param>
//    /// <returns>The sequence of <see cref="DddApplicationPart"/> instances.</returns>
//    public static IEnumerable<DddApplicationPart> GetDefaultApplicationParts(Assembly assembly)
//    {
//        if (assembly == null)
//        {
//            throw new ArgumentNullException(nameof(assembly));
//        }

//        yield return new AssemblyPart(assembly);
//    }

//    /// <inheritdoc />
//    public override IEnumerable<DddApplicationPart> GetApplicationParts(Assembly assembly)
//    {
//        return GetDefaultApplicationParts(assembly);
//    }
//}
