// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.WindowsRuntimeSystemExtensions
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
  public static class WindowsRuntimeSystemExtensions
  {
    public static Task AsTask(this IAsyncAction action)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      WindowsRuntimeSystemExtensions.\u003C\u003Ec__DisplayClass1 cDisplayClass1 = new WindowsRuntimeSystemExtensions.\u003C\u003Ec__DisplayClass1();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass1.action = action;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass1.tcs = new TaskCompletionSource<object>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      cDisplayClass1.action.put_Completed(new AsyncActionCompletedHandler((object) cDisplayClass1, __methodptr(\u003CAsTask\u003Eb__0)));
      // ISSUE: reference to a compiler-generated field
      return (Task) cDisplayClass1.tcs.Task;
    }

    public static Task AsTask<TProgress>(this IAsyncActionWithProgress<TProgress> action)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      WindowsRuntimeSystemExtensions.\u003C\u003Ec__DisplayClass4<TProgress> cDisplayClass4 = new WindowsRuntimeSystemExtensions.\u003C\u003Ec__DisplayClass4<TProgress>();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass4.action = action;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass4.tcs = new TaskCompletionSource<object>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      cDisplayClass4.action.put_Completed(new AsyncActionWithProgressCompletedHandler<TProgress>((object) cDisplayClass4, __methodptr(\u003CAsTask\u003Eb__3)));
      // ISSUE: reference to a compiler-generated field
      return (Task) cDisplayClass4.tcs.Task;
    }

    public static Task<TResult> AsTask<TResult>(this IAsyncOperation<TResult> operation)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      WindowsRuntimeSystemExtensions.\u003C\u003Ec__DisplayClass7<TResult> cDisplayClass7 = new WindowsRuntimeSystemExtensions.\u003C\u003Ec__DisplayClass7<TResult>();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass7.operation = operation;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass7.tcs = new TaskCompletionSource<TResult>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      cDisplayClass7.operation.put_Completed(new AsyncOperationCompletedHandler<TResult>((object) cDisplayClass7, __methodptr(\u003CAsTask\u003Eb__6)));
      // ISSUE: reference to a compiler-generated field
      return cDisplayClass7.tcs.Task;
    }

    public static Task<TResult> AsTask<TResult, TProgress>(
      this IAsyncOperationWithProgress<TResult, TProgress> operation)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      WindowsRuntimeSystemExtensions.\u003C\u003Ec__DisplayClassa<TResult, TProgress> cDisplayClassa = new WindowsRuntimeSystemExtensions.\u003C\u003Ec__DisplayClassa<TResult, TProgress>();
      // ISSUE: reference to a compiler-generated field
      cDisplayClassa.operation = operation;
      // ISSUE: reference to a compiler-generated field
      cDisplayClassa.tcs = new TaskCompletionSource<TResult>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      cDisplayClassa.operation.put_Completed(new AsyncOperationWithProgressCompletedHandler<TResult, TProgress>((object) cDisplayClassa, __methodptr(\u003CAsTask\u003Eb__9)));
      // ISSUE: reference to a compiler-generated field
      return cDisplayClassa.tcs.Task;
    }

    public static TaskAwaiter GetAwaiter(this IAsyncAction source) => source.AsTask().GetAwaiter();

    public static TaskAwaiter GetAwaiter<TProgress>(
      this IAsyncActionWithProgress<TProgress> source)
    {
      return source.AsTask<TProgress>().GetAwaiter();
    }

    public static TaskAwaiter<TResult> GetAwaiter<TResult>(
      this IAsyncOperation<TResult> source)
    {
      return source.AsTask<TResult>().GetAwaiter();
    }

    public static TaskAwaiter<TResult> GetAwaiter<TResult, TProgress>(
      this IAsyncOperationWithProgress<TResult, TProgress> source)
    {
      return source.AsTask<TResult, TProgress>().GetAwaiter();
    }
  }
}
