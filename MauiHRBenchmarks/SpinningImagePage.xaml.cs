using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiHRBenchmarks;

public partial class SpinningImagePage : ContentPage
{
    private CancellationTokenSource animateTimerCancellationTokenSource;

    
    public SpinningImagePage()
    {
        InitializeComponent();
        StartCoverAnimation(new CancellationTokenSource());
    }
    
    void StartCoverAnimation(CancellationTokenSource tokenSource)
    {
        try
        {
            animateTimerCancellationTokenSource = tokenSource;

            Microsoft.Maui.Controls.Application.Current!.Dispatcher.DispatchAsync(async () =>
            {
                if (!animateTimerCancellationTokenSource.IsCancellationRequested)
                {
                    await CoverImage.RelRotateTo(360, 5000, Easing.Linear);

                    StartCoverAnimation(animateTimerCancellationTokenSource);
                }
            });
        }
        catch (TaskCanceledException ex)
        {
            Debug.WriteLine(ex);
        }
    }
}