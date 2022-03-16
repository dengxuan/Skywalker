using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace Skywalker.Extensions.Linq;

/// <summary>
/// A factory to create dynamic classes, based on <see href="http://stackoverflow.com/questions/29413942/c-sharp-anonymous-object-with-properties-from-dictionary" />.
/// </summary>
public static class DynamicClassFactory
{
    // EmptyTypes is used to indicate that we are looking for someting without any parameters. 
    private static readonly Type[] s_emptyTypes = Array.Empty<Type>();

    private static readonly ConcurrentDictionary<string, Type> s_generatedTypes = new();

    private static readonly ModuleBuilder s_moduleBuilder;

    // Some objects we cache
    private static readonly CustomAttributeBuilder s_compilerGeneratedAttributeBuilder = new(typeof(CompilerGeneratedAttribute).GetConstructor(s_emptyTypes)!, Array.Empty<object>());
    private static readonly CustomAttributeBuilder s_debuggerBrowsableAttributeBuilder = new(typeof(DebuggerBrowsableAttribute).GetConstructor(new[] { typeof(DebuggerBrowsableState) })!, new object[] { DebuggerBrowsableState.Never });
    private static readonly CustomAttributeBuilder s_debuggerHiddenAttributeBuilder = new(typeof(DebuggerHiddenAttribute).GetConstructor(s_emptyTypes)!, Array.Empty<object>());

    private static readonly ConstructorInfo s_objectCtor = typeof(object).GetConstructor(s_emptyTypes)!;
    private static readonly MethodInfo s_objectToString = typeof(object).GetMethod("ToString", BindingFlags.Instance | BindingFlags.Public, null, s_emptyTypes, null)!;

    private static readonly ConstructorInfo s_stringBuilderCtor = typeof(StringBuilder).GetConstructor(s_emptyTypes)!;

