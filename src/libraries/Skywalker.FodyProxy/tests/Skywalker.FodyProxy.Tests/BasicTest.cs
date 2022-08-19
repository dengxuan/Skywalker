using System.Collections;
using BasicUsage;
using BasicUsage.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skywalker.FodyProxy;
using Xunit;

namespace Rougamo.Fody.Tests
{
    public class BasicTest : TestBase
    {
        public BasicTest()
        {
            WeaveAssembly("BasicUsage.dll");
        }

        [Fact]
        public void InstanceSingleMethodTest()
        {
            var instance = GetInstance("BasicUsage.InstanceSingleMethod");
            var arrArg = new object[] { Guid.NewGuid(), 1.2, new object() };
            instance.Entry(1, "2", arrArg);
            Assert.AreEqual(instance.Context.Arguments.Length, 3);
            for (var i = 0; i < arrArg.Length; i++)
            {
                Assert.AreEqual(arrArg[i], instance.Context.Arguments[2][i]);
            }
            Assert.IsFalse(instance.Context.IsAsync);
            Assert.IsFalse(instance.Context.IsIterator);

            instance.Context = null;
            Assert.ThrowsException<InvalidOperationException>(() => instance.Exception());
            Assert.IsNotNull(instance.Context.Exception);
            // Combine the following two steps will throws an exception.
            IDictionary data = instance.Context.Exception.Data;
            Assert.AreEqual(1, data.Count);

            instance.Context = null;
            var successValue = instance.Success();
            Assert.AreEqual(successValue, instance.Context.ReturnValue);

            instance.Context = null;
            Assert.ThrowsException<InvalidOperationException>(() => instance.ExitWithException());
            Assert.IsNull(instance.Context.ReturnValue);
            Assert.IsNotNull(instance.Context.Exception);

            instance.Context = null;
            var exitWithSuccessValue = instance.ExitWithSuccess();
            Assert.AreEqual(exitWithSuccessValue, instance.Context.ReturnValue);
            Assert.IsNull(instance.Context.Exception);
        }

        [Fact]
        public async Task AsyncInstanceSingleMethodTest()
        {
            var instance = GetInstance("BasicUsage.AsyncInstanceSingleMethod");
            var arrArg = new object[] { Guid.NewGuid(), 1.2, new object() };
#if NET461 || NET6
            await (Task)instance.EntryAsync(1, "2", arrArg);
#else
            await (ValueTask<string>)instance.EntryAsync(1, "2", arrArg);
#endif
            Assert.AreEqual(instance.Context.Arguments.Length, 3);
            for (var i = 0; i < arrArg.Length; i++)
            {
                Assert.AreEqual(arrArg[i], instance.Context.Arguments[2][i]);
            }
            Assert.IsTrue(instance.Context.IsAsync);
            Assert.IsFalse(instance.Context.IsIterator);

            instance.Context = null;
#if NET461 || NET6
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => (Task)instance.ExceptionAsync());
#else
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => ((ValueTask<string>)instance.ExceptionAsync()).AsTask());
#endif
            Assert.IsNotNull(instance.Context.Exception);
            // Combine the following two steps will throws an exception.
            IDictionary data = instance.Context.Exception.Data;
            Assert.AreEqual(1, data.Count);

            instance.Context = null;
#if NET461 || NET6
            var successValue = await (Task<int>)instance.SuccessAsync();
#else
            var successValue = await (ValueTask<int>)instance.SuccessAsync();
#endif
            Assert.AreEqual(successValue, instance.Context.ReturnValue);

            instance.Context = null;
#if NET461 || NET6
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => (Task)instance.ExitWithExceptionAsync());
#else
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => ((ValueTask<string>)instance.ExitWithExceptionAsync()).AsTask());
#endif
            Assert.IsNull(instance.Context.ReturnValue);
            Assert.IsNotNull(instance.Context.Exception);

            instance.Context = null;
#if NET461 || NET6
            var exitWithSuccessValue = await (Task<string>)instance.ExitWithSuccessAsync();
#else
            var exitWithSuccessValue = await (ValueTask<string>)instance.ExitWithSuccessAsync();
