using Drastic.Services;
using Drastic.Tools;
using Drastic.ViewModels;

namespace MvvmTestCanExecute;

public partial class MainPage : ContentPage
{
	public MainPageViewModel vm;
	
	public MainPage(IServiceProvider provider)
	{
		InitializeComponent();
		this.BindingContext = vm = provider.GetRequiredService<MainPageViewModel>();
	}
}

public class MainPageViewModel : BaseViewModel
{
	private AsyncCommand counterCommand;
	private int counter;

	private bool canUpdateCounter;
	
	public int Counter
	{
		get { return this.counter; }
		set
		{
			this.SetProperty(ref this.counter, value);
			this.OnPropertyChanged(nameof(CounterClickedText));
		} 
	}

	public string CounterClickedText => this.Counter <= 0 ? "Click me!!!!" : $"Clicked {this.Counter} times!";

	public bool CanUpdateCounter
	{
		get { return this.canUpdateCounter; }
		set
		{
			this.SetProperty(ref this.canUpdateCounter, value);
			this.RaiseCanExecuteChanged();
		} 
	}
	
	public MainPageViewModel(IServiceProvider services)
		: base(services)
	{
		counterCommand = new AsyncCommand(UpdateCounter, () => this.CanUpdateCounter, this.Dispatcher, this.ErrorHandler);
	}
	
	public AsyncCommand CounterCommand => this.counterCommand;

	public override void RaiseCanExecuteChanged()
	{
		base.RaiseCanExecuteChanged();
		this.CounterCommand.RaiseCanExecuteChanged();
	}

	public Task UpdateCounter()
	{
		Counter++;
		return Task.CompletedTask;
	}
}

public class MauiAppDispatcher : IAppDispatcher
{
	public bool Dispatch(Action action)
	{
		return Microsoft.Maui.Controls.Application.Current!.Dispatcher.Dispatch(action);
	}
}

public class MauiErrorHandler : IErrorHandlerService
{
	public void HandleError(Exception ex)
	{
		System.Diagnostics.Debug.WriteLine(ex.Message);
	}
}
