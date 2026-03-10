//// Licensed to the Gordon under one or more agreements.
//// Gordon licenses this file to you under the MIT license.

//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Skywalker.Ddd.Parts;

///// <summary>
///// Specifies an assembly to be added as an <see cref="DddApplicationPart" />.
///// <para>
///// In the ordinary case, MVC will generate <see cref="DddApplicationPartAttribute" />
///// instances on the entry assembly for each dependency that references MVC.
///// Each of these assemblies is treated as an <see cref="DddApplicationPart" />.
///// </para>
///// </summary>
//[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
//public sealed class DddApplicationPartAttribute : Attribute
//{
//    /// <summary>
//    /// Initializes a new instance of <see cref="DddApplicationPartAttribute" />.
//    /// </summary>
//    /// <param name="assemblyName">The assembly name.</param>
//    public DddApplicationPartAttribute(string assemblyName)
//    {
//        AssemblyName = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));
//    }

//    /// <summary>
//    /// Gets the assembly name.
//    /// </summary>
//    public string AssemblyName { get; }
//}
