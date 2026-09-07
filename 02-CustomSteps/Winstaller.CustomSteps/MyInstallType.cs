using System;
using System.Threading.Tasks;
using Winstaller.Sdk;

namespace Winstaller.CustomSteps
{
    [Step("MyInstallType")]
    [WindowsView(typeof(MyInstallTypeView))]
    [ConsoleView(typeof(MyInstallTypeConsoleView))]
    public class MyInstallType : IStep, IStepVisibility
    {
        public InstallerContext Context { get; set; } = null!;
        public int CurrentStep { get; set; }
        public int TotalSteps { get; set; }

        public event EventHandler? Canceled;
        public event EventHandler? Next;
        public event EventHandler? Back;

        public bool ShouldShow(InstallerContext context) => true;

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

        public Task RollbackAsync() => Task.FromResult(0);
    }
}
