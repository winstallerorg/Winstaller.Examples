using System;
using System.Threading.Tasks;
using Winstaller.Sdk;

namespace Winstaller.CustomSteps
{
    [Step("MyInstall")]
    [WindowsView(typeof(MyInstallView))]
    [ConsoleView(typeof(MyInstallConsoleView))]
    public class MyInstall : IStep, IBackgroundRunnableStep
    {
        public InstallerContext Context { get; set; } = null!;
        public int CurrentStep { get; set; }
        public int TotalSteps { get; set; }

        public event EventHandler? Canceled;
        public event EventHandler? Next;
#pragma warning disable CS0067
        public event EventHandler? Back;
#pragma warning restore CS0067

        public void Initialize(StepConfig config)
        {
        }

        public Task ExecuteAsync(IProgress<InstallationProgress>? progress) => ExecuteInstallAsync(progress);

        public void Start(IView? view)
        {
            if (Context.IsSilent)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await ExecuteInstallAsync(Context.Progress);
                        Next?.Invoke(this, EventArgs.Empty);
                    }
                    catch
                    {
                        Canceled?.Invoke(this, EventArgs.Empty);
                    }
                });
                return;
            }

            view?.DisplayStep(this);
        }

        public void End()
        {
        }

        public void RaiseNext() => Next?.Invoke(this, EventArgs.Empty);
        public void RaiseCanceled() => Canceled?.Invoke(this, EventArgs.Empty);

        public async Task ExecuteInstallAsync(IProgress<InstallationProgress>? progress)
        {
            var service = Context.GetService<IInstallStep>();
            if (service != null)
            {
                await service.ExecuteInstallAsync(this, progress);
            }
        }

        public async Task RollbackAsync()
        {
            var service = Context.GetService<IInstallStep>();
            if (service != null)
            {
                await service.RollbackAsync(this);
            }
        }
    }
}
