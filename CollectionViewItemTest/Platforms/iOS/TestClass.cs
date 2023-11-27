using System;
using UIKit;
using Foundation;
using CoreFoundation;
using CoreGraphics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CFTimeInterval = System.Double;
namespace CollectionViewItemTest;

public class TWWatchdogInspectorViewController : UIViewController
{
    private const double kBestFrameRate = 60.0;
    private const float kBarViewWidth = 10.0f;
    private const float kBarViewPaddingX = 8.0f;
    private const double kBarViewAnimationDuration = 2.0;
    private const float kLabelWidth = 150.0f;
    private double lastUpdateFPSTIme = 0.0;

    private UILabel fpsLabel;
    private UILabel timeLabel;
    private HashSet<UIView> barViews = new HashSet<UIView>();

    public override void LoadView()
    {
        base.LoadView();
        View.BackgroundColor = UIColor.LightGray;

        fpsLabel = new UILabel
        {
            BackgroundColor = UIColor.Clear,
            Font = UIFont.BoldSystemFontOfSize(14),
        };
        View.AddSubview(fpsLabel);

        timeLabel = new UILabel
        {
            BackgroundColor = UIColor.Clear,
            Font = UIFont.BoldSystemFontOfSize(14),
        };
        View.AddSubview(timeLabel);
    }

    public override void ViewWillLayoutSubviews()
    {
        base.ViewWillLayoutSubviews();

        fpsLabel.Frame = new CGRect(15, 15, kLabelWidth, View.Bounds.Size.Height);
        timeLabel.Frame = new CGRect(fpsLabel.Frame.Right, 0, kLabelWidth, View.Bounds.Size.Height);
    }

    public void UpdateFPS(double fps)
    {
        if (fps > 0)
        {
            fpsLabel.Text = $"fps: {fps:F2}";
        }
        else
        {
            fpsLabel.Text = null;
        }
        UpdateColorWithFPS(fps);
        AddBarWithFPS(fps);
        lastUpdateFPSTIme = NSDate.Now.SecondsSinceReferenceDate;
    }

    public void UpdateStallingTime(double stallingTime)
    {
        if (stallingTime > 0)
        {
            timeLabel.Text = $"Stalling: {stallingTime:F2} Sec";
        }
        else
        {
            timeLabel.Text = null;
        }
    }

    private void UpdateColorWithFPS(double fps)
    {
        // Fade from green to red
        var n = 1 - (fps / kBestFrameRate);
        var red = (255 * n);
        var green = (255 * (1 - n) / 2);
        var blue = 0;
        UIColor color = UIColor.FromRGBA((nfloat)(red / 255.0f), (nfloat)(green / 255.0f), blue / 255.0f, 1.0f);
        if (fps == 0.0)
        {
            color = UIColor.LightGray;
        }

        UIView.Animate(0.2, () =>
        {
            View.Layer.BackgroundColor = color.CGColor;
        });
    }

    private void AddBarWithFPS(double fps)
    {
        double duration = kBarViewAnimationDuration;
        if (lastUpdateFPSTIme > 0)
        {
            duration = NSDate.Now.SecondsSinceReferenceDate - lastUpdateFPSTIme;
        }
        nfloat xPos = View.Bounds.Size.Width;
        nfloat height = View.Bounds.Size.Height * (nfloat)(fps / kBestFrameRate);
        nfloat yPos = View.Bounds.Size.Height - height;
        UIView barView = new UIView(new CGRect(xPos, yPos, kBarViewWidth, height))
        {
            BackgroundColor = UIColor.FromWhiteAlpha(1.0f, 0.2f),
        };
        View.AddSubview(barView);
        barViews.Add(barView);

        View.BringSubviewToFront(fpsLabel);
        View.BringSubviewToFront(timeLabel);
        foreach (UIView view in barViews)
        {
            view.Layer.RemoveAllAnimations();
            CGRect rect = view.Frame;
            rect.X = rect.X - rect.Width - kBarViewPaddingX;
            UIView.Animate(duration, () =>
            {
                view.Frame = rect;
            }, () =>
            {
                RemoveBarViewIfNeeded(view);
            });
        }
    }

    private void RemoveBarViewIfNeeded(UIView barView)
    {
        if (barView.Frame.GetMaxX() <= -kBarViewPaddingX)
        {
            barView.RemoveFromSuperview();
            barViews.Remove(barView);
        }
    }
}

