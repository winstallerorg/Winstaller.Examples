using System;
using System.IO;
using System.Threading.Tasks;
using Winstaller.Sdk;

namespace Winstaller.CustomSteps
{
    [Step("MyLicense")]
    [WindowsView(typeof(MyLicenseView))]
    [ConsoleView(typeof(MyLicenseConsoleView))]
    public class MyLicense : IStep
    {
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
                RaiseNext();
                return;
            }

            view?.DisplayStep(this);
        }

        public void End()
        {
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
