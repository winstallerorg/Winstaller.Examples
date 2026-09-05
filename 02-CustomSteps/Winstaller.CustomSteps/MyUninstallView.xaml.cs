using Winstaller.Sdk;
using Winstaller.Sdk.Exceptions;
using Winstaller.Sdk.Services;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Winstaller.CustomSteps
{
    /// <summary>
    /// Interaction logic for MyUninstallView.xaml
    /// </summary>
    public partial class MyUninstallView : StepViewBase
    {
        private readonly MyUninstall _step;
        private bool _uninstallationComplete = false;
        private bool _isUninstalling = false;
        private readonly System.Threading.CancellationTokenSource _cts = new();

        public MyUninstallView(MyUninstall step)
        {
            _step = step;
            Step = step;
            InitializeComponent();
            Loaded += MyUninstallView_Loaded;
        }

        private void MyUninstallView_Loaded(object sender, RoutedEventArgs e)
        {
            var localization = _step.Context.GetService<ILocalizationService>();

            Title = localization != null 
                ? localization.GetString("InstallStep_UninstallingTitle") 
                : "Uninstalling";

            Header = Title;
            HeaderTextBlock.Text = Header;

            IsBackVisible = false;
            IsCancelVisible = true;
            IsCancelEnabled = true;
            IsNextVisible = true;
            IsNextEnabled = false;
            IsFinishVisible = false;

            _isUninstalling = true;
            IsPerformingCriticalWork = true;

            AppendLog("Plugin-based uninstallation started");

            Task.Run(async () =>
            {
                try
                {
                    _step.Context.CancellationToken = _cts.Token;
                    var progress = new Progress<InstallationProgress>(p =>
                    {
                        Dispatcher.Invoke(() => UpdateProgress(p));
                    });
                    _step.Context.Progress = progress;

                    bool success = false;
                    while (!success)
                    {
                        try
                        {
                            await _step.ExecuteUninstallAsync(progress);
                            success = true;
                        }
                        catch (Exception ex) when (ex is AppLockedException || ex is System.IO.IOException || ex is UnauthorizedAccessException || ex is ServiceRunningException || ex is ElevationRejectedException)
                        {
                            bool retry = false;
                            
                            await Dispatcher.InvokeAsync(() =>
                            {
                                string friendlyMsg = GetFriendlyErrorMessage(ex);
                                string title = localization != null ? localization.GetString("App_Error") : "Uninstallation Error";

                                var dialogService = _step.Context.GetService<IDialogService>();
                                if (dialogService != null)
                                {
                                    retry = dialogService.ShowRetry(friendlyMsg, title);
                                }
                                else
                                {
                                    var result = MessageBox.Show(
                                        Application.Current.MainWindow,
                                        friendlyMsg,
                                        title,
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Warning);
                                    retry = (result == MessageBoxResult.Yes);
                                }

                                if (retry)
                                {
                                    AppendLog("User requested retry after error");
                                }
                            });

                            if (!retry)
                            {
                                throw new OperationCanceledException("User aborted the uninstallation after error", ex);
                            }
                        }
                    }

                    _isUninstalling = false;
                    Dispatcher.Invoke(() => OnComplete());
                }
                catch (Exception ex)
                {
                    _isUninstalling = false;
                    Dispatcher.Invoke(() => OnError(ex));
                }
            });
        }

        private void UpdateProgress(InstallationProgress progress)
        {
            StatusTextBlock.Text = progress.Status;
            UninstallProgressBar.Value = progress.Percentage;
            ProgressTextBlock.Text = $"{progress.Percentage}%";
            if (!string.IsNullOrEmpty(progress.LogMessage))
            {
                AppendLog(progress.LogMessage!);
            }
        }

        private void OnComplete()
        {
            _uninstallationComplete = true;
            IsPerformingCriticalWork = false;
            UninstallProgressBar.Value = 100;
            ProgressTextBlock.Text = "100%";
            
            var localization = _step.Context.GetService<ILocalizationService>();
            StatusTextBlock.Text = localization != null
                ? localization.GetString("InstallStep_UninstallationComplete")
                : "Uninstallation completed successfully.";
                
            AppendLog("Uninstallation complete");

            IsNextEnabled = true;
            IsCancelEnabled = false;
        }

        private void OnError(Exception ex)
        {
            IsPerformingCriticalWork = false;
            StatusTextBlock.Text = "Error: " + ex.Message;
            AppendLog("Error occurred: " + ex.Message);

            if (ex is OperationCanceledException || ex is TaskCanceledException)
            {
                _step.RaiseCanceled();
                return;
            }

            var localization = _step.Context.GetService<ILocalizationService>();
            string title = localization != null ? localization.GetString("App_Error") : "Error";

            var dialogService = _step.Context.GetService<IDialogService>();
            if (dialogService != null)
            {
                dialogService.Show(ex.Message, title, DialogButton.OK, DialogIcon.Error);
            }
            else
            {
                MessageBox.Show(
                    Application.Current.MainWindow,
                    ex.Message,
                    title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            IsCancelEnabled = true;
        }

        private string GetFriendlyErrorMessage(Exception ex)
        {
            if (ex is ServiceRunningException serviceEx)
            {
                return $"Service '{serviceEx.ServiceName}' is running. Please stop it manually and click Yes to retry, or No to abort.";
            }
            if (ex is AppLockedException appLockedEx)
            {
                string files = appLockedEx.LockedFiles != null ? string.Join(", ", appLockedEx.LockedFiles) : string.Empty;
                return $"The application files are locked by other processes ({files}). Close the application and click Yes to retry.";
            }
            if (ex is ElevationRejectedException)
            {
                return "Administrator privileges are required to continue the uninstallation. Click Yes to try prompting again.";
            }
            if (ex is System.IO.IOException)
            {
                return "Files are currently in use by another process. Please close all related applications and click Yes to retry.";
            }
            if (ex is UnauthorizedAccessException)
            {
                return "Access denied. You do not have permissions to write to the destination directory. Click Yes to retry.";
            }
            return $"An error occurred: {ex.Message}. Would you like to try again?";
        }

        private void AppendLog(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            LogTextBox.AppendText($"[{timestamp}] {message}\n");
            LogTextBox.ScrollToEnd();
        }

        public override void OnNext()
        {
            if (_uninstallationComplete)
            {
                _step.RaiseNext();
            }
        }

        public override void OnCancel()
        {
            _cts.Cancel();
            if (!_isUninstalling)
            {
                _step.RaiseCanceled();
            }
        }
    }
}