#endif
            Assert.AreEqual(instance.Context.ReturnValue, exitWithSuccessValue);
            Assert.IsNull(instance.Context.Exception);
        }

        [Fact]
        public void ModifyReturnValueTest()
        {
            var originInstance = new ModifyReturnValue();
            var instance = GetInstance("BasicUsage.ModifyReturnValue");

            Assert.ThrowsException<InvalidOperationException>(() => originInstance.Exception());
            var exceptionHandledValue = instance.Exception();
            Assert.AreEqual(ExceptionHandleAttribute.StringValue, exceptionHandledValue);

            Assert.ThrowsException<InvalidOperationException>(() => originInstance.ExceptionWithUnbox());
            var exceptionHandledUnboxValue = instance.ExceptionWithUnbox();
            Assert.AreEqual(ExceptionHandleAttribute.IntValue, exceptionHandledUnboxValue);

            Assert.ThrowsException<InvalidOperationException>(() => originInstance.ExceptionUnhandled());
            Assert.ThrowsException<InvalidOperationException>(() => instance.ExceptionUnhandled());

            var args = new object[] { 1, '-', nameof(ModifyReturnValueTest), 3.14 };
            var originSuccessValue = originInstance.Succeeded(args);
            var replacedSuccessValue = instance.Succeeded(args);
            Assert.AreNotEqual(originSuccessValue, replacedSuccessValue);
            Assert.AreEqual(ReturnValueReplaceAttribute.StringValue, replacedSuccessValue);

            var originUnboxValue = originInstance.SucceededWithUnbox();
            var replacedUnboxValue = instance.SucceededWithUnbox();
            Assert.AreNotEqual(originUnboxValue, replacedUnboxValue);
            Assert.AreEqual(ReturnValueReplaceAttribute.IntValue, replacedUnboxValue);

            var originValue = originInstance.SucceededUnrecognized();
            var unrecognizedValue = instance.SucceededUnrecognized();
            Assert.AreEqual(originValue, unrecognizedValue);

            var originNullableValue = originInstance.Nullable();
            var replacedNullableValue = instance.Nullable();
            Assert.IsNull(originNullableValue);
            Assert.AreEqual(ReturnValueReplaceAttribute.IntValue, replacedNullableValue);

            var originArrayValue = originInstance.CachedArray();
            var cachedArrayValue = instance.CachedArray();
            Assert.AreNotEqual(originArrayValue, cachedArrayValue);
            Assert.AreEqual(ReplaceValueOnEntryAttribute.ArrayValue, cachedArrayValue);

            Assert.ThrowsException<NullReferenceException>(() => originInstance.CachedEvenThrows());
            var cachedArrayValueWithoutThrows = instance.CachedEvenThrows();
            Assert.AreEqual(ReplaceValueOnEntryAttribute.ArrayValue, cachedArrayValueWithoutThrows);

            Assert.ThrowsException<ArgumentException>(() => instance.TryReplaceLongToNull());
            Assert.IsNull(instance.TryReplaceNullableToNull());
        }

        [Fact]
        public async Task AsyncModifyReturnValueTest()
        {
            var originInstance = new AsyncModifyReturnValue();
            var instance = GetInstance("BasicUsage.AsyncModifyReturnValue");

#if NET461 || NET6
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => originInstance.ExceptionAsync());
            var exceptionHandledValue = await (Task<string>)instance.ExceptionAsync();
#else
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => originInstance.ExceptionAsync().AsTask());
            var exceptionHandledValue = await (ValueTask<string>)instance.ExceptionAsync();
#endif
            Assert.AreEqual(ExceptionHandleAttribute.StringValue, exceptionHandledValue);

#if NET461 || NET6
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => originInstance.ExceptionWithUnboxAsync());
            var exceptionHandledUnboxValue = await (Task<int>)instance.ExceptionWithUnboxAsync();
#else
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => originInstance.ExceptionWithUnboxAsync().AsTask());
            var exceptionHandledUnboxValue = await (ValueTask<int>)instance.ExceptionWithUnboxAsync();
#endif
            Assert.AreEqual(ExceptionHandleAttribute.IntValue, exceptionHandledUnboxValue);

#if NET461 || NET6
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => originInstance.ExceptionUnhandledAsync());
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => (Task)instance.ExceptionUnhandledAsync());
#else
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => originInstance.ExceptionUnhandledAsync().AsTask());
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => ((ValueTask<double>)instance.ExceptionUnhandledAsync()).AsTask());
#endif

            var args = new object[] { 1, '-', nameof(ModifyReturnValueTest), 3.14 };
            var originSuccessValue = await originInstance.SucceededAsync(args);
