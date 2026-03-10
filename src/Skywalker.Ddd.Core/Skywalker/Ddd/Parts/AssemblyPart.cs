//// Licensed to the Gordon under one or more agreements.
//// Gordon licenses this file to you under the MIT license.

//using System.Reflection;

//namespace Skywalker.Ddd.Parts;

///// <summary>
///// An <see cref="DddApplicationPart"/> backed by an <see cref="System.Reflection.Assembly"/>.
///// </summary>
//public class AssemblyPart : DddApplicationPart, IApplicationPartTypeProvider
//{
//    /// <summary>
//    /// Initializes a new <see cref="AssemblyPart"/> instance.
//    /// </summary>
//    /// <param name="assembly">The backing <see cref="System.Reflection.Assembly"/>.</param>
//    public AssemblyPart(Assembly assembly)
//    {
//        Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
//    }

//    /// <summary>
//    /// Gets the <see cref="Assembly"/> of the <see cref="DddApplicationPart"/>.
//    /// </summary>
//    public Assembly Assembly { get; }

//    /// <summary>
//    /// Gets the name of the <see cref="DddApplicationPart"/>.
//    /// </summary>
//    public override string Name => Assembly.GetName().Name!;

//    /// <inheritdoc />
//    public IEnumerable<TypeInfo> Types => Assembly.DefinedTypes;
//}
