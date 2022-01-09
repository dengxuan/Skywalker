using System.Linq.Expressions;
using System.Reflection;

namespace Skywalker.Extensions.Linq.Parser.SupportedMethods;

internal class MethodFinder
{
    private readonly ParsingConfig _parsingConfig;

    /// <summary>
    /// Get an instance
    /// </summary>
    /// <param name="parsingConfig"></param>
    public MethodFinder(ParsingConfig parsingConfig)
    {
        _parsingConfig = parsingConfig;
    }

    public bool ContainsMethod(Type type, string methodName, bool staticAccess, Expression? instance, ref Expression[] args)
    {
        // NOTE: `instance` is not passed by ref in the method signature by design. The ContainsMethod should not change the instance.
        // However, args by reference is required for backward compatibility (removing "ref" will break some tests)

        return FindMethod(type, methodName, staticAccess, ref instance, ref args, out _) == 1;
    }

    public int FindMethod(Type type, string methodName, bool staticAccess, ref Expression? instance, ref Expression[] args, out MethodBase? method)
    {
        var flags = BindingFlags.Public | BindingFlags.DeclaredOnly | (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
        foreach (var t in SelfAndBaseTypes(type))
        {
            var members = t.FindMembers(MemberTypes.Method, flags, Type.FilterNameIgnoreCase, methodName);
            var count = FindBestMethod(members.Cast<MethodBase>(), ref args, out method);
            if (count != 0)
            {
                return count;
            }
        }

        if (instance != null)
        {
            // Try to solve with registered extension methods 
            if (_parsingConfig.CustomTypeProvider.GetExtensionMethods().TryGetValue(type, out var methods))
            {
                var argsList = args.ToList();
                argsList.Insert(0, instance);
                var extensionMethodArgs = argsList.ToArray();
                var count = FindBestMethod(methods.Cast<MethodBase>(), ref extensionMethodArgs, out method);
                if (count != 0)
                {
                    instance = null;
                    args = extensionMethodArgs;
                    return count;
                }
            }
        }

        method = null;
        return 0;
    }

    public int FindBestMethod(IEnumerable<MethodBase> methods, ref Expression[] args, out MethodBase? method)
    {
        // passing args by reference is now required with the params array support.

        var inlineArgs = args;

        var applicable = methods.Select(m => new MethodData(m, m.GetParameters()))
                                .Where(m => IsApplicable(m, inlineArgs))
                                .ToArray();

        if (applicable.Length > 1)
        {
            applicable = applicable.Where(m => applicable.All(n => m == n || IsBetterThan(inlineArgs, m, n))).ToArray();
        }

        if (args.Length == 2 && applicable.Length > 1 && (args[0].Type == typeof(Guid?) || args[1].Type == typeof(Guid?)))
        {
            applicable = applicable.Take(1).ToArray();
        }

        if (applicable.Length == 1)
        {
            var md = applicable[0];
            method = md.MethodBase;
            args = md.Args!;
        }
        else
        {
            method = null;
        }

        return applicable.Length;
    }

    public int FindIndexer(Type type, Expression[] args, out MethodBase? method)
    {
        foreach (var t in SelfAndBaseTypes(type))
        {
            var members = t.GetDefaultMembers();
            if (members.Length != 0)
            {
                var methods = members.OfType<PropertyInfo>()
                                     .Select(selector => selector.GetGetMethod() as MethodBase)
                                     .Where(predicate => predicate != null)
                                     .Cast<MethodBase>();
                var count = FindBestMethod(methods, ref args, out method);
                if (count != 0)
                {
                    return count;
                }
            }
        }

        method = null;
        return 0;
    }

    bool IsApplicable(MethodData method, Expression[] args)
    {
        var isParamArray = method.Parameters.Length > 0 && method.Parameters.Last().IsDefined(typeof(ParamArrayAttribute), false);

        // if !paramArray, the number of parameter must be equal
        // if paramArray, the last parameter is optional
        if ((!isParamArray && method.Parameters.Length != args.Length) || (isParamArray && method.Parameters.Length - 1 > args.Length))
        {
            return false;
        }

        var promotedArgs = new Expression[method.Parameters.Length];
        for (var i = 0; i < method.Parameters.Length; i++)
        {
            if (isParamArray && i == method.Parameters.Length - 1)
            {
                if (method.Parameters.Length == args.Length + 1 || (method.Parameters.Length == args.Length && args[i] is ConstantExpression constantExpression && constantExpression.Value == null))
                {
                    promotedArgs[promotedArgs.Length - 1] = Expression.Constant(null, method.Parameters.Last().ParameterType);
                }
                else if (method.Parameters.Length == args.Length && method.Parameters.Last().ParameterType == args.Last().Type)
                {
                    promotedArgs[promotedArgs.Length - 1] = args.Last();
                }
                else
                {
                    var paramType = method.Parameters.Last().ParameterType;
                    var paramElementType = paramType.GetElementType();

                    var arrayInitializerExpressions = new List<Expression>();

                    for (var j = method.Parameters.Length - 1; j < args.Length; j++)
                    {
                        var promoted = _parsingConfig.ExpressionPromoter.Promote(args[j], paramElementType, false, method.MethodBase.DeclaringType != typeof(IEnumerableSignatures));
                        if (promoted == null)
                        {
                            return false;
                        }

                        arrayInitializerExpressions.Add(promoted);
                    }

                    var paramExpression = Expression.NewArrayInit(paramElementType!, arrayInitializerExpressions);

                    promotedArgs[promotedArgs.Length - 1] = paramExpression;
                }
            }
            else
            {
                var pi = method.Parameters[i];
                if (pi.IsOut)
                {
                    return false;
                }

                var promoted = _parsingConfig.ExpressionPromoter.Promote(args[i], pi.ParameterType, false, method.MethodBase.DeclaringType != typeof(IEnumerableSignatures));
                if (promoted == null)
                {
                    return false;
                }
                promotedArgs[i] = promoted;
            }
        }

        method.Args = promotedArgs;
        return true;
    }

    private static bool IsBetterThan(Expression[] args, MethodData first, MethodData second)
    {
        // If args count is 0 -> parametereless method is better than method method with parameters
        if (args.Length == 0)
        {
            return first.Parameters.Length == 0 && second.Parameters.Length != 0;
        }

        var better = false;
        for (var i = 0; i < args.Length; i++)
        {
            var result = CompareConversions(args[i].Type, first.Parameters[i].ParameterType, second.Parameters[i].ParameterType);

            // If second is better, return false
            if (result == CompareConversionType.Second)
            {
                return false;
            }

            // If first is better, return true
            if (result == CompareConversionType.First)
            {
                return true;
            }

            // If both are same, just set better to true and continue
            if (result == CompareConversionType.Both)
            {
                better = true;
            }
        }

        return better;
    }

    // Return "First" if s -> t1 is a better conversion than s -> t2
    // Return "Second" if s -> t2 is a better conversion than s -> t1
    // Return "Both" if neither conversion is better
    private static CompareConversionType CompareConversions(Type source, Type first, Type second)
    {
        if (first == second)
        {
            return CompareConversionType.Both;
        }
        if (source == first)
        {
            return CompareConversionType.First;
        }
        if (source == second)
        {
            return CompareConversionType.Second;
        }

        var firstIsCompatibleWithSecond = TypeHelper.IsCompatibleWith(first, second);
        var secondIsCompatibleWithFirst = TypeHelper.IsCompatibleWith(second, first);

        if (firstIsCompatibleWithSecond && !secondIsCompatibleWithFirst)
        {
            return CompareConversionType.First;
        }
        if (secondIsCompatibleWithFirst && !firstIsCompatibleWithSecond)
        {
            return CompareConversionType.Second;
        }

        if (TypeHelper.IsSignedIntegralType(first) && TypeHelper.IsUnsignedIntegralType(second))
        {
            return CompareConversionType.First;
        }
        if (TypeHelper.IsSignedIntegralType(second) && TypeHelper.IsUnsignedIntegralType(first))
        {
            return CompareConversionType.Second;
        }

        return CompareConversionType.Both;
    }

    IEnumerable<Type> SelfAndBaseTypes(Type type)
    {
        if (type.GetTypeInfo().IsInterface)
        {
            var types = new List<Type>();
            AddInterface(types, type);
            return types;
        }
        return SelfAndBaseClasses(type);
    }

    private static IEnumerable<Type> SelfAndBaseClasses(Type? type)
    {
        while (type != null)
        {
            yield return type;
            type = type.GetTypeInfo().BaseType;
        }
    }

    void AddInterface(List<Type> types, Type type)
    {
        if (!types.Contains(type))
        {
            types.Add(type);
            foreach (var t in type.GetInterfaces())
            {
                AddInterface(types, t);
            }
        }
    }
}
