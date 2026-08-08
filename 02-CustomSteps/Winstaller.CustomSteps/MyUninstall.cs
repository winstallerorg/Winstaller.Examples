using System;
using System.Threading.Tasks;
using Winstaller.Interfaces;

namespace Winstaller.CustomSteps
{
    [PluginName("MyUninstall")]
    public class MyUninstall : IStep, IBackgroundRunnableStep
    {
        private MyUninstallView? _view;

        public InstallerContext Context { get; set; } = null!;
        public int CurrentStep { get; set; }
        public int TotalSteps { get; set; }

        public Task ExecuteAsync(IProgress<InstallationProgress>? progress) => ExecuteUninstallAsync(progress);

        public event EventHandler? Canceled;
        public event EventHandler? Next;
#pragma warning disable CS0067
        public event EventHandler? Back;
#pragma warning restore CS0067

        public void Initialize(StepConfig config)
        {
        }

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
                    catch (Exception)
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
                    _view = new MyUninstallView(this);
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

        public async Task ExecuteUninstallAsync(IProgress<InstallationProgress>? progress)
        {
            var service = Context.GetService<IUninstallStep>();
            if (service == null)
            {
                throw new InvalidOperationException("IUninstallStep service not registered in context");
            }
            await service.ExecuteUninstallAsync(this, progress);
        }

        public Task RollbackAsync()
        {
            return Task.FromResult(0);
        }
    }
}
