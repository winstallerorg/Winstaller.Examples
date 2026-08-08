using System;
using System.IO;
using System.Threading.Tasks;
using Winstaller.Interfaces;

namespace Winstaller.CustomSteps
{
    [PluginName("MyLicense")]
    public class MyLicense : IStep
    {
        private MyLicenseView? _view;

        public InstallerContext Context { get; set; } = null!;
        public int CurrentStep { get; set; }
        public int TotalSteps { get; set; }

        public event EventHandler? Canceled;
        public event EventHandler? Next;
        public event EventHandler? Back;

        public void Initialize(StepConfig config)
        {
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
                    _view = new MyLicenseView(this);
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

        public Stream? GetLicenseStream()
        {
            var service = Context.GetService<ILicenseStep>();
            return service?.GetLicenseStream(Context);
        }

        public string GetLicenseText()
        {
            var service = Context.GetService<ILicenseStep>();
            return service?.GetLicenseText(Context) ?? string.Empty;
        }

        public bool IsRtfFormat()
        {
            var service = Context.GetService<ILicenseStep>();
            return service?.IsRtfFormat(Context) ?? false;
        }

        public Task RollbackAsync()
        {
            return Task.FromResult(0);
        }
    }
}