    private static readonly MethodInfo s_stringBuilderAppendString = typeof(StringBuilder).GetMethod("Append", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, null)!;
    private static readonly MethodInfo s_stringBuilderAppendObject = typeof(StringBuilder).GetMethod("Append", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(object) }, null)!;

    private static readonly Type s_equalityComparer = typeof(EqualityComparer<>);
    private static readonly Type s_equalityComparerGenericArgument = s_equalityComparer.GetGenericArguments()[0];

    private static readonly MethodInfo s_equalityComparerDefault = s_equalityComparer.GetMethod("get_Default", BindingFlags.Static | BindingFlags.Public, null, s_emptyTypes, null)!;
    private static readonly MethodInfo s_equalityComparerEquals = s_equalityComparer.GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public, null, new[] { s_equalityComparerGenericArgument, s_equalityComparerGenericArgument }, null)!;
    private static readonly MethodInfo s_equalityComparerGetHashCode = s_equalityComparer.GetMethod("GetHashCode", BindingFlags.Instance | BindingFlags.Public, null, new[] { s_equalityComparerGenericArgument }, null)!;

    private static int s_index = -1;

    private static readonly string s_dynamicAssemblyName = "Skywalker.Extensions.Linq.DynamicClasses, Version=1.0.0.0";
    private static readonly string s_dynamicModuleName = "Skywalker.Extensions.Linq.DynamicClasses";

    /// <summary>
    /// Initializes the <see cref="DynamicClassFactory"/> class.
    /// </summary>
    static DynamicClassFactory()
    {
        var assemblyName = new AssemblyName(s_dynamicAssemblyName);
        var assemblyBuilder = AssemblyBuilderFactory.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

        s_moduleBuilder = assemblyBuilder.DefineDynamicModule(s_dynamicModuleName);
    }

    /// <summary>
    /// The CreateType method creates a new data class with a given set of public properties and returns the System.Type object for the newly created class. If a data class with an identical sequence of properties has already been created, the System.Type object for this class is returned.        
    /// Data classes implement private instance variables and read/write property accessors for the specified properties.Data classes also override the Equals and GetHashCode members to implement by-value equality.
    /// Data classes are created in an in-memory assembly in the current application domain. All data classes inherit from <see cref="DynamicClass"/> and are given automatically generated names that should be considered private (the names will be unique within the application domain but not across multiple invocations of the application). Note that once created, a data class stays in memory for the lifetime of the current application domain. There is currently no way to unload a dynamically created data class.
    /// The dynamic expression parser uses the CreateClass methods to generate classes from data object initializers. This feature in turn is often used with the dynamic Select method to create projections.
    /// </summary>
    /// <param name="properties">The DynamicProperties</param>
    /// <param name="createParameterCtor">Create a constructor with parameters. Default set to true. Note that for Linq-to-Database objects, this needs to be set to false.</param>
    /// <returns>Type</returns>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// DynamicProperty[] props = new DynamicProperty[] { new DynamicProperty("Name", typeof(string)), new DynamicProperty("Birthday", typeof(DateTime)) };
    /// Type type = DynamicClassFactory.CreateType(props);
    /// DynamicClass dynamicClass = Activator.CreateInstance(type) as DynamicClass;
    /// dynamicClass.SetDynamicProperty("Name", "Albert");
    /// dynamicClass.SetDynamicProperty("Birthday", new DateTime(1879, 3, 14));
    /// Console.WriteLine(dynamicClass);
    /// ]]>
    /// </code>
    /// </example>
    public static Type CreateType(IList<DynamicProperty> properties, bool? createParameterCtor = true)
    {
        Check.HasNoNulls(properties, nameof(properties));

        var types = properties.Select(p => p.Type).ToArray();
        var names = properties.Select(p => p.Name).ToArray();

        var key = GenerateKey(properties, createParameterCtor);

        if (!s_generatedTypes.TryGetValue(key, out var type))
        {
            // We create only a single class at a time, through this lock
            // Note that this is a variant of the double-checked locking.
            // It is safe because we are using a thread safe class.
            lock (s_generatedTypes)
            {
                if (!s_generatedTypes.TryGetValue(key, out type))
                {
                    var index = Interlocked.Increment(ref s_index);

                    var name = names.Length != 0 ? $"<>f__AnonymousType{index}`{names.Length}" : $"<>f__AnonymousType{index}";

                    var tb = s_moduleBuilder.DefineType(name, TypeAttributes.AnsiClass | TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit, typeof(DynamicClass));
                    tb.SetCustomAttribute(s_compilerGeneratedAttributeBuilder);

                    GenericTypeParameterBuilder[] generics;

                    if (names.Length != 0)
                    {
                        var genericNames = names.Select(genericName => $"<{genericName}>j__TPar").ToArray();
                        generics = tb.DefineGenericParameters(genericNames);
                        foreach (var b in generics)
                        {
                            b.SetCustomAttribute(s_compilerGeneratedAttributeBuilder);
                        }
                    }
                    else
                    {
                        generics = Array.Empty<GenericTypeParameterBuilder>();
                    }

                    var fields = new FieldBuilder[names.Length];

                    // There are two for cycles because we want to have all the getter methods before all the other methods
                    for (var i = 0; i < names.Length; i++)
                    {
                        // field
                        fields[i] = tb.DefineField($"<{names[i]}>i__Field", generics[i].AsType(), FieldAttributes.Private | FieldAttributes.InitOnly);
                        fields[i].SetCustomAttribute(s_debuggerBrowsableAttributeBuilder);

                        var property = tb.DefineProperty(names[i], PropertyAttributes.None, CallingConventions.HasThis, generics[i].AsType(), s_emptyTypes);

                        // getter
                        var getter = tb.DefineMethod($"get_{names[i]}", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, CallingConventions.HasThis, generics[i].AsType(), null);
                        getter.SetCustomAttribute(s_compilerGeneratedAttributeBuilder);
                        var ilgeneratorGetter = getter.GetILGenerator();
                        ilgeneratorGetter.Emit(OpCodes.Ldarg_0);
                        ilgeneratorGetter.Emit(OpCodes.Ldfld, fields[i]);
                        ilgeneratorGetter.Emit(OpCodes.Ret);
                        property.SetGetMethod(getter);

                        // setter
                        var setter = tb.DefineMethod($"set_{names[i]}", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, CallingConventions.HasThis, null, new[] { generics[i].AsType() });
                        setter.SetCustomAttribute(s_compilerGeneratedAttributeBuilder);

                        // workaround for https://github.com/dotnet/corefx/issues/7792
                        setter.DefineParameter(1, ParameterAttributes.In, generics[i].Name);

                        var ilgeneratorSetter = setter.GetILGenerator();
                        ilgeneratorSetter.Emit(OpCodes.Ldarg_0);
                        ilgeneratorSetter.Emit(OpCodes.Ldarg_1);
                        ilgeneratorSetter.Emit(OpCodes.Stfld, fields[i]);
                        ilgeneratorSetter.Emit(OpCodes.Ret);
                        property.SetSetMethod(setter);
                    }

                    // ToString()
                    var toString = tb.DefineMethod("ToString", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.HasThis, typeof(string), s_emptyTypes);
                    toString.SetCustomAttribute(s_debuggerHiddenAttributeBuilder);
                    var ilgeneratorToString = toString.GetILGenerator();
                    ilgeneratorToString.DeclareLocal(typeof(StringBuilder));
                    ilgeneratorToString.Emit(OpCodes.Newobj, s_stringBuilderCtor);
                    ilgeneratorToString.Emit(OpCodes.Stloc_0);

                    // Equals
                    var equals = tb.DefineMethod("Equals", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.HasThis, typeof(bool), new[] { typeof(object) });
                    equals.SetCustomAttribute(s_debuggerHiddenAttributeBuilder);
                    equals.DefineParameter(1, ParameterAttributes.In, "value");

                    var ilgeneratorEquals = equals.GetILGenerator();
                    ilgeneratorEquals.DeclareLocal(tb.AsType());
                    ilgeneratorEquals.Emit(OpCodes.Ldarg_1);
                    ilgeneratorEquals.Emit(OpCodes.Isinst, tb.AsType());
                    ilgeneratorEquals.Emit(OpCodes.Stloc_0);
                    ilgeneratorEquals.Emit(OpCodes.Ldloc_0);

                    var equalsLabel = ilgeneratorEquals.DefineLabel();

                    // GetHashCode()
                    var getHashCode = tb.DefineMethod("GetHashCode", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.HasThis, typeof(int), s_emptyTypes);
                    getHashCode.SetCustomAttribute(s_debuggerHiddenAttributeBuilder);
                    var ilgeneratorGetHashCode = getHashCode.GetILGenerator();
                    ilgeneratorGetHashCode.DeclareLocal(typeof(int));

                    if (names.Length == 0)
                    {
                        ilgeneratorGetHashCode.Emit(OpCodes.Ldc_I4_0);
                    }
                    else
                    {
                        // As done by Roslyn
                        // Note that initHash can vary, because string.GetHashCode() isn't "stable" for different compilation of the code
                        var initHash = 0;

                        for (var i = 0; i < names.Length; i++)
                        {
                            initHash = unchecked(initHash * (-1521134295) + fields[i].Name.GetHashCode());
                        }

                        // Note that the CSC seems to generate a different seed for every anonymous class
                        ilgeneratorGetHashCode.Emit(OpCodes.Ldc_I4, initHash);
                    }

                    for (var i = 0; i < names.Length; i++)
                    {
                        var equalityComparerT = s_equalityComparer.MakeGenericType(generics[i].AsType());

                        // Equals()
                        var equalityComparerTDefault = TypeBuilder.GetMethod(equalityComparerT, s_equalityComparerDefault);
                        var equalityComparerTEquals = TypeBuilder.GetMethod(equalityComparerT, s_equalityComparerEquals);

                        // Illegal one-byte branch at position: 9. Requested branch was: 143.
                        // So replace OpCodes.Brfalse_S to OpCodes.Brfalse
                        ilgeneratorEquals.Emit(OpCodes.Brfalse, equalsLabel);
                        ilgeneratorEquals.Emit(OpCodes.Call, equalityComparerTDefault);
                        ilgeneratorEquals.Emit(OpCodes.Ldarg_0);
                        ilgeneratorEquals.Emit(OpCodes.Ldfld, fields[i]);
                        ilgeneratorEquals.Emit(OpCodes.Ldloc_0);
                        ilgeneratorEquals.Emit(OpCodes.Ldfld, fields[i]);
                        ilgeneratorEquals.Emit(OpCodes.Callvirt, equalityComparerTEquals);

                        // GetHashCode();
                        var equalityComparerTGetHashCode = TypeBuilder.GetMethod(equalityComparerT, s_equalityComparerGetHashCode);
                        ilgeneratorGetHashCode.Emit(OpCodes.Stloc_0);
                        ilgeneratorGetHashCode.Emit(OpCodes.Ldc_I4, -1521134295);
                        ilgeneratorGetHashCode.Emit(OpCodes.Ldloc_0);
                        ilgeneratorGetHashCode.Emit(OpCodes.Mul);
                        ilgeneratorGetHashCode.Emit(OpCodes.Call, equalityComparerTDefault);
                        ilgeneratorGetHashCode.Emit(OpCodes.Ldarg_0);
                        ilgeneratorGetHashCode.Emit(OpCodes.Ldfld, fields[i]);
                        ilgeneratorGetHashCode.Emit(OpCodes.Callvirt, equalityComparerTGetHashCode);
                        ilgeneratorGetHashCode.Emit(OpCodes.Add);

                        // ToString();
                        ilgeneratorToString.Emit(OpCodes.Ldloc_0);
                        ilgeneratorToString.Emit(OpCodes.Ldstr, i == 0 ? $"{{ {names[i]} = " : $", {names[i]} = ");
                        ilgeneratorToString.Emit(OpCodes.Callvirt, s_stringBuilderAppendString);
                        ilgeneratorToString.Emit(OpCodes.Pop);
                        ilgeneratorToString.Emit(OpCodes.Ldloc_0);
                        ilgeneratorToString.Emit(OpCodes.Ldarg_0);
                        ilgeneratorToString.Emit(OpCodes.Ldfld, fields[i]);
                        ilgeneratorToString.Emit(OpCodes.Box, generics[i].AsType());
                        ilgeneratorToString.Emit(OpCodes.Callvirt, s_stringBuilderAppendObject);
                        ilgeneratorToString.Emit(OpCodes.Pop);
                    }

                    // Only create the default and with params constructor when there are any params.
                    // Otherwise default constructor is not needed because it matches the default
                    // one provided by the runtime when no constructor is present
                    if (createParameterCtor == true && names.Any())
                    {
                        // .ctor default
                        var constructorDef = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, s_emptyTypes);
                        constructorDef.SetCustomAttribute(s_debuggerHiddenAttributeBuilder);

                        var ilgeneratorConstructorDef = constructorDef.GetILGenerator();
                        ilgeneratorConstructorDef.Emit(OpCodes.Ldarg_0);
                        ilgeneratorConstructorDef.Emit(OpCodes.Call, s_objectCtor);
                        ilgeneratorConstructorDef.Emit(OpCodes.Ret);

                        // .ctor with params
                        var constructor = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, generics.Select(p => p.AsType()).ToArray());
                        constructor.SetCustomAttribute(s_debuggerHiddenAttributeBuilder);

                        var ilgeneratorConstructor = constructor.GetILGenerator();
                        ilgeneratorConstructor.Emit(OpCodes.Ldarg_0);
                        ilgeneratorConstructor.Emit(OpCodes.Call, s_objectCtor);

                        for (var i = 0; i < names.Length; i++)
                        {
                            constructor.DefineParameter(i + 1, ParameterAttributes.None, names[i]);
                            ilgeneratorConstructor.Emit(OpCodes.Ldarg_0);

                            if (i == 0)
                            {
                                ilgeneratorConstructor.Emit(OpCodes.Ldarg_1);
                            }
                            else if (i == 1)
                            {
                                ilgeneratorConstructor.Emit(OpCodes.Ldarg_2);
                            }
                            else if (i == 2)
                            {
                                ilgeneratorConstructor.Emit(OpCodes.Ldarg_3);
                            }
                            else if (i < 255)
                            {
                                ilgeneratorConstructor.Emit(OpCodes.Ldarg_S, (byte)(i + 1));
                            }
                            else
                            {
                                // Ldarg uses a ushort, but the Emit only accepts short, so we use a unchecked(...), cast to short and let the CLR interpret it as ushort.
                                ilgeneratorConstructor.Emit(OpCodes.Ldarg, unchecked((short)(i + 1)));
                            }

                            ilgeneratorConstructor.Emit(OpCodes.Stfld, fields[i]);
                        }

                        ilgeneratorConstructor.Emit(OpCodes.Ret);
                    }

                    // Equals()
                    if (names.Length == 0)
                    {
                        ilgeneratorEquals.Emit(OpCodes.Ldnull);
                        ilgeneratorEquals.Emit(OpCodes.Ceq);
                        ilgeneratorEquals.Emit(OpCodes.Ldc_I4_0);
                        ilgeneratorEquals.Emit(OpCodes.Ceq);
                    }
                    else
                    {
                        ilgeneratorEquals.Emit(OpCodes.Ret);
                        ilgeneratorEquals.MarkLabel(equalsLabel);
                        ilgeneratorEquals.Emit(OpCodes.Ldc_I4_0);
                    }

                    ilgeneratorEquals.Emit(OpCodes.Ret);

                    // GetHashCode()
                    ilgeneratorGetHashCode.Emit(OpCodes.Stloc_0);
                    ilgeneratorGetHashCode.Emit(OpCodes.Ldloc_0);
                    ilgeneratorGetHashCode.Emit(OpCodes.Ret);

                    // ToString()
                    ilgeneratorToString.Emit(OpCodes.Ldloc_0);
                    ilgeneratorToString.Emit(OpCodes.Ldstr, names.Length == 0 ? "{ }" : " }");
                    ilgeneratorToString.Emit(OpCodes.Callvirt, s_stringBuilderAppendString);
                    ilgeneratorToString.Emit(OpCodes.Pop);
                    ilgeneratorToString.Emit(OpCodes.Ldloc_0);
                    ilgeneratorToString.Emit(OpCodes.Callvirt, s_objectToString);
                    ilgeneratorToString.Emit(OpCodes.Ret);

                    type = tb.CreateType();

                    type = s_generatedTypes.GetOrAdd(key, type!);
                }
            }
        }

        if (types.Length != 0)
        {
            type = type.MakeGenericType(types);
        }

        return type;
    }

    /// <summary>
    /// Generates the key.
    /// Anonymous classes are generics based. The generic classes are distinguished by number of parameters and name of parameters. The specific types of the parameters are the generic arguments.
    /// </summary>
    /// <param name="dynamicProperties">The dynamic propertys.</param>
    /// <param name="createParameterCtor">if set to <c>true</c> [create parameter ctor].</param>
    /// <returns></returns>
    private static string GenerateKey(IEnumerable<DynamicProperty> dynamicProperties, bool? createParameterCtor)
    {
        // We recreate this by creating a fullName composed of all the property names and types, separated by a "|".
        // And append and extra field depending on createParameterCtor.
        return string.Format("{0}_{1}", string.Join("|", dynamicProperties.Select(p => Escape(p.Name) + "~" + p.Type.FullName).ToArray()), createParameterCtor == true ? "c" : string.Empty);
    }

    private static string Escape(string str)
    {
        // We escape the \ with \\, so that we can safely escape the "|" (that we use as a separator) with "\|"
        str = str.Replace(@"\", @"\\");
        str = str.Replace(@"|", @"\|");
        return str;
    }
}
