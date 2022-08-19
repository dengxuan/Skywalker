﻿namespace Skywalker.Fody;

internal static class Constants
{
    public const string TYPE_IMo = "Skywalker.FodyProxy.IInterceptor";
    public const string TYPE_MoAttribute = "Skywalker.FodyProxy.InterceptorAttribute";
    public const string TYPE_IgnoreMoAttribute = "Skywalker.FodyProxy.IgnoreInterceptorAttribute";
    public const string TYPE_MoProxyAttribute = "Skywalker.FodyProxy.ProxyAttribute";
    public const string TYPE_MoRepulsion = "Skywalker.FodyProxy.Repulsion";
    public const string TYPE_AccessFlags = "Skywalker.FodyProxy.AccessFlags";
    public const string TYPE_IRougamo_1 = "Skywalker.FodyProxy.IProxy`1";
    public const string TYPE_IRougamo_2 = "Skywalker.FodyProxy.IProxy`2";
    public const string TYPE_IRepulsionsRougamo = "Skywalker.FodyProxy.IRepulsionsProxy`2";
    public const string TYPE_MethodContext = "Skywalker.FodyProxy.Context.MethodContext";

    public const string TYPE_Void = "System.Void";
    public const string TYPE_Task = "System.Threading.Tasks.Task";
    public const string TYPE_ValueTask = "System.Threading.Tasks.ValueTask";
    public const string TYPE_MulticastDelegate = "System.MulticastDelegate";
    public const string TYPE_AsyncStateMachineAttribute = "System.Runtime.CompilerServices.AsyncStateMachineAttribute";
    public const string TYPE_IteratorStateMachineAttribute = "System.Runtime.CompilerServices.IteratorStateMachineAttribute";
    public const string TYPE_AsyncIteratorStateMachineAttribute = "System.Runtime.CompilerServices.AsyncIteratorStateMachineAttribute";
    public const string TYPE_AsyncTaskMethodBuilder = "System.Runtime.CompilerServices.AsyncTaskMethodBuilder"; // async Task
    public const string TYPE_AsyncValueTaskMethodBuilder = "System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder"; // async ValueTask
    public const string TYPE_AsyncVoidMethodBuilder = "System.Runtime.CompilerServices.AsyncVoidMethodBuilder"; // async void
    public const string TYPE_ManualResetValueTaskSourceCore = "System.Threading.Tasks.Sources.ManualResetValueTaskSourceCore"; // async IAsyncEnumerable

    public const string TYPE_ARRAY_Type = "System.Type[]";

    public const string PROP_Flags = "Flags";
    public const string PROP_Repulsions = "Repulsions";
    public const string PROP_MoTypes = "MoTypes";
    public const string PROP_Exception = "Exception";
    public const string PROP_HasException = "HasException";
    public const string PROP_ReturnValue = "ReturnValue";
    public const string PROP_ExceptionHandled = "ExceptionHandled";
    public const string PROP_ReturnValueReplaced = "ReturnValueReplaced";

    public const string FIELD_Flags = "<Flags>k__BackingField";
    public const string FIELD_Repulsions = "<Repulsions>k__BackingField";
    // Private fields cannot be accessed externally even using IL
    //public const string FIELD_Exception = "<Exception>k__BackingField";
    //public const string FIELD_ReturnValue = "<ReturnValue>k__BackingField";
    public const string FIELD_RougamoMos = ">_<rougamo_mos";
    public const string FIELD_RougamoContext = ">_<rougamo_context";
    public const string FIELD_IteratorReturnList = ">_<returns";

    public const string METHOD_OnEntry = "OnEntry";
    public const string METHOD_OnSuccess = "OnSuccess";
    public const string METHOD_OnException = "OnException";
    public const string METHOD_OnExit = "OnExit";
    public const string METHOD_Create = "Create";
    public const string METHOD_MoveNext = "MoveNext";
    public const string METHOD_SetResult = "SetResult";
}