public class TWWatchdogInspector
{
    private const double kBestWatchdogFramerate = 60.0;
    private const string kExceptionName = "TWWatchdogInspectorStallingTimeout";

    private static UIWindow kInspectorWindow = null;
    private static CFTimeInterval updateWatchdogInterval = 2.0;
    private static CFTimeInterval watchdogMaximumStallingTimeInterval = 3.0;
    private static bool enableWatchdogStallingException = true;
    private static int numberOfFrames = 0;
    private static bool useLogs = true;
    private static CFTimeInterval lastMainThreadEntryTime = 0;
    private static DispatchSource.Timer watchdogTimer;
    private static DispatchSource.Timer mainthreadTimer;
    private static IntPtr kObserverRef = IntPtr.Zero;
    private static double NSEC_PER_SEC = 1000000000;

    public static void Start()
    {
        if (useLogs)
        {
            Console.WriteLine("Start WatchdogInspector");
        }
        AddRunLoopObserver();
        AddWatchdogTimer();
        AddMainThreadWatchdogCounter();
        if (kInspectorWindow == null)
        {
            SetupStatusView();
        }
    }

    public static void Stop()
    {
        if (useLogs)
        {
            Console.WriteLine("Stop WatchdogInspector");
        }
        if (watchdogTimer is not null)
        {
            watchdogTimer.Cancel();
            watchdogTimer = null;
        }

        if (mainthreadTimer is not null)
        {
            mainthreadTimer.Cancel();
            mainthreadTimer = null;
        }
        
        if (kObserverRef != IntPtr.Zero)
        {
            Interop.CFRunLoopRemoveObserver(Interop.CFRunLoopGetMain(), kObserverRef, Interop.kCFRunLoopCommonModes);
           // Interop.CFRelease(kObserverRef);
            kObserverRef = IntPtr.Zero;
        }
        ResetCountValues();
        kInspectorWindow.Hidden = true;
        kInspectorWindow = null;
    }

    public static bool IsRunning()
    {
        return (watchdogTimer is not null);
    }

    public static void SetStallingThreshhold(double time)
    {
        watchdogMaximumStallingTimeInterval = time;
    }

    public static void SetEnableMainthreadStallingException(bool enable)
    {
        enableWatchdogStallingException = enable;
    }

    public static void SetUpdateWatchdogInterval(double time)
    {
        updateWatchdogInterval = time;
    }

    public static void SetUseLogs(bool use)
    {
        useLogs = use;
    }
    
    

    private static void AddMainThreadWatchdogCounter()
    {
        mainthreadTimer = new DispatchSource.Timer (DispatchQueue.MainQueue);
        var updateWatchdog =( 1.0 / kBestWatchdogFramerate) * 1000000000;

        mainthreadTimer.SetTimer(DispatchTime.Now, (long)updateWatchdog, 0);
        mainthreadTimer.SetEventHandler(() =>
        {
            numberOfFrames++;
        });
        mainthreadTimer.Resume();
    }