#if NET461 || NET6
            var replacedSuccessValue = await (Task<string>)instance.SucceededAsync(args);
#else
            var replacedSuccessValue = await (ValueTask<string>)instance.SucceededAsync(args);
#endif
            Assert.AreNotEqual(originSuccessValue, replacedSuccessValue);
            Assert.AreEqual(ReturnValueReplaceAttribute.StringValue, replacedSuccessValue);

            var originUnboxValue = await originInstance.SucceededWithUnboxAsync();
#if NET461 || NET6
            var replacedUnboxValue = await (Task<int>)instance.SucceededWithUnboxAsync();
#else
            var replacedUnboxValue = await (ValueTask<int>)instance.SucceededWithUnboxAsync();
#endif
            Assert.AreNotEqual(originUnboxValue, replacedUnboxValue);
            Assert.AreEqual(ReturnValueReplaceAttribute.IntValue, replacedUnboxValue);

            var originValue = await originInstance.SucceededUnrecognizedAsync();
#if NET461 || NET6
            var unrecognizedValue = await (Task<double>)instance.SucceededUnrecognizedAsync();
#else
            var unrecognizedValue = await (ValueTask<double>)instance.SucceededUnrecognizedAsync();
#endif
            Assert.AreEqual(originValue, unrecognizedValue);


            var originArrayValue = await originInstance.CachedArrayAsync();
#if NET461 || NET6
            var cachedArrayValue = await (Task<string[]>)instance.CachedArrayAsync();
#else
            var cachedArrayValue = await (ValueTask<string[]>)instance.CachedArrayAsync();
#endif
            Assert.AreNotEqual(originArrayValue, cachedArrayValue);
            Assert.AreEqual(ReplaceValueOnEntryAttribute.ArrayValue, cachedArrayValue);

#if NET461 || NET6
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => originInstance.CachedEvenThrowsAsync());
            var cachedArrayValueWithoutThrows = await (Task<string[]>)instance.CachedEvenThrowsAsync();
#else
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => originInstance.CachedEvenThrowsAsync().AsTask());
            var cachedArrayValueWithoutThrows = await (ValueTask<string[]>)instance.CachedEvenThrowsAsync();
#endif
            Assert.AreEqual(ReplaceValueOnEntryAttribute.ArrayValue, cachedArrayValueWithoutThrows);

#if NET461 || NET6
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => (Task<long>)instance.TryReplaceLongToNullAsync());
            Assert.IsNull(await (Task<long?>)instance.TryReplaceNullableToNullAsync());
#else
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => ((ValueTask<long>)instance.TryReplaceLongToNullAsync()).AsTask());
            Assert.IsNull(await (ValueTask<long?>)instance.TryReplaceNullableToNullAsync());
