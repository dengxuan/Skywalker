﻿using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Skywalker.Fody.Exceptions;
using Skywalker.Fody.Extensions;

namespace Skywalker.Fody.Extensions;

internal static class MonoExtension
{
    public static bool Is(this TypeReference typeRef, string? fullName)
    {
        return typeRef.Resolve()?.FullName == fullName;
    }

    public static bool Is(this CustomAttribute attribute, string fullName)
    {
        return attribute.AttributeType.Is(fullName);
    }

    public static bool Is(this MethodReference methodRef, string fullName)
    {
        return methodRef.FullName == fullName;
    }

    public static bool IsOrDerivesFrom(this TypeReference typeRef, string className)
    {
        return typeRef.Is(className) || typeRef.DerivesFrom(className);
    }

    public static bool Implement(this TypeDefinition typeDef, string @interface)
    {
        do
        {
            if (typeDef.Interfaces.Any(x => x.InterfaceType.FullName == @interface)) return true;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            typeDef = typeDef.BaseType?.Resolve();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        } while (typeDef != null);

        return false;
    }

    public static bool DerivesFrom(this TypeReference typeRef, string baseClass)
    {
        do
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            if ((typeRef = typeRef.Resolve()?.BaseType)?.FullName == baseClass) return true;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        } while (typeRef != null);

