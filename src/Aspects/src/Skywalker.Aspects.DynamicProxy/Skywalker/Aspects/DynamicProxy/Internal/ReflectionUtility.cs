﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Skywalker.Aspects.DynamicProxy
{
    internal static class ReflectionUtility
    {
        #region Fields
        private static ConstructorInfo _constructorOfObject;
        private static ConstructorInfo _constructorOfDefaultInvocationContext;
        private static ConstructorInfo _constructorOfInterceptDelegate;                              

        private static MethodInfo _getMethodFromHandleMethodOfMethodBase1;
        private static MethodInfo _getMethodFromHandleMethodOfMethodBase2;
        private static MethodInfo _invokeMethodOfInterceptorDelegate;
        private static MethodInfo _invokeMethodOfInterceptDelegate;     
        private static MethodInfo _waitMethodOfTask;
        private static MethodInfo _invokeHandlerMethod;
        private static MethodInfo _getMethodOfArgumentsOfInvocationContext;  
        private static MethodInfo _getMethodOfCompletedTaskOfTask;
        private static MethodInfo _getMethodOfReturnValueOfInvocationContext;
        private static MethodInfo _setMethodOfReturnValueOfInvocationContext;
        #endregion

        #region Properties
        public static MethodInfo InvokeHandlerMethod
        {
            get{ return _invokeHandlerMethod ?? (_invokeHandlerMethod = GetMethod(() => TargetInvoker.InvokeHandler(null, null, null))); }
        }  
        public static ConstructorInfo ConstructorOfObject
        {
            get { return _constructorOfObject ?? (_constructorOfObject = GetConstructor(() => new object())); }
        }
        public static ConstructorInfo ConstructorOfDefaultInvocationContext
        {
            get
            {
                return _constructorOfDefaultInvocationContext
                ?? (_constructorOfDefaultInvocationContext = GetConstructor(() => new DefaultInvocationContext(null, null, null, null)));
            }
        }
        public static ConstructorInfo ConstructorOfInterceptDelegate
        {
            get
            {
                return _constructorOfInterceptDelegate ?? (_constructorOfInterceptDelegate = typeof(InterceptDelegate).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
            }
        }          
        public static MethodInfo GetMethodOfArgumentsPropertyOfInvocationContext
        {
            get { return _getMethodOfArgumentsOfInvocationContext ?? (_getMethodOfArgumentsOfInvocationContext = GetProperty<InvocationContext, object[]>(_ => _.Arguments).GetMethod);}
        }      
        public static MethodInfo GetMethodOfCompletedTaskOfTask
        {
            get { return _getMethodOfCompletedTaskOfTask ?? (_getMethodOfCompletedTaskOfTask = GetProperty<Task, Task>(_ => Task.CompletedTask).GetMethod); }
        }           
        public static MethodInfo GetMethodOfReturnValueOfInvocationContext
        {
            get { return _getMethodOfReturnValueOfInvocationContext?? (_getMethodOfReturnValueOfInvocationContext = GetProperty<InvocationContext, object>(_=>_.ReturnValue).GetMethod); }
        } 
        public static MethodInfo SetMethodOfReturnValueOfInvocationContext
        {
            get { return _setMethodOfReturnValueOfInvocationContext ?? (_setMethodOfReturnValueOfInvocationContext = GetProperty<InvocationContext, object>(_ => _.ReturnValue).SetMethod); }
        }
        public static MethodInfo GetMethodFromHandleMethodOfMethodBase1
        {
            get { return _getMethodFromHandleMethodOfMethodBase1 ?? (_getMethodFromHandleMethodOfMethodBase1 = GetMethod<MethodBase>(_ => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle))));}
        }  
        public static MethodInfo GetMethodFromHandleMethodOfMethodBase2
        {
            get { return _getMethodFromHandleMethodOfMethodBase2 ?? (_getMethodFromHandleMethodOfMethodBase2 = GetMethod<MethodBase>(_ => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle),default(RuntimeTypeHandle)))); }
        } 
        public static MethodInfo InvokeMethodOfInterceptorDelegate
        {
            get { return _invokeMethodOfInterceptorDelegate ?? (_invokeMethodOfInterceptorDelegate = GetMethod<InterceptorDelegate>(_ => _.Invoke(null))); }
        }    
        public static MethodInfo InvokeMethodOfInterceptDelegate
        {
            get { return _invokeMethodOfInterceptDelegate ?? (_invokeMethodOfInterceptDelegate = GetMethod<InterceptDelegate>(_ => _.Invoke(null))); }
        }
        public static MethodInfo WaitMethodOfTask
        {
            get { return _waitMethodOfTask ?? (_waitMethodOfTask = GetMethod<Task>(_ => _.Wait())); }
        }
        #endregion

        #region Public Methods
        public static ConstructorInfo GetConstructor<T>(Expression<Func<T>> newExpression)
        {   
            return ((NewExpression)newExpression.Body).Constructor;
        }     
        public static MethodInfo GetMethod<T>(Expression<Action<T>> methodCall)
        {
            return ((MethodCallExpression)methodCall.Body).Method;
        }    
        public static MethodInfo GetMethod(Expression<Action> methodCall)
        {
            return ((MethodCallExpression)methodCall.Body).Method;
        }     
        public static PropertyInfo GetProperty<TTarget, TProperty>(Expression<Func<TTarget, TProperty>> propertyAccessExpression)
        {
            return (PropertyInfo)((MemberExpression)propertyAccessExpression.Body).Member;
        }
        #endregion
    }  
}
