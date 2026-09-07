using System;
using System.Threading.Tasks;
using Winstaller.Sdk;

namespace Winstaller.CustomSteps
{
    [Step("MyUninstall")]
    [WindowsView(typeof(MyUninstallView))]
    [ConsoleView(typeof(MyUninstallConsoleView))]
    public class MyUninstall : IStep, IBackgroundRunnableStep
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

        public Task ExecuteAsync(IProgress<InstallationProgress>? progress) => ExecuteUninstallAsync(progress);

        public void Start(IView? view)
        {
            if (Context.IsSilent)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await ExecuteUninstallAsync(Context.Progress);
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

        public async Task ExecuteUninstallAsync(IProgress<InstallationProgress>? progress)
        {
            var service = Context.GetService<IUninstallStep>();
            if (service != null)
            {
                await service.ExecuteUninstallAsync(this, progress);
            }
        }

        public Task RollbackAsync()
        {
            return Task.FromResult(0);
        }
    }
}