        return false;
    }

    public static bool DerivesFromAny(this TypeReference typeRef, params string[] baseClasses)
    {
        foreach (var baseClass in baseClasses)
        {
            if (typeRef.DerivesFrom(baseClass)) return true;
        }

        return false;
    }

    public static bool DerivesFrom(this CustomAttribute attribute, string baseClass)
    {
        return attribute.AttributeType.DerivesFrom(baseClass);
    }

    public static bool IsDelegate(this TypeReference typeRef)
    {
        return typeRef.DerivesFrom(Constants.TYPE_MulticastDelegate);
    }

    public static bool IsArray(this TypeReference typeRef, out TypeReference? elementType)
    {
        elementType = null;
        if (!typeRef.IsArray)
            return false;

        elementType = ((ArrayType)typeRef).ElementType;
        return true;
    }

    public static bool IsEnum(this TypeReference typeRef, out TypeReference? underlyingType)
    {
        var typeDef = typeRef.Resolve();
        if (typeDef != null && typeDef.IsEnum)
        {
            underlyingType = typeDef.Fields.First(f => f.Name == "value__").FieldType;
            return true;
        }

        underlyingType = null;
        return false;
    }

    public static OpCode GetStElemCode(this TypeReference typeRef)
    {
        var typeDef = typeRef.Resolve();
        if (typeDef.IsEnum(out var underlying))
            return underlying!.MetadataType.GetStElemCode();
        if (typeRef.IsValueType)
            return typeRef.MetadataType.GetStElemCode();
        return OpCodes.Stelem_Ref;
    }

    public static OpCode GetStElemCode(this MetadataType type)
    {
        switch (type)
        {
            case MetadataType.Boolean:
            case MetadataType.Int32:
            case MetadataType.UInt32:
                return OpCodes.Stelem_I4;
            case MetadataType.Byte:
            case MetadataType.SByte:
                return OpCodes.Stelem_I1;
            case MetadataType.Char:
            case MetadataType.Int16:
            case MetadataType.UInt16:
                return OpCodes.Stelem_I2;
            case MetadataType.Double:
                return OpCodes.Stelem_R8;
            case MetadataType.Int64:
            case MetadataType.UInt64:
                return OpCodes.Stelem_I8;
            case MetadataType.Single:
                return OpCodes.Stelem_R4;
            default:
                return OpCodes.Stelem_Ref;
        }
    }

    public static MethodDefinition GetZeroArgsCtor(this TypeDefinition typeDef)
    {
        var zeroCtor = typeDef.GetConstructors().FirstOrDefault(ctor => !ctor.HasParameters);
        if (zeroCtor == null)
            throw new RougamoException($"could not found zero arguments constructor from {typeDef.FullName}");
        return zeroCtor;
    }

    public static MethodReference RecursionImportPropertySet(this CustomAttribute attribute,
        ModuleDefinition moduleDef, string propertyName)
    {
        return attribute.AttributeType.Resolve().RecursionImportPropertySet(moduleDef, propertyName);
    }

    public static MethodReference RecursionImportPropertySet(this TypeDefinition typeDef,
        ModuleDefinition moduleDef, string propertyName)
    {
        var propertyDef = typeDef.Properties.FirstOrDefault(pd => pd.Name == propertyName);
        if (propertyDef != null) return moduleDef.ImportReference(propertyDef.SetMethod);

        var baseTypeDef = typeDef.BaseType.Resolve();
        if (baseTypeDef.FullName == typeof(object).FullName)
            throw new RougamoException($"can not find property({propertyName}) from {typeDef.FullName}");
        return baseTypeDef.RecursionImportPropertySet(moduleDef, propertyName);
    }

    public static MethodReference RecursionImportPropertyGet(this TypeDefinition typeDef,
        ModuleDefinition moduleDef, string propertyName)
    {
        var propertyDef = typeDef.Properties.FirstOrDefault(pd => pd.Name == propertyName);
        if (propertyDef != null) return moduleDef.ImportReference(propertyDef.GetMethod);

        var baseTypeDef = typeDef.BaseType.Resolve();
        if (baseTypeDef.FullName == typeof(object).FullName)
            throw new RougamoException($"can not find property({propertyName}) from {typeDef.FullName}");
        return baseTypeDef.RecursionImportPropertyGet(moduleDef, propertyName);
    }

    public static MethodReference RecursionImportMethod(this CustomAttribute attribute, ModuleDefinition moduleDef,
        string methodName, Func<MethodDefinition, bool> predicate)
    {
        return attribute.AttributeType.Resolve().RecursionImportMethod(moduleDef, methodName, predicate);
    }

    public static MethodReference RecursionImportMethod(this TypeDefinition typeDef, ModuleDefinition moduleDef,
        string methodName, Func<MethodDefinition, bool> predicate)
    {
        var methodDef = typeDef.Methods.FirstOrDefault(md => md.Name == methodName && predicate(md));
        if (methodDef != null) return moduleDef.ImportReference(methodDef);

        var baseTypeDef = typeDef.BaseType.Resolve();
        if (baseTypeDef.FullName == typeof(object).FullName)
            throw new RougamoException($"can not find method({methodName}) from {typeDef.FullName}");
        return baseTypeDef.RecursionImportMethod(moduleDef, methodName, predicate);
    }

    public static VariableDefinition CreateVariable(this MethodBody body, TypeReference variableTypeReference)
    {
        var variable = new VariableDefinition(variableTypeReference);
        body.Variables.Add(variable);
        return variable;
    }

    public static List<GenericInstanceType> GetGenericInterfaces(this TypeDefinition typeDef, string interfaceName)
    {
        var interfaces = new List<GenericInstanceType>();
        do
        {
            var titf = typeDef.Interfaces.Select(itf => itf.InterfaceType)
                .Where(itfRef => itfRef.FullName.StartsWith(interfaceName + "<"));
            interfaces.AddRange(titf.Cast<GenericInstanceType>());
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            typeDef = typeDef.BaseType?.Resolve();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        } while (typeDef != null);

        return interfaces;
    }

    public static TypeDefinition? GetInterfaceDefinition(this TypeDefinition typeDef, string interfaceName)
    {
        do
        {
            var interfaceRef = typeDef.Interfaces.Select(itf => itf.InterfaceType)
                .Where(itfRef => itfRef.FullName == interfaceName);
            if (interfaceRef.Any()) return interfaceRef.First().Resolve();
            typeDef = typeDef.BaseType.Resolve();
        } while (typeDef != null);

        return null;
    }

    #region Import

    public static TypeReference ImportInto(this TypeReference typeRef, ModuleDefinition moduleDef) =>
        moduleDef.ImportReference(typeRef);

    public static FieldReference ImportInto(this FieldReference fieldRef, ModuleDefinition moduleDef) =>
        moduleDef.ImportReference(fieldRef);

    public static MethodReference ImportInto(this MethodReference methodRef, ModuleDefinition moduleDef) =>
        moduleDef.ImportReference(methodRef);

    #endregion Import

    public static TypeDefinition ResolveStateMachine(this MethodDefinition methodDef, string stateMachineAttributeName)
    {
        var stateMachineAttr = methodDef.CustomAttributes.Single(attr => attr.Is(stateMachineAttributeName));
        var obj = stateMachineAttr.ConstructorArguments[0].Value;
        return obj as TypeDefinition ?? ((TypeReference)obj).Resolve();
    }

    public static Instruction Ldloc(this VariableDefinition variable)
    {
        return Instruction.Create(OpCodes.Ldloc, variable);
    }

    public static Instruction LdlocOrA(this VariableDefinition variable)
    {
        var variableTypeDef = variable.VariableType.Resolve();
        return variable.VariableType.IsGenericParameter || variableTypeDef.IsValueType && !variableTypeDef.IsEnum && !variableTypeDef.IsPrimitive ? Instruction.Create(OpCodes.Ldloca, variable) : Instruction.Create(OpCodes.Ldloc, variable);
    }

    public static Instruction Ldind(this TypeReference typeRef)
    {
        if (typeRef == null) throw new ArgumentNullException(nameof(typeRef), "Ldind argument null");
        var typeDef = typeRef.Resolve();
        if (!typeRef.IsValueType) return Instruction.Create(OpCodes.Ldind_Ref);
        if (typeDef.Is(typeof(byte).FullName)) return Instruction.Create(OpCodes.Ldind_I1);
        if (typeDef.Is(typeof(short).FullName)) return Instruction.Create(OpCodes.Ldind_I2);
        if (typeDef.Is(typeof(int).FullName)) return Instruction.Create(OpCodes.Ldind_I4);
        if (typeDef.Is(typeof(long).FullName)) return Instruction.Create(OpCodes.Ldind_I8);
        if (typeDef.Is(typeof(sbyte).FullName)) return Instruction.Create(OpCodes.Ldind_U1);
        if (typeDef.Is(typeof(ushort).FullName)) return Instruction.Create(OpCodes.Ldind_U2);
        if (typeDef.Is(typeof(uint).FullName)) return Instruction.Create(OpCodes.Ldind_U4);
        if (typeDef.Is(typeof(ulong).FullName)) return Instruction.Create(OpCodes.Ldind_I8);
        if (typeDef.Is(typeof(float).FullName)) return Instruction.Create(OpCodes.Ldind_R4);
        if (typeDef.Is(typeof(double).FullName)) return Instruction.Create(OpCodes.Ldind_R8);
        if (typeDef.IsEnum)
        {
            if (typeDef.Fields.Count == 0) return Instruction.Create(OpCodes.Ldind_I);
            return typeDef.Fields[0].FieldType.Ldind();
        }
        return Instruction.Create(OpCodes.Ldobj, typeRef); // struct
    }

    public static MethodReference GenericTypeMethodReference(this TypeReference typeRef, MethodReference methodRef, ModuleDefinition moduleDefinition)
    {
        var genericMethodRef = new MethodReference(methodRef.Name, methodRef.ReturnType, typeRef)
        {
            HasThis = methodRef.HasThis,
            ExplicitThis = methodRef.ExplicitThis,
            CallingConvention = methodRef.CallingConvention
        };
        foreach (var parameter in methodRef.Parameters)
        {
            genericMethodRef.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
        }
        foreach (var parameter in methodRef.GenericParameters)
        {
            genericMethodRef.GenericParameters.Add(new GenericParameter(parameter.Name, genericMethodRef));
        }

        return genericMethodRef.ImportInto(moduleDefinition);
    }

    public static MethodReference GenericMethodReference(this MethodReference methodRef, params TypeReference[] genericTypeRefs)
    {
        var genericInstanceMethod = new GenericInstanceMethod(methodRef);
        genericInstanceMethod.GenericArguments.Add(genericTypeRefs);

        return genericInstanceMethod;
    }

    private static Code[] _EmptyCodes = new[] { Code.Nop, Code.Ret };
    public static bool IsEmpty(this MethodDefinition methodDef)
    {
        foreach (var instruction in methodDef.Body.Instructions)
        {
            if (!_EmptyCodes.Contains(instruction.OpCode.Code)) return false;
        }

        return true;
    }

    private static readonly Dictionary<Code, OpCode> _OptimizeCodes = new Dictionary<Code, OpCode>
    {
        { Code.Leave_S, OpCodes.Leave }, { Code.Br_S, OpCodes.Br },
        { Code.Brfalse_S, OpCodes.Brfalse }, { Code.Brtrue_S, OpCodes.Brtrue },
        { Code.Beq_S, OpCodes.Beq }, { Code.Bne_Un_S, OpCodes.Bne_Un },
        { Code.Bge_S, OpCodes.Bge }, { Code.Bgt_S, OpCodes.Bgt },
        { Code.Ble_S, OpCodes.Ble }, { Code.Blt_S, OpCodes.Blt },
        { Code.Bge_Un_S, OpCodes.Bge_Un }, { Code.Bgt_Un_S, OpCodes.Bgt_Un },
        { Code.Ble_Un_S, OpCodes.Ble_Un }, { Code.Blt_Un_S, OpCodes.Blt_Un }
    };
    public static void OptimizePlus(this MethodBody body)
    {
        foreach (var instruction in body.Instructions)
        {
            if (_OptimizeCodes.TryGetValue(instruction.OpCode.Code, out var opcode))
            {
                instruction.OpCode = opcode;
            }
        }
        body.Optimize();
    }
}