    private static void AddWatchdogTimer()
    {
        watchdogTimer = new DispatchSource.Timer (DispatchQueue.MainQueue);
        if (watchdogTimer is not null)
        {
            watchdogTimer.SetTimer(DispatchTime.Now, (long)(updateWatchdogInterval * NSEC_PER_SEC), (long)(updateWatchdogInterval * NSEC_PER_SEC) / 10);
            watchdogTimer.SetEventHandler(() =>
            {
                double fps = numberOfFrames / updateWatchdogInterval;
                numberOfFrames = 0;
                if (useLogs)
                {
                    Console.WriteLine("fps {0:F2}", fps);
                }
                ThrowExceptionForStallingIfNeeded();
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    ((TWWatchdogInspectorViewController)kInspectorWindow.RootViewController).UpdateFPS(fps);
                });
            });
            watchdogTimer.Resume();
        }
    }

    private static void AddRunLoopObserver()
    {
        kObserverRef = Interop.CFRunLoopObserverCreate(IntPtr.Zero, 
            Interop.CFOptionFlags.kCFRunLoopAfterWaiting | Interop.CFOptionFlags.kCFRunLoopBeforeSources |
            Interop.CFOptionFlags.kCFRunLoopBeforeWaiting, 
            true, 0, ObserverCallback, IntPtr.Zero);
        var tun = Interop.CFRunLoopGetMain();
        Interop.CFRunLoopAddObserver(tun, kObserverRef, Interop.kCFRunLoopCommonModes);
    }

    [MonoPInvokeCallback(typeof(Interop.CFRunLoopObserverCallback))]
    private static void ObserverCallback(IntPtr observer, Interop.CFOptionFlags activity, IntPtr info)
    {
        if (activity == Interop.CFOptionFlags.kCFRunLoopAfterWaiting)
        {
            lastMainThreadEntryTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
        else if (activity == Interop.CFOptionFlags.kCFRunLoopBeforeSources)
        {
            ThrowExceptionForStallingIfNeeded();
        }
    }
    
    [MonoPInvokeCallback(typeof(Interop.CFRunLoopTimerCallback))]
    private static void MainthreadTimerCallback(IntPtr timer, IntPtr info)
    {
        numberOfFrames++;
    }

    private static void ThrowExceptionForStallingIfNeeded()
    {
        if (enableWatchdogStallingException)
        {
            CFTimeInterval time = DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastMainThreadEntryTime;
            if (time > watchdogMaximumStallingTimeInterval && lastMainThreadEntryTime > 0)
            {
                throw new Exception($"Watchdog timeout: Mainthread stalled for {time:F2} seconds");
            }
        }
    }

    private static void ResetCountValues()
    {
        lastMainThreadEntryTime = 0;
        numberOfFrames = 0;
    }

    private static void SetupStatusView()
    {
        CGRect statusBarFrame = UIApplication.SharedApplication.StatusBarFrame;
        CGSize size = statusBarFrame.Size;
        CGRect frame = new CGRect(0, 0, size.Width, size.Height);
        UIWindow window = new UIWindow(frame)
        {
            RootViewController = new TWWatchdogInspectorViewController(),
            Hidden = false,
        };
        window.WindowLevel = UIWindowLevel.StatusBar + 50;
        kInspectorWindow = window;
    }
}

internal class Interop
{
    internal const string CoreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
    internal static NativeHandle kCFRunLoopCommonModes = CFString.CreateNative("kCFRunLoopCommonModes");

    [Flags]
    internal enum CFOptionFlags : ulong
    {
        kCFRunLoopBeforeSources = (1UL << 2),
        kCFRunLoopAfterWaiting = (1UL << 6),
        kCFRunLoopBeforeWaiting = (1UL << 5)
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void CFRunLoopObserverCallback(IntPtr observer, CFOptionFlags activity, IntPtr info);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void CFRunLoopTimerCallback(IntPtr timer, IntPtr info);
    
    [DllImport(CoreFoundationLibrary)]
    internal static extern IntPtr CFRunLoopGetMain();

    [DllImport(CoreFoundationLibrary)]
    internal static extern IntPtr CFRunLoopObserverCreate(IntPtr allocator, CFOptionFlags activities,
        bool repeats, int index, CFRunLoopObserverCallback callout, IntPtr context);

    [DllImport(CoreFoundationLibrary)]
    internal static extern IntPtr CFRunLoopAddObserver(IntPtr loop, IntPtr observer, IntPtr mode);
    
    [DllImport(CoreFoundationLibrary)]
    internal static extern IntPtr CFRunLoopRemoveObserver(IntPtr loop, IntPtr observer, IntPtr mode);

    [DllImport(CoreFoundationLibrary)]
    internal static extern IntPtr CFRunLoopTimerCreate(IntPtr allocator, double firstDate, double interval,
        CFOptionFlags flags, int order, CFRunLoopTimerCallback callout, IntPtr context);

    [DllImport(CoreFoundationLibrary)]
    internal static extern void CFRunLoopTimerSetTolerance(IntPtr timer, double tolerance);

    [DllImport(CoreFoundationLibrary)]
    internal static extern void CFRunLoopTimerSetNextFireDate(IntPtr timer, double fireDate);

    [DllImport(CoreFoundationLibrary)]
    internal static extern void CFRunLoopAddTimer(IntPtr loop, IntPtr timer, IntPtr mode);

    [DllImport(CoreFoundationLibrary)]
    internal static extern double CFAbsoluteTimeGetCurrent();
}

[Flags]
public enum CFAllocatorFlags : ulong
{
    GCScannedMemory = 0x200uL,
    GCObjectMemory = 0x400uL
}