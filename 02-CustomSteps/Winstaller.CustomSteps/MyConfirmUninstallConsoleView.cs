using System.Threading.Tasks;
using Winstaller.Sdk;

namespace Winstaller.CustomSteps
{
    public class MyConfirmUninstallConsoleView : IConsoleStepView
    {
        public Task ExecuteAsync(IStep step, IConsoleContext console)
        {
            if (step is MyConfirmUninstall confirm)
            {
                if (confirm.Context.IsSilent)
                {
                    confirm.RaiseNext();
                    return Task.FromResult(0);
                }

                bool proceed = console.PromptYesNo($"[CUSTOM] Are you sure you want to remove {confirm.Context.AppName}?", defaultYes: true);
                if (proceed)
                {
                    confirm.RaiseNext();
                }
                else
                {
                    console.WriteLine("Uninstallation canceled.");
                    confirm.RaiseCanceled();
                }
            }
            return Task.FromResult(0);
        }
    }
}
