using System;
using System.Threading.Tasks;
using Winstaller.Sdk;

namespace Winstaller.CustomSteps
{
    [PluginName("MyInstall")]
    public class MyInstall : IStep, IBackgroundRunnableStep
    {
        private MyInstallView? _view;

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

            if (view != null)
            {
                view.SetStepView(() =>
                {
                    _view = new MyInstallView(this);
                    return _view;
                });
            }
        }

        public void End()
        {
            _view = null;
        }

        public void RaiseNext() => Next?.Invoke(this, EventArgs.Empty);
        public void RaiseCanceled() => Canceled?.Invoke(this, EventArgs.Empty);

        public async Task ExecuteInstallAsync(IProgress<InstallationProgress>? progress)
        {
            var service = Context.GetService<IInstallStep>();
            if (service == null)
            {
                throw new InvalidOperationException("IInstallStep service not registered in context");
            }
            await service.ExecuteInstallAsync(this, progress);
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

