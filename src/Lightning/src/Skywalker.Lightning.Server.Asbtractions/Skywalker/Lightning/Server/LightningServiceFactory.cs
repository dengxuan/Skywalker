using Skywalker.Lightning.Abstractions;
using Skywalker.Lightning.Server.Abstractions;
using Skywalker.Reflection.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Server
{

    internal class LightningServiceFactory : ILightningServiceFactory
    {
        private readonly IDictionary<string, LightningServiceDescriptor> _lightningServiceDescriptors = new ConcurrentDictionary<string, LightningServiceDescriptor>();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILightningServiceNameGenerator _lightningServiceNameGenerator;

        public event Action<List<LightningServiceDescriptor>>? OnServiceLoaded;

        public LightningServiceFactory(IServiceProvider serviceProvider, ILightningServiceNameGenerator LightningServiceNameGenerator)
        {
            _serviceProvider = serviceProvider;
            _lightningServiceNameGenerator = LightningServiceNameGenerator;
        }

        protected static List<Type> GetLightningServiceTypes(List<Assembly> assemblies)
        {
            List<Type> types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                types.AddRange(assembly.ExportedTypes.Where(predicate => predicate.IsAssignableTo(typeof(ILightningService))));
            }
            return types;
        }

        private static List<MethodInfo> GetMethodInfos(Type type)
        {
            List<MethodInfo> methodInfos = new List<MethodInfo>();
            if (type.GetSingleAttributeOfTypeOrBaseTypesOrNull<LightningAttribute>()?.Disabled == true)
            {
                return methodInfos;
            }
            foreach (MethodInfo methodInfo in type.GetMethods())
            {
                if (methodInfo.GetType().GetSingleAttributeOfTypeOrBaseTypesOrNull<LightningAttribute>()?.Disabled == true)
                {
                    continue;
                }
                methodInfos.Add(methodInfo);
            }
            return methodInfos;
        }

        private static void EmitFastInt(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1); break;
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0); break;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1); break;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2); break;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3); break;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4); break;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5); break;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6); break;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7); break;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if (value > -129 && value < 128)
                    {
                        il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I4, value);
                    }
                    break;
            }
        }

        private static void EmitCastToReference(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        private static void EmitBoxIfNeeded(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
        }

        public static FastInvokeAsyncHandler GetAsyncMethodInvoker(MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(Task<object>), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType!.Module);
            ILGenerator il = dynamicMethod.GetILGenerator();
            ParameterInfo[] ps = methodInfo.GetParameters();
            Type[] paramTypes = new Type[ps.Length];
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                {
                    paramTypes[i] = ps[i].ParameterType.GetElementType()!;
                }
                else
                {
                    paramTypes[i] = ps[i].ParameterType;
                }
            }
            LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

            for (int i = 0; i < paramTypes.Length; i++)
            {
                locals[i] = il.DeclareLocal(paramTypes[i], true);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitFastInt(il, i);
                il.Emit(OpCodes.Ldelem_Ref);
                EmitCastToReference(il, paramTypes[i]);
                il.Emit(OpCodes.Stloc, locals[i]);
            }
            if (!methodInfo.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldloca_S, locals[i]);
                }
                else
                {
                    il.Emit(OpCodes.Ldloc, locals[i]);
                }
            }
            if (methodInfo.IsStatic)
            {
                il.EmitCall(OpCodes.Call, methodInfo, null);
            }
            else
            {
                il.EmitCall(OpCodes.Callvirt, methodInfo, null);
            }

            if (methodInfo.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Ldnull);
            }
            else
            {
                EmitBoxIfNeeded(il, methodInfo.ReturnType);
            }

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(il, i);
                    il.Emit(OpCodes.Ldloc, locals[i]);
                    if (locals[i].LocalType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, locals[i].LocalType);
                    }

                    il.Emit(OpCodes.Stelem_Ref);
                }
            }

            il.Emit(OpCodes.Ret);
            FastInvokeAsyncHandler invoder = (FastInvokeAsyncHandler)dynamicMethod.CreateDelegate(typeof(FastInvokeAsyncHandler));
            return invoder;
        }

        public static FastInvokeHandler GetMethodInvoker(MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType!.Module);
            ILGenerator il = dynamicMethod.GetILGenerator();
            ParameterInfo[] ps = methodInfo.GetParameters();
            Type[] paramTypes = new Type[ps.Length];
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                {
                    paramTypes[i] = ps[i].ParameterType.GetElementType()!;
                }
                else
                {
                    paramTypes[i] = ps[i].ParameterType;
                }
            }
            LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

            for (int i = 0; i < paramTypes.Length; i++)
            {
                locals[i] = il.DeclareLocal(paramTypes[i], true);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitFastInt(il, i);
                il.Emit(OpCodes.Ldelem_Ref);
                EmitCastToReference(il, paramTypes[i]);
                il.Emit(OpCodes.Stloc, locals[i]);
            }
            if (!methodInfo.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldloca_S, locals[i]);
                }
                else
                {
                    il.Emit(OpCodes.Ldloc, locals[i]);
                }
            }
            if (methodInfo.IsStatic)
            {
                il.EmitCall(OpCodes.Call, methodInfo, null);
            }
            else
            {
                il.EmitCall(OpCodes.Callvirt, methodInfo, null);
            }

            if (methodInfo.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Ldnull);
            }
            else
            {
                EmitBoxIfNeeded(il, methodInfo.ReturnType);
            }

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(il, i);
                    il.Emit(OpCodes.Ldloc, locals[i]);
                    if (locals[i].LocalType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, locals[i].LocalType);
                    }

                    il.Emit(OpCodes.Stelem_Ref);
                }
            }

            il.Emit(OpCodes.Ret);
            FastInvokeHandler invoder = (FastInvokeHandler)dynamicMethod.CreateDelegate(typeof(FastInvokeHandler));
            return invoder;
        }

        public List<LightningServiceDescriptor> GetLightningServiceDescriptors()
        {
            return _lightningServiceDescriptors.Values.ToList();
        }

        public void LoadServices(List<Assembly> assemblies)
        {
            List<Type> LightningServices = GetLightningServiceTypes(assemblies);
            foreach (var item in LightningServices)
            {
                foreach (var methodInfo in GetMethodInfos(item))
                {
                    if (methodInfo.ReturnType.BaseType == typeof(Task))
                    {
                        FastInvokeAsyncHandler fastInvoke = GetAsyncMethodInvoker(methodInfo);
                        ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                        string name = _lightningServiceNameGenerator.GetLightningServiceName(methodInfo, parameterInfos);
                        Task<object> InvokeHandler(IDictionary<string, object> keyValuePair)
                        {
                            var parameters = new List<object>();
                            foreach (ParameterInfo parameterInfo in parameterInfos)
                            {
                                keyValuePair.TryGetValue(parameterInfo.Name!, out var value);
                                var paraType = parameterInfo.ParameterType;
                                var parameter = Convert.ChangeType(value, paraType);
                                parameters.Add(parameter!);
                            }
                            var instance = _serviceProvider.GetService(item);
                            var result = fastInvoke(instance!, parameters.ToArray());
                            return result;
                        }
                        LightningServiceDescriptor LightningService = new LightningServiceDescriptor(name, InvokeHandler);
                        _lightningServiceDescriptors.Add(LightningService.Name, LightningService);
                    }
                    else
                    {
                        FastInvokeHandler fastInvoke = GetMethodInvoker(methodInfo);
                        //FastInvokeHandler fastInvoke = methodInfo.CreateDelegate<FastInvokeHandler>();
                        ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                        string name = _lightningServiceNameGenerator.GetLightningServiceName(methodInfo, parameterInfos);
                        Task<object> InvokeHandler(IDictionary<string, object> keyValuePair)
                        {
                            var parameters = new List<object>();
                            foreach (ParameterInfo parameterInfo in parameterInfos)
                            {
                                keyValuePair.TryGetValue(parameterInfo.Name!, out var value);
                                var paraType = parameterInfo.ParameterType;
                                var parameter = Convert.ChangeType(value, paraType);
                                parameters.Add(parameter!);
                            }
                            var instance = _serviceProvider.GetService(item);
                            var result = fastInvoke(instance!, parameters.ToArray());
                            return Task.FromResult(result);
                        };
                        LightningServiceDescriptor LightningService = new LightningServiceDescriptor(name, InvokeHandler);
                        _lightningServiceDescriptors.Add(LightningService.Name, LightningService);
                    }
                }
            }
            OnServiceLoaded?.Invoke(_lightningServiceDescriptors.Values.ToList());
        }

        public LightningServiceDescriptor GetLightningServiceDescriptor(string name)
        {
            return _lightningServiceDescriptors.GetOrDefault(name);
        }
    }
}
