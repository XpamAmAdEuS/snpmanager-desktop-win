using System;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.Foundation;
using Microsoft.UI.Dispatching;

namespace SnpApp
{
    internal class IdleSynchronizer
    {
        private const uint SIdleTimeoutMs = 100000;
        private const int SDefaultWaitForEventMs = 10000;

        private const string SHasAnimationsHandleName = "HasAnimations";
        private const string SAnimationsCompleteHandleName = "AnimationsComplete";
        private const string SHasDeferredAnimationOperationsHandleName = "HasDeferredAnimationOperations";
        private const string SDeferredAnimationOperationsCompleteHandleName = "DeferredAnimationOperationsComplete";
        private const string SRootVisualResetHandleName = "RootVisualReset";
        private const string SImageDecodingIdleHandleName = "ImageDecodingIdle";
        private const string SFontDownloadsIdleHandleName = "FontDownloadsIdle";

        private readonly DispatcherQueue _mDispatcherQueue;

        private readonly Handle _mHasAnimationsHandle;
        private readonly Handle _mAnimationsCompleteHandle;
        private readonly Handle _mHasDeferredAnimationOperationsHandle;
        private readonly Handle _mDeferredAnimationOperationsCompleteHandle;
        private readonly Handle _mRootVisualResetHandle;
        private readonly Handle _mImageDecodingIdleHandle;
        private readonly Handle _mFontDownloadsIdleHandle;

        private bool _mWaitForAnimationsIsDisabled;
        private bool _mIsRs2OrHigherInitialized;
        private bool _mIsRs2OrHigher;

        private Handle OpenNamedEvent(uint processId, uint threadId, string eventNamePrefix)
        {
            var eventName = $"{eventNamePrefix}.{processId}.{threadId}";
            var handle = new Handle(
                NativeMethods.OpenEvent(
                    (uint)(SyncObjectAccess.EVENT_MODIFY_STATE | SyncObjectAccess.SYNCHRONIZE),
                    false /* inherit handle */,
                    eventName));

            if (!handle.IsValid)
            {
                // Warning: Opening a session wide event handle, test may listen for events coming from the wrong process
                handle = new Handle(
                    NativeMethods.OpenEvent(
                        (uint)(SyncObjectAccess.EVENT_MODIFY_STATE | SyncObjectAccess.SYNCHRONIZE),
                        false /* inherit handle */,
                        eventNamePrefix));
            }

            if (!handle.IsValid)
            {
                throw new Exception("Failed to open " + eventName + " handle.");
            }

            return handle;
        }

        private Handle OpenNamedEvent(DispatcherQueue dispatcherQueue, string eventNamePrefix)
        {
            return OpenNamedEvent(NativeMethods.GetCurrentProcessId(), GetUiThreadId(dispatcherQueue), eventNamePrefix);
        }

        private uint GetUiThreadId(DispatcherQueue dispatcherQueue)
        {
            uint threadId = 0;
            if (dispatcherQueue.HasThreadAccess)
            {
                threadId = NativeMethods.GetCurrentThreadId();
            }
            else
            {
                var threadIdReceivedEvent = new AutoResetEvent(false);

                dispatcherQueue.TryEnqueue(
                    DispatcherQueuePriority.Normal,
                    () =>
                    {
                        threadId = NativeMethods.GetCurrentThreadId();
                        threadIdReceivedEvent.Set();
                    });

                threadIdReceivedEvent.WaitOne(SDefaultWaitForEventMs);
            }

            return threadId;
        }

        private static IdleSynchronizer? _instance;

        private static IdleSynchronizer Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new Exception("Init() must be called on the UI thread before retrieving Instance.");
                }

