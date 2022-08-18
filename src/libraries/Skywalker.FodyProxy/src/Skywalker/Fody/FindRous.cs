﻿using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rougamo.Fody
{
    partial class ModuleWeaver
    {
        private void FindRous()
        {
            _rouTypes = new List<RouType>();
            FullScan();
            ExtractTypeReferences();
        }

        private void FullScan()
        {
            var globalMos = FindGlobalAttributes();

            if (globalMos.GlobalIgnore) return;

            foreach (var typeDef in ModuleDefinition.Types)
            {
                if (!typeDef.IsClass || typeDef.IsValueType || typeDef.IsDelegate() || !typeDef.HasMethods) continue;
                if (typeDef.Implement(Constants.TYPE_IMo) || typeDef.DerivesFromAny(Constants.TYPE_MoRepulsion, Constants.TYPE_IgnoreMoAttribute, Constants.TYPE_MoProxyAttribute)) continue;

                var typeIgnores = ExtractIgnores(typeDef.CustomAttributes);
                if (typeIgnores == null) continue;

                var rouType = new RouType(typeDef);
                var implementations = ExtractClassImplementations(typeDef);
                var classExtracts = ExtractAttributes(typeDef.CustomAttributes, globalMos.Proxies!, $"class[{typeDef.FullName}]");

                foreach (var methodDef in typeDef.Methods)
                {
                    if (methodDef.IsConstructor || (methodDef.Attributes & MethodAttributes.Abstract) != 0) continue;

                    var methodIgnores = ExtractIgnores(methodDef.CustomAttributes);
                    if (methodIgnores == null) continue;

                    var methodExtracts = ExtractAttributes(methodDef.CustomAttributes, globalMos.Proxies!, $"method[{methodDef.FullName}]");
                    rouType.Initialize(methodDef, globalMos.Directs!, implementations, classExtracts.Mos, classExtracts.Proxied, methodExtracts.Mos, methodExtracts.Proxied, globalMos.Ignores!, typeIgnores, methodIgnores);
                }
                if (rouType.HasMo)
                {
                    _rouTypes.Add(rouType);
                }
            }
        }

        /// <summary>
        /// 提取后续常用的TypeReference
        /// </summary>
        private void ExtractTypeReferences()
        {
            if(_rouTypes.Count > 0)
            {
                var sampleMo = _rouTypes.First().Methods.First().Mos.First();
                var typeDef = sampleMo.Attribute == null ? sampleMo.TypeDef! : sampleMo.Attribute.AttributeType.Resolve();
                var imoTypeDef = typeDef.GetInterfaceDefinition(Constants.TYPE_IMo);
                _methodIMosRef = new Dictionary<string, MethodReference>(4);
                foreach (var methodDef in imoTypeDef!.Methods)
                {
                    if(methodDef.Name == Constants.METHOD_OnEntry ||
                        methodDef.Name == Constants.METHOD_OnSuccess ||
                        methodDef.Name == Constants.METHOD_OnException ||
                        methodDef.Name == Constants.METHOD_OnExit)
                    {
                        _methodIMosRef.Add(methodDef.Name, methodDef.ImportInto(ModuleDefinition));
                        if (_typeMethodContextRef == null)
                        {
                            _typeMethodContextRef = methodDef.Parameters.First().ParameterType.ImportInto(ModuleDefinition);
                        }
                    }
                }
                _typeIMoRef = imoTypeDef.ImportInto(ModuleDefinition);
                _typeIMoArrayRef = new ArrayType(_typeIMoRef);
                var typeMethodContextDef = _typeMethodContextRef.Resolve();
                _methodMethodContextCtorRef = typeMethodContextDef.GetConstructors().First().ImportInto(ModuleDefinition);
                _methodMethodContextSetExceptionRef = typeMethodContextDef.RecursionImportPropertySet(ModuleDefinition, Constants.PROP_Exception);
                _methodMethodContextSetReturnValueRef = typeMethodContextDef.RecursionImportPropertySet(ModuleDefinition, Constants.PROP_ReturnValue);
                _methodMethodContextGetReturnValueRef = typeMethodContextDef.RecursionImportPropertyGet(ModuleDefinition, Constants.PROP_ReturnValue);
                _methodMethodContextGetExceptionHandledRef = typeMethodContextDef.RecursionImportPropertyGet(ModuleDefinition, Constants.PROP_ExceptionHandled);
                _methodMethodContextGetReturnValueReplacedRef = typeMethodContextDef.RecursionImportPropertyGet(ModuleDefinition, Constants.PROP_ReturnValueReplaced);
                // Private fields cannot be accessed externally even using IL
                //_fieldMethodContextExceptionRef = typeMethodContextDef.Fields.Single(x => x.Name == Constants.FIELD_Exception).ImportInto(ModuleDefinition);
                //_fieldMethodContextReturnValueRef = typeMethodContextDef.Fields.Single(x => x.Name == Constants.FIELD_ReturnValue).ImportInto(ModuleDefinition);
                _methodMethodContextGetHasExceptionRef = typeMethodContextDef.RecursionImportPropertyGet(ModuleDefinition, Constants.PROP_HasException);
                _typeObjectArrayRef = _methodMethodContextCtorRef.Parameters.Last().ParameterType;
            }
        }

        /// <summary>
        /// 查找程序集级别继承自MoAttribute以及使用MoProxyAttribute代理的Attribute，module级别会覆盖assembly级别
        /// </summary>
        /// <returns>
        /// directs: 继承自MoAttribute的类型
        /// proxies: 通过MoProxyAttribute代理的类型
        /// ignores: 需要忽略的实现了IMo的织入类型
        /// </returns>
        private SimplifyGlobalMos FindGlobalAttributes()
        {
            var assemblyMos = FindGlobalAttributes(ModuleDefinition.Assembly.CustomAttributes, "assembly");
            var moduleMos = FindGlobalAttributes(ModuleDefinition.CustomAttributes, "module");

            if (assemblyMos.GlobalIgnore || moduleMos.GlobalIgnore) return new SimplifyGlobalMos();

            foreach (var direct in moduleMos.Directs)
            {
                if (assemblyMos.Directs.ContainsKey(direct.Key))
                {
                    WriteInfo($"module replaces assembly MoAttribute: {direct.Key}");
                }
                assemblyMos.Directs[direct.Key] = direct.Value;
            }

            foreach (var proxy in moduleMos.Proxies)
            {
                if (assemblyMos.Proxies.ContainsKey(proxy.Key))
                {
                    WriteInfo($"module replaces assembly MoProxyAttribute: {proxy.Key}");
                }
                assemblyMos.Proxies[proxy.Key] = proxy.Value;
            }

            // above GlobalIgnore has been checked null reference
            assemblyMos.Ignores!.AddRange(moduleMos.Ignores!);

            foreach (var ignore in assemblyMos.Ignores!.Keys)
            {
                if (assemblyMos.Directs.ContainsKey(ignore))
                {
                    assemblyMos.Directs.Remove(ignore);
                }
                var keys = assemblyMos.Proxies.Where(x => x.Value.FullName == ignore).Select(x => x.Key);
                foreach (var key in keys)
                {
                    assemblyMos.Proxies.Remove(key);
                }
            }

            return new SimplifyGlobalMos(assemblyMos.Directs.Values.ToArray(), assemblyMos.Proxies, assemblyMos.Ignores.Keys.ToArray());
        }

        /// <summary>
        /// 从attributes中查找继承自MoAttribute以及使用MoProxyAttribute代理的Attribute
        /// </summary>
        /// <param name="attributes">给定查找范围</param>
        /// <param name="locationName">全局范围名称</param>
        /// <returns>
        /// directs: 继承自MoAttribute的类型
        /// proxies: 通过MoProxyAttribute代理的类型
        /// ignores: 需要忽略的实现了IMo的织入类型
        /// </returns>
        private GlobalMos FindGlobalAttributes(Collection<CustomAttribute> attributes, string locationName)
        {
            var directs = new Dictionary<string, CustomAttribute>();
            var proxies = new Dictionary<string, ProxyReleation>();
            var ignores = new Dictionary<string, TypeDefinition>();

            foreach (var attribute in attributes)
            {
                var attrType = attribute.AttributeType;
                if (attrType.DerivesFrom(Constants.TYPE_MoAttribute))
                {
                    ExtractMoAttributeUniq(directs, attribute, locationName);
                }
                else if (attrType.Is(Constants.TYPE_MoProxyAttribute))
                {
                    var arg1 = attribute.ConstructorArguments[0].Value;
                    var arg2 = attribute.ConstructorArguments[1].Value;
                    var origin = arg1 is TypeDefinition ? (TypeDefinition)arg1 : ((TypeReference)arg1).Resolve();
                    var proxy = arg2 is TypeDefinition ? (TypeDefinition)arg2 : ((TypeReference)arg2).Resolve();
                    if (!proxy.DerivesFrom(Constants.TYPE_MoAttribute))
                    {
                        WriteError($"Mo proxy type must inherit from Rougamo.MoAttribute");
                    }
                    else if (!proxy.GetConstructors().Any(ctor => !ctor.HasParameters))
                    {
                        WriteError($"Mo proxy type must contains non-parameters constructor");
                    }
                    else
                    {
                        var key = $"{origin.FullName}|{proxy.FullName}";
                        if (proxies.TryAdd(key, new ProxyReleation(origin, proxy)))
                        {
                            WriteInfo($"{locationName} MoProxyAttribute found: {key}");
                        }
                        else
                        {
                            WriteError($"duplicate {locationName} MoProxyAttribute found: {key}");
                        }
                    }
                }
                else if (attrType.Is(Constants.TYPE_IgnoreMoAttribute))
                {
                    if (!ExtractIgnores(ref ignores, attribute)) break;
                }
            }

            return new GlobalMos(directs, proxies.Values.ToDictionary(x => x.Origin.FullName, x => x.Proxy), ignores);
        }

        /// <summary>
        /// 从接口继承的方式中提取IMo已经对应的互斥类型
        /// </summary>
        /// <param name="typeDef">程序集中的类型</param>
        /// <returns>
        ///         mo: 实现IMo接口的类型
        /// repulsions: 与mo互斥的类型
        /// </returns>
        private RepulsionMo[] ExtractClassImplementations(TypeDefinition typeDef)
        {
            var mos = new List<RepulsionMo>();
            var mosInterfaces = typeDef.GetGenericInterfaces(Constants.TYPE_IRougamo_1);
            var repMosInterfaces = typeDef.GetGenericInterfaces(Constants.TYPE_IRougamo_2);
            var multiRepMosInterfaces = typeDef.GetGenericInterfaces(Constants.TYPE_IRepulsionsRougamo);

            mos.AddRange(mosInterfaces.Select(x => new RepulsionMo(x.GenericArguments[0].Resolve(), new TypeDefinition[0])));
            mos.AddRange(repMosInterfaces.Select(x => new RepulsionMo(x.GenericArguments[0].Resolve(), new TypeDefinition[] { x.GenericArguments[1].Resolve() })));
            mos.AddRange(multiRepMosInterfaces.Select(x => new RepulsionMo(x.GenericArguments[0].Resolve(), ExtractRepulsionFromIl(x.GenericArguments[1].Resolve()))));

            return mos.ToArray();
        }

        /// <summary>
        /// 从IRepulsionsRougamo的泛型类型IL代码中提取互斥类型
        /// </summary>
        /// <param name="typeDef">IRepulsionsRougamo</param>
        /// <returns>互斥类型</returns>
        private TypeDefinition[] ExtractRepulsionFromIl(TypeDefinition typeDef)
        {
            return ExtractRepulsionFromProp(typeDef) ?? ExtractRepulsionFromCtor(typeDef) ?? new TypeDefinition[0];
        }

        /// <summary>
        /// 从IRepulsionsRougamo泛型类型的属性Get方法中提取互斥类型
        /// </summary>
        /// <param name="typeDef">IRepulsionsRougamo</param>
        /// <returns>互斥类型</returns>
        private TypeDefinition[]? ExtractRepulsionFromProp(TypeDefinition typeDef)
        {
            do
            {
                var property = typeDef.Properties.FirstOrDefault(prop => prop.Name == Constants.PROP_Repulsions);
                if(property != null)
                {
                    Dictionary<string, TypeDefinition>? repulsions = null;
                    foreach (var instruction in property.GetMethod.Body.Instructions)
                    {
                        if(instruction.OpCode == OpCodes.Newarr)
                        {
                            repulsions = new Dictionary<string, TypeDefinition>();
                        }
                        else if(repulsions != null && instruction.IsLdtoken(Constants.TYPE_IMo, out var def) && !repulsions.ContainsKey(def!.FullName))
                        {
                            repulsions.Add(def.FullName, def);
                        }
                    }
                    return repulsions == null ? null : repulsions.Values.ToArray();
                }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                typeDef = typeDef.BaseType?.Resolve();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            } while (typeDef != null);
            return null;
        }

        /// <summary>
        /// 从IRepulsionsRougamo泛型类型的构造方法中提取互斥类型
        /// </summary>
        /// <param name="typeDef">IRepulsionsRougamo</param>
        /// <returns>互斥类型</returns>
        private TypeDefinition[]? ExtractRepulsionFromCtor(TypeDefinition typeDef)
        {
            do
            {
                var nonCtor = typeDef.GetConstructors().FirstOrDefault(ctor => !ctor.HasParameters);
                if (nonCtor != null)
                {
                    Dictionary<string, TypeDefinition>? repulsions = null;
                    var instructions = nonCtor.Body.Instructions;
                    for (int i = instructions.Count - 1; i >= 0; i--)
                    {
                        if (instructions[i].IsStfld(Constants.FIELD_Repulsions, Constants.TYPE_ARRAY_Type))
                        {
                            repulsions = new Dictionary<string, TypeDefinition>();
                        }
                        else if(repulsions != null && instructions[i].IsLdtoken(Constants.TYPE_IMo, out var def) && !repulsions.ContainsKey(def!.FullName))
                        {
                            repulsions.Add(def.FullName, def);
                        }
                        else if(instructions[i].OpCode == OpCodes.Newarr && repulsions != null)
                        {
                            return repulsions.Values.ToArray();
                        }
                    }
                }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                typeDef = typeDef.BaseType?.Resolve();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            } while (typeDef != null);
            return null;
        }

        /// <summary>
        /// 从一堆CustomAttribute中提取MoAttribute的子类以及被代理的Attribute
        /// </summary>
        /// <param name="attributes">一堆CustomAttribute</param>
        /// <param name="proxies">代理声明</param>
        /// <param name="locationName">CustomAttribute的来源</param>
        /// <returns>
        ///     mos: 继承自MoAttribute的类型
        /// proxied: 通过代理设置的实现IMo接口的类型
        /// </returns>
        private ExtractMos ExtractAttributes(Collection<CustomAttribute> attributes, Dictionary<string, TypeDefinition> proxies, string locationName, params string[][] ignores)
        {
            var mos = new Dictionary<string, CustomAttribute>();
            var proxied = new Dictionary<string, TypeDefinition>();
            foreach (var attribute in attributes)
            {
                if (attribute.AttributeType.DerivesFrom(Constants.TYPE_MoAttribute))
                {
                    ExtractMoAttributeUniq(mos, attribute, locationName);
                }
                else if (proxies.TryGetValue(attribute.AttributeType.FullName, out var proxy))
                {
                    proxied.TryAdd(proxy.FullName, proxy);
                }
            }

            // proxies在FindGlobalAttributes中已经过滤了
            return new ExtractMos(mos.Values.ToArray(), proxied.Values.ToArray());
        }

        /// <summary>
        /// 去重的将MoAttribute添加到已有集合中并记录日志
        /// </summary>
        /// <param name="mos">已有的MoAttribute子类</param>
        /// <param name="attribute">CustomAttribute</param>
        /// <param name="locationName">CustomAttribute的来源</param>
        private void ExtractMoAttributeUniq(Dictionary<string, CustomAttribute> mos, CustomAttribute attribute, string locationName)
        {
            if(mos.TryAdd(attribute.AttributeType.FullName, attribute))
            {
                WriteInfo($"{locationName} MoAttribute found: {attribute.AttributeType.FullName}");
            }
            else
            {
                WriteError($"duplicate {locationName} MoAttribute found: {attribute.AttributeType.FullName}");
            }
        }

        /// <summary>
        /// 从一堆Attribute中找到IgnoreAttribute并提取忽略的织入类型
        /// </summary>
        /// <param name="attributes">一堆Attribute</param>
        /// <returns>忽略的织入类型，如果返回null表示忽略全部</returns>
        private string[]? ExtractIgnores(Collection<CustomAttribute> attributes)
        {
            var ignores = new Dictionary<string, TypeDefinition>();
            foreach (var attribute in attributes)
            {
                if (attribute.AttributeType.Is(Constants.TYPE_IgnoreMoAttribute) && !ExtractIgnores(ref ignores, attribute)) break;
            }
            return ignores?.Keys.ToArray();
        }

        /// <summary>
        /// 从IgnoreMoAttribute中提取忽略的织入类型
        /// </summary>
        /// <param name="ignores">已有的忽略类型</param>
        /// <param name="attribute">IgnoreAttribute</param>
        /// <returns>如果忽略全部返回false，否则返回true</returns>
        private bool ExtractIgnores(ref Dictionary<string, TypeDefinition>? ignores, CustomAttribute attribute)
        {
            if (!attribute.HasProperties || !attribute.Properties.TryGet(Constants.PROP_MoTypes, out var property))
            {
                ignores = null;
                return false;
            }

            var enumerable = (IEnumerable<CustomAttributeArgument>)property!.Value.Argument.Value;
            foreach (CustomAttributeArgument item in enumerable)
            {
                var value = item.Value;
                var def = value as TypeDefinition;
                if (def == null && value is TypeReference @ref)
                {
                    def = @ref.Resolve();
                }
                if (def != null && def.Implement(Constants.TYPE_IMo))
                {
                    ignores!.TryAdd(def.FullName, def);
                }
            }
            return true;
        }
    }
}
