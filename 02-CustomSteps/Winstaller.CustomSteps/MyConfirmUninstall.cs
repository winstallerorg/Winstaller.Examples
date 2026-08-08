using System;
using System.Threading.Tasks;
using Winstaller.Interfaces;

namespace Winstaller.CustomSteps
{
    [PluginName("MyConfirmUninstall")]
    public class MyConfirmUninstall : IStep
    {
        private MyConfirmUninstallView? _view;
        private StepConfig? _config;

        public InstallerContext Context { get; set; } = null!;
        public int CurrentStep { get; set; }
        public int TotalSteps { get; set; }

        public string? Message => _config?.Message;

        public event EventHandler? Canceled;
        public event EventHandler? Next;
        public event EventHandler? Back;

        public void Initialize(StepConfig config)
        {
            _config = config;
        }

        public void Start(IView? view)
        {
            if (Context.IsSilent)
            {
                Next?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (view != null)
            {
                view.SetStepView(() =>
                {
                    _view = new MyConfirmUninstallView(this);
                    return _view;
                });
            }
        }

        public void End()
        {
            _view = null;
        }

        public void RaiseNext() => Next?.Invoke(this, EventArgs.Empty);
        public void RaiseBack() => Back?.Invoke(this, EventArgs.Empty);
        public void RaiseCanceled() => Canceled?.Invoke(this, EventArgs.Empty);

        public Task RollbackAsync()
        {
            return Task.FromResult(0);
        }
    }
}
