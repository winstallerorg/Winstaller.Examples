using System;
using System.Threading.Tasks;
using Winstaller.Interfaces;

namespace Winstaller.CustomSteps
{
    [PluginName("MySuccess")]
    public class MySuccess : IStep
    {
        private MySuccessView? _view;

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
            if (view != null)
            {
                view.SetStepView(() =>
                {
                    _view = new MySuccessView(this);
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