                return _instance;
            }
        }

        private string? Log { get; set; }
        private int TickCountBegin { get; set; }

        private IdleSynchronizer(DispatcherQueue dispatcherQueue)
        {
            _mDispatcherQueue = dispatcherQueue;
            _mHasAnimationsHandle = OpenNamedEvent(_mDispatcherQueue, SHasAnimationsHandleName);
            _mAnimationsCompleteHandle = OpenNamedEvent(_mDispatcherQueue, SAnimationsCompleteHandleName);
            _mHasDeferredAnimationOperationsHandle = OpenNamedEvent(_mDispatcherQueue, SHasDeferredAnimationOperationsHandleName);
            _mDeferredAnimationOperationsCompleteHandle = OpenNamedEvent(_mDispatcherQueue, SDeferredAnimationOperationsCompleteHandleName);
            _mRootVisualResetHandle = OpenNamedEvent(_mDispatcherQueue, SRootVisualResetHandleName);
            _mImageDecodingIdleHandle = OpenNamedEvent(_mDispatcherQueue, SImageDecodingIdleHandleName);
            _mFontDownloadsIdleHandle = OpenNamedEvent(_mDispatcherQueue, SFontDownloadsIdleHandleName);
        }

        public static void Init()
        {
            DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            if (dispatcherQueue == null)
            {
                throw new Exception("Init() must be called on the UI thread.");
            }

            _instance = new IdleSynchronizer(dispatcherQueue);
        }

        public static void Wait()
        {
            Wait(out _);
        }

        public static void Wait(out string logMessage)
        {
            string errorString = Instance.WaitInternal(out logMessage);

            if (errorString.Length > 0)
            {
                throw new Exception(errorString);
            }
        }

        public static string TryWait()
        {
            return Instance.WaitInternal(out _);
        }

        public static string TryWait(out string logMessage)
        {
            return Instance.WaitInternal(out logMessage);
        }

        public void AddLog(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);

            if (Log != null && Log != "LOG: ")
            {
                Log += "; ";
            }

            Log += (Environment.TickCount - TickCountBegin).ToString() + ": ";
            Log += message;
        }

        private string WaitInternal(out string logMessage)
        {
            logMessage = string.Empty;
            if (_mDispatcherQueue.HasThreadAccess)
            {
                return "Cannot wait for UI thread idle from the UI thread.";
            }

            Log = "LOG: ";
            TickCountBegin = Environment.TickCount;

            bool isIdle = false;
            while (!isIdle)
            {
                bool hadBuildTreeWork = false;

                var errorString = WaitForRootVisualReset();
                if (errorString.Length > 0) { return errorString; }
                AddLog("After WaitForRootVisualReset");

                errorString = WaitForImageDecodingIdle();
                if (errorString.Length > 0) { return errorString; }
                AddLog("After WaitForImageDecodingIdle");

                // SynchronouslyTickUIThread(1);
                // AddLog("After SynchronouslyTickUIThread(1)");

                errorString = WaitForFontDownloadsIdle();
                if (errorString.Length > 0) { return errorString; }
                AddLog("After WaitForFontDownloadsIdle");

                WaitForIdleDispatcher();
                AddLog("After WaitForIdleDispatcher");

                bool hadAnimations;
                // At this point, we know that the UI thread is idle - now we need to make sure
                // that XAML isn't animating anything.
                // TODO 27870237: Remove this #if once BuildTreeServiceDrained is properly signaled in WinUI desktop apps.

                // The AnimationsComplete handle sometimes is never set in RS1,
                // so we'll skip waiting for animations to complete
                // if we've timed out once while waiting for animations in RS1.
                if (!_mWaitForAnimationsIsDisabled)
                {
                    errorString = WaitForAnimationsComplete(out hadAnimations);
                    if (errorString.Length > 0) { return errorString; }
                    AddLog("After WaitForAnimationsComplete");
                }
                else
                {
                    hadAnimations = false;
                }

                bool hadDeferredAnimationOperations;
                errorString = WaitForDeferredAnimationOperationsComplete(out hadDeferredAnimationOperations);
                if (errorString.Length > 0) { return errorString; }
                AddLog("After WaitForDeferredAnimationOperationsComplete");

                // In the case where we waited for an animation to complete there's a possibility that
                // XAML, at the completion of the animation, scheduled a new tick. We will loop
                // for as long as needed until we complete an idle dispatcher callback without
                // waiting for a pending animation to complete.
                isIdle = !hadAnimations && !hadDeferredAnimationOperations && !hadBuildTreeWork;

                AddLog("IsIdle? " + isIdle);
            }

            AddLog("End");

            logMessage = Log;
            return string.Empty;
        }

        private string WaitForRootVisualReset()
        {
            uint waitResult = NativeMethods.WaitForSingleObject(_mRootVisualResetHandle.NativeHandle, 5000);

            if (waitResult != NativeMethods.WaitObject0 && waitResult != NativeMethods.WaitTimeout)
            {
                return "Waiting for root visual reset handle returned an invalid value.";
            }

            return string.Empty;
        }

        private string WaitForImageDecodingIdle()
        {
            uint waitResult = NativeMethods.WaitForSingleObject(_mImageDecodingIdleHandle.NativeHandle, 5000);

            if (waitResult != NativeMethods.WaitObject0 && waitResult != NativeMethods.WaitTimeout)
            {
                return "Waiting for image decoding idle handle returned an invalid value.";
            }

            return string.Empty;
        }

        private string WaitForFontDownloadsIdle()
        {
            uint waitResult = NativeMethods.WaitForSingleObject(_mFontDownloadsIdleHandle.NativeHandle, 5000);

            if (waitResult != NativeMethods.WaitObject0 && waitResult != NativeMethods.WaitTimeout)
            {
                return "Waiting for font downloads handle returned an invalid value.";
            }

            return string.Empty;
        }

        private void WaitForIdleDispatcher()
        {
            AutoResetEvent shouldContinueEvent = new AutoResetEvent(false);

            // DispatcherQueueTimer runs at below idle priority, so we can use it to ensure that we only raise the event when we're idle.
            var timer = _mDispatcherQueue.CreateTimer();
            timer.Interval = TimeSpan.FromMilliseconds(0);
            timer.IsRepeating = false;

            TypedEventHandler<DispatcherQueueTimer, object>? tickHandler = null;

            tickHandler = (_, _) =>
            {
                timer.Tick -= tickHandler;
                shouldContinueEvent.Set();
            };

            timer.Tick += tickHandler;

            timer.Start();
            shouldContinueEvent.WaitOne(SDefaultWaitForEventMs);
        }

        private string WaitForAnimationsComplete(out bool hadAnimations)
        {
            hadAnimations = false;

            if (!NativeMethods.ResetEvent(_mAnimationsCompleteHandle.NativeHandle))
            {
                return "Failed to reset AnimationsComplete handle.";
            }

            AddLog("WaitForAnimationsComplete: After ResetEvent");

            // This will be signaled if and only if XAML plans to at some point in the near
            // future set the animations complete event.
            uint waitResult = NativeMethods.WaitForSingleObject(_mHasAnimationsHandle.NativeHandle, 0);

            if (waitResult != NativeMethods.WaitObject0 && waitResult != NativeMethods.WaitTimeout)
            {
                return "HasAnimations handle wait returned an invalid value.";
            }

            AddLog("WaitForAnimationsComplete: After Wait(m_hasAnimationsHandle)");

            bool hasAnimations = (waitResult == NativeMethods.WaitObject0);

            if (hasAnimations)
            {
                uint animationCompleteWaitResult = NativeMethods.WaitForSingleObject(_mAnimationsCompleteHandle.NativeHandle, SIdleTimeoutMs);

                AddLog("WaitForAnimationsComplete: HasAnimations, After Wait(m_animationsCompleteHandle)");

                if (animationCompleteWaitResult != NativeMethods.WaitObject0)
                {
                    if (!IsRs2OrHigher())
                    {
                        // The AnimationsComplete handle is sometimes just never signaled on RS1, ever.
                        // If we run into this problem, we'll just disable waiting for animations to complete
                        // and continue execution.  When the current test completes, we'll then close and reopen
                        // the test app to minimize the effects of this problem.
                        _mWaitForAnimationsIsDisabled = true;

                        hadAnimations = false;
                    }

                    return "Animation complete wait took longer than idle timeout.";
                }
            }

            hadAnimations = hasAnimations;
            return string.Empty;
        }

        private string WaitForDeferredAnimationOperationsComplete(out bool hadDeferredAnimationOperations)
        {
            hadDeferredAnimationOperations = false;

            if (!NativeMethods.ResetEvent(_mDeferredAnimationOperationsCompleteHandle.NativeHandle))
            {
                return "Failed to reset DeferredAnimationOperations handle.";
            }

            // This will be signaled if and only if XAML plans to at some point in the near
            // future set the animations complete event.
            uint waitResult = NativeMethods.WaitForSingleObject(_mHasDeferredAnimationOperationsHandle.NativeHandle, 0);

            if (waitResult != NativeMethods.WaitObject0 && waitResult != NativeMethods.WaitTimeout)
            {
                return "HasDeferredAnimationOperations handle wait returned an invalid value.";
            }

            bool hasDeferredAnimationOperations = (waitResult == NativeMethods.WaitObject0);

            if (hasDeferredAnimationOperations)
            {
                uint animationCompleteWaitResult = NativeMethods.WaitForSingleObject(_mDeferredAnimationOperationsCompleteHandle.NativeHandle, SIdleTimeoutMs);

                if (animationCompleteWaitResult != NativeMethods.WaitObject0 && animationCompleteWaitResult != NativeMethods.WaitTimeout)
                {
                    return "Deferred animation operations complete wait took longer than idle timeout.";
                }
            }

            hadDeferredAnimationOperations = hasDeferredAnimationOperations;
            return string.Empty;
        }

        private bool IsRs2OrHigher()
        {
            if (!_mIsRs2OrHigherInitialized)
            {
                _mIsRs2OrHigherInitialized = true;
                _mIsRs2OrHigher = Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4);
            }

            return _mIsRs2OrHigher;
        }
    }

    internal class Handle
    {
        public IntPtr NativeHandle { get; private set; }

        public bool IsValid
        {
            get
            {
                return NativeHandle != IntPtr.Zero;
            }
        }

        public Handle(IntPtr nativeHandle)
        {
            Attach(nativeHandle);
        }

        ~Handle()
        {
            Release();
        }

        public void Attach(IntPtr nativeHandle)
        {
            Release();
            NativeHandle = nativeHandle;
        }

        public IntPtr Detach()
        {
            IntPtr returnValue = NativeHandle;
            NativeHandle = IntPtr.Zero;
            return returnValue;
        }

        public void Release()
        {
            NativeMethods.CloseHandle(NativeHandle);
            NativeHandle = IntPtr.Zero;
        }
    }

    internal static class NativeMethods
    {
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenEvent(uint dwDesiredAccess, bool bInheritHandle, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        public const uint Infinite = 0xFFFFFFFF;
        public const uint WaitAbandoned = 0x00000080;
        public const uint WaitObject0 = 0x00000000;
        public const uint WaitTimeout = 0x00000102;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ResetEvent(IntPtr hEvent);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentProcessId();

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();
    }

    [Flags]
    public enum SyncObjectAccess : uint
    {
        DELETE = 0x00010000,
        READ_CONTROL = 0x00020000,
        WRITE_DAC = 0x00040000,
        WRITE_OWNER = 0x00080000,
        SYNCHRONIZE = 0x00100000,
        EVENT_ALL_ACCESS = 0x001F0003,
        EVENT_MODIFY_STATE = 0x00000002,
        MUTEX_ALL_ACCESS = 0x001F0001,
        MUTEX_MODIFY_STATE = 0x00000001,
        SEMAPHORE_ALL_ACCESS = 0x001F0003,
        SEMAPHORE_MODIFY_STATE = 0x00000002,
        TIMER_ALL_ACCESS = 0x001F0003,
        TIMER_MODIFY_STATE = 0x00000002,
        TIMER_QUERY_STATE = 0x00000001
    }
}