#endif
        }

        //[Fact]
        //public async Task ModifyIteratorReturnValueTest()
        //{
        //    // iterator can not handle exception and modify return value
        //    var originInstance = new ModifyIteratorReturnValue();
        //    var instance = GetInstance("BasicUsage.ModifyIteratorReturnValue");

        //    var min = 5;
        //    var max = 30;
        //    var originSucceedValue = originInstance.Succeed(10, min, max).ToArray();
        //    var modifiedSucceedValue = ((IEnumerable<int>)instance.Succeed(10, min, max)).ToArray();
        //    Assert.AreNotEqual(ReturnValueReplaceAttribute.IteratorValue, modifiedSucceedValue);
        //    Assert.AreEqual(originSucceedValue.Length, modifiedSucceedValue.Length);
        //    //Assert.All(modifiedSucceedValue, x => Assert.True(x >= min && x < max));

        //    Assert.ThrowsException<InvalidOperationException>(() => originInstance.Exception(min).ToArray());
        //    Assert.ThrowsException<InvalidOperationException>(() => ((IEnumerable<int>)instance.Exception(min)).ToArray());

        //    min = 10;
        //    max = 50;
        //    //var originAsyncSucceedValue = await originInstance.SucceedAsync(10, min, max).ToArrayAsync();
        //    //var modifiedAsyncSucceedValue = await ((IAsyncEnumerable<int>)instance.SucceedAsync(10, min, max)).ToArrayAsync();
        //    //Assert.AreNotEqual(ExceptionHandleAttribute.IteratorValue, modifiedAsyncSucceedValue);
        //    //Assert.AreEqual(originAsyncSucceedValue.Length, modifiedAsyncSucceedValue.Length);
        //    //Assert.All(modifiedAsyncSucceedValue, x => Assert.True(x >= min && x < max));

        //    //await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => originInstance.ExceptionAsync(min).ToArrayAsync().AsTask());
        //    //await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => ((IAsyncEnumerable<int>)instance.ExceptionAsync(min)).ToArrayAsync().AsTask());
        //}

        [Fact]
        public void SyncTest()
        {
            var instance = GetInstance("BasicUsage.SyncExecution");

            var input = new List<string>();
            instance.Void(input);
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(SyncExecution.Void), nameof(IInterceptor.OnSuccess), nameof(IInterceptor.OnExit) }, input);

            input = new List<string>();
            Assert.ThrowsException<InvalidOperationException>(() => { instance.VoidThrows(input); });
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(SyncExecution.VoidThrows), nameof(IInterceptor.OnException), nameof(IInterceptor.OnExit) }, input);

            input = new List<string>();
            var output = instance.ReplaceOnEntry(input);
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(IInterceptor.OnExit) }, input);
            Assert.AreEqual(nameof(IInterceptor.OnEntry), output);

            input = new List<string>();
            output = instance.ReplaceOnException(input);
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(SyncExecution.ReplaceOnException), nameof(IInterceptor.OnException), nameof(IInterceptor.OnExit) }, input);
            Assert.AreEqual(nameof(IInterceptor.OnException), output);

            input = new List<string>();
            output = instance.ReplaceOnSuccess(input);
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(SyncExecution.ReplaceOnSuccess), nameof(IInterceptor.OnSuccess), nameof(IInterceptor.OnExit) }, input);
            Assert.AreEqual(nameof(IInterceptor.OnSuccess), output);

            input = new List<string>();
            instance.Empty(input);
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(IInterceptor.OnSuccess), nameof(IInterceptor.OnExit) }, input);

            input = new List<string>();
            instance.EmptyReplaceOnEntry(input);
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(IInterceptor.OnExit) }, input);
        }

        [Fact]
        public async Task AsyncTest()
        {
            var instance = GetInstance("BasicUsage.AsyncExecution");

            var input = new List<string>();
            await (Task)instance.Void1(input);
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(AsyncExecution.Void1), nameof(IInterceptor.OnSuccess), nameof(IInterceptor.OnExit) }, input);

            input = new List<string>();
            await (Task)instance.Void2(input);
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(AsyncExecution.Void2), nameof(IInterceptor.OnSuccess), nameof(IInterceptor.OnExit) }, input);

            input = new List<string>();
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => instance.VoidThrows1(input));
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(AsyncExecution.VoidThrows1), nameof(IInterceptor.OnException), nameof(IInterceptor.OnExit) }, input);

            input = new List<string>();
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => instance.VoidThrows2(input));
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(AsyncExecution.VoidThrows2), nameof(IInterceptor.OnException), nameof(IInterceptor.OnExit) }, input);

            input = new List<string>();
            var output = await (Task<string>)instance.ReplaceOnEntry(input);
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(IInterceptor.OnExit) }, input);
            Assert.AreEqual(nameof(IInterceptor.OnEntry), output);

            input = new List<string>();
            output = await (Task<string>)instance.ReplaceOnException(input);
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(SyncExecution.ReplaceOnException), nameof(IInterceptor.OnException), nameof(IInterceptor.OnExit) }, input.ToArray());
            Assert.AreEqual(nameof(IInterceptor.OnException), output);

            input = new List<string>();
            output = await (Task<string>)instance.ReplaceOnSuccess(input);
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(SyncExecution.ReplaceOnSuccess), nameof(IInterceptor.OnSuccess), nameof(IInterceptor.OnExit) }, input);
            Assert.AreEqual(nameof(IInterceptor.OnSuccess), output);

            input = new List<string>();
            await (Task)instance.Empty(input);
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(IInterceptor.OnSuccess), nameof(IInterceptor.OnExit) }, input);

            input = new List<string>();
            await (Task)instance.EmptyReplaceOnEntry(input);
            Assert.AreEqual(new[] { nameof(IInterceptor.OnEntry), nameof(IInterceptor.OnExit) }, input);
        }
    }
}
